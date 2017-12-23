using IAGrim.Parsers.Arz;
using IAGrim.UI;
using IAGrim.Utilities.HelperClasses;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IAGrim.Utilities {
    static class GlobalSettings {
        static GlobalSettings() {
            if (string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile) || !File.Exists(Properties.Settings.Default.LocalizationFile))
                Language = new EnglishLanguage();
            else {
                Language = new LocalizationLoader().LoadLanguage(Properties.Settings.Default.LocalizationFile);
            }
        }

        public static string RemoteBackupServer {
            get {

#if DEBUG
                return "http://items.dreamcrash.org";
                //return "http://localhost:53726";
#else
                return "http://items.dreamcrash.org";
#endif
            }
        }

        public static string Uuid { get; set; }


        public static event EventHandler StashStatusChanged;
        private static StashAvailability _stashStatus = StashAvailability.UNKNOWN;
        private static StashAvailability _previousStashStatus = StashAvailability.UNKNOWN;

        public static StashAvailability PreviousStashStatus => _previousStashStatus;

        public static bool GrimDawnRunning { get; set; } // V1.0.4.0 hotfix
        public static StashAvailability StashStatus {
            get {
                return _stashStatus;
            }
            set {
                if (_stashStatus != value) {
                    _previousStashStatus = _stashStatus;
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
