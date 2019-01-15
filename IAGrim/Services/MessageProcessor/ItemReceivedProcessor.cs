using EvilsoftCommons;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.UI.Misc;
using IAGrim.UI.Tabs;
using log4net;
using System;
using System.Collections.Generic;

namespace IAGrim.Services.MessageProcessor
{
    /// <summary>
    /// Process InventorySack::AddItem
    /// </summary>
    class ItemReceivedProcessor : IMessageProcessor {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemReceivedProcessor));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly SplitSearchWindow _searchWindow;
        private readonly StashFileMonitor _stashFileMonitor;

        public ItemReceivedProcessor(SplitSearchWindow searchWindow, StashFileMonitor stashFileMonitor, IPlayerItemDao playerItemDao) {
            _searchWindow = searchWindow;
            _stashFileMonitor = stashFileMonitor;
            _playerItemDao = playerItemDao;
        }

        public void Process(MessageType type, byte[] data) {
            switch (type) {
                // This is generally called when adding an item by clicking on the stash tab
                case MessageType.TYPE_InventorySack_AddItem: 
                    {
                        int pos = 2;
                        var items = GetPlayerItemFromInventorySack(data, pos);
                        _playerItemDao.Save(items);
                        Logger.InfoFormat("A gnome has delivered {0} new items to IA.", items.Count);
                        _searchWindow?.UpdateListViewDelayed();
                        _stashFileMonitor.CancelQueuedNotify();
                        //logger.Debug("TYPE_InventorySack_AddItem ignored");
                    }
                    break;

                // This is generally called when adding an item to a specific position in the bag
                case MessageType.TYPE_InventorySack_AddItem_Vec2: 
                    {
                        int pos = 0;
                        float x = IOHelper.GetFloat(data, 0); pos += 4;
                        float y = IOHelper.GetFloat(data, 4); pos += 4;
                        bool b = data[pos] > 0; pos++;

                        var items = GetPlayerItemFromInventorySack(data, pos);
                        _playerItemDao.Save(items);
                        Logger.InfoFormat("A helpful goblin has delivered {0} new items to IA.", items.Count);
                        _searchWindow?.UpdateListViewDelayed();
                        _stashFileMonitor.CancelQueuedNotify();
                    }
                    break;
            }
        }

        private static List<PlayerItem> GetPlayerItemFromInventorySack(byte[] data, int pos) {

            bool isHardcore = data[pos] > 0; pos++;
            string mod = IOHelper.GetPrefixString(data, pos);
            pos += 4 + IOHelper.GetInt(data, pos);

            //logger.DebugFormat("Received a InventorySack_AddItem2({0}, {1}), {2})", x, y, data[8]);
            UInt32 noClue = IOHelper.GetUInt(data, (uint)pos); pos += 4;

            UInt32 seed = IOHelper.GetUInt(data, (uint)pos); pos += 4;
            Int32 relicSeed = IOHelper.GetInt(data, pos); pos += 4;

            Int32 unknown = IOHelper.GetInt(data, pos); pos += 4;
            Int32 enchantSeed = IOHelper.GetInt(data, pos); pos += 4;

            Int32 materiaCombines = IOHelper.GetInt(data, pos); pos += 4;
            Int64 item54 = IOHelper.GetLong(data, pos); pos += 8;

            Int32 item56 = IOHelper.GetInt(data, pos); pos += 4;
            Int32 item57 = IOHelper.GetInt(data, pos); pos += 4;
            Int32 stackCount = IOHelper.GetInt(data, pos); pos += 4;
            Int32 item59 = IOHelper.GetInt(data, pos); pos += 4;

            //UInt32 ptr = IOHelper.GetUInt(data, (uint)pos); pos += 4;

            int slen04 = IOHelper.GetInt(data, pos);
            string baseRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen04;

            int slen28 = IOHelper.GetInt(data, pos);
            string prefixRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen28;

            int slen52 = IOHelper.GetInt(data, pos);
            string suffixRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen52;

            int slen80 = IOHelper.GetInt(data, pos);
            string modifierRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen80;

            int slen104 = IOHelper.GetInt(data, pos);
            string materiaRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen104;

            int slen128 = IOHelper.GetInt(data, pos);
            string relicCompletionBonusRecord = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen128;

            int slen156 = IOHelper.GetInt(data, pos);
            string transmuteOrEnchantRecord01 = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen156;

            int slen188 = IOHelper.GetInt(data, pos);
            string transmuteOrEnchantRecord02 = IOHelper.GetPrefixString(data, pos);
            pos += 4 + slen188;

            //logger.Debug($"Ptr: {ptr:X}");
            //logger.Debug($"Replica(seed:{seed}, relic:{relicSeed}, uk:{unknown}, enchant:{enchantSeed}, combines:{materiaCombines}, ?:{item54}, ?:{item56}, ?:{item57}, count:{stackCount}, ?:{item59})");
            Logger.Debug($"HC:{isHardcore}, Mod:{mod}, Replica({seed}, {relicSeed}, {materiaCombines}, {enchantSeed}, {baseRecord}, {prefixRecord}, {suffixRecord}, {modifierRecord}, {materiaRecord}, {stackCount})");

            List<PlayerItem> result = new List<PlayerItem>();
            for (int i = 0; i < Math.Max(1, stackCount); i++) {
                result.Add(new PlayerItem {
                    BaseRecord = baseRecord,
                    EnchantmentRecord = transmuteOrEnchantRecord01, // have no items with this
                    EnchantmentSeed = enchantSeed,
                    MateriaCombines = materiaCombines,
                    MateriaRecord = materiaRecord, // got items
                    ModifierRecord = modifierRecord, // got items
                    PrefixRecord = prefixRecord,
                    RelicCompletionBonusRecord = relicCompletionBonusRecord,
                    RelicSeed = relicSeed,
                    Seed = seed,
                    StackCount = 1,
                    SuffixRecord = suffixRecord,
                    TransmuteRecord = transmuteOrEnchantRecord02, // have no items with this
                    UNKNOWN = unknown,
                    Mod = mod,
                    IsHardcore = isHardcore
                });
            }

            return result;
        }
    }
}
