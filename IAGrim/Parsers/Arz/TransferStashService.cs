using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Properties;
using IAGrim.Services;
using IAGrim.StashFile;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using IAGrim.Parser.Stash;
using IAGrim.Parsers.TransferStash;

namespace IAGrim.Parsers.Arz {
    internal class TransferStashService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransferStashService));

        private readonly ItemSizeService _itemSizeService;

        /// <summary>
        /// Used for the crafting tab, I believe.
        /// </summary>
        private ConcurrentBag<Item> _unlootedItems = new ConcurrentBag<Item>();

        /// <summary>
        /// Used to prevent feedback spam, potentially overriding more important new information from other sources
        /// </summary>
        private bool _hasRecentlyUpdatedTimerFeedback;

        public static bool HasLootedItemsOnceThisSession;

        public readonly int NumStashTabs;

        public IEnumerable<Item> UnlootedItems => _unlootedItems.ToList();

        [Obsolete]
        public event EventHandler StashUpdated;

        public TransferStashService(IDatabaseItemStatDao dbItemStatDao) {
            _itemSizeService = new ItemSizeService(dbItemStatDao);

            // TODO: Should also check for transfer.gsh and pick whichever is newest / has the highest number
            var path = Path.Combine(GlobalPaths.SavePath, "transfer.gst");

            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                UpdateUnlooted(path);

                var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(path));
                var stash = new Stash();

                if (stash.Read(pCrypto)) {
                    NumStashTabs = stash.Tabs.Count;
                }
            }
        }

        public void UpdateUnlooted(string filename) {
            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
            var stash = new Stash();

            if (stash.Read(pCrypto)) {
                // Update the internal listing of unlooted items (in stash tabs)
                var unlootedLocal = new List<Item>();
                foreach (var tab in stash.Tabs) {
                    unlootedLocal.AddRange(tab.Items);
                }

                Interlocked.Exchange(ref _unlootedItems, new ConcurrentBag<Item>(unlootedLocal));
            }
        }

        public static int GetStashToLootFrom(Stash stash) {
            if (Settings.Default.StashToLootFrom == 0) {
                return stash.Tabs.Count - 1;
            }

            return Settings.Default.StashToLootFrom - 1;
        }

        private static int GetStashToDepositTo(Stash stash) {
            if (Settings.Default.StashToDepositTo == 0) {
                return stash.Tabs.Count - 2;
            }

            return Settings.Default.StashToDepositTo - 1;
        }

        /// <summary>
        ///     Loot all the items stored on page X, and store them to the local database
        /// </summary>
        /// <param name="filename"></param>
        ///
         /*
        private string EmptyPageX(string filename) {
            Logger.InfoFormat("Looting {0}", filename);

            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
            var stash = new Stash();

            if (stash.Read(pCrypto)) {
                var lootFromIndex = GetStashToLootFrom(stash);
                var isHardcore = GlobalPaths.IsHardcore(filename);

#if DEBUG
                if (stash.Tabs.Count < 5) {
                    while (stash.Tabs.Count < 5) {
                        stash.Tabs.Add(new StashTab());
                    }

                    SafelyWriteStash(filename, stash);
                    Logger.Info("Upgraded stash to 5 pages.");

                    return string.Empty;
                }
#endif

                // Update the internal listing of unlooted items (in other stash tabs)
                var unlootedLocal = new List<Item>();

                for (var idx = 0; idx < stash.Tabs.Count; idx++) {
                    if (idx != lootFromIndex) {
                        unlootedLocal.AddRange(stash.Tabs[idx].Items);
                    }
                }

                Interlocked.Exchange(ref _unlootedItems, new ConcurrentBag<Item>(unlootedLocal));
                StashUpdated?.Invoke(null, null);

                if (stash.Tabs.Count < 2) {
                    Logger.WarnFormat($"File \"{filename}\" only contains {stash.Tabs.Count} pages. IA requires at least 2 stash pages to function properly.");
                    return GlobalSettings.Language.GetTag("iatag_not_enough_stash", filename, stash.Tabs.Count);
                }

                if (stash.Tabs.Count < lootFromIndex + 1) {
                    Logger.Warn($"You have configured IA to loot from stash {lootFromIndex + 1} but you only have {stash.Tabs.Count} pages");
                    return GlobalSettings.Language.GetTag("iatag_invalid_loot_stash_number", lootFromIndex + 1, stash.Tabs.Count);
                }

                if (stash.Tabs[lootFromIndex].Items.Count > 0) {
                    HasLootedItemsOnceThisSession = true;

                    // Grab the items and clear the tab
                    var items = stash.Tabs[lootFromIndex].Items
                        .Where(m => m.StackCount <= 1)
                        .Where(m => !_playerItemDao.Exists(Map(m, stash.ModLabel, isHardcore)))
                        .ToList();
                    var notLootedDueToStackSize = stash.Tabs[lootFromIndex].Items.Where(m => m.StackCount > 1).ToList();

                    if (notLootedDueToStackSize.Count > 0) {
                        notLootedDueToStackSize.ForEach(item => Logger.Debug($"NotLootedStacksize: {item}"));
                        _setFeedback(
                            "Warning", 
                            GlobalSettings.Language.GetTag("iatag_feedback_stacked_not_looted", notLootedDueToStackSize.Count),
                            HelpService.GetUrl(HelpService.HelpType.NoStacks)
                            );
                    }

                    var notLootedDueToDuplicate = stash.Tabs[lootFromIndex].Items.ToList();
                    notLootedDueToDuplicate.RemoveAll(m => items.Contains(m) || m.StackCount > 1);

                    if (notLootedDueToDuplicate.Count > 0) {
                        notLootedDueToDuplicate.ForEach(item => Logger.Debug($"NotLootedDuplicate: {item}"));
                        _setFeedback(
                            "Warning",
                            GlobalSettings.Language.GetTag("iatag_feedback_duplicates_not_looted", notLootedDueToDuplicate.Count),
                            HelpService.GetUrl(HelpService.HelpType.DuplicateItem)
                        );
                    }

                    stash.Tabs[lootFromIndex].Items.RemoveAll(e => items.Any(m => m.Equals(e)));

                    var storedItems = StoreItemsToDatabase(items, stash.ModLabel, isHardcore, stash.IsExpansion1);
                    var message = GlobalSettings.Language.GetTag("iatag_looted_from_stash", items.Count, lootFromIndex + 1);

                    if (storedItems != null) {
                        Logger.Info(message);

                        // Delete items from IA if we failed to remove them from GD.
                        if (!SafelyWriteStash(filename, stash)) {
                            _playerItemDao.Remove(storedItems);
                        }
                    }

                    _performedLootCallback();

                    return message;
                }

                Logger.Info($"Looting of stash {lootFromIndex + 1} halted, no items available.");

                return GlobalSettings.Language.GetTag("iatag_feedback_no_items_to_loot");
            }

            Logger.Error("Could not load stash file.");
            Logger.Error("An update from the developer is most likely required.");

            return string.Empty;
        }
        */
        public List<PlayerItem> EmptyStash(string filename) {
            Logger.InfoFormat("Looting {0}", filename);

            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
            var items = new List<Item>();
            var stash = new Stash();

            if (stash.Read(pCrypto)) {
                var isHardcore = GlobalPaths.IsHardcore(filename);

                foreach (var tab in stash.Tabs) {
                    // Grab the items and clear the tab
                    items.AddRange(tab.Items);
                    tab.Items.Clear();
                }

                SafeTransferStashWriter.SafelyWriteStash(filename, stash); // TODO: Ideally we should check if it worked. 
                Logger.InfoFormat("Looted {0} items from stash", items.Count);

                return items.Select(m => Map(m, stash.ModLabel, isHardcore)).ToList();
            }

            Logger.Error("Could not load stash file.");
            Logger.Error("An update from the developer is most likely required.");

            return new List<PlayerItem>();
        }

        /// <summary>
        /// Attempt to get the name of the current mod
        /// Vanilla leaves this tag empty
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetModLabel(string filename, out string result) {
            if (File.Exists(filename)) {
                var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
                var stash = new Stash();

                if (stash.Read(pCrypto)) {
                    result = stash.ModLabel;
                    return true;
                }
                else {
                    Logger.Warn($"Discarding transfer file \"{filename}\", could not read the file.");
                }
            }

            result = string.Empty;
            return false;
        }

        private Item GetItem(PlayerItem item) {
            var stashItem = Map(item);

            // Calculate the final stack count (stored stack may be larger than max transfer size)
            if (StashTab.CanStack(item.Slot) || StashTab.HardcodedRecords.Contains(item.BaseRecord)) {
                var maxStack = Math.Min(item.StackCount, 100);
                stashItem.StackCount = (uint) Math.Max(1, Math.Min(stashItem.StackCount, maxStack));
            }
            else {
                stashItem.StackCount = 1;
            }

            item.StackCount -= stashItem.StackCount;
            _itemSizeService.MapItemSize(stashItem);

            return stashItem;
        }

        private List<Item> ConvertToStashItems(IList<PlayerItem> playerItems, int itemsRemaining, StashTab tab) {
            var result = new List<Item>();
            var packer = new Packer(tab.Height, tab.Width);

            _itemSizeService.CacheItemSizes(playerItems);

            // Position existing items
            _itemSizeService.MapItemSizes(tab.Items);
            foreach (var item in tab.Items) {
                packer.Insert(new Shape {
                    Height = item.Height,
                    Width = item.Width
                });
            }

            // Add and position any items that fits
            foreach (var playerItem in playerItems) {
                if (playerItem.StackCount == 0) {
                    playerItem.StackCount = 1;
                }

                while (playerItem.StackCount > 0 && itemsRemaining > 0) {
                    var stashItem = GetItem(playerItem);

                    // Map item size and create a shape map
                    if (!PositionItem(packer, stashItem)) {
                        Logger.Info("Could not fit all items in stash, stopping early");
                        playerItem.StackCount += stashItem.StackCount;

                        return result;
                    }

                    result.Add(stashItem);
                    itemsRemaining -= (int) stashItem.StackCount;
                }
            }

            Logger.Debug("All items fit into stash");

            return result;
        }

        private static bool PositionItem(Packer packer, Item item) {
            var shape = new Shape {
                Height = item.Height,
                Width = item.Width
            };
            var position = packer.Insert(shape);

            if (position == null) {
                return false;
            }

            item.XOffset = position.UX;
            item.YOffset = position.UY;

            return true;
        }

        public static Stash GetStash(string filename) {
            var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
            var stash = new Stash();

            return stash.Read(pCrypto) ? stash : null;
        }

        public class DepositException : Exception {
        }

        /// <summary>
        ///     Deposit the provided items to bank page Y
        ///     The items deposited, caller responsibility to delete them from DB if stacksize is LE 0, and update if not
        /// </summary>
        public void Deposit(string filename, IList<PlayerItem> playerItems, int maxItemsToTransfer, out string error) {
            error = string.Empty;

            const int maxItemsInTab = 18 * 10;

            var stash = GetStash(filename);
            if (stash != null) {
                var depositToIndex = GetStashToDepositTo(stash);

#if DEBUG
                while (stash.Tabs.Count < 5) {
                    stash.Tabs.Add(new StashTab());
                }
#endif
                if (stash.Tabs.Count < 2) {
                    Logger.WarnFormat("File \"{0}\" only contains {1} pages, must have at least 2 pages to function properly.", filename, stash.Tabs.Count);
                    error = GlobalSettings.Language.GetTag("iatag_purchase_stash");
                }
                else if (stash.Tabs.Count < depositToIndex + 1) {
                    Logger.Warn($"You have configured IA to deposit to stash {depositToIndex + 1} but you only have {stash.Tabs.Count} pages");
                    error = GlobalSettings.Language.GetTag("iatag_invalid_deposit_stash_number", depositToIndex + 1, stash.Tabs.Count);
                }

                else if (stash.Tabs[depositToIndex].Items.Count < maxItemsInTab && playerItems.Count > 0) {
                    var tab = stash.Tabs[depositToIndex];
                    var stashItems = ConvertToStashItems(playerItems, maxItemsToTransfer, tab);

                    tab.Items.AddRange(stashItems);

                    foreach (var item in stashItems) {
                        Logger.Debug($"Depositing: {item}");
                    }

                    // Store to stash
                    if (!SafeTransferStashWriter.SafelyWriteStash(filename, stash)) {
                        Logger.Error("Could not deposit items");
                        throw new DepositException();
                    }

                    var numItemsNotDeposited = playerItems.Sum(m => m.StackCount);

                    if (numItemsNotDeposited > 0) {
                        error = GlobalSettings.Language.GetTag("iatag_stash_might_be_full", depositToIndex + 1);
                    }
                }
            }
        }

        public static PlayerItem Map(Item item, string mod, bool isHardcore) {
            return new PlayerItem {
                BaseRecord = item.BaseRecord,
                EnchantmentRecord = item.EnchantmentRecord,
                EnchantmentSeed = item.EnchantmentSeed,
                MateriaCombines = item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                RelicSeed = item.RelicSeed,
                Seed = item.Seed,
                StackCount = Math.Max(1, item.StackCount),
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                UNKNOWN = item.UNKNOWN,
                Mod = mod,
                IsHardcore = isHardcore,
                CreationDate = DateTime.UtcNow.ToTimestamp()
            };
        }

        private Item Map(PlayerItem item) {
            return new Item {
                BaseRecord = item.BaseRecord,
                EnchantmentRecord = item.EnchantmentRecord,
                EnchantmentSeed = (uint) item.EnchantmentSeed,
                MateriaCombines = (uint) item.MateriaCombines,
                MateriaRecord = item.MateriaRecord,
                ModifierRecord = item.ModifierRecord,
                PrefixRecord = item.PrefixRecord,
                RelicCompletionBonusRecord = item.RelicCompletionBonusRecord,
                RelicSeed = (uint) item.RelicSeed,
                Seed = item.USeed,
                StackCount = Math.Max(1, (uint) item.StackCount),
                SuffixRecord = item.SuffixRecord,
                TransmuteRecord = item.TransmuteRecord,
                UNKNOWN = (uint) item.UNKNOWN
            };
        }
    }
}