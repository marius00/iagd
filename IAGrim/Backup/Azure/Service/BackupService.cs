using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ActionCooldown _deletionCooldown;
        private readonly ActionCooldown _uploadCooldown;
        private readonly ActionCooldown _downloadCooldown;
        private DateTimeOffset _lastSearchDt = DateTimeOffset.UtcNow;

        public event EventHandler OnUploadComplete;

        public BackupService(AzureAuthService azureAuthService, IPlayerItemDao playerItemDao, IAzurePartitionDao azurePartitionDao) {
            _azureAuthService = azureAuthService;
            _playerItemDao = playerItemDao;
            _azurePartitionDao = azurePartitionDao;
            _deletionCooldown = new ActionCooldown(1000 * 60); // ~1min
            _uploadCooldown = new ActionCooldown(1000 * 10); // ~10 sec
            _downloadCooldown = new ActionCooldown(1000 * 60 * 15); // ~15min
        }

        public void Execute() {
            if (!_azureAuthService.IsAuthenticated()) {
                return;
            }

            if (_azureSyncService == null) {
                _azureSyncService = new AzureSyncService(_azureAuthService.GetRestService());
            }

            
            _deletionCooldown.ExecuteIfReady(SyncDeletions);
            _uploadCooldown.ExecuteIfReady(SyncUp);


            // Downloads will eventually stop
            const int syncFreezeTime = 31;
            if ((DateTimeOffset.UtcNow - _lastSearchDt).TotalMinutes < syncFreezeTime) {
                _downloadCooldown.ExecuteIfReady(SyncDown);
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
                    var mapping = _azureSyncService.Save(batch);
                    _playerItemDao.SetAzureIds(mapping);

                    numRemaining -= numTaken;
                }

                Logger.Info("Upload complete");
                OnUploadComplete?.Invoke(this, null);
            }
        }

        private void SyncDown() {
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
        }
    }
}
