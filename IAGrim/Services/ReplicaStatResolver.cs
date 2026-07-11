using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataAccess;
using GrimDawnItemStats;
using IAGrim.Database.DAO.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Services.Dto;
using IAGrim.Utilities;
using log4net;
using StatTranslator;

namespace IAGrim.Services {
    /// <summary>
    /// Computes an item's real, seed-applied stats locally using the GrimDawnItemStats SDK
    /// (replaying the game's MINSTD random stream), instead of asking the game to compute and
    /// report them back. Replaces the former ItemReplicaRequesterService / ItemReplicaParser
    /// round-trip.
    /// </summary>
    public class ReplicaStatResolver {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReplicaStatResolver));
        private readonly IDatabaseItemStatDao _databaseItemStatDao;

        public ReplicaStatResolver(IDatabaseItemStatDao databaseItemStatDao) {
            _databaseItemStatDao = databaseItemStatDao;
        }

        /// <summary>
        /// Resolves the rolled, translated stat lines for an item. Returns an empty list if the
        /// item cannot be resolved (no base record, no seed, or an unsupported item class).
        /// </summary>
        public List<TranslatedStat> Resolve(string baseRecord, string prefixRecord, string suffixRecord, uint seed) {
            if (string.IsNullOrEmpty(baseRecord) || seed == 0)
                return new List<TranslatedStat>();

            var records = new List<string> { baseRecord };
            if (!string.IsNullOrEmpty(prefixRecord)) records.Add(prefixRecord);
            if (!string.IsNullOrEmpty(suffixRecord)) records.Add(suffixRecord);

            var statMap = _databaseItemStatDao.GetStats(records, StatFetch.PlayerItems);

            statMap.TryGetValue(baseRecord, out var baseRows);
            List<DBStatRow> prefixRows = null, suffixRows = null;
            if (!string.IsNullOrEmpty(prefixRecord)) statMap.TryGetValue(prefixRecord, out prefixRows);
            if (!string.IsNullOrEmpty(suffixRecord)) statMap.TryGetValue(suffixRecord, out suffixRows);

            if (baseRows == null || baseRows.Count == 0)
                return new List<TranslatedStat>();

            var baseStats = ToFieldDict(baseRows);
            var prefixStats = prefixRows != null ? ToFieldDict(prefixRows) : null;
            var suffixStats = suffixRows != null ? ToFieldDict(suffixRows) : null;

            IReadOnlyList<Calculator.StatEntry> rolled;
            try {
                rolled = Calculator.Calculate(baseStats, prefixStats, suffixStats, seed);
            }
            catch (Exception ex) {
                Logger.Warn($"Failed to compute replica stats for {baseRecord} (seed {seed}): {ex.Message}");
                return new List<TranslatedStat>();
            }

            var stats = Merge(baseRecord, baseRows, prefixRows, suffixRows, rolled);
            return RuntimeSettings.StatManager.ProcessStats(stats, TranslatedStatType.BODY);
        }

        /// <summary>
        /// Builds the final stat set for translation: the SDK's rolled values (in draw order,
        /// preserving repeated proc lines) plus any remaining raw fields the SDK does not touch
        /// (text-only fields like conversion damage types, and fixed/cosmetic fields).
        /// </summary>
        private static ISet<IItemStat> Merge(string baseRecord, List<DBStatRow> baseRows, List<DBStatRow> prefixRows,
            List<DBStatRow> suffixRows, IReadOnlyList<Calculator.StatEntry> rolled) {
            var raw = new Dictionary<string, DBStatRow>(StringComparer.Ordinal);
            void AddRaw(List<DBStatRow> rows) {
                if (rows == null) return;
                foreach (var r in rows)
                    if (!raw.ContainsKey(r.Stat))
                        raw[r.Stat] = r;
            }
            AddRaw(baseRows);
            AddRaw(prefixRows);
            AddRaw(suffixRows);

            var result = new HashSet<IItemStat>();
            var emitted = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in rolled) {
                result.Add(new DBStatRow {
                    Record = baseRecord,
                    Stat = entry.Field,
                    // The game's tooltips never show fractional values; some SDK fields (e.g.
                    // conversion percentages, damage %-modifiers) aren't pre-truncated like the
                    // flat min/max pairs are (which use Math.Truncate), so truncate here too to
                    // match in-game display and stay consistent with the engine's own convention.
                    Value = Math.Truncate(entry.Value),
                    TextValue = null,
                });
                emitted.Add(entry.Field);
            }

            foreach (var kv in raw) {
                if (emitted.Contains(kv.Key)) continue;
                result.Add(kv.Value);
            }

            return result;
        }

        private static Dictionary<string, string> ToFieldDict(List<DBStatRow> rows) {
            var dict = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var r in rows)
                dict[r.Stat] = !string.IsNullOrEmpty(r.TextValue)
                    ? r.TextValue
                    : r.Value.ToString(CultureInfo.InvariantCulture);
            return dict;
        }
    }
}
