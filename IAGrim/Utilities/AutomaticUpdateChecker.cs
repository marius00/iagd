using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoUpdaterDotNET;
using EvilsoftCommons.Exceptions;
using IAGrim.Properties;
using log4net.Repository.Hierarchy;
using Timer = System.Timers.Timer;

namespace IAGrim.Utilities {
    class AutomaticUpdateChecker {
        private Timer _timer;
        private DateTime _lastTimeNotMinimized = DateTime.UtcNow;
        private DateTime _lastAutomaticUpdateCheck = DateTime.UtcNow;

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        private string UPDATE_XML {
            get {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                string version = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";

                if ((bool)Settings.Default.SubscribeExperimentalUpdates) {
                    return $"http://grimdawn.dreamcrash.org/ia/version.php?beta&version={version}";
                }
                return $"http://grimdawn.dreamcrash.org/ia/version.php?version={version}";
            }
        }

        public AutomaticUpdateChecker() {
            int min = 1000 * 60;
            int hour = 60 * min;
            _timer = new Timer();
            _timer.Start();
            _timer.Elapsed += (a1, a2) => {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "CheckUpdatesThread";
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


        public void CheckForUpdates() {
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            AutoUpdater.RemindLaterAt = 7;
            AutoUpdater.Start(UPDATE_XML);
        }

        public void Dispose() {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
