using System.Collections.Generic;
using System.Linq;
using GrimDawnItemStats;
using IAGrim.Services.Dto;

namespace IAGrim.Services.ItemStats {
    /// <summary>
    /// Bridges IA's raw <see cref="DBStatRow"/> stat rows to the seed-stat engine
    /// (<see cref="GrimDawnItemStats.ItemStatEngine"/>).
    ///
    /// When an item has no game-provided replica stats (cloud/buddy synced items), we can still
    /// reconstruct its real, seed-applied values by replaying the game's random stream over the
    /// base/prefix/suffix records. This is used instead of displaying the item's raw base stats
    /// with a "you are seeing base stats" warning.
    /// </summary>
    internal static class SeedStatCalculator {

        /// <summary>
        /// Rolls the item's real stats from its base/prefix/suffix stat rows and seed.
        /// </summary>
        /// <returns>
        /// A map of stat-field to seed-applied value, or <c>null</c> when the roll can't be trusted
        /// (no seed, no base rows, or the item carries rollable fields the engine does not model —
        /// in which case downstream draws may be desynced and the caller should fall back to the
        /// raw base stats).
        /// </returns>
        public static IReadOnlyDictionary<string, double> Compute(
            List<DBStatRow> baseRows,
            List<DBStatRow> prefixRows,
            List<DBStatRow> suffixRows,
            uint seed) {

            if (seed == 0 || baseRows == null || baseRows.Count == 0) {
                return null;
            }

            var result = ItemStatEngine.Compute(
                ToInputStats(baseRows),
                seed,
                prefixStats: prefixRows != null && prefixRows.Count > 0 ? ToInputStats(prefixRows) : null,
                suffixStats: suffixRows != null && suffixRows.Count > 0 ? ToInputStats(suffixRows) : null);

            // Unmodeled rollable fields mean the shared random stream may have desynced, making every
            // subsequent value unreliable. Rather than show wrong numbers, bail out so the caller
            // keeps the raw base stats (and the warning).
            if (result.UnmodeledFields.Count > 0) {
                return null;
            }

            var stats = new Dictionary<string, double>(result.Stats.Count);
            foreach (var kv in result.Stats) {
                stats[kv.Key] = kv.Value;
            }

            // "N% Chance of ..." proc lines are split off from the merged totals; fold their rolled
            // value back in so the stat still shows (IA has no separate proc-line rendering here).
            if (result.ProcLines != null) {
                foreach (var p in result.ProcLines) {
                    if (p.Min is { } min) {
                        stats[p.Field] = stats.TryGetValue(p.Field, out var existing) ? existing + min : min;
                    }
                }
            }

            return stats;
        }

        private static IEnumerable<ItemStatEngine.InputStat> ToInputStats(IEnumerable<DBStatRow> rows) {
            // Multiple arz records (base + expansions) can carry the same field; take the highest
            // value per field, mirroring the Filter() the stat service already applies.
            return rows
                .GroupBy(r => r.Stat)
                .Select(g => g.OrderByDescending(r => r.Value).First())
                .Select(r => new ItemStatEngine.InputStat(r.Stat, r.TextValue ?? string.Empty, r.Value));
        }
    }
}
