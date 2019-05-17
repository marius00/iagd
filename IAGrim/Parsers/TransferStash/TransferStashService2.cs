using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parser.Stash;
using IAGrim.Parsers.Arz;
using IAGrim.Properties;
using IAGrim.Services;
using IAGrim.StashFile;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;

// ReSharper disable InconsistentlySynchronizedField

namespace IAGrim.Parsers.TransferStash {
    class TransferStashService2 {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransferStashService2));
        private readonly object _fileLock = new object();
        private readonly IPlayerItemDao _playerItemDao;
        private readonly TransferStashServiceCache _cache;
        public event EventHandler OnUpdate;

        public TransferStashService2(IPlayerItemDao playerItemDao, TransferStashServiceCache cache) {
            _playerItemDao = playerItemDao;
            _cache = cache;
        }

        private static List<UserFeedback> Validate(Stash stash, int lootFromIndex) {
#if DEBUG
            if (stash.Tabs.Count < 5) {
                Logger.Debug($"Transfer stash only has {stash.Tabs.Count} tabs, upgrading to 5 tabs.");
                while (stash.Tabs.Count < 5) {
                    stash.Tabs.Add(new StashTab());
                }
            }
#endif

            if (stash.Tabs.Count < 2) {
                Logger.WarnFormat($"Transfer stash contains {stash.Tabs.Count} pages. IA requires at least 2 stash pages to function properly.");
                return new List<UserFeedback> {new UserFeedback(UserFeedbackLevel.Warning, GlobalSettings.Language.GetTag("iatag_not_enough_stash", stash.Tabs.Count))};
            }

            if (stash.Tabs.Count < lootFromIndex + 1) {
                Logger.Warn($"You have configured IA to loot from stash {lootFromIndex + 1} but you only have {stash.Tabs.Count} pages");
                return new List<UserFeedback> {new UserFeedback(UserFeedbackLevel.Warning, GlobalSettings.Language.GetTag("iatag_invalid_loot_stash_number", lootFromIndex + 1, stash.Tabs.Count))};
            }

            return new List<UserFeedback>(0);
        }

        private (Stash, List<UserFeedback>) Load(string transferFile) {
            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(transferFile));
            var stash = new Stash();
            if (stash.Read(pCrypto)) {
                var lootFromStashIdx = Arz.TransferStashService.GetStashToLootFrom(stash);
                var errors = Validate(stash, lootFromStashIdx);
                if (errors.Count > 0) {
                    return (null, errors);
                }

                return (stash, new List<UserFeedback>(0));
            }

            return (null, UserFeedback.FromTagSingleton("iatag_stash_corrupted_or_unreadable"));
        }

        public (bool, List<UserFeedback>) IsTransferStashLootable() {
            if (!Settings.Default.SecureTransfers) {
                Logger.Debug("Secure transfers are disabled, ignoring stash loot safety checks.");
                return (true, new List<UserFeedback>());
            }

            if (GlobalSettings.StashStatus != StashAvailability.CLOSED) {
                Logger.Info($"Delaying stash loot, stash status {GlobalSettings.StashStatus} != CLOSED.");
                return (false, UserFeedback.FromTagSingleton("iatag_feedback_delaying_stash_loot_status"));
            }

            if (!GrimStateTracker.IsFarFromStash) {
                var distance = GrimStateTracker.Distance;
                if (distance.HasValue) {
                    Logger.Info($"Delaying stash loot, too close to stash. ({distance.Value}m away, minimum is {GrimStateTracker.MinDistance}m)");
                    return (false, UserFeedback.FromTagSingleton("iatag_feedback_too_close_to_stash"));
                }
                else {
                    //Logger.Info("Delaying stash loot, player location is unknown.");
                    return (false, UserFeedback.FromTagSingleton("iatag_feedback_stash_position_unknown"));
                }
            }


            return (true, new List<UserFeedback>());
        }

        public List<UserFeedback> Loot(string transferFile) {
            Logger.Info($"Checking {transferFile} for items to loot");
            lock (_fileLock) {
                var (stash, errors) = Load(transferFile);
                if (errors.Count > 0) {
                    return errors; // Cannot recover from any of these
                }

                // TODO: Delegate the stash to the crafting service
                List<UserFeedback> feedbacks = new List<UserFeedback>();
                var lootFromStashIdx = Arz.TransferStashService.GetStashToLootFrom(stash);
                if (HasItems(stash, lootFromStashIdx)) {
                    var classifiedItems = Classify(stash.Tabs[lootFromStashIdx]);

                    // Warn or auto delete bugged duplicates
                    if (classifiedItems.Duplicates.Count > 0) {
                        if (Settings.Default.DeleteDuplicates) {
                            stash.Tabs[lootFromStashIdx].Items.RemoveAll(e => classifiedItems.Duplicates.Any(m => m.Equals(e)));

                            // No items to loot, so just delete the duplicates.
                            if (classifiedItems.Remaining.Count == 0) {
                                if (!SafeTransferStashWriter.SafelyWriteStash(transferFile, stash)) {
                                    Logger.Error("Fatal error deleting items from Grim Dawn, items has been duplicated.");
                                }
                            }
                        }
                        else {
                            classifiedItems.Duplicates.ForEach(item => Logger.Debug($"NotLootedDuplicate: {item}"));
                            feedbacks.Add(new UserFeedback(UserFeedbackLevel.Warning,
                                GlobalSettings.Language.GetTag("iatag_feedback_duplicates_not_looted", classifiedItems.Duplicates.Count),
                                HelpService.GetUrl(HelpService.HelpType.DuplicateItem)
                            ));
                        }
                    }

                    if (classifiedItems.Stacked.Count > 0) {
                        classifiedItems.Stacked.ForEach(item => Logger.Debug($"NotLootedStacked: {item}"));
                        feedbacks.Add(new UserFeedback(GlobalSettings.Language.GetTag("iatag_feedback_stacked_not_looted", classifiedItems.Stacked.Count)));
                    }

                    // Unknown items, database not parsed for these items, IA can't deal with them.
                    if (classifiedItems.Unknown.Count > 0) {
                        classifiedItems.Unknown.ForEach(item => Logger.Debug($"NotLootedUnknown: {item}"));
                        feedbacks.Add(new UserFeedback(
                            UserFeedbackLevel.Warning,
                            GlobalSettings.Language.GetTag("iatag_feedback_unknown_not_looted", classifiedItems.Unknown.Count),
                            HelpService.GetUrl(HelpService.HelpType.NotLootingUnidentified)
                        ));
                    }

                    // The items we can actually loot (or delete duplicates)
                    if (classifiedItems.Remaining.Count > 0) {
                        OnUpdate?.Invoke(this, null);
                        
                        stash.Tabs[lootFromStashIdx].Items.RemoveAll(e => classifiedItems.Remaining.Any(m => m.Equals(e)));

                        var isHardcore = GlobalPaths.IsHardcore(transferFile);
                        if (StoreItemsToDatabase(classifiedItems.Remaining, stash.ModLabel, isHardcore)) {
                            feedbacks.Add(new UserFeedback(GlobalSettings.Language.GetTag("iatag_looted_from_stash", classifiedItems.Remaining.Count, lootFromStashIdx + 1)));
                            
                            if (!SafeTransferStashWriter.SafelyWriteStash(transferFile, stash)) {
                                Logger.Error("Fatal error deleting items from Grim Dawn, items has been duplicated.");
                            }

                            // TODO: Do a quick check to see if the items are TRUUUULY gone from the stash?
                        } else {
                            feedbacks.Add(UserFeedback.FromTag("iatag_feedback_unable_to_loot_stash")); // TODO: Not sure what to report here.. 
                        }

                    }
                }
                else {
                    Logger.Info($"No items found in transfer stash {lootFromStashIdx + 1}.");
                    feedbacks.Add(UserFeedback.FromTag("iatag_feedback_no_items_to_loot"));
                }

                return feedbacks;
            }
        }

        private bool HasItems(Stash stash, int tab) => stash.Tabs[tab].Items.Count > 0;


        private ClassifiedItems Classify(StashTab tab) {
            var stacked = tab.Items.Where(item =>
                    item.StackCount > 1
                    || _cache.StackableRecords.Contains(item.BaseRecord)
                    || _cache.SpecialRecords.Contains(item.BaseRecord) // Special "single seed" items.
            ).ToList();

            var unknownItems = tab.Items.Where(item => !_cache.AllRecords.Contains(item.BaseRecord)).ToList();

            var duplicates = tab.Items
                .Where(item => _playerItemDao.Exists(Arz.TransferStashService.Map(item, null, false))) // We don't care about mod/hardcore for dupe checking.
                .ToList();

            var remaining = tab.Items
                .SkipWhile(item => duplicates.Contains(item))
                .SkipWhile(item => stacked.Contains(item))
                .SkipWhile(item => unknownItems.Contains(item))
                .ToList();

            return new ClassifiedItems {
                Stacked = stacked,
                Duplicates = duplicates,
                Unknown = unknownItems,
                Remaining = remaining
            };
        }

        class ClassifiedItems {
            public List<Item> Stacked { get; set; }
            public List<Item> Duplicates { get; set; }
            public List<Item> Unknown { get; set; }
            public List<Item> Remaining { get; set; }
        }


        private bool StoreItemsToDatabase(ICollection<Item> items, string mod, bool isHardcore) {
            var playerItems = items.Select(item => TransferStashService.Map(item, mod, isHardcore)).ToList();

            try {
                _playerItemDao.Save(playerItems);
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "StoreItems");
                return false;
            }

            return true;
        }
    }
}