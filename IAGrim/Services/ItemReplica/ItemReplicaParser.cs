using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using log4net;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using NHibernate.Exceptions;


// Release notes:
/*
 * No longer supports migration from the Azure online backup service over to the existing one.
 * Now supports real item stats for items from mods
 * Rewrote how IA communicates real stat generation with GD
 * Should now allow for free text search on items from mods
 * Should no longer show all items as craftable when comparing items
 * Should now display which items have dummy-stats when comparing items with a mix of real and dummy stats
 * Can now use the free text search on buddy items
 * IA now stores real item stats as .json files during generation (can potentially be useful for others)
 *
 * This update requires re-parsing all item stats
 */

namespace IAGrim.Services.ItemReplica {
    class ItemReplicaParser : IDisposable {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ItemReplicaParser));
        private readonly IReplicaItemDao _replicaItemDao;
        private readonly IPlayerItemDao _playerItemDao;
        private Thread _thread = null;
        private volatile bool _isCancelled;
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        

        public ItemReplicaParser(IReplicaItemDao replicaItemDao, IPlayerItemDao playerItemDao) {
            this._replicaItemDao = replicaItemDao;
            this._playerItemDao = playerItemDao;
        }


        class JsonObj {
            public string playerItemId { get; set; }
            public string buddyItemId { get; set; }
            public JsonReplicaObj replica { get; set; }
            public List<JsonStatObj> stats { get; set; }
        }

        class JsonStatObj {
            public string text { get; set; }
            public string type { get; set; }
        }

        class JsonReplicaObj {
            public string baseRecord { get; set; }
            public string prefixRecord { get; set; }
        }

        public void Preload() {
            ExceptionReporter.EnableLogUnhandledOnThread();

            var playerItemIDs = new HashSet<long>(_replicaItemDao.GetPlayerItemIds());
            var buddyItemIDs = new HashSet<string>(_replicaItemDao.GetBuddyItemIds());

            var fileNames = Directory
                .EnumerateFiles(GlobalPaths.CsvReplicaReadLocation, "*.json", SearchOption.TopDirectoryOnly).ToList();
            foreach (var filename in fileNames) {
                var itemName = Path.GetFileName(filename).Replace(".json", "");
                if (long.TryParse(itemName, out long playerItemId)) {
                    if (playerItemIDs.Contains(playerItemId)) {
                        if (File.Exists(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)))) {
                            File.Delete(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                        }
                        File.Move(filename, Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                        continue;
                    }
                }
                else {
                    if (buddyItemIDs.Contains(itemName)) {
                        if (File.Exists(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)))) {
                            File.Delete(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                        }
                        File.Move(filename, Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                        continue;
                    }
                }

                Enqueue(filename);
            }
        }

        public void Start() {
            _thread = new Thread(Exec);
            _thread.Start();
        }

        private void Exec() {
            ExceptionReporter.EnableLogUnhandledOnThread();
            while (!_isCancelled) {
                while (_queue.TryDequeue(out var filename) && !_isCancelled) {
                    Process(filename);
                }

                try {
                    Thread.Sleep(1500);
                }
                catch (Exception) {
                }
            }
        }
        public void Enqueue(EventArgs arg) {
            var csvEvent = arg as CsvFileMonitor.CsvEvent; // Stupid name, its json..
            Enqueue(csvEvent.Filename);
        }

        public void Enqueue(string filename) {
            _queue.Enqueue(filename);
        }

        private void Process(string filename) {
            string latest = string.Empty;
            try {
                var json = File.ReadAllText(filename);
                _logger.Info($"Parsing file {filename} for item stats");
                var arr = JsonConvert.DeserializeObject<Dictionary<string, JsonObj>>(json);

                var playerItemIds = arr.Select(m => long.Parse(m.Value.playerItemId));
                var playerRecordMap = _playerItemDao.FindRecordsFromIds(playerItemIds);

                foreach (var itemTemplate in arr) {
                    var template = itemTemplate.Value;
                    var stats = template.stats
                        .Select(m => new ReplicaItemRow {
                            Text = Regex.Replace(m.text.Trim(), @"(\^.?)", ""),
                            TextLowercase = Regex.Replace(m.text.Trim(), @"(\^.?)", "").ToLowerInvariant(),
                            Type = Int32.Parse(m.type),
                        }).ToList();


                    var playerItemId = long.Parse(template.playerItemId);
                    if (playerItemId > 0 && string.IsNullOrEmpty(template.buddyItemId)) {
                        if (!playerRecordMap.ContainsKey(playerItemId)) {
                            _logger.Warn($"Could not find playerItemId({playerItemId} in the db");
                            continue;
                        } else if (playerRecordMap[playerItemId] != template.replica.baseRecord) {
                            _logger.Warn($"Got record '{template.replica.baseRecord}' for PID({playerItemId}), expected record '{playerRecordMap[playerItemId]}'");
                            continue;
                        }
                    }

                    var item = new ReplicaItem {
                        PlayerItemId = playerItemId,
                        BuddyItemId = template.buddyItemId
                    };

                    if (item.PlayerItemId == 0) {
                        // Unique constraint will fail if its 0, this is likely a buddy item
                        item.PlayerItemId = null;
                    }

                    if (string.IsNullOrEmpty(item.BuddyItemId)) {
                        item.BuddyItemId = null;
                    }


                    latest = $"PID({item.PlayerItemId}), BID({item.BuddyItemId})";
                    _logger.Debug("Storing replica item stats for item " + item.PlayerItemId + item.BuddyItemId);
                    _replicaItemDao.Save(item, stats);
                }
            }
            catch (IOException ex) {
                _logger.Warn("IOException reading replica file: " + ex.Message);
                if (ex.Message.Contains("because it is being used by another process")) {
                    var locks = DebugLockedFileUtil.WhoIsLocking(filename);
                    foreach (var process in locks) {
                        _logger.Warn($"The process \"{process.ProcessName}\" is locking {filename}");
                    }
                }
            }
            catch (System.Data.SQLite.SQLiteException ex) {
                _logger.Warn($"Error storing replica item stats for item {latest}: " + ex.Message);
            }
            catch (GenericADOException ex) {
                _logger.Warn($"Error storing replica item stats for item {latest}: " + ex.Message);
            }
            catch (Exception ex) {
                _logger.Warn($"Error storing replica item stats for item {latest}: " + ex.Message);
            }
            finally {
                try {
                    if (File.Exists(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)))) {
                        File.Delete(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                    }
                    File.Move(filename, Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                } catch (Exception) { /* Ok whatever, IO stuff happens. */ }
            }
        }

        public void Dispose() {
            _isCancelled = true;
        }
    }
}