using System.Collections.Generic;
using System.Linq;
using IAGrim.Services.ItemStats;

namespace IAGrim.UI.Filters {

    /// <summary>
    /// Turns (checkbox, stat-fields) pairs into the per-checkbox numeric <see cref="StatValueFilter"/>s,
    /// keeping only checkboxes that actually have a numeric filter configured.
    /// </summary>
    internal static class FilterBuilder {
        public static List<StatValueFilter> From(IEnumerable<(FirefoxCheckBox cb, string[] fields)> groups) {
            return groups
                .Where(g => g.cb.HasFilter)
                .Select(g => new StatValueFilter {
                    Fields = g.fields,
                    Operator = g.cb.FilterOperator.Value,
                    Threshold = g.cb.FilterThreshold,
                })
                .ToList();
        }
    }
}
