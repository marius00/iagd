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
    class UsageStatisticsReporter : IDisposable {
        private Timer _timer;
        private DateTime _lastTimeNotMinimized = DateTime.UtcNow;

        public UsageStatisticsReporter() {
            var reportUsageStatistics = new Stopwatch();
            reportUsageStatistics.Start();


            int min = 1000 * 60;
            int hour = 60 * min;
            _timer = new Timer();
            _timer.Start();
            _timer.Elapsed += (a1, a2) => {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "ReportUsageThread";
                }

                // If the application has been minimized for 38 hours or more, stop checking for updates / statistics.
                // This keeps IA from bringing annoying update notifications to those who just run it but no longer uses it (maybe just on weekends etc)
                // And keeps daily usage statistics semi accurate, not counting the idlers.
                if ((DateTime.UtcNow - _lastTimeNotMinimized).TotalHours < 38) {
                    if (reportUsageStatistics.Elapsed.Hours > 12) {
                        ReportUsage();
                        reportUsageStatistics.Restart();
                    }
                }
            };
            _timer.Interval = 12 * hour;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void ResetLastMinimized() {
            _lastTimeNotMinimized = DateTime.UtcNow;
        }

        /// <summary>
        /// Report usage once every 12 hours, in case the user runs it 'for ever'
        /// Will halt if not opened for 38 hours
        /// </summary>
        private void ReportUsage() {
            ThreadPool.QueueUserWorkItem(m => ExceptionReporter.ReportUsage());
        }

        public void Dispose() {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
