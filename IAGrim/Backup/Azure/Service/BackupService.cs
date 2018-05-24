using System;
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


        private void SyncUp() {
            var items = _playerItemDao.GetUnsynchronizedItems();
            if (items.Count > 0) {
                int numRemaining = items.Count;

                Logger.Info($"There are {numRemaining} items remaining to be synchronized to azure");
                while (numRemaining > 0) {
                    int numTaken = Math.Min(items.Count, 100);
                    var batch = items
                        .Skip(items.Count - numRemaining)
                        .Take(numTaken)
                        .Select(ItemConverter.ToUpload)
                        .ToList();

                    Logger.Info($"Synchronizing {numTaken} items to azure");
                    try {
                        var mapping = _azureSyncService.Save(batch);
                        _playerItemDao.SetAzureIds(mapping.Items);

                        if (mapping.IsClosed) {
                            _azurePartitionDao.Save(new AzurePartition {
                                Id = mapping.Partition,
                                IsActive = false
                            });

                            Logger.Debug($"Storing partition {mapping.Partition} as closed.");
                        }

                        numRemaining -= numTaken;
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
                        Logger.Debug("No remote change to items");
                    }
                    else if (partialPartition) {
                        _playerItemDao.Save(items);
                        _playerItemDao.Delete(sync.Removed);

                        // Update partition
                        var localPartition = localPartitions.First(p => p.Id == partition.Partition);
                        localPartition.IsActive = partition.IsActive;
                        _azurePartitionDao.Update(localPartition);

                        Logger.Debug($"Updated/Merged in {items.Count} items");
                    }
                    else {
                        _playerItemDao.Save(items);
                        _playerItemDao.Delete(sync.Removed);

                        _azurePartitionDao.Save(new AzurePartition {
                            Id = partition.Partition,
                            IsActive = partition.IsActive
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
