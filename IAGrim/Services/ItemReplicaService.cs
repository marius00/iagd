using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private volatile bool _isShuttingDown = false;
        private Thread _t = null;
        private readonly ActionCooldown _cooldown = new ActionCooldown(2500);
        private readonly ReplicaCache _cache = new ReplicaCache();

        public ItemReplicaService(IPlayerItemDao playerItemDao, IBuddyItemDao buddyItemDao) {
            _playerItemDao = playerItemDao;
            _buddyItemDao = buddyItemDao;
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
            int count = 0;

            // Items already queued
            foreach (var pi in Directory.EnumerateFiles(GlobalPaths.CsvReplicaWriteLocation)) {
                _cache.Add("pi/" + pi);
            }
            foreach (var mod in Directory.EnumerateDirectories(GlobalPaths.CsvReplicaWriteLocation)) {
                foreach (var pi in Directory.EnumerateFiles(Path.Combine(GlobalPaths.CsvReplicaWriteLocation, mod))) {
                    _cache.Add("pi/" + pi);
                }
            }

            {
                var items = _playerItemDao.ListMissingReplica();
                count += items.Count;
                foreach (var item in items) {
                    var hash = "pi/" + item.Id;
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;


                    
                    string filename = "";
                    if (string.IsNullOrEmpty(item.Mod)) {
                        filename = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, "" + item.Id);

                    } else {
                        Directory.CreateDirectory(Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod));
                        filename = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod, "" + item.Id);
                    }

                    var csv = Serialize(item);
                    if (!File.Exists(filename)) {
                        File.WriteAllText(filename, csv);
                    }

                    _cache.Add(hash);
                    Thread.Sleep(1);
                }
            }


            // Items already queued
            foreach (var pi in Directory.EnumerateFiles(GlobalPaths.CsvReplicaWriteLocation)) {
                _cache.Add("bi/" + pi);
            }
            foreach (var mod in Directory.EnumerateDirectories(GlobalPaths.CsvReplicaWriteLocation)) {
                foreach (var pi in Directory.EnumerateFiles(Path.Combine(GlobalPaths.CsvReplicaWriteLocation, mod))) {
                    _cache.Add("bi/" + pi);
                }
            }
            {
                var items = _buddyItemDao.ListMissingReplica();
                count += items.Count;

                foreach (var item in items) {
                    var hash = "bi/" + item.RemoteItemId;
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;

                    string filename = "";
                    if (string.IsNullOrEmpty(item.Mod)) {
                        filename = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, "" + item.RemoteItemId);

                    }
                    else {
                        Directory.CreateDirectory(Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod));
                        filename = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod, "" + item.RemoteItemId);
                    }

                    var csv = Serialize(item);
                    if (!File.Exists(filename)) {
                        File.WriteAllText(filename, csv);
                    }

                    _cache.Add(hash);
                    Thread.Sleep(1);
                }
            }

            return count > 0;
        }



        private static string Serialize(BuddyItem bi) {
            if (bi.BaseRecord.Length > 255 || bi.SuffixRecord?.Length > 255 || bi.PrefixRecord?.Length > 255
                || bi.MateriaRecord?.Length > 255 || bi.ModifierRecord?.Length > 255 || bi.EnchantmentRecord?.Length > 255
                || bi.TransmuteRecord?.Length > 255) {
                Logger.Warn("Received a buddy item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

            //const int TYPE_PLAYERITEM = 1;
            const int TYPE_BUDDYITEM = 2;

            StringBuilder sb = new StringBuilder();

            sb.Append(TYPE_BUDDYITEM + ";");
            sb.Append(bi.RemoteItemId + ";");
            sb.Append(bi.Seed + ";");
            sb.Append(bi.RelicSeed + ";");
            sb.Append(bi.EnchantmentSeed + ";");
            sb.Append(bi.BaseRecord + ";");
            sb.Append(bi.PrefixRecord + ";");
            sb.Append(bi.SuffixRecord + ";");
            sb.Append(bi.ModifierRecord + ";");
            sb.Append(bi.MateriaRecord + ";");
            sb.Append(bi.EnchantmentRecord + ";");
            sb.Append(bi.TransmuteRecord);
            Logger.Debug($"Dispatching: {sb.ToString()}");
            if (sb.ToString().Count(s => s == ';') != 11) {
                Logger.Warn("Could not serialize item, invalid ; count");
                return null;
            }

            return sb.ToString();
        }

        private static string Serialize(PlayerItem pi) {
            if (pi.BaseRecord?.Length > 255 || pi.SuffixRecord?.Length > 255 || pi.PrefixRecord?.Length > 255
                || pi.MateriaRecord?.Length > 255 || pi.ModifierRecord?.Length > 255 || pi.EnchantmentRecord?.Length > 255
                || pi.TransmuteRecord?.Length > 255) {
                Logger.Warn("Received a player item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

            const int TYPE_PLAYERITEM = 1;
            //const int TYPE_BUDDYITEM = 2;

            StringBuilder sb = new StringBuilder();

            sb.Append(TYPE_PLAYERITEM + ";");
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
            sb.Append(pi.TransmuteRecord);
            Logger.Debug($"Dispatching: {sb.ToString()}");
            if (sb.ToString().Count(s => s == ';') != 11) {
                Logger.Warn("Could not serialize item, invalid ; count");
                return null;
            }

            return sb.ToString();
        }

        public void Dispose() {
            _isShuttingDown = true;
        }
    }
}
