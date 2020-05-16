using IAGrim.Parsers.Arz;
using IAGrim.UI;
using IAGrim.Utilities.HelperClasses;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace IAGrim.Utilities {
    static class RuntimeSettings {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RuntimeSettings));

        public static void InitializeLanguage(string localizationFile, Dictionary<string, string> dbTags) {
            var english = new EnglishLanguage(dbTags);
            if (string.IsNullOrEmpty(localizationFile)) {
                Language = english;
            } else if (!File.Exists(localizationFile)) {
                Language = english;
                Logger.Warn($"Could not locate {localizationFile}, defaulting to English.");
            }
            else {
                Language = new LocalizationLoader().LoadLanguage(localizationFile, english);
            }
        }

        public static string Uuid { get; set; }


        public static event EventHandler StashStatusChanged;
        private static StashAvailability _stashStatus = StashAvailability.UNKNOWN;

        public static StashAvailability PreviousStashStatus { get; private set; } = StashAvailability.UNKNOWN;

        public static StashAvailability StashStatus {
            get {
                return _stashStatus;
            }
            set {
                if (_stashStatus != value) {
                    PreviousStashStatus = _stashStatus;
                    _stashStatus = value;

                    StashStatusChanged?.Invoke(null, null);
                }
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

    }
}
