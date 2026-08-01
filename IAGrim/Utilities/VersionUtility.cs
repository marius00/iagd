using System;
using log4net;

namespace IAGrim.Utilities {
    /// <summary>
    /// Numeric comparison of IA version strings.
    ///
    /// Versions look like 1.5.9707.9210, where the revision is 2-second ticks since midnight (0..43199) and so
    /// is either four or five digits wide. That makes string comparison wrong: "9500" sorts above "11000", and
    /// the padded and unpadded spellings of the same version ("1.5.9707.09210" / "1.5.9707.9210") don't compare
    /// equal. Both spellings are in circulation on purpose -- FileVersion is a numeric win32 resource that
    /// cannot carry leading zeros, while AssemblyInformationalVersion (and the git tags cut from it) is padded,
    /// so any comparison has to normalise instead of assuming one form.
    /// </summary>
    public static class VersionUtility {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VersionUtility));

        /// <summary>
        /// Parses a version, tolerating a "v" prefix, a "+commitsha" suffix and zero-padded components.
        /// Returns null if it isn't a version at all.
        /// </summary>
        public static Version? Parse(string? raw) {
            if (string.IsNullOrWhiteSpace(raw)) {
                return null;
            }

            var text = raw.Trim();

            // AssemblyInformationalVersion may carry "+<commit sha>", and older tags were prefixed with "v".
            var plus = text.IndexOf('+');
            if (plus >= 0) {
                text = text.Substring(0, plus);
            }

            if (text.StartsWith("v", StringComparison.OrdinalIgnoreCase)) {
                text = text.Substring(1);
            }

            return Version.TryParse(text, out var version) ? version : null;
        }

        /// <summary>
        /// Compares two versions numerically. Returns null when either side can't be parsed, so callers can
        /// decline to act rather than draw a conclusion from a string compare that doesn't mean anything.
        /// </summary>
        public static int? Compare(string? a, string? b) {
            var versionA = Parse(a);
            var versionB = Parse(b);

            if (versionA == null || versionB == null) {
                Logger.Warn($"Could not compare versions \"{a}\" and \"{b}\", at least one is not a valid version.");
                return null;
            }

            return versionA.CompareTo(versionB);
        }

        /// <summary>
        /// True only when both versions parse and <paramref name="version"/> is strictly newer than
        /// <paramref name="other"/>. Unparseable input is never "newer".
        /// </summary>
        public static bool IsNewerThan(string? version, string? other) {
            return Compare(version, other) > 0;
        }

        /// <summary>
        /// True only when both versions parse and <paramref name="version"/> is strictly older than
        /// <paramref name="other"/>. Unparseable input is never "older".
        /// </summary>
        public static bool IsOlderThan(string? version, string? other) {
            return Compare(version, other) < 0;
        }
    }
}
