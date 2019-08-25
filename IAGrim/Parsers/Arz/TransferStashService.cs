﻿using EvilsoftCommons;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.StashFile;
using IAGrim.Utilities;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IAGrim.Parser.Stash;
using IAGrim.Parsers.TransferStash;
using IAGrim.Settings;

namespace IAGrim.Parsers.Arz {
    internal class TransferStashService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransferStashService));

        private readonly ItemSizeService _itemSizeService;
        public readonly int NumStashTabs;
        private readonly SettingsService _settings;
        private readonly SafeTransferStashWriter _stashWriter;

        public TransferStashService(IDatabaseItemStatDao dbItemStatDao, SettingsService settings, SafeTransferStashWriter stashWriter) {
            _settings = settings;
            _stashWriter = stashWriter;
            _itemSizeService = new ItemSizeService(dbItemStatDao);

            // TODO: Should also check for transfer.gsh and pick whichever is newest / has the highest number
            var path = Path.Combine(GlobalPaths.SavePath, "transfer.gst");

            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                var pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(path));
                var stash = new Stash();

                if (stash.Read(pCrypto)) {
                    NumStashTabs = stash.Tabs.Count;
                }
            }
        }

        public int GetStashToLootFrom(Stash stash) {
            if (_settings.GetLocal().StashToLootFrom == 0) {

                return stash.Tabs.Count - 1;
            }

            return (int)_settings.GetLocal().StashToLootFrom - 1;
        }

        private int GetStashToDepositTo(Stash stash) {
            if (_settings.GetLocal().StashToDepositTo == 0) {
                return stash.Tabs.Count - 2;
            }
            
            return (int)_settings.GetLocal().StashToDepositTo - 1;
        }

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

                _stashWriter.SafelyWriteStash(filename, stash); // TODO: Ideally we should check if it worked. 
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
                    error = RuntimeSettings.Language.GetTag("iatag_purchase_stash");
                }
                else if (stash.Tabs.Count < depositToIndex + 1) {
                    Logger.Warn($"You have configured IA to deposit to stash {depositToIndex + 1} but you only have {stash.Tabs.Count} pages");
                    error = RuntimeSettings.Language.GetTag("iatag_invalid_deposit_stash_number", depositToIndex + 1, stash.Tabs.Count);
                }

                else if (stash.Tabs[depositToIndex].Items.Count < maxItemsInTab && playerItems.Count > 0) {
                    var tab = stash.Tabs[depositToIndex];
                    var stashItems = ConvertToStashItems(playerItems, maxItemsToTransfer, tab);

                    tab.Items.AddRange(stashItems);

                    foreach (var item in stashItems) {
                        Logger.Debug($"Depositing: {item}");
                    }

                    // Store to stash
                    if (!_stashWriter.SafelyWriteStash(filename, stash)) {
                        Logger.Error("Could not deposit items");
                        throw new DepositException();
                    }

                    var numItemsNotDeposited = playerItems.Sum(m => m.StackCount);

                    if (numItemsNotDeposited > 0) {
                        error = RuntimeSettings.Language.GetTag("iatag_stash_might_be_full", depositToIndex + 1);
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
                CreationDate = DateTime.UtcNow.ToTimestamp(),
                AzureUuid = Guid.NewGuid().ToString() 
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