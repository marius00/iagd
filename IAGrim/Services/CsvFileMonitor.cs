using System;
using System.IO;
using System.Security.Permissions;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.Services {
    class CsvFileMonitor : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CsvFileMonitor));
        private FileSystemWatcher? _watcher = new();
        public event EventHandler<CsvEvent>? OnModified;
        private volatile Thread? _thread = null;

        public class CsvEvent(string filename) : EventArgs {
            public string Filename { get; } = filename;

            public ActionCooldown Cooldown { get; } = new(2500, true);
        }

        ~CsvFileMonitor() {
            Dispose();
        }

        public bool StartMonitoring(string path, string filter) {
            if (WineDetector.IsRunningInWine()) {
                Logger.Info("Running under wine, using alternate scanner flow");
                _thread = new Thread(() => FileSystemScanner(path, filter)) {
                    IsBackground = true
                };
                _thread.Start();
            }
            else {
                Logger.Info("Running under windows, using regular event flow");
                _watcher = new FileSystemWatcher();
                _watcher.Path = path;
                _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                        NotifyFilters.DirectoryName;
                _watcher.Filter = filter;
                _watcher.IncludeSubdirectories = false;
                _watcher.Created += _watcher_Created;
                _watcher.Renamed += _watcher_Created;
                _watcher.InternalBufferSize = 65536;

                _watcher.Error += Watcher_Error;
                _watcher.EnableRaisingEvents = true;

                Logger.InfoFormat("Monitoring at: {0}", _watcher.Path);
            }

            return true;
        }

        private void FileSystemScanner(string path, string filter) {
            var knownFiles = new HashSet<string>(
                Directory.GetFiles(path, filter).Select(Path.GetFileName)
            );


            while (_thread != null) {
                try {
                    var files = Directory.GetFiles(path, filter)
                        .Select(Path.GetFileName);

                    foreach (var file in files) {
                        if (knownFiles.Contains(file)) continue;

                        knownFiles.Add(file);
                        OnModified?.Invoke(this, new CsvEvent(Path.Combine(path, file)));
                    }
                }
                catch (Exception ex) {
                    Logger.Error("Polling watcher error", ex);
                }

                Thread.Sleep(2500);
            }
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

            _thread = null;
        }
    }
}