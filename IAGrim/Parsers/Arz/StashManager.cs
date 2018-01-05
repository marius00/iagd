using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Properties;
using IAGrim.Services;
using IAGrim.StashFile;
using IAGrim.UI;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using Timer = System.Timers.Timer;

namespace IAGrim.Parsers.Arz {
    internal class StashManager {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StashManager));
        private Action<string> _setFeedback;
        private Action _performedLootCallback;
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ItemSizeService _itemSizeService;
        public List<Item> UnlootedItems => _unlootedItems.ToList();
        private ConcurrentBag<Item> _unlootedItems = new ConcurrentBag<Item>();
        public readonly int NumStashTabs;
        //private bool _hasLootedItemsOnceThisSession = false;

        public event EventHandler StashUpdated;

        public StashManager(IPlayerItemDao playerItemDao, IDatabaseItemStatDao dbItemStatDao) {
            _playerItemDao = playerItemDao;
            _itemSizeService = new ItemSizeService(dbItemStatDao);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
                "Grim Dawn", "Save", "transfer.gst");

            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                UpdateUnlooted(path);

                GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(path));
                Stash stash = new Stash();
                if (stash.Read(pCrypto)) {
                    NumStashTabs = stash.Tabs.Count;
                }
            }
        }


        /// <summary>
        ///     Used to prevent feedback spam, potentially overriding more important new information from other sources
        /// </summary>
        private bool _hasRecentlyUpdatedTimerFeedback;

        private static ulong TranslatePosition(int x, int y) {
            return 0;
        }

        #region TimerStuff

        private Timer _delayedLootTimer;
        private string _filenameTimed;

        private void LootStashDelayed(string filename) {
            _filenameTimed = filename;
            _delayedLootTimer?.Stop();

            _delayedLootTimer = new Timer {Enabled = true, Interval = 1500, AutoReset = true};
            //_delayedLootTimer.Tag = filename;
            _delayedLootTimer.Elapsed += HandleDelayedTextChangedTimerTick;
            _delayedLootTimer.Start();
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e) {
            bool isValid =
                (GlobalSettings.StashStatus == StashAvailability.CLOSED && GrimStateTracker.IsFarFromStash) ||
                !(bool) Settings.Default.SecureTransfers;

            if (GlobalSettings.PreviousStashStatus == StashAvailability.CRAFTING) {
                _delayedLootTimer?.Stop();
                _delayedLootTimer = null;
                Logger.Debug("Stash is now available for looting, but the previous state was crafting, no new items should have been added");

            }
            else if (!isValid) {
                if (!GrimStateTracker.IsFarFromStash) {
                    if (!_hasRecentlyUpdatedTimerFeedback) {
                        Logger.Info(
                            "Delaying stash loot, too close to stash. (this is to prevent item dupes on quick re-open)");
                        _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_too_close_to_stash"));
                    }
                    _hasRecentlyUpdatedTimerFeedback = true;
                }
                else {
                    if (!_hasRecentlyUpdatedTimerFeedback) {
                        _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_delaying_stash_loot_status"));
                        Logger.InfoFormat("Delaying stash loot, stash status {0}.", GlobalSettings.StashStatus);
                    }
                    _hasRecentlyUpdatedTimerFeedback = true;
                }
            }

            else if (MainWindow.Instance.InvokeRequired) {
                MainWindow.Instance.Invoke((MethodInvoker) delegate { HandleDelayedTextChangedTimerTick(sender, e); });
            }

            else {
                _hasRecentlyUpdatedTimerFeedback = false;

                _delayedLootTimer?.Stop();
                _delayedLootTimer = null;

                Logger.InfoFormat("Looting stash, IsStashOpen: {0}, IsFarFromStash: {1}",
                    GlobalSettings.StashStatus != StashAvailability.CLOSED, GrimStateTracker.IsFarFromStash);


                try {
                    string message = EmptyPageX(_filenameTimed);
                    _setFeedback(message);
                }
                catch (NullReferenceException ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_unable_to_loot_stash4"));
                }
                catch (IOException ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    Logger.Info("Exception not reported, IOExceptions are bound to happen.");
                    _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_unable_to_loot_stash4"));
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionReporter.ReportException(ex, "EmptyPageX??");
                    _setFeedback(GlobalSettings.Language.GetTag("iatag_feedback_unable_to_loot_stash4"));
                }
            }
        }

        #endregion TimerStuff


        public void CancelQueuedLoot() {
            _delayedLootTimer?.Stop();
            _delayedLootTimer = null;
        }

        public void UpdateUnlooted(string filename) {
            GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

            Stash stash = new Stash();
            if (stash.Read(pCrypto)) {
                // Update the internal listing of unlooted items (in stash tabs)
                List<Item> unlootedLocal = new List<Item>();
                foreach (StashTab tab in stash.Tabs) {
                    unlootedLocal.AddRange(tab.Items);
                }
                Interlocked.Exchange(ref _unlootedItems, new ConcurrentBag<Item>(unlootedLocal));
            }
        }

        void DeleteItemsInPageX(string filename) {
            
            GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

            Stash stash = new Stash();
            if (stash.Read(pCrypto)) {
                int lootFromIndex;
                if (Settings.Default.StashToLootFrom == 0) {
                    lootFromIndex = stash.Tabs.Count - 1;
                }
                else {
                    lootFromIndex = Settings.Default.StashToLootFrom - 1;
                }

                Logger.Debug($"Deleting all items in stash #{lootFromIndex}");
                if (stash.Tabs.Count >= lootFromIndex + 1) {
                    if (stash.Tabs[lootFromIndex].Items.Count > 0) {
                        stash.Tabs[lootFromIndex].Items.Clear();
                        SafelyWriteStash(filename, stash);
                    }
                }
            }
        }

        public static int GetNumStashPages(string filename) {
            if (File.Exists(filename)) {
                GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

                Stash stash = new Stash();
                if (stash.Read(pCrypto)) {
                    return stash.Tabs.Count;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Loot all the items stored on page X, and store them to the local database
        /// </summary>
        /// <param name="filename"></param>
        private string EmptyPageX(string filename) {
            Logger.InfoFormat("Looting {0}", filename);

            GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

            Stash stash = new Stash();
            if (stash.Read(pCrypto)) {
                int lootFromIndex;
                if (Settings.Default.StashToLootFrom == 0) {
                    lootFromIndex = stash.Tabs.Count - 1;
                }
                else {
                    lootFromIndex = Settings.Default.StashToLootFrom - 1;
                }

                bool isHardcore = GlobalPaths.IsHardcore(filename);

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
                List<Item> unlootedLocal = new List<Item>();
                for (var idx = 0; idx < stash.Tabs.Count; idx++) {
                    if (idx != lootFromIndex) {
                        unlootedLocal.AddRange(stash.Tabs[idx].Items);
                    }
                }
                Interlocked.Exchange(ref _unlootedItems, new ConcurrentBag<Item>(unlootedLocal));
                StashUpdated?.Invoke(null, null);

                if (stash.Tabs.Count < 2) {
                    Logger.WarnFormat(
                        "File \"{0}\" only contains {1} pages, must have at least 2 pages to function properly.",
                        filename, stash.Tabs.Count);
                    return
                        $"File \"{filename}\" only contains {stash.Tabs.Count} pages, must have at least 2 pages to function properly.";
                }
                if (stash.Tabs.Count < lootFromIndex + 1) {
                    var message =
                        $"You have configured IA to loot from {lootFromIndex + 1} but you only have {stash.Tabs.Count} pages";
                    Logger.Warn(message);
                    return message;
                }

                if (stash.Tabs[lootFromIndex].Items.Count > 0) {
                    //_hasLootedItemsOnceThisSession = true;

                    // Grab the items and clear the tab
                    List<Item> items = new List<Item>(stash.Tabs[lootFromIndex].Items);
                    stash.Tabs[lootFromIndex].Items.Clear();

                    List<PlayerItem> storedItems =
                        StoreItemsToDatabase(items, stash.ModLabel, isHardcore, stash.IsExpansion1);
                    if (storedItems != null) {
                        Logger.Info($"Looted {items.Count} items from stash {lootFromIndex + 1}");

                        if (!SafelyWriteStash(filename, stash)) {
                            // TODO: Delete from DB
                            _playerItemDao.Remove(storedItems);
                        }
                    }

                    _performedLootCallback();
                    return $"Looted {items.Count} items from stash {lootFromIndex + 1}";
                }
                Logger.Info($"Looting of stash {lootFromIndex + 1} halted, no items available.");
                return GlobalSettings.Language.GetTag("iatag_feedback_no_items_to_loot");
            }
            Logger.Error("Could not load stash file.");
            Logger.Error("An update from the developer is most likely required.");

            return string.Empty;
        }

        

        public List<PlayerItem> EmptyStash(string filename) {
            Logger.InfoFormat("Looting {0}", filename);

            string tempname = string.Format("{0}.ia", filename);
            GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

            List<Item> items = new List<Item>();
            Stash stash = new Stash();
            if (stash.Read(pCrypto)) {
                bool isHardcore = GlobalPaths.IsHardcore(filename);

                foreach (var tab in stash.Tabs) {
                    // Grab the items and clear the tab
                    items.AddRange(tab.Items);
                    tab.Items.Clear();
                }
                SafelyWriteStash(filename, stash);
                Logger.InfoFormat("Looted {0} items from stash", items.Count);

                return items.Select(m => Map(m, stash.ModLabel, isHardcore, stash.IsExpansion1)).ToList();
            }
            Logger.Error("Could not load stash file.");
            Logger.Error("An update from the developer is most likely required.");


            return new List<PlayerItem>();
        }

        /// <summary>
        ///     Write the GD Stash file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stash"></param>
        /// <returns></returns>
        private static bool SafelyWriteStash(string filename, Stash stash) {
            try {
                string tempname = string.Format("{0}.ia", filename);

                // Store the stash file in a temporary location
                DataBuffer dbuffer = new DataBuffer();
                stash.Write(dbuffer);
                DataBuffer.WriteBytesToDisk(tempname, dbuffer.Data);

                // Get the current backup number
                int BackupNumber = (int) Settings.Default.BackupNumber;
                Settings.Default.BackupNumber = (BackupNumber + 1) % 100;
                Settings.Default.Save();

                // Back up the existing stash and replace with new stash file
                var backupLocation = Path.Combine(GlobalPaths.BackupLocation,
                    string.Format("transfer.{0}.gs_", BackupNumber.ToString("00")));
                File.Copy(filename, backupLocation, true);
                File.Copy(tempname, filename, true);

                // Delete the temporary file
                if (File.Exists(tempname))
                    File.Delete(tempname);

                return true;
            }
            catch (UnauthorizedAccessException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "SafelyWriteDatabase");
                return false;
            }
        }

        /// <summary>
        ///     Attempt to get the name of the current mod
        ///     Vanilla leaves this tag empty
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetModLabel(string filename, out string result) {
            if (File.Exists(filename)) {
                GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));
                Stash stash = new Stash();

                if (stash.Read(pCrypto)) {
                    result = stash.ModLabel;
                    return true;
                }
            }

            result = string.Empty;
            return false;
        }


        public Item GetItem(PlayerItem item, int itemsRemaining) {
            Item stashItem = Map(item);

            // Calculate the final stack count (stored stack may be larger than max transfer size)
            if (StashTab.CanStack(item.Slot) || StashTab.HardcodedRecords.Contains(item.BaseRecord)) {
                var maxStack = Math.Min(itemsRemaining, 100);
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
            List<Item> result = new List<Item>();
            Packer packer = new Packer(tab.Height, tab.Width);
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
            for (int i = 0; i < playerItems.Count; i++) {
                if (playerItems[i].StackCount == 0)
                    playerItems[i].StackCount = 1;
                while (playerItems[i].StackCount > 0 && itemsRemaining > 0) {
                    Item stashItem = GetItem(playerItems[i], itemsRemaining);

                    // Map item size and create a shape map
                    if (!PositionItem(packer, stashItem)) {
                        Logger.Info("Could not fit all items in stash, stopping early");
                        playerItems[i].StackCount += stashItem.StackCount;
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
            GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(filename));

            Stash stash = new Stash();
            if (stash.Read(pCrypto)) {
                return stash;
            }
            return null;
        }


        /// <summary>
        ///     Deposit the provided items to bank page Y
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="playerItems">
        ///     The items deposited, caller responsibility to delete them from DB if stacksize is <= 0, and update if not</param>
        /// <returns></returns>
        public void Deposit(string filename, IList<PlayerItem> playerItems, int maxItemsToTransfer, out string error) {
            string tempname = string.Format("{0}.ia", filename);
            error = string.Empty;

            const int MAX_ITEMS_IN_TAB = 16 * 8;

            Stash stash = GetStash(filename);
            if (stash != null) {
                int depositToIndex;
                if (Settings.Default.StashToDepositTo == 0) {
                    depositToIndex = stash.Tabs.Count - 2;
                }
                else {
                    depositToIndex = Settings.Default.StashToDepositTo - 1;
                }


#if DEBUG
                while (stash.Tabs.Count < 5) {
                    stash.Tabs.Add(new StashTab());
                }
#endif
                if (stash.Tabs.Count < 2) {
                    Logger.WarnFormat(
                        "File \"{0}\" only contains {1} pages, must have at least 2 pages to function properly.",
                        filename, stash.Tabs.Count);
                    error = "Please purchase more stash pages!";
                }
                else if (stash.Tabs.Count < depositToIndex + 1) {
                    var message =
                        $"You have configured IA to deposit to tab {depositToIndex + 1} but you only have {stash.Tabs.Count} pages";
                    Logger.Warn(message);
                    error = message;
                }

                else if (stash.Tabs[depositToIndex].Items.Count < MAX_ITEMS_IN_TAB && playerItems.Count > 0) {
                    var tab = stash.Tabs[depositToIndex];

                    List<Item> stashItems = ConvertToStashItems(playerItems, maxItemsToTransfer, tab);
                    tab.Items.AddRange(stashItems);

                    foreach (var item in stashItems) {
                        Logger.Debug($"Depositing: {item}");
                    }

                    // Store to stash
                    SafelyWriteStash(filename, stash);

                    var numItemsNotDeposited = playerItems.Sum(m => m.StackCount);
                    if (numItemsNotDeposited > 0) {
                        error = $"Some items not deposited, stash {depositToIndex + 1} may be full.";
                    }
                }
            }
        }


        private PlayerItem Map(Item item, string mod, bool isHardcore, bool isExpansion1) {
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
                //IsExpansion1 = isExpansion1
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

        /// <summary>
        ///     Store all the items
        ///     ----UNTRUE?: If an item cannot be stored (missing record), it is returned so it can be re-written to stash.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<PlayerItem> StoreItemsToDatabase(ICollection<Item> items, string mod, bool isHardcore,
            bool isExpansion1) {
            List<PlayerItem> playerItems = new List<PlayerItem>();

            foreach (Item item in items) {
                PlayerItem newItem = Map(item, mod, isHardcore, isExpansion1);
                playerItems.Add(newItem);
            }

            try {
                _playerItemDao.Save(playerItems);
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "StoreItems");
                return null;
            }

            return playerItems;
        }

        /// <summary>
        ///     Ugly, but it has to be disposed!
        /// </summary>
        public void Dispose() {
            if (_watcher != null) {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        ~StashManager() {
            Dispose();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool StartMonitorStashfile(Action<string> feedback, Action performedLootCallback) {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
                "Grim Dawn", "Save");
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path)) {
                _setFeedback = feedback;
                _performedLootCallback = performedLootCallback;

                _watcher = new FileSystemWatcher();
                _watcher.Path = path;
                _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                        NotifyFilters.DirectoryName;
                _watcher.Filter = "transfer.gs?";

                _watcher.IncludeSubdirectories = true;
                _watcher.Changed += (sender, args) => OnChanged(this, args); //new FileSystemEventHandler(OnChanged);
                _watcher.Created += (sender, args) => OnChanged(this, args); //new FileSystemEventHandler(OnChanged);
                _watcher.Deleted += (sender, args) => OnChanged(this, args); //new FileSystemEventHandler(OnChanged);
                _watcher.Renamed += OnRenamed;


                _watcher.Error += Watcher_Error;
                _watcher.EnableRaisingEvents = true;

                Logger.InfoFormat("Monitoring stashfiles at: {0}", _watcher.Path);
                return true;
            }

            return false;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e) {
            Logger.Info(e.GetException().Message);
        }

        public string LastSeenModLabel { get; private set; }

        public bool LastSeenIsHardcore { get; private set; }

        //private static void OnChanged(object source, FileSystemEventArgs e) {
        private static void OnChanged(StashManager obj, FileSystemEventArgs e) {
            Logger.Debug("File: " + e.FullPath + " " + e.ChangeType);

            /*
                        // Specify what is done when a file is changed, created, or deleted.
                        if (e.FullPath.EndsWith(".gst") || e.FullPath.EndsWith(".gsh")) {
                            Logger.Debug("File: " + e.FullPath + " " + e.ChangeType);
                            using (TemporaryCopy copy = new TemporaryCopy(e.FullPath)) {
                                obj.LastSeenIsHardcore = GlobalSettings.IsHardcore(copy.Filename);

                                GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(copy.Filename));

                                Stash stash = new Stash();
                                if (stash.Read(pCrypto)) {
                                    obj.LastSeenModLabel = stash.ModLabel;
                                }
                            }
                        }
            */

            // Logger.InfoFormat("Detected a change");
        }

        private void OnRenamed(object source, RenamedEventArgs e) {
            // Specify what is done when a file is renamed.
            Logger.DebugFormat("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            if (!e.FullPath.EndsWith(".bak")) {
                if (File.Exists(e.FullPath)) {
                    /*
                    if (GlobalSettings.PreviousStashStatus == StashAvailability.CRAFTING ||
                        GlobalSettings.StashStatus == StashAvailability.CRAFTING) {
                        Logger.Info("Detected an update to stash file, but ignoring due to crafting-safety-check");
                        // OBS: Can only do this if we've previously looted! CAnnot risk it containing unlooted items
                        if (_hasLootedItemsOnceThisSession) {
                            if (_delayedLootTimer == null) {
                                Logger.Info(
                                    "Items has already been looted this session, post-crafting safety measures required.");

                                DeleteItemsInPageX(e.FullPath);
                            }
                            else {
                                Logger.Info("Player may have opened devotion screen before running away.. leaving items be..");
                            }
                        }
                        else {
                            Logger.Info("No items has been looted this session, ignoring safety measures.");
                        }
                    }
                    else*/ {
                        Logger.Info("Detected an update to stash file, checking for loot..");

                        LootStashDelayed(e.FullPath);
                    }
                }
                else {
                    Logger.Warn("Detected an update to stash file, but stash file does not appear to exist.");
                }
            }
        }
    }
}