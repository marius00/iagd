using System;
using System.IO;
using System.Security.Permissions;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    class CsvFileMonitor : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CsvFileMonitor));
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        public event EventHandler OnModified;

        public class CsvEvent : EventArgs {
            public CsvEvent(string filename) {
                Filename = filename;
                Cooldown = new ActionCooldown(2500, true);
            }
            public string Filename { get; }

            public ActionCooldown Cooldown { get; }
        }

        ~CsvFileMonitor() {
            Dispose();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool StartMonitoring(string path, string filter) {
            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                    NotifyFilters.DirectoryName;
            _watcher.Filter = filter;
            _watcher.IncludeSubdirectories = false;
            _watcher.Created += _watcher_Created;
            _watcher.Renamed += _watcher_Created;

            _watcher.Error += Watcher_Error;
            _watcher.EnableRaisingEvents = true;

            Logger.InfoFormat("Monitoring at: {0}", _watcher.Path);
            return true;
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e) {
            OnModified?.Invoke(this, new CsvEvent(e.FullPath));
        }

        private void Watcher_Error(object sender, ErrorEventArgs e) {
            Logger.Info(e.GetException().Message);
        }

        public void Dispose() {
            if (_watcher != null) {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }
    }
}