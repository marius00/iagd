using IAGrim.Parsers.Arz;
using IAGrim.Services;
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

        public static string Uuid { get; set; }


        private static volatile StashAvailability _stashStatus = StashAvailability.UNKNOWN;

        public static StashAvailability PreviousStashStatus { get; private set; } = StashAvailability.UNKNOWN;

        public static StashAvailability StashStatus {
            get => _stashStatus;
            set {
                if (_stashStatus == value) return;


                PreviousStashStatus = _stashStatus;
                _stashStatus = value;
            }
        }

        private static ILocalizedLanguage _language;
        public static ILocalizedLanguage Language {
            get {
                return _language;
            }
            set {
                _language = value;
                StatManager = new StatManager(value);
            }
        }
        public static StatManager StatManager { get; set; }

        public static ReplicaStatResolver ReplicaStatResolver { get; set; }

    }
}
