using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    class CsvParsingService : IDisposable {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CsvParsingService));
        private readonly ConcurrentQueue<QueuedCsv> _queue = new ConcurrentQueue<QueuedCsv>();
        private volatile bool _isCancelled;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IReplicaItemDao _replicaItemDao;
        private readonly UserFeedbackService _userFeedbackService;

        public CsvParsingService(IPlayerItemDao playerItemDao, IReplicaItemDao replicaItemDao, UserFeedbackService userFeedbackService) {
            _playerItemDao = playerItemDao;
            _replicaItemDao = replicaItemDao;
            _userFeedbackService = userFeedbackService;
        }

        private class QueuedCsv {
            public string Filename { get; set; }
            public ActionCooldown Cooldown { get; set; }
        }


        public void Start() {
            // Queue any existing files
            foreach (var file in Directory.EnumerateFiles(GlobalPaths.CsvLocation)) {
                _queue.Enqueue(new QueuedCsv {
                    Filename = file,
                    Cooldown = new ActionCooldown(0)
                });
            }


            // Process any newly added files. Threaded to ensure a proper delay between write and read.
            var t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                while (!_isCancelled) {
                    Thread.Sleep(500);
                    if (!_queue.TryDequeue(out var entry)) continue;

                    if (entry.Cooldown.IsReady) {
                        var item = Parse(File.ReadAllText(entry.Filename));
                        if (item == null)
                            continue;

                        // CSV probably wont have stackcount
                        item.StackCount = Math.Max(item.StackCount, 1);
                        item.CreationDate = DateTime.UtcNow.ToTimestamp();
                        _playerItemDao.Save(item);
                        File.Delete(entry.Filename);

                        // Update replica reference
                        var hash = ItemReplicaService.GetHash(item);
                        _replicaItemDao.UpdatePlayerItemId(hash, item.Id);
                    }
                    else {
                        _queue.Enqueue(entry);
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

        private PlayerItem Parse(string csv) {
            var pieces = csv.Split(';');

            if (pieces.Length != 13) {
                _logger.Warn($"Expected 13 columns in row, got {pieces.Length}");
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
            };
        }
    }
}