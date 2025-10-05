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
        private readonly UserFeedbackService _userFeedbackService;
        private readonly TransferStashServiceCache _cache;
        private readonly TransferStashService _transferStashService;
        public event EventHandler OnItemLooted;

        public CsvParsingService(IPlayerItemDao playerItemDao, UserFeedbackService userFeedbackService, TransferStashServiceCache cache, TransferStashService transferStashService) {
            _playerItemDao = playerItemDao;
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
                                OnItemLooted?.Invoke(this, null);
                            }
                            else if (classificationService.Duplicates.Count > 0) {
                                Logger.Info("Deleting duplicate item file");
                                var target = Path.Combine(GlobalPaths.CsvLocationIngoingDeleted, Path.GetFileName(entry.Filename));
                                if (File.Exists(target)) {
                                    target = Path.Combine(GlobalPaths.CsvLocationIngoingDeleted, Guid.NewGuid().ToString() + "-conflict.csv");
                                }
                                File.Move(entry.Filename, target);
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

            if (pieces.Length != 13 && pieces.Length != 16) {
                Logger.Warn($"Expected 13 columns in row, got {pieces.Length}");
                return null;
            }

            var isNewDlc = pieces.Length == 16;

            int n = 0;
            return new PlayerItem {
                Mod = pieces[n++],
                IsHardcore = "1".Equals(pieces[n++]),
                BaseRecord = pieces[n++].Trim(),
                PrefixRecord = pieces[n++]?.Trim(),
                SuffixRecord = pieces[n++]?.Trim(),
                Seed = ToInt(pieces[n++]),
                RerollsUsed = isNewDlc ? ToInt(pieces[n++]) : 0,
                ModifierRecord = pieces[n++]?.Trim(),
                MateriaRecord = pieces[n++]?.Trim(),
                RelicCompletionBonusRecord = pieces[n++]?.Trim(),
                RelicSeed = ToInt(pieces[n++]),
                EnchantmentRecord = pieces[n++]?.Trim(),
                EnchantmentSeed = ToInt(pieces[n++]),
                TransmuteRecord = pieces[n++]?.Trim(),
                AscendantAffixNameRecord = isNewDlc ? pieces[n++] : null,
                AscendantAffix2hNameRecord = isNewDlc ? pieces[n++] : null,
                Tags = new HashSet<DBStatRow>(0)
            };
        }

        public static string Serialize(PlayerItem item) {
            var sep = ";";
            return ""
                + item.Mod + sep
                + (item.IsHardcore ? 1 : 0) + sep
                + item.BaseRecord?.Trim() + sep
                + item.PrefixRecord?.Trim() + sep
                + item.SuffixRecord?.Trim() + sep
                + item.Seed + sep
                + item.RerollsUsed + sep
                + item.ModifierRecord?.Trim() + sep
                + item.MateriaRecord?.Trim() + sep
                + item.RelicCompletionBonusRecord?.Trim() + sep
                + item.RelicSeed + sep
                + item.EnchantmentRecord?.Trim() + sep
                + item.EnchantmentSeed + sep
                + item.TransmuteRecord?.Trim() + sep
                + item.AscendantAffixNameRecord?.Trim() + sep
                + item.AscendantAffix2hNameRecord?.Trim();
        }
    }
}