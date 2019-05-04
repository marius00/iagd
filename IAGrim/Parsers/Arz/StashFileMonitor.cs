using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Timers;
using IAGrim.Parsers.Arz.dto;
using log4net;

namespace IAGrim.Parsers.Arz {
    class StashFileMonitor : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StashFileMonitor));
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        public event EventHandler OnStashModified;

        ~StashFileMonitor() {
            Dispose();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public bool StartMonitorStashfile(string path) {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path)) {

                _watcher = new FileSystemWatcher();
                _watcher.Path = path;
                _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                        NotifyFilters.DirectoryName;
                _watcher.Filter = "transfer.gs?";
                _watcher.IncludeSubdirectories = true;
                _watcher.Renamed += OnRenamed;
                _watcher.Error += Watcher_Error;
                _watcher.EnableRaisingEvents = true;

                Logger.InfoFormat("Monitoring stashfiles at: {0}", _watcher.Path);
                return true;
            }

            return false;
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
        
        private void OnRenamed(object source, RenamedEventArgs e) {
            // Specify what is done when a file is renamed.
            Logger.DebugFormat("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            if (!e.FullPath.EndsWith(".bak")) {
                if (File.Exists(e.FullPath)) {
                    OnStashModified?.Invoke(this, new StashEventArg(e.FullPath));
                }
                else {
                    Logger.Warn("Detected an update to stash file, but stash file does not appear to exist.");
                }
            }
        }
    }
}