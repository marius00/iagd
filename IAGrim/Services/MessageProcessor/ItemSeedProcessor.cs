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

        enum ExpectedRowType {
            PlayerId,
            ItemReplicaInfo,
            Text,
        }

        public void Process(MessageType type, byte[] data, string dataString) {
            ExpectedRowType expectedType;
            switch (type) {

                case MessageType.TYPE_ITEMSEEDDATA_EQ:
                case MessageType.TYPE_ITEMSEEDDATA_REL:
                case MessageType.TYPE_ITEMSEEDDATA_BASE:
                    expectedType = ExpectedRowType.ItemReplicaInfo;
                    break;

                case MessageType.TYPE_ITEMSEEDDATA_PLAYERID:
                    expectedType = ExpectedRowType.PlayerId;
                    break;

                default:
                    return;
            }

            _logger.Info($"Experimental hook success: {type} {dataString}");


            GameItem item = null;
            var lines = dataString.Split('\n');
            foreach (var line in lines) {
                if (expectedType == ExpectedRowType.PlayerId) {
                    expectedType = ExpectedRowType.ItemReplicaInfo;
                    // TODO:
                }
                else if (expectedType == ExpectedRowType.ItemReplicaInfo) {
                    expectedType = ExpectedRowType.Text;
                    item = ToGameItem(line);
                    if (item == null) {
                        int x3 = 9;// // TODO: Log and return
                    }
                }
                else {
                    var row = Parse(line);
                    if (row != null)
                        item.Text.Add(row);
                }
            }

            if (item.Text.Count <= 1) {
                // Discard
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
