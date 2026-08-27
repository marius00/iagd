using System.Text.RegularExpressions;

namespace IAGrim.Services.ItemReplica {
    /// <summary>
    /// Formats the raw tooltip rows the game dumps for an item replica.
    ///
    /// A row carries ^-prefixed color codes marking its segments (^E label, ^H value, ^S/^W weapon
    /// header, ^Z skill name), which the UI turns into per-segment coloring, and may end in a
    /// [min-max] range added by the Asterkarn DLC.
    /// </summary>
    static class ReplicaTextFormatter {
        // A trailing " [min-max]" / " (min-max)", including the color code in front of it.
        private static readonly Regex RangeSuffix = new Regex(@"\s(\^.)?(\[|\().+(\]|\))$", RegexOptions.Compiled);
        private static readonly Regex ColorCodes = new Regex(@"\^.?", RegexOptions.Compiled);

        /// <summary>
        /// Row text as shown to the user: color codes intact, trailing damage range removed.
        /// </summary>
        public static string Display(string? text) {
            return RangeSuffix.Replace((text ?? string.Empty).Trim(), string.Empty);
        }

        /// <summary>
        /// Row text as indexed for wildcard search: color codes removed, lowercased.
        /// </summary>
        public static string Searchable(string? text) {
            return ColorCodes.Replace(Display(text), string.Empty).ToLowerInvariant();
        }
    }
}
