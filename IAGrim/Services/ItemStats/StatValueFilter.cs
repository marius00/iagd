using System;

namespace IAGrim.Services.ItemStats {

    /// <summary>
    /// A per-checkbox numeric stat filter: "the item's real value for <see cref="Fields"/> (summed) must
    /// satisfy <see cref="Operator"/> <see cref="Threshold"/>". Applied in-database against the
    /// pre-computed <c>ComputedItemStat</c> table, so the seed engine is not replayed at search time.
    /// </summary>
    public sealed class StatValueFilter {
        public enum Op { GreaterThan, GreaterOrEqual, LessThan, LessOrEqual, Equal }

        /// <summary>The stat field names contributed by the checkbox; their computed values are summed.</summary>
        public string[] Fields { get; set; } = Array.Empty<string>();

        public Op Operator { get; set; }

        public double Threshold { get; set; }
    }
}
