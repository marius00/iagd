using System.Collections.Generic;
using IAGrim.Database.Model;
// PlayerItem lives in the IAGrim.Database namespace.

namespace IAGrim.Database.Interfaces {
    public interface IComputedItemStatDao : IBaseDao<ComputedItemStat> {
        /// <summary>
        /// Replaces the computed stats for a single player item with <paramref name="stats"/> (plus a
        /// sentinel marker row), in one transaction. An empty map still writes the sentinel, marking the
        /// item as processed so it is not picked up again.
        /// </summary>
        void SaveComputed(long playerItemId, IReadOnlyDictionary<string, double>? stats);

        /// <summary>
        /// Player items (up to <paramref name="limit"/>) that have no rows in the computed-stat table yet,
        /// i.e. still need pre-computing. Returns the full item so the caller has its records and seed.
        /// </summary>
        IList<PlayerItem> ListItemsMissingComputedStats(int limit);

        /// <summary>Removes all computed rows for the given player items (used when items are deleted).</summary>
        void DeleteForPlayerItems(IEnumerable<long> playerItemIds);

        /// <summary>Wipes the entire computed-stat table (used by "Clear cache").</summary>
        void DeleteAll();
    }
}
