using System.Collections.Generic;
using System.Linq;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services.ItemStats {
    /// <summary>
    /// Background worker that slowly pre-computes each player item's seed-applied stats and stores them in
    /// the ComputedItemStat table, so stat filtering can happen directly in SQLite instead of replaying the
    /// seed engine at search time.
    ///
    /// Runs on its own low-priority loop: a small batch of not-yet-computed items per tick with a sleep
    /// between each item, so it never consumes meaningful CPU. New (looted) items are picked up
    /// automatically because they appear as "missing" until computed.
    /// </summary>
    class ItemStatPrecomputeService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemStatPrecomputeService));

        private const int BatchSize = 25;

        private readonly IComputedItemStatDao _computedItemStatDao;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        private volatile bool _isShuttingDown;
        private Thread? _t;
        private readonly ActionCooldown _cooldown = new(2500);

        public ItemStatPrecomputeService(IComputedItemStatDao computedItemStatDao, IDatabaseItemStatDao databaseItemStatDao) {
            _computedItemStatDao = computedItemStatDao;
            _databaseItemStatDao = databaseItemStatDao;
        }

        public void Start() {
            if (_t != null) {
                throw new ArgumentException("Max one thread running per instance");
            }

            _t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                while (!_isShuttingDown) {
                    if (_cooldown.IsReady) {
                        try {
                            Process();
                        }
                        catch (Exception ex) {
                            Logger.Warn("Error while pre-computing item stats", ex);
                        }

                        _cooldown.Reset();
                    }

                    Thread.Sleep(1);
                }
            }) {
                IsBackground = true,
                Priority = ThreadPriority.Lowest,
                Name = "ItemStatPrecompute",
            };

            _t.Start();
        }

        private void Process() {
            var items = _computedItemStatDao.ListItemsMissingComputedStats(BatchSize);
            if (items.Count == 0) {
                return;
            }

            // Fetch the base-game stat rows for every record referenced across this batch in one query.
            var records = items.SelectMany(GetRecords).Distinct().ToList();
            Dictionary<string, List<DBStatRow>> statMap =
                _databaseItemStatDao.GetStats(records, StatFetch.PlayerItems);

            List<DBStatRow> Rows(string? record) =>
                !string.IsNullOrEmpty(record) && statMap.ContainsKey(record) ? statMap[record] : new List<DBStatRow>();

            foreach (var item in items) {
                if (_isShuttingDown) {
                    return;
                }

                // Compute may be null (no seed, no base rows, or an unmodeled rollable field). SaveComputed
                // still writes a sentinel in that case, so the item is not endlessly retried.
                var stats = SeedStatCalculator.Compute(
                    Rows(item.BaseRecord), Rows(item.PrefixRecord), Rows(item.SuffixRecord), item.USeed);

                _computedItemStatDao.SaveComputed(item.Id, stats);

                Thread.Sleep(1);
            }
        }

        private static IEnumerable<string> GetRecords(PlayerItem item) {
            if (!string.IsNullOrEmpty(item.BaseRecord)) yield return item.BaseRecord;
            if (!string.IsNullOrEmpty(item.PrefixRecord)) yield return item.PrefixRecord;
            if (!string.IsNullOrEmpty(item.SuffixRecord)) yield return item.SuffixRecord;
        }

        public void Dispose() {
            _isShuttingDown = true;
        }
    }
}
