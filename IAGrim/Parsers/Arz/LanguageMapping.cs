using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IAGrim.Parsers.Arz {
    /// <summary>
    /// Maps GD language codes (from Text_XX.arc filenames) to display names,
    /// and resolves bundled IA translation override files.
    /// </summary>
    public static class LanguageMapping {
        public static readonly Dictionary<string, string> CodeToDisplayName = new Dictionary<string, string> {
            { "EN", "English" },
            { "CS", "Čeština" },
            { "DE", "Deutsch" },
            { "ES", "Español" },
            { "FR", "Français" },
            { "IT", "Italiano" },
            { "JA", "日本語" },
            { "KO", "한국어" },
            { "PL", "Polski" },
            { "PT", "Português (BR)" },
            { "RU", "Русский" },
            { "VI", "Tiếng Việt" },
            { "ZH", "简体中文" },
        };

        /// <summary>
        /// Language codes for which we ship a full IA translation override file.
        /// </summary>
        private static readonly HashSet<string> FullySupportedCodes = new HashSet<string> {
            "ES", "JA", "ZH", "PT", "RU"
        };

        public static bool IsFullySupported(string code) => FullySupportedCodes.Contains(code.ToUpperInvariant());

        public static string GetDisplayName(string code) {
            return CodeToDisplayName.TryGetValue(code.ToUpperInvariant(), out var name) ? name : code;
        }

        /// <summary>
        /// Returns the path to the bundled IA translation override file, or null if it doesn't exist.
        /// </summary>
        public static string? GetIaTranslationFile(string code) {
            if (string.IsNullOrEmpty(code) || code.Equals("EN", System.StringComparison.OrdinalIgnoreCase))
                return null;

            var appDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(appDir, "Resources", "translations", $"{code.ToLowerInvariant()}.txt");
            return File.Exists(path) ? path : null;
        }

        /// <summary>
        /// Scans a GD install path's resources folder for available Text_XX.arc files,
        /// returns the distinct language codes found.
        /// </summary>
        public static IEnumerable<string> GetAvailableLanguages(IEnumerable<string> grimDawnPaths) {
            var codes = new HashSet<string>();
            foreach (var basePath in grimDawnPaths) {
                var resourcesDir = Path.Combine(basePath, "resources");
                if (!Directory.Exists(resourcesDir)) continue;

                foreach (var file in Directory.EnumerateFiles(resourcesDir, "Text_*.arc")) {
                    var fileName = Path.GetFileNameWithoutExtension(file); // e.g. "Text_DE"
                    var code = fileName.Substring("Text_".Length).ToUpperInvariant();
                    codes.Add(code);
                }
            }

            return codes.OrderBy(c => c);
        }
    }
}


