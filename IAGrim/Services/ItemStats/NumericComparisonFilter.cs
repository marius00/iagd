using System.Globalization;
using System.Text.RegularExpressions;

namespace IAGrim.Services.ItemStats {

    /// <summary>
    /// A parsed numeric comparison typed into the search box (e.g. <c>&gt;100</c>, <c>&lt;=50</c>, <c>=25</c>).
    ///
    /// When the user types such an expression <em>and</em> has exactly one stat checkbox selected, the
    /// search box stops being a name/text wildcard and instead post-filters the result set by the item's
    /// real, computed value for that stat (see the comparison branch in the search controller).
    /// </summary>
    public sealed class NumericComparisonFilter {
        public enum Op { GreaterThan, GreaterOrEqual, LessThan, LessOrEqual, Equal }

        private static readonly Regex Pattern =
            new Regex(@"^\s*(>=|<=|>|<|=)\s*(-?\d+(?:\.\d+)?)\s*$", RegexOptions.Compiled);

        public Op Operator { get; }
        public double Threshold { get; }

        private NumericComparisonFilter(Op op, double threshold) {
            Operator = op;
            Threshold = threshold;
        }

        /// <summary>
        /// Parses a numeric comparison out of the raw search text, or returns <c>null</c> when the text is
        /// not a pure comparison (in which case normal wildcard searching applies).
        /// </summary>
        public static NumericComparisonFilter TryParse(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return null;
            }

            var match = Pattern.Match(text);
            if (!match.Success) {
                return null;
            }

            if (!double.TryParse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var threshold)) {
                return null;
            }

            var op = match.Groups[1].Value switch {
                ">=" => Op.GreaterOrEqual,
                "<=" => Op.LessOrEqual,
                ">" => Op.GreaterThan,
                "<" => Op.LessThan,
                _ => Op.Equal,
            };

            return new NumericComparisonFilter(op, threshold);
        }

        public bool Matches(double value) {
            switch (Operator) {
                case Op.GreaterThan: return value > Threshold;
                case Op.GreaterOrEqual: return value >= Threshold;
                case Op.LessThan: return value < Threshold;
                case Op.LessOrEqual: return value <= Threshold;
                case Op.Equal: return value == Threshold;
                default: return false;
            }
        }
    }
}
