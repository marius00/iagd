using EvilsoftCommons.Exceptions;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using IAGrim.Backup;
using IAGrim.Backup.Cloud;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Settings;

namespace IAGrim.BuddyShare {
    class BuddyItemsService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BuddyItemsService));
        private BackgroundWorker _bw = new BackgroundWorker();
        private bool _disposed = false;

        private readonly IBuddyItemDao _buddyItemDao;
        private readonly Dictionary<long, ActionCooldown> _cooldowns = new Dictionary<long, ActionCooldown>();
        private readonly SettingsService _settings;
        private readonly AuthService _authService;
        private readonly IBuddySubscriptionDao _subscriptionRepo;
        private readonly long _defaultCooldown;

        public BuddyItemsService(
            IBuddyItemDao buddyItemDao,
            long cooldown,
            SettingsService settings,
            AuthService authService,
            IBuddySubscriptionDao subscriptionRepo
        ) {
            _buddyItemDao = buddyItemDao;
            _settings = settings;
            _authService = authService;
            _subscriptionRepo = subscriptionRepo;
            _defaultCooldown = cooldown;

            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = false;
            _bw.RunWorkerAsync();
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "BuddyBackground";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            ExceptionReporter.EnableLogUnhandledOnThread();

            BackgroundWorker worker = sender as BackgroundWorker;
            while (!worker.CancellationPending) {
                try {
                    Thread.Sleep(5000);

                    var missingNames = _buddyItemDao.ListItemsWithMissingName();
                    _buddyItemDao.UpdateNames(missingNames);

                    if (_settings.GetLocal().OptOutOfBackups) {
                        Logger.Info("User opted out of online features, disabling buddy items.");
                        return;
                    }

                    if (_authService.GetRestService() == null) {
                        // Logger.Info("Not logged into online backups, skipping buddy sync");
                        _cooldowns.Clear();
                        continue;
                    }
                    
                    var subscriptions = _subscriptionRepo.ListAll();
                    foreach (var subscription in subscriptions) {
                        if (subscription.Id <= 9999) {
                            // TODO: Delete legacy
                            continue;
                        }
                        
                        // Cooldown per user (so that newly added ones gets fetched fairly fast)
                        if (!_cooldowns.ContainsKey(subscription.Id)) {
                            _cooldowns[subscription.Id] = new ActionCooldown(_defaultCooldown);
                        }
                        if (!_cooldowns[subscription.Id].IsReady) continue;

                        Logger.Debug($"Downloading items for buddy {subscription.Nickname} ({subscription.Id}) with TS > {subscription.LastSyncTimestamp}");
                        SyncDown(subscription);
                        _cooldowns[subscription.Id].Reset();
                    }

                    // Fetch own buddy id, if missing.
                    var buddyId = _settings.GetPersistent().BuddySyncUserIdV3;
                    if (!buddyId.HasValue || buddyId <= 0) {
                        Logger.Info("Fetching own buddy ID from cloud");
                        var id = _authService.GetRestService()?.Get<BuddyIdResult>(Uris.GetBuddyIdUrl);
                        _settings.GetPersistent().BuddySyncUserIdV3 = id.Id;
                    }

                }
                catch (NullReferenceException ex) {
                    Logger.Info("The following exception is logged, but can safely be ignored:");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
            }
        }


        private ItemDownloadDto Get(BuddySubscription subscription) {
            var url = $"{Uris.BuddyItemsUrl}?id={subscription.Id}&ts={subscription.LastSyncTimestamp}";
            return _authService.GetRestService()?.Get<ItemDownloadDto>(url);
        }

        private void SyncDown(BuddySubscription subscription) {
            try {
                Logger.Debug("Checking buddy cloud for new items..");
                // Fetching the known IDs will allow us to skip the items we just uploaded. A massive issue if you just logged on and have 10,000 items for download.
                var knownItems = _buddyItemDao.GetOnlineIds(subscription);
                var sync = Get(subscription);

                // Skip items we've already have
                var items = sync.Items
                    .Where(item => !knownItems.Contains(item.Id))
                    .Select(item => ToBuddyItem(subscription, item))
                    .ToList();
                

                // Store items in batches, to prevent IA just freezing up if we happen to get 10-20,000 items.
                var batches = BatchUtil.ToBatches<BuddyItem>(items);
                foreach (var batch in batches) {
                    Logger.Debug($"Storing batch of {batch.Count} items");
                    _buddyItemDao.Save(subscription, batch);
                }
                _buddyItemDao.UpdateNames(items);

                // Delete items that no longer exist
                _buddyItemDao.Delete(subscription, sync.Removed);

                // Store timestamp to db
                subscription.LastSyncTimestamp = sync.Timestamp;
                _subscriptionRepo.Update(subscription);


                Logger.Debug($"Fetched {items.Count} items, new timestamp is {sync.Timestamp}");
            }
            catch (Exception ex) {
                Logger.Warn(ex);
            }
        }

        private static BuddyItem ToBuddyItem(BuddySubscription subscription, CloudItemDto itemDto) {
            return new BuddyItem {
                BaseRecord = itemDto.BaseRecord,
                EnchantmentRecord = itemDto.EnchantmentRecord,
                IsHardcore = itemDto.IsHardcore,
                MateriaRecord = itemDto.MateriaRecord,
                Mod = itemDto.Mod,
                ModifierRecord = itemDto.ModifierRecord,
                PrefixRecord = itemDto.PrefixRecord,
                StackCount = itemDto.StackCount,
                SuffixRecord = itemDto.SuffixRecord,
                TransmuteRecord = itemDto.TransmuteRecord,
                RemoteItemId = itemDto.Id,
                CreationDate = itemDto.CreatedAt,
                MinimumLevel = itemDto.LevelRequirement,
                Rarity = itemDto.Rarity,
                BuddyId = subscription.Id,
                PrefixRarity = itemDto.PrefixRarity,
                Seed = itemDto.Seed,
                RelicSeed = itemDto.RelicSeed,
                EnchantmentSeed = itemDto.EnchantmentSeed
            };
        }

        void Dispose(bool disposing) {
            if (!_disposed && disposing) {
                if (_bw != null) {
                    _bw.CancelAsync();
                    _bw = null;
                }
            }

            _disposed = true;
        }


        public void Dispose() {
            Dispose(true);
        }

        class BuddyIdResult {
            public long Id { get; set; }
        }
    }
}