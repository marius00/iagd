using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Backup.Cloud.Util;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Settings;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Backup.Cloud.Service {
    public class BackupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupService));
        private readonly SettingsService _settings;
        private CloudSyncService _cloudSyncService;
        private readonly AuthService _authService;
        private readonly IPlayerItemDao _playerItemDao;
        private Limitations _cooldowns;
        private bool _hasSyncedDownOnce = false; // When not using DualPC enabled, IA will only download items once per session. They won't be updating anyways.
        private DateTimeOffset _lastSearchDt = DateTimeOffset.UtcNow;
        const int MaxBatchSize = 100;

        public BackupService(
            AuthService authService,
            IPlayerItemDao playerItemDao,
            SettingsService settings) {
            _authService = authService;
            _playerItemDao = playerItemDao;
            _settings = settings;
        }

        public void Execute() {
            if (_authService.CheckAuthentication() != AuthService.AccessStatus.Authorized) {
                return;
            }

            if (_cloudSyncService == null) {
                _cloudSyncService = new CloudSyncService(_authService.GetRestService());
            }

            if (_cooldowns == null) {
                var limits = _cloudSyncService.GetLimitations();
                var regular = new LimitationSet(cooldownDeletion: limits.Regular.Delete, cooldownUpload: limits.Regular.Upload, cooldownDownload: limits.Regular.Download);
                var multi = new LimitationSet(cooldownDeletion: limits.MultiUsage.Delete, cooldownUpload: limits.MultiUsage.Upload, cooldownDownload: limits.MultiUsage.Download);
                _cooldowns = new Limitations(regular: regular, multiUsage: multi);
            }


            var isDualPc = _settings.GetPersistent().UsingDualComputer;
            var limitations = isDualPc ? _cooldowns.MultiUsage : _cooldowns.Regular;

            //Logger.Debug($"UploadCooldown IsReady={limitations.UploadCooldown.IsReady}, isDualPc={isDualPc}");
            //Logger.Debug(limitations.ToString());
            limitations.DeletionCooldown.ExecuteIfReady(SyncDeletions);
            limitations.UploadCooldown.ExecuteIfReady(SyncUp);

            // Downloads will go into 'idle mode' and stop trying to download new items when IA is not in use.
            const int syncFreezeTime = 31;
            var canSyncDown = isDualPc || !_hasSyncedDownOnce; // Either first sync since startup, or we're in dual-pc mode.
            if (canSyncDown && (DateTimeOffset.UtcNow - _lastSearchDt).TotalMinutes < syncFreezeTime) {
                if (!limitations.DownloadCooldown.IsReady) return;

                if (!SyncDown()) {
                    _hasSyncedDownOnce = true;
                }

                limitations.DownloadCooldown.Reset();
            }
        }

        public void OnSearch() {
            _lastSearchDt = DateTimeOffset.UtcNow;
        }

        private void SyncDeletions() {
            var items = _playerItemDao.GetItemsMarkedForOnlineDeletion();
            if (items.Count <= 0) return;

            Logger.Debug($"Detected {items.Count} items marked for removal from cloud");
            List<DeleteItemDto> dtos = new List<DeleteItemDto>(items.Count);
            dtos.AddRange(items.Select(item => new DeleteItemDto {Id = item.Id}));

            if (_cloudSyncService.Delete(dtos)) {
                _playerItemDao.ClearItemsMarkedForOnlineDeletion();
                Logger.Debug($"Removal successful ({items.Count} items)");
            }
            else {
                Logger.Warn("Got an error while removing remote items");
            }
        }


        private static void EnsureLegacyCompatible(IList<PlayerItem> items) {
            foreach (var item in items.Where(item => string.IsNullOrEmpty(item.CloudId))) {
                var guid = Guid.NewGuid().ToString().Replace("-", "");
                item.CloudId = guid;
            }
        }

        private static List<List<PlayerItem>> ToBatches(IList<PlayerItem> items) {
            List<PlayerItem> currentBatch = new List<PlayerItem>();
            List<List<PlayerItem>> batches = new List<List<PlayerItem>>();

            EnsureLegacyCompatible(items);

            // Max 100 items per batch, no mix of partitions in a batch.
            foreach (var item in items) {
                if (currentBatch.Count >= MaxBatchSize) {
                    batches.Add(currentBatch);
                    currentBatch = new List<PlayerItem>();
                }

                currentBatch.Add(item);
            }

            if (currentBatch.Count > 0) {
                batches.Add(currentBatch);
            }

            return batches;
        }

        private static List<List<DeleteItemDto>> ToBatches(IList<DeleteItemDto> items) {
            List<DeleteItemDto> currentBatch = new List<DeleteItemDto>();
            List<List<DeleteItemDto>> batches = new List<List<DeleteItemDto>>();

            if (items == null)
                return batches;


            // Max 100 items per batch, no mix of partitions in a batch.
            foreach (var item in items) {
                if (currentBatch.Count >= MaxBatchSize) {
                    batches.Add(currentBatch);
                    currentBatch = new List<DeleteItemDto>();
                }

                currentBatch.Add(item);
            }

            if (currentBatch.Count > 0) {
                batches.Add(currentBatch);
            }

            return batches;
        }

        private void SyncUp() {
            var items = _playerItemDao.GetUnsynchronizedItems();
            var batches = ToBatches(items);

            if (items.Count == 0) {
                return;
            }

            Logger.Debug($"Got {items.Count} items to sync up");
            foreach (var batch in batches) {
                Logger.Info($"Synchronizing batch with {batch.Count} items to cloud");
                try {
                    if (_cloudSyncService.Save(batch.Select(ItemConverter.ToUpload).ToList())) {
                        // TODO: Hopefully all were stored?
                        Logger.Info($"Upload successful, marking {batch.Count} as synchronized");
                        _playerItemDao.SetAsSynchronized(batch);
                    }
                    else {
                        Logger.Warn($"Upload of {batch.Count} items unsuccessful");
                    }
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message, ex);
                    return;
                }
            }

            Logger.Info($"Upload complete ({items.Count} items)");
        }

        /// <summary>
        /// Download items from online sync
        /// </summary>
        /// <returns>True if there are additional items to be downloaded</returns>
        private bool SyncDown() {
            try {
                Logger.Debug("Checking cloud sync for new items..");

                // Fetching the known IDs will allow us to skip the items we just uploaded. A massive issue if you just logged on and have 10,000 items for download.
                var knownItems = _playerItemDao.GetOnlineIds();
                var deletedItems = _playerItemDao.GetItemsMarkedForOnlineDeletion();
                var timestamp = _settings.GetPersistent().CloudUploadTimestamp;
                var sync = _cloudSyncService.Get(timestamp);

                // Skip items we've deleted locally
                var items = sync.Items
                    .Where(item => deletedItems.All(deleted => deleted.Id != item.Id))
                    .Where(item => !knownItems.Contains(item.Id))
                    .Select(ItemConverter.ToPlayerItem)
                    .ToList();

                // Store items in batches, to prevent IA just freezing up if we happen to get 10-20,000 items.
                var batches = ToBatches(items);
                foreach (var batch in batches) {
                    Logger.Debug($"Storing batch of {batch.Count} items");
                    _playerItemDao.Save(batch);
                }

                foreach (var batch in ToBatches(sync.Removed)) {
                    _playerItemDao.Delete(batch);
                }
                
                _settings.GetPersistent().CloudUploadTimestamp = sync.Timestamp;

                Logger.Debug($"Updated/Merged in {items.Count} items, new timestamp is {sync.Timestamp}");

                return sync.IsPartial;
            }
            catch (AggregateException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
            catch (WebException ex) {
                Logger.Warn(ex.Message, ex);
                return false;
            }
            catch (Exception ex) {
                Logger.Warn(ex);
                return false;
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


            public override string ToString() {
                return $"DeletionCooldown[{DeletionCooldown}], UploadCooldown[{UploadCooldown}], DownloadCooldown[{DownloadCooldown}]";
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