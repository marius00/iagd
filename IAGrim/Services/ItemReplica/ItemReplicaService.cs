using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Services.ItemReplica;
using IAGrim.Settings;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    // TODO: Class does too much, and is somewhat of a mess.
    class ItemReplicaService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemReplicaService));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly SettingsService _settingsService;

        private volatile bool _isShuttingDown = false;
        private Thread _t = null;
        private readonly ActionCooldown _cooldown = new ActionCooldown(2500);
        private ReplicaCache _cache = new ReplicaCache();

        public ItemReplicaService(IPlayerItemDao playerItemDao, IBuddyItemDao buddyItemDao, SettingsService settingsService) {
            _playerItemDao = playerItemDao;
            _buddyItemDao = buddyItemDao;
            _settingsService = settingsService;
        }

        public void Reset() {
            _cache = new ReplicaCache();
        }

        public void Start() {
            if (_t != null)
                throw new ArgumentException("Max one thread running per instance");

            _t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                // Items already queued
                foreach (var pi in Directory.EnumerateFiles(GlobalPaths.CsvReplicaWriteLocation)) {
                    _cache.Add(Path.GetFileName(pi));
                    
                }
                foreach (var mod in Directory.EnumerateDirectories(GlobalPaths.CsvReplicaWriteLocation)) {
                    foreach (var pi in Directory.EnumerateFiles(Path.Combine(GlobalPaths.CsvReplicaWriteLocation, mod))) {
                        _cache.Add(Path.GetFileName(pi));
                    }
                }
                
                // Items awaiting processing
                foreach (var pi in Directory.EnumerateFiles(GlobalPaths.CsvReplicaReadLocation)) {
                    _cache.Add(Path.GetFileName(pi).Replace(".json", ".csv"));
                }

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


            {
                var items = _playerItemDao.ListMissingReplica();
                count += items.Count;
                foreach (var item in items) {
                    var hash = item.Id + ".csv";
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;


                    var path = "";
                    var filename = item.Id + ".csv";
                    if (string.IsNullOrEmpty(item.Mod)) {
                        path = GlobalPaths.CsvReplicaWriteLocation;
                    }
                    else {
                        path = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod);
                    }

                    Directory.CreateDirectory(path);
                    filename = Path.Combine(path, filename);

                    var csv = Serialize(item);
                    if (csv != null) {
                        if (!File.Exists(filename)) {
                            File.WriteAllText(filename, csv);
                            _cache.Add(hash);
                        }
                    }

                    Thread.Sleep(1);
                    if (_isShuttingDown) {
                        return false;
                    }
                }
            }

            

            if (!_settingsService.GetLocal().OptOutOfBackups) {
                var items = _buddyItemDao.ListMissingReplica();
                count += items.Count;

                foreach (var item in items) {
                    var hash = item.RemoteItemId + ".csv";
                    if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                        continue;


                    var path = "";
                    var filename = item.RemoteItemId + ".csv";
                    if (string.IsNullOrEmpty(item.Mod)) {
                        path = Path.Combine(GlobalPaths.CsvReplicaWriteLocation);
                    }
                    else {
                        path = Path.Combine(GlobalPaths.CsvReplicaWriteLocation, item.Mod);
                    }
                    Directory.CreateDirectory(path);
                    filename = Path.Combine(path, filename);

                    var csv = Serialize(item);
                    if (csv != null) {
                        if (!File.Exists(filename)) {
                            File.WriteAllText(filename, csv);
                            _cache.Add(hash);
                        }
                    }

                    Thread.Sleep(1);
                }
            }

            return count > 0;
        }



        private static string Serialize(BuddyItem bi) {
            if (bi.BaseRecord.Length > 255 || bi.SuffixRecord?.Length > 255 || bi.PrefixRecord?.Length > 255
                || bi.MateriaRecord?.Length > 255 || bi.ModifierRecord?.Length > 255 || bi.EnchantmentRecord?.Length > 255
                || bi.TransmuteRecord?.Length > 255 || bi.AscendantAffixNameRecord?.Length > 255 || bi.AscendantAffix2hNameRecord?.Length > 255) {
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
            sb.Append(bi.RerollsUsed + ";");
            sb.Append(bi.BaseRecord?.Trim() + ";");
            sb.Append(bi.PrefixRecord?.Trim() + ";");
            sb.Append(bi.SuffixRecord?.Trim() + ";");
            sb.Append(bi.ModifierRecord?.Trim() + ";");
            sb.Append(bi.MateriaRecord?.Trim() + ";");
            sb.Append(bi.EnchantmentRecord?.Trim() + ";");
            sb.Append(bi.TransmuteRecord?.Trim() + ";");
            sb.Append(bi.AscendantAffixNameRecord?.Trim() + ";");
            sb.Append(bi.AscendantAffix2hNameRecord?.Trim());
            Logger.Debug($"Dispatching: {sb.ToString()}");
            if (sb.ToString().Count(s => s == ';') != 14) {
                Logger.Warn("Could not serialize item, invalid ; count");
                return null;
            }

            return sb.ToString();
        }

        private static string Serialize(PlayerItem pi) {
            if (pi.BaseRecord?.Length > 255 || pi.SuffixRecord?.Length > 255 || pi.PrefixRecord?.Length > 255
                || pi.MateriaRecord?.Length > 255 || pi.ModifierRecord?.Length > 255 || pi.EnchantmentRecord?.Length > 255
                || pi.TransmuteRecord?.Length > 255 || pi.AscendantAffixNameRecord?.Length > 255 || pi.AscendantAffix2hNameRecord?.Length > 255) {
                Logger.Warn("Received a player item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

            const int TYPE_PLAYERITEM = 1;
            //const int TYPE_BUDDYITEM = 2;

            StringBuilder sb = new StringBuilder();

            sb.Append(TYPE_PLAYERITEM + ";");
            sb.Append(pi.Id + ";");
            sb.Append(pi.USeed + ";");
            sb.Append((uint)pi.RelicSeed + ";");
            sb.Append((uint)pi.EnchantmentSeed + ";");
            sb.Append(pi.RerollsUsed + ";");
            sb.Append(pi.BaseRecord?.Trim() + ";");
            sb.Append(pi.PrefixRecord?.Trim() + ";");
            sb.Append(pi.SuffixRecord?.Trim() + ";");
            sb.Append(pi.ModifierRecord?.Trim() + ";");
            sb.Append(pi.MateriaRecord?.Trim() + ";");
            sb.Append(pi.EnchantmentRecord?.Trim() + ";");
            sb.Append(pi.TransmuteRecord?.Trim() + ";");
            sb.Append(pi.AscendantAffixNameRecord?.Trim() + ";");
            sb.Append(pi.AscendantAffix2hNameRecord?.Trim());
            Logger.Debug($"Dispatching: {sb.ToString()}");
            if (sb.ToString().Count(s => s == ';') != 14) {
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
