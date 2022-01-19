using System.Collections.Generic;
using System.ServiceModel.Channels;
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
        private readonly IBuddyReplicaItemDao _buddyReplicaItemDao;

        public ItemReplicaProcessor(IReplicaItemDao replicaItemDao, IBuddyReplicaItemDao buddyReplicaItemDao) {
            this._replicaItemDao = replicaItemDao;
            _buddyReplicaItemDao = buddyReplicaItemDao;
        }

        private bool IsResponsible(MessageType type) {
            switch (type) {
                case MessageType.TYPE_ITEMSEEDDATA_EQ:
                case MessageType.TYPE_ITEMSEEDDATA_REL:
                case MessageType.TYPE_ITEMSEEDDATA_BASE:
                case MessageType.TYPE_ITEMSEEDDATA_PLAYERID:
                    return true;

                default:
                    return false;
            }
        }


        private List<ItemStatInfo> ParseStats(string[] lines, int s) {
            var text = new List<ItemStatInfo>();
            for (int i = s; i < lines.Length; i++) {
                var line = lines[i];
                var row = Parse(line);
                if (row != null)
                    text.Add(row);
            }

            return text;
        }

        public void Process(MessageType type, byte[] data, string dataString) {
            if (!IsResponsible(type))
                return;

#if DEBUG
            _logger.Info($"Experimental hook success: {type} {dataString.Replace('\n','N')}");
#endif

            var lines = dataString.Split('\n');
            if (lines.Length < 3) {
                _logger.Debug("Discarding replica, text too short");
                return;
            }

            int s = 0;
            long? playerItemId = null;
            string buddyItemId = string.Empty;
            if (type == MessageType.TYPE_ITEMSEEDDATA_PLAYERID) {
                // Player item Id
                if (long.TryParse(lines[s].Trim(), out var itemId))
                    playerItemId = itemId;

                s++;

                // Buddy item id
                buddyItemId = lines[s++];
            }

            // TODO: Cleanup and dedup this
            if (string.IsNullOrEmpty(buddyItemId)) {
                var item = ToGameItem(lines[s], playerItemId);
                s++;
                if (item == null) {
                    _logger.Warn("Unable to create ItemReplica");
                    return;
                }

                var text = ParseStats(lines, s);
                if (text.Count <= 1) {
                    _logger.Debug("Only got a single text row, discarding replica");
                    return;
                }

                item.Text = JsonConvert.SerializeObject(text);
                item.UqHash = GetHash(item);

                _replicaItemDao.Save(item);
            }
            else {
                // Buddy item
                var item = new BuddyReplicaItem {
                    BuddyItemId = buddyItemId
                };
                
                s++;

                var text = ParseStats(lines, s);
                if (text.Count <= 1) {
                    _logger.Debug("Only got a single text row, discarding replica");
                    return;
                }

                item.Text = JsonConvert.SerializeObject(text);
                _buddyReplicaItemDao.Save(item);
            }
        }


        public static int GetHash(ReplicaItem item) {
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

        private static uint ToInt(string s) {
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
