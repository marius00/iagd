using IAGrim.Parsers.Arz;
using IAGrim.UI;
using IAGrim.Utilities.HelperClasses;
using StatTranslator;

using log4net;

namespace IAGrim.Utilities {
    static class RuntimeSettings {
        public static void InitializeLanguage(string languageCode, Dictionary<string, string> dbTags) {
            var english = new EnglishLanguage(dbTags);
            if (string.IsNullOrEmpty(languageCode) || languageCode.Equals("EN", System.StringComparison.OrdinalIgnoreCase)) {
                Language = english;
            }
            else {
                Language = new LocalizationLoader().LoadLanguage(languageCode, english);
            }
        }

        public static string? Uuid { get; set; }

        private static ILocalizedLanguage? _language;
        public static ILocalizedLanguage? Language {
            get {
                return _language;
            }
            set {
                _language = value;
                StatManager = value == null ? null : new StatManager(value);
            }
        }
        public static StatManager? StatManager { get; set; }

    }
}
