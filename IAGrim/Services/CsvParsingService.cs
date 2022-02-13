using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.TransferStash;
using IAGrim.Services.Dto;
using IAGrim.Settings;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
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
        private readonly ItemTransferController _itemTransferController;
        private readonly SettingsService _settings;

        public CsvParsingService(IPlayerItemDao playerItemDao, IReplicaItemDao replicaItemDao, UserFeedbackService userFeedbackService, TransferStashServiceCache cache, ItemTransferController itemTransferController, TransferStashService transferStashService, SettingsService settings) {
            _playerItemDao = playerItemDao;
            _replicaItemDao = replicaItemDao;
            _userFeedbackService = userFeedbackService;
            _cache = cache;
            _itemTransferController = itemTransferController;
            _transferStashService = transferStashService;
            _settings = settings;
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
                    try {
                        if (entry.Cooldown.IsReady) {
                            PlayerItem item = Parse(File.ReadAllText(entry.Filename));
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
                            }
                            else if (classificationService.Duplicates.Count > 0 && _settings.GetPersistent().DeleteDuplicates) {
                                Logger.Info("Deleting duplicate item file");
                                File.Delete(entry.Filename);
                            }
                            else {
                                // TODO: Transfer back in-game, should never have been looted.
                                // TODO: Separate transfer logic.. no delete-from-db etc..
                                ;
                                string stashfile = _itemTransferController.GetTransferFile();
                                _transferStashService.Deposit(stashfile, new List<PlayerItem> { item }, out string error);
                                if (string.IsNullOrEmpty(error)) {
                                    Logger.Info("Deposited item back in-game, did not pass item classification.");
                                    File.Delete(entry.Filename);
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

        private PlayerItem Parse(string csv) {
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
    }
}