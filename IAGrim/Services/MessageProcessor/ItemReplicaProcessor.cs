using System.Collections.Generic;
using System.Text;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Services.ItemReplica;
using IAGrim.UI.Misc;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Services.MessageProcessor {
    class ItemReplicaProcessor : IMessageProcessor {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ItemReplicaProcessor));
        private const char Separator = ';';
        private readonly IReplicaItemDao _replicaItemDao;

        public ItemReplicaProcessor(IReplicaItemDao replicaItemDao) {
            this._replicaItemDao = replicaItemDao;
        }


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

#if DEBUG
            _logger.Info($"Experimental hook success: {type} {dataString.Replace('\n','N')}");
#endif

            var lines = dataString.Split('\n');
            if (lines.Length < 2) {
                _logger.Debug("Discarding replica, text too short");
                return;
            }

            int s = 0;
            long? playerItemId = null;
            if (type == MessageType.TYPE_ITEMSEEDDATA_PLAYERID) {
                if (long.TryParse(lines[0].Trim(), out var itemId))
                    playerItemId = itemId;

                s++;
            }

            var item = ToGameItem(lines[s], playerItemId);
            s++;
            if (item == null) {
                _logger.Warn("Unable to create ItemReplica");
                return;
            }

            var text = new List<ItemStatInfo>();
            for (int i = s; i < lines.Length; i++) {
                var line = lines[i];
                var row = Parse(line);
                if (row != null)
                    text.Add(row);
            }

            if (text.Count <= 1) {
                _logger.Debug("Only got a single text row, discarding replica");
                return;
            }

            item.Text = JsonConvert.SerializeObject(text);
            item.UqHash = GetHash(item);

#if DEBUG
            if (!dataString.Contains("^")) {
                _logger.Warn("This might be the drones you are looking for...");
            }
#endif

            _replicaItemDao.Save(item);
        }

        /// <summary>
        /// Create a ReplicaItem hash from a PlayerItem instance
        /// TODO: Move somewhere more fitting?
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static int GetHash(PlayerItem pi) {
            ReplicaItem replica =  new ReplicaItem {
                BaseRecord = pi.BaseRecord,
                EnchantmentRecord = pi.EnchantmentRecord,
                EnchantmentSeed = (uint)pi.EnchantmentSeed,
                MateriaRecord = pi.MateriaRecord,
                ModifierRecord = pi.ModifierRecord,
                PrefixRecord = pi.PrefixRecord,
                RelicCompletionBonusRecord = pi.RelicCompletionBonusRecord,
                RelicSeed = (uint)pi.RelicSeed,
                Seed = pi.USeed,
                SuffixRecord = pi.SuffixRecord,
                TransmuteRecord = pi.TransmuteRecord,
            };

            return GetHash(replica);
        }

        private static int GetHash(ReplicaItem item) {
            StringBuilder sb = new StringBuilder();
            sb.Append(item.BaseRecord);
            sb.Append(item.PrefixRecord);
            sb.Append(item.SuffixRecord);
            sb.Append(item.Seed);
            sb.Append(item.ModifierRecord);
            sb.Append(item.MateriaRecord);
            sb.Append(item.RelicCompletionBonusRecord);
            sb.Append(item.RelicSeed);
            sb.Append(item.EnchantmentRecord);
            sb.Append(item.EnchantmentSeed);
            sb.Append(item.TransmuteRecord);

            return sb.ToString().GetHashCode(); // WARN: This will fail with .Net 5, as it becomes unique-per-run
        }
        
        private ReplicaItem ToGameItem(string line, long? playerItemId) {
            var pieces = line.Split(Separator);

            if (pieces.Length != 11) {
                _logger.Warn($"Expected 11 columns in row, got {pieces.Length}: {line}");
                return null;
            }

            return new ReplicaItem {
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
                PlayerItemId = playerItemId
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


    }
}
