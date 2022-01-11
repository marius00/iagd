using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Services.ItemReplica;
using IAGrim.Services.MessageProcessor;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    // TODO: Class does too much, and is somewhat of a mess.
    class ItemReplicaService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemReplicaService));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IBuddyItemDao _buddyItemDao;

        private readonly ItemReplicaDispatchService _dispatchService;
        private volatile bool _isShuttingDown = false;
        private Thread _t = null;
        private readonly ActionCooldown _cooldown = new ActionCooldown(2500);
        private readonly ReplicaCache _cache = new ReplicaCache();

        public ItemReplicaService(IPlayerItemDao playerItemDao, IBuddyItemDao buddyItemDao) {
            _playerItemDao = playerItemDao;
            _buddyItemDao = buddyItemDao;
            _dispatchService = new ItemReplicaDispatchService();
        }

        public void SetIsGrimDawnRunning(bool b) {
            _dispatchService.SetIsGrimDawnRunning(b);
        }

        public void Start() {
            if (_t != null)
                throw new ArgumentException("Max one thread running per instance");

            _t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                while (!_isShuttingDown) {
                    if (_cooldown.IsReady) {
                        Process();
                        _cooldown.Reset();
                    }
                    
                    Thread.Sleep(1);
                }
            });

            _t.Start();
        }

        private bool Process() {
            if (!_dispatchService.GetIsGrimDawnRunning)
                return false;

            int count = 0;

            {
                var items = _playerItemDao.ListMissingReplica(300);
                count += items.Count;
                foreach (var item in items) {
                    var hash = GetHash(item);
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;


                    List<byte> buffer = Serialize(item);
                    if (!_dispatchService.DispatchItemSeedInfoRequest(buffer))
                        return false; //

                    _cache.Add(hash);
                    Thread.Sleep(1);
                }
            }

            {
                var items = _buddyItemDao.ListMissingReplica(300);
                count += items.Count;

                foreach (var item in items) {
                    var hash = GetHash(item);
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;


                    List<byte> buffer = Serialize(item);
                    if (!_dispatchService.DispatchItemSeedInfoRequest(buffer))
                        return false;

                    _cache.Add(hash);
                    Thread.Sleep(1);
                }
            }

            return count > 0;
        }


        /// <summary>
        /// Create a ReplicaItem hash from a PlayerItem instance
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static int GetHash(PlayerItem pi) {
            ReplicaItem replica = new ReplicaItem {
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

            return ItemReplicaProcessor.GetHash(replica);
        }


        /// <summary>
        /// Create a ReplicaItem hash from a BuddyItem instance
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static int GetHash(BuddyItem pi) {
            ReplicaItem replica = new ReplicaItem {
                BaseRecord = pi.BaseRecord,
                EnchantmentRecord = pi.EnchantmentRecord,
                EnchantmentSeed = (uint)pi.EnchantmentSeed,
                MateriaRecord = pi.MateriaRecord,
                ModifierRecord = pi.ModifierRecord,
                PrefixRecord = pi.PrefixRecord,
                RelicCompletionBonusRecord = string.Empty,
                RelicSeed = (uint)pi.RelicSeed,
                Seed = (uint)pi.Seed,
                SuffixRecord = pi.SuffixRecord,
                TransmuteRecord = pi.TransmuteRecord,
            };

            return ItemReplicaProcessor.GetHash(replica);
        }

        private static List<byte> Serialize(BuddyItem bi) {
            List<byte> buffer = new List<byte>();

            if (bi.BaseRecord.Length > 255 || bi.SuffixRecord?.Length > 255 || bi.PrefixRecord?.Length > 255
                || bi.MateriaRecord?.Length > 255 || bi.ModifierRecord?.Length > 255 || bi.EnchantmentRecord?.Length > 255
                || bi.TransmuteRecord?.Length > 255) {
                Logger.Warn("Received a buddy item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

            int type = 2; // BuddyItem
            buffer.AddRange(BitConverter.GetBytes(type));
            buffer.AddRange(BitConverter.GetBytes(bi.RemoteItemId.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(bi.RemoteItemId));

            buffer.AddRange(BitConverter.GetBytes((int)bi.Seed));
            buffer.AddRange(BitConverter.GetBytes((int)bi.RelicSeed));
            buffer.AddRange(BitConverter.GetBytes((int)bi.EnchantmentSeed));

            buffer.AddRange(BitConverter.GetBytes(bi.BaseRecord.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(bi.BaseRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.PrefixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.PrefixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.PrefixRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.SuffixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.SuffixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.SuffixRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.ModifierRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.ModifierRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.ModifierRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.MateriaRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.MateriaRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.MateriaRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.EnchantmentRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.EnchantmentRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.EnchantmentRecord));

            buffer.AddRange(BitConverter.GetBytes(bi.TransmuteRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(bi.TransmuteRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(bi.TransmuteRecord));

            return buffer;
        }

        private static List<byte> Serialize(PlayerItem pi) {
            List<byte> buffer = new List<byte>();

            if (pi.BaseRecord?.Length > 255 || pi.SuffixRecord?.Length > 255 || pi.PrefixRecord?.Length > 255
                || pi.MateriaRecord?.Length > 255 || pi.ModifierRecord?.Length > 255 || pi.EnchantmentRecord?.Length > 255
                || pi.TransmuteRecord?.Length > 255) {
                Logger.Warn("Received a player item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

#if DEBUG
            StringBuilder sb = new StringBuilder();
            sb.Append(pi.Id + ";");
            sb.Append(pi.Seed + ";");
            sb.Append(pi.RelicSeed + ";");
            sb.Append(pi.EnchantmentSeed + ";");
            sb.Append(pi.BaseRecord + ";");
            sb.Append(pi.PrefixRecord + ";");
            sb.Append(pi.SuffixRecord + ";");
            sb.Append(pi.ModifierRecord + ";");
            sb.Append(pi.MateriaRecord + ";");
            sb.Append(pi.EnchantmentRecord + ";");
            sb.Append(pi.TransmuteRecord + ";");
            Logger.Debug($"Dispatching: {sb.ToString()}");
#endif

            int type = 1; // PlayerItem
            buffer.AddRange(BitConverter.GetBytes(type));
            buffer.AddRange(BitConverter.GetBytes((long)pi.Id));
            buffer.AddRange(BitConverter.GetBytes((int)pi.Seed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.RelicSeed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.EnchantmentSeed));

            buffer.AddRange(BitConverter.GetBytes(pi.BaseRecord.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(pi.BaseRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.PrefixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.PrefixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.PrefixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.SuffixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.SuffixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.SuffixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.ModifierRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.ModifierRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.ModifierRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.MateriaRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.MateriaRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.MateriaRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.EnchantmentRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.EnchantmentRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.EnchantmentRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.TransmuteRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.TransmuteRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.TransmuteRecord));

            return buffer;
        }

        public void Dispose() {
            _isShuttingDown = true;
        }
    }
}
