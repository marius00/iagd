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
            if (type != MessageType.TYPE_ITEMSEEDDATA)
                return;

            _logger.Info($"Experimental hook success: {dataString}");

            GameItem item = null;
            bool first = true;
            var lines = dataString.Split('\n');
            foreach (var line in lines) {
                _logger.Debug(line);
            }
            foreach (var line in lines) {
                if (first) {
                    first = false;
                    item = ToGameItem(line);
                }
                else {
                    var row = Parse(line);
                    if (row != null)
                        item.Text.Add(row);
                }
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

            if (pieces.Length != 10) {
                // TODO: Just throw? What?
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
            uint i;
            uint.TryParse(s, out i);
            return i;
        }

        private ItemStatInfo Parse(string line) {
            var idx = line.IndexOf(Separator);
            if (idx < 0)
                return null;

            var text = line.Substring(idx + 1);
            var typeText = line.Substring(0, idx);
            int type;

            if (!int.TryParse(typeText, out type))
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
