using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using log4net;
using Newtonsoft.Json;


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

        public ItemReplicaParser(IReplicaItemDao replicaItemDao) {
            this._replicaItemDao = replicaItemDao;
        }

        public void Process(EventArgs arg) {
            var csvEvent = arg as CsvFileMonitor.CsvEvent; // Stupid name, its json..
            Process(csvEvent.Filename);
        }

        class JsonObj {
            public string playerItemId { get; set; }
            public string buddyItemId { get; set; }
            public List<JsonStatObj> stats { get; set; }
        }

        class JsonStatObj {
            public string text { get; set; }
            public string type { get; set; }
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

                Process(filename);
            }
        }

        private void Process(string filename) {
            try {
                var json = File.ReadAllText(filename);
                _logger.Info($"Parsing file {filename} for item stats");

                var template = JsonConvert.DeserializeObject<JsonObj>(json);
                var stats = template.stats
                    .Select(m => new ReplicaItemRow {
                        Text = Regex.Replace(m.text.Trim(), @"(\^.?)", ""),
                        Type = Int32.Parse(m.type),
                    }).ToList();


                var item = new ReplicaItem {
                    PlayerItemId = long.Parse(template.playerItemId),
                    BuddyItemId = template.buddyItemId
                };

                if (item.PlayerItemId == 0) {
                    // Unique constraint will fail if its 0, this is likely a buddy item
                    item.PlayerItemId = null;
                }

                if (string.IsNullOrEmpty(item.BuddyItemId)) {
                    // Unique constraint will fail if its 0, this is likely a buddy item
                    item.BuddyItemId = null;
                }


                _logger.Debug("Storing replica item stats for item " + item.PlayerItemId + item.BuddyItemId);
                _replicaItemDao.Save(item, stats);


            }
            catch (Exception ex) {
                _logger.Warn("Error storing replica item stats for item: " + ex.Message);
            }
            finally {
                if (File.Exists(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)))) {
                    File.Delete(Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
                }
                File.Move(filename, Path.Combine(GlobalPaths.CsvReplicaDumpLocation, Path.GetFileName(filename)));
            }
        }

        public void Dispose() {
        }
    }
}