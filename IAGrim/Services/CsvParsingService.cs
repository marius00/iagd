using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.TransferStash;
using IAGrim.Services.Dto;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;

namespace IAGrim.Services {
    class CsvParsingService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CsvParsingService));
        private readonly ConcurrentQueue<QueuedCsv> _queue = new ConcurrentQueue<QueuedCsv>();
        private volatile bool _isCancelled;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IReplicaItemDao _replicaItemDao;
        private readonly UserFeedbackService _userFeedbackService;
        private readonly TransferStashServiceCache _cache;
        private readonly TransferStashService _transferStashService;
        public event EventHandler OnItemLooted;

        public CsvParsingService(IPlayerItemDao playerItemDao, IReplicaItemDao replicaItemDao, UserFeedbackService userFeedbackService, TransferStashServiceCache cache, TransferStashService transferStashService) {
            _playerItemDao = playerItemDao;
            _replicaItemDao = replicaItemDao;
            _userFeedbackService = userFeedbackService;
            _cache = cache;
            _transferStashService = transferStashService;
        }

        private class QueuedCsv {
            public string Filename { get; set; }
            public ActionCooldown Cooldown { get; set; }
        }


        public void Start() {
            // Queue any existing files
            foreach (var file in Directory.EnumerateFiles(GlobalPaths.CsvLocationIngoing)) {
                _queue.Enqueue(new QueuedCsv {
                    Filename = file,
                    Cooldown = new ActionCooldown(0)
                });
            }

            var now = DateTime.Now;


            try {
                foreach (var root in new string[] { GlobalPaths.CsvLocationOutgoingDeleted, GlobalPaths.CsvLocationIngoingDeleted }) {
                    foreach (var file in Directory.EnumerateFiles(root, "*.csv", SearchOption.AllDirectories)) {
                        var daysOld = (now - File.GetLastWriteTime(file)).TotalDays;
                        if (daysOld > 3) {
                            Logger.Info($"Deleting old file {file}, {(now - File.GetLastWriteTime(file)).TotalDays} days old.");
                            File.Delete(file);
                        }
                    }
                }
            } catch (IOException ex) {
                Logger.Warn(ex);
            }

            // Process any newly added files. Threaded to ensure a proper delay between write and read.
            var t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                while (!_isCancelled) {
                    Thread.Sleep(500);
                    if (!_queue.TryDequeue(out var entry)) continue;
                    try {
                        if (entry.Cooldown.IsReady) {
                            PlayerItem item = Deserialize(File.ReadAllText(entry.Filename));
                            if (item == null)
                                continue;

                            // CSV probably wont have stackcount
                            item.StackCount = Math.Max(item.StackCount, 1);
                            item.CreationDate = DateTime.UtcNow.ToTimestamp();

                            var classificationService = new ItemClassificationService(_cache, _playerItemDao);
                            classificationService.Add(item);

                            // Items to loot
                            if (classificationService.Remaining.Count > 0) {
                                _playerItemDao.Save(item);
                                File.Delete(entry.Filename);

                                // Update replica reference
                                var hash = ItemReplicaService.GetHash(item);
                                _replicaItemDao.UpdatePlayerItemId(hash, item.Id);
                                OnItemLooted?.Invoke(this, null);
                            }
                            else if (classificationService.Duplicates.Count > 0) {
                                Logger.Info("Deleting duplicate item file");
                                File.Move(entry.Filename, Path.Combine(GlobalPaths.CsvLocationIngoingDeleted, Path.GetFileName(entry.Filename)));
                            }
                            else {
                                // Transfer back in-game, should never have been looted.
                                // TODO: Separate transfer logic.. no delete-from-db etc..
                                if (RuntimeSettings.StashStatus == StashAvailability.CLOSED) {
                                    // TODO: Could just MOVE it.. same CSV format..
                                    _transferStashService.Deposit(new List<PlayerItem> { item }, null);
                                    Logger.Info("Deposited item back in-game, did not pass item classification.");
                                    Logger.Info("New GD patch? Go to the Grim Dawn tab and parse the game files again.");
                                    File.Delete(entry.Filename);
                                }
                                else {
                                    _queue.Enqueue(entry);
                                }

                            }
                        }
                        else {
                            _queue.Enqueue(entry);
                        }
                    }
                    catch (Exception ex) {
                        Logger.Warn("Error handling CSV item file", ex);
                    }
                } 
            });

            t.Start();


        }


        public void Queue(string filename, ActionCooldown cooldown) {
            _queue.Enqueue(new QueuedCsv {
                Filename = filename,
                Cooldown = cooldown
            });
            _userFeedbackService.SetFeedback(UserFeedback.FromTagSingleton("iatag_feedback_instalooted"));
        }

        public void Dispose() {
            _isCancelled = true;
        }


        private static uint ToInt(string s) {
            uint.TryParse(s, out var i);
            return i;
        }

        private PlayerItem Deserialize(string csv) {
            var pieces = csv.Split(';');

            if (pieces.Length != 13) {
                Logger.Warn($"Expected 13 columns in row, got {pieces.Length}");
                return null;
            }

            return new PlayerItem {
                Mod = pieces[0],
                IsHardcore = "1".Equals(pieces[1]),
                BaseRecord = pieces[2],
                PrefixRecord = pieces[3],
                SuffixRecord = pieces[4],
                Seed = ToInt(pieces[5]),
                ModifierRecord = pieces[6],
                MateriaRecord = pieces[7],
                RelicCompletionBonusRecord = pieces[8],
                RelicSeed = ToInt(pieces[9]),
                EnchantmentRecord = pieces[10],
                EnchantmentSeed = ToInt(pieces[11]),
                TransmuteRecord = pieces[12],
                Tags = new HashSet<DBStatRow>(0)
            };
        }

        public static string Serialize(PlayerItem item) {
            var sep = ";";
            return ""
                + item.Mod + sep
                + (item.IsHardcore ? 1 : 0) + sep
                + item.BaseRecord + sep
                + item.PrefixRecord + sep
                + item.SuffixRecord + sep
                + item.Seed + sep
                + item.ModifierRecord + sep
                + item.MateriaRecord + sep
                + item.RelicCompletionBonusRecord + sep
                + item.RelicSeed + sep
                + item.EnchantmentRecord + sep
                + item.EnchantmentSeed + sep
                + item.TransmuteRecord;
        }
    }
}