using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAGrim.UI.Misc;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Services.MessageProcessor {
    class ItemSeedProcessor : IMessageProcessor {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ItemSeedProcessor));
        private const char Separator = ';';


        public void Process(MessageType type, byte[] data, string dataString) {
            switch (type) {
                case MessageType.TYPE_ITEMSEEDDATA_EQ:
                case MessageType.TYPE_ITEMSEEDDATA_REL:
                case MessageType.TYPE_ITEMSEEDDATA_BASE:
                    break;

                case MessageType.TYPE_ITEMSEEDDATA_PLAYERID:
                    break;

                default:
                    return;
            }

            _logger.Info($"Experimental hook success: {type} {dataString}");

            var lines = dataString.Split('\n');
            if (lines.Length < 2) {
                _logger.Debug("Discarding replica, text too short");
                return;
            }

            int s = 1;
            long itemId;
            if (type == MessageType.TYPE_ITEMSEEDDATA_PLAYERID) {
                long.TryParse(lines[0].Trim(), out itemId);
                s++;
            }

            var item = ToGameItem(lines[s]);
            if (item == null) {
                _logger.Warn("Unable to create ItemReplica");
                return;
            }

            for (int i = s; i < lines.Length; i++) {
                var line = lines[i];
                var row = Parse(line);
                if (row != null)
                    item.Text.Add(row);
            }

            if (item.Text.Count <= 1) {
                _logger.Debug("Only got a single text row, discarding replica");
                return;
            }

            int x = 9;
            // TODO: DO something with the item..
        }

        class GameItem {
            public string BaseRecord { get; set; } = "";
            public string PrefixRecord { get; set; } = "";
            public string SuffixRecord { get; set; } = "";
            public string ModifierRecord { get; set; } = "";
            public string TransmuteRecord { get; set; } = "";
            public uint Seed { get; set; } = 0u;
            public string MateriaRecord { get; set; } = "";
            public string RelicCompletionBonusRecord { get; set; } = "";
            public uint RelicSeed { get; set; } = 0u;
            public string EnchantmentRecord { get; set; } = "";
            public uint EnchantmentSeed { get; set; } = 0u;
            public List<ItemStatInfo> Text { get; set; }
        }
        
        private GameItem ToGameItem(string line) {
            var pieces = line.Split(Separator);

            if (pieces.Length != 11) {
                _logger.Warn($"Expected 11 columns in row, got {pieces.Length}: {line}");
                return null;
            }

            return new GameItem {
                BaseRecord = pieces[0],
                PrefixRecord = pieces[1],
                SuffixRecord = pieces[2],
                Seed = ToInt(pieces[3]),
                ModifierRecord = pieces[4],
                MateriaRecord = pieces[5],
                RelicCompletionBonusRecord = pieces[6],
                RelicSeed = ToInt(pieces[7]),
                EnchantmentRecord = pieces[8],
                EnchantmentSeed = ToInt(pieces[9]),
                TransmuteRecord = pieces[10],
                Text = new List<ItemStatInfo>()
            };
        }

        private uint ToInt(string s) {
            uint.TryParse(s, out var i);
            return i;
        }

        private ItemStatInfo Parse(string line) {
            var idx = line.IndexOf(Separator);
            if (idx < 0)
                return null;

            var text = line.Substring(idx + 1);
            var typeText = line.Substring(0, idx);

            if (!int.TryParse(typeText, out var type))
                return null;

            return new ItemStatInfo {
                Type = type,
                Text = text,
            };
        }

        class ItemStatInfo {
            public int Type { get; set; }
            public string Text { get; set; }
        }
    }
}
