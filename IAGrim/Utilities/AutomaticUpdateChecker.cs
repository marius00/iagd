using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using AutoUpdaterDotNET;
using EvilsoftCommons.Exceptions;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using Timer = System.Timers.Timer;

namespace IAGrim.Utilities {
    class AutomaticUpdateChecker {
        private Timer _timer;
        private DateTime _lastTimeNotMinimized = DateTime.UtcNow;
        private DateTime _lastAutomaticUpdateCheck = DateTime.UtcNow;
        private readonly SettingsService _settings;

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        public const string Url = "https://grimdawn.evilsoft.net/version.php";

        private string GetUpdateXml(bool requestLatest) {
            var version = ExceptionReporter.VersionString;

            var is64Bit = Environment.Is64BitOperatingSystem;
            if (requestLatest) {
                return $"{Url}?beta&version={version}&is64bit={is64Bit}";
            }

            return $"{Url}?version={version}&is64bit={is64Bit}";
        }

        public AutomaticUpdateChecker(SettingsService settings) {
            _settings = settings;
            int min = 1000 * 60;
            int hour = 60 * min;
            _timer = new Timer();
            _timer.Start();
            _timer.Elapsed += (a1, a2) => {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "CheckUpdatesThread";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }

                bool hasBeenMinimizedRecently = (DateTime.UtcNow - _lastTimeNotMinimized).TotalHours < 38;
                bool hasCheckedForUpdatesRecently = (DateTime.UtcNow - _lastAutomaticUpdateCheck).TotalHours < 36;
                if (GetTickCount64() > 5 * 60 * 1000 && hasBeenMinimizedRecently && !hasCheckedForUpdatesRecently) {
                    CheckForUpdates();
                    _lastAutomaticUpdateCheck = DateTime.UtcNow;
                }
            };
            _timer.Interval = 12 * hour;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void ResetLastMinimized() {
            _lastTimeNotMinimized = DateTime.UtcNow;
        }


        // Intentionally public -- Do not refactor.
        public void CheckForUpdates(bool manualUpdate = false) {
            AutoUpdater.LetUserSelectRemindLater = !manualUpdate;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            AutoUpdater.RemindLaterAt = 7;
            AutoUpdater.Start(GetUpdateXml(manualUpdate || _settings.GetPersistent().SubscribeExperimentalUpdates));
        }

        public void Dispose() {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}