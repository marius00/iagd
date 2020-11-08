using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Azure.Dto;
using IAGrim.Backup.Azure.Util;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Model;
using IAGrim.Utilities;
using log4net;
using NHibernate.Criterion;
using NHibernate.Util;

namespace IAGrim.Backup.Azure.Service {
    public class BackupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupService));
        private AzureSyncService _azureSyncService;
        private readonly AzureAuthService _azureAuthService;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IAzurePartitionDao _azurePartitionDao;
        private Limitations _cooldowns;
        private readonly Func<bool> _isDualComputerInstance;
        private bool _hasSyncedDownOnce = false;
        private DateTimeOffset _lastSearchDt = DateTimeOffset.UtcNow;

        public event EventHandler OnUploadComplete;

        public BackupService(
            AzureAuthService azureAuthService,
            IPlayerItemDao playerItemDao,
            IAzurePartitionDao azurePartitionDao,
            Func<bool> isDualComputerInstance
        ) {
            _azureAuthService = azureAuthService;
            _playerItemDao = playerItemDao;
            _azurePartitionDao = azurePartitionDao;
            _isDualComputerInstance = isDualComputerInstance;
        }

        public void Execute() {
            if (_azureAuthService.CheckAuthentication() != AzureAuthService.AccessStatus.Authorized) {
                return;
            }

            if (_azureSyncService == null) {
                _azureSyncService = new AzureSyncService(_azureAuthService.GetRestService());
            }

            if (_cooldowns == null) {
                var limits = _azureSyncService.GetLimitations();
                var regular = new LimitationSet(cooldownDeletion: limits.Regular.Delete, cooldownUpload: limits.Regular.Upload, cooldownDownload: limits.Regular.Download);
                var multi = new LimitationSet(cooldownDeletion: limits.MultiUsage.Delete, cooldownUpload: limits.MultiUsage.Upload, cooldownDownload: limits.MultiUsage.Download);
                _cooldowns = new Limitations(regular: regular, multiUsage: multi);
            }

            var limitations = _isDualComputerInstance() ? _cooldowns.MultiUsage : _cooldowns.Regular;

            limitations.DeletionCooldown.ExecuteIfReady(SyncDeletions);
            limitations.UploadCooldown.ExecuteIfReady(SyncUp);

            // TODO:
            // Downloads will eventually stop
            const int syncFreezeTime = 31;
            var canSyncDown = !_hasSyncedDownOnce || _isDualComputerInstance(); // Either first sync since startup, or we're in dual-pc mode.
            if (canSyncDown && (DateTimeOffset.UtcNow - _lastSearchDt).TotalMinutes < syncFreezeTime) {
                limitations.DownloadCooldown.ExecuteIfReady(SyncDown);
            }
        }

        public void OnSearch() {
            _lastSearchDt = DateTimeOffset.UtcNow;
        }

        private void SyncDeletions() {
            var items = _playerItemDao.GetItemsMarkedForOnlineDeletion();
            if (items.Count > 0) {
                Logger.Debug($"Detected {items.Count} items marked for removal");
                List<AzureItemDeletionDto> dtos = new List<AzureItemDeletionDto>(items.Count);
                foreach (var item in items) {
                    dtos.Add(new AzureItemDeletionDto {
                        Id = item.Id,
                        Partition = item.Partition
                    });
                }

                if (_azureSyncService.Delete(dtos)) {
                    _playerItemDao.ClearItemsMarkedForOnlineDeletion();
                    Logger.Debug($"Removal successful ({items.Count} items)");
                }
                else {
                    Logger.Warn("Got an error while removing remote items");
                }
            }
        }

        private static void EnsureLegacyCompatible(IList<PlayerItem> items) {
            // Set values on fallback items (v1/v2 backwards compatability)
            var fallbackPartition = Guid.NewGuid().ToString().Replace("-", ""); // TODO: We may be sticking thousands of items into this partition.
            foreach (var item in items.Where(item => string.IsNullOrEmpty(item.AzureUuid) || string.IsNullOrEmpty(item.AzurePartition))) {
                // Legacy items did not have a UUID prior to sync (v1 & v2 sync)
                if (string.IsNullOrEmpty(item.AzureUuid)) {
                    item.AzureUuid = Guid.NewGuid().ToString().Replace("-", "");
                }

                // Legacy items did not have a partition prior to sync (v1 & v2 sync)
                if (string.IsNullOrEmpty(item.AzurePartition)) {
                    item.AzurePartition = fallbackPartition;
                }
            }
        }

        private static List<List<PlayerItem>> ToBatches(IList<PlayerItem> items) {
            var previousPartition = string.Empty;
            List<PlayerItem> currentBatch = new List<PlayerItem>();
            List<List<PlayerItem>> batches = new List<List<PlayerItem>>();

            EnsureLegacyCompatible(items);

            // Max 100 items per batch, no mix of partitions in a batch.
            foreach (var item in items.OrderBy(item => item.AzurePartition)) {
                if (item.AzurePartition != previousPartition || currentBatch.Count > 99) {
                    if (currentBatch.Count > 0) {
                        batches.Add(currentBatch);
                        currentBatch = new List<PlayerItem>();
                    }
                }

                currentBatch.Add(item);
                previousPartition = item.AzurePartition;
            }

            if (currentBatch.Count > 0) {
                batches.Add(currentBatch);
            }

            return batches;
        }

        private void SyncUp() {
            var items = _playerItemDao.GetUnsynchronizedItems();
            var batches = ToBatches(items);

            foreach (var batch in batches) {
                Logger.Info($"Synchronizing batch {batch.First().AzurePartition} with {batch.Count} items to azure");
                try {
                    if (_azureSyncService.Save(batch.Select(ItemConverter.ToUpload).ToList())) {
                        Logger.Info($"Upload successful, marking {batch.Count} as synchronized");
                        _playerItemDao.SetAsSynchronized(batch);
                    }
                    else {
                        Logger.Info($"Upload of {batch.Count} items unsuccessful");
                    }
                }
                catch (AggregateException ex) {
                    Logger.Warn(ex.Message, ex);
                    return;
                }
                catch (WebException ex) {
                    Logger.Warn(ex.Message, ex);
                    return;
                }
                catch (Exception ex) {
                    ExceptionReporter.ReportException(ex, "SyncUp");
                    return;
                }
            }

            Logger.Info("Upload complete");
            OnUploadComplete?.Invoke(this, null);
        }

        private void SyncDown() {
            try {
                // Get my own partitions (db)
                // Filter downloaded partitions by my COMPLETED ones
                // for item insert, filter out existing (only for partial partitions?)

                // Filter out existing "closed" partitions
                var localPartitions = new HashSet<AzurePartition>(_azurePartitionDao.ListAll());
                var remotePartitions = _azureSyncService.GetPartitions();

                var partitions = remotePartitions
                    .Where(p => !string.IsNullOrEmpty(p.Partition))
                    .Where(p => !localPartitions.Any(m => m.Id == p.Partition && !m.IsActive))
                    .ToList();

                var deletedItems = _playerItemDao.GetItemsMarkedForOnlineDeletion();
                foreach (var partition in partitions) {
                    var partialPartition = localPartitions.Any(p => p.Id == partition.Partition);
                    var sync = _azureSyncService.GetItems(partition.Partition);
                    var items = sync.Items.Select(ItemConverter.ToPlayerItem)
                        .Where(m => !deletedItems.Any(d => d.Id == m.AzureUuid && d.Partition == m.AzurePartition))
                        .ToList();

                    if (items.Count == 0 && sync.Removed.Count == 0) {
                        var localPartition = localPartitions.FirstOrDefault(p => p.Id == partition.Partition);
                        if (sync.DisableNow) {
                            if (localPartition != null) {
                                localPartition.IsActive = false;
                                _azurePartitionDao.Update(localPartition);
                            }
                        }

                        // Empty remote partition. TODO: Should have been removed server-side.
                        if (localPartition == null) {
                            _azurePartitionDao.Save(new AzurePartition {
                                Id = partition.Partition,
                                IsActive = false
                            });
                        }

                        Logger.Debug($"No remote change to items for {partition}");
                    }
                    else if (partialPartition) {
                        _playerItemDao.Save(items);
                        _playerItemDao.Delete(sync.Removed);

                        // Update partition
                        var localPartition = localPartitions.FirstOrDefault(p => p.Id == partition.Partition);
                        if (localPartition != null) {
                            localPartition.IsActive = partition.IsActive && !sync.DisableNow;
                            _azurePartitionDao.Update(localPartition);
                        }

                        Logger.Debug($"Updated/Merged in {items.Count} items");
                    }
                    else {
                        _playerItemDao.Save(items);
                        _playerItemDao.Delete(sync.Removed);

                        _azurePartitionDao.Save(new AzurePartition {
                            Id = partition.Partition,
                            IsActive = partition.IsActive && !sync.DisableNow
                        });

                        Logger.Debug($"Received {items.Count} new items, and {sync.Removed.Count} removed");
                    }
                }

                _hasSyncedDownOnce = true;
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return;
            }
            catch (Exception ex) {
                ExceptionReporter.ReportException(ex, "SyncDown");
                Logger.Warn(ex);
                return;
            }
        }

        private class LimitationSet {
            public ActionCooldown DeletionCooldown { get; }
            public ActionCooldown UploadCooldown { get; }
            public ActionCooldown DownloadCooldown { get; }

            public LimitationSet(long cooldownDeletion, long cooldownUpload, long cooldownDownload) {
                DeletionCooldown = new ActionCooldown(cooldownDeletion);
                UploadCooldown = new ActionCooldown(cooldownUpload);
                DownloadCooldown = new ActionCooldown(cooldownDownload);
            }
        }

        private class Limitations {
            public LimitationSet Regular { get; }
            public LimitationSet MultiUsage { get; }

            public Limitations(LimitationSet regular, LimitationSet multiUsage) {
                Regular = regular;
                MultiUsage = multiUsage;
            }
        }
    }
}