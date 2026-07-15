namespace IAGrim.Database.Model {
    /// <summary>
    /// A single pre-computed, seed-applied stat value for a player item, stored so SQLite can filter on
    /// it directly (e.g. <c>WHERE stat = 'defensiveCold' AND value &gt; 30</c>) without replaying the seed
    /// engine at search time.
    ///
    /// The values are produced by the seed-stat engine (see <see cref="Services.ItemStats.SeedStatCalculator"/>)
    /// and populated in the background by <see cref="Services.ItemStats.ItemStatPrecomputeService"/>. One row
    /// per stat field per item. A single sentinel row (<see cref="SentinelStat"/>) is written for every
    /// processed item so items that produced no usable stats are not endlessly re-processed.
    /// </summary>
    public class ComputedItemStat {
        /// <summary>
        /// Marker stat written once per processed item. Its presence means "this item has been computed",
        /// even when the item yielded no real filterable stats (or the engine could not model it).
        /// </summary>
        public const string SentinelStat = "__computed__";

        public virtual long Id { get; set; }
        public virtual long PlayerItemId { get; set; }
        public virtual string? Stat { get; set; }
        public virtual double Value { get; set; }
    }
}
