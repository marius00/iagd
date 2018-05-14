using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using IAGrim.Parsers.Arz.dto;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Parsers.Arz {
    class StashFileMonitor : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StashFileMonitor));
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private Timer _delayedLootTimer;
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
                _watcher.Changed += (sender, args) => OnChanged(args); //new FileSystemEventHandler(OnChanged);
                _watcher.Created += (sender, args) => OnChanged(args); //new FileSystemEventHandler(OnChanged);
                _watcher.Deleted += (sender, args) => OnChanged(args); //new FileSystemEventHandler(OnChanged);
                _watcher.Renamed += OnRenamed;


                _watcher.Error += Watcher_Error;
                _watcher.EnableRaisingEvents = true;

                Logger.InfoFormat("Monitoring stashfiles at: {0}", _watcher.Path);
                return true;
            }

            return false;
        }

        public void CancelQueuedNotify() {
            _delayedLootTimer?.Stop();
            _delayedLootTimer = null;
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

        private void NotifyStashChangeDelayed(string filename) {
            _delayedLootTimer?.Stop();

            _delayedLootTimer = new Timer { Enabled = true, Interval = 1500, AutoReset = true };
            _delayedLootTimer.Elapsed += (sender, e) => {
                OnStashModified?.Invoke(this, new StashEventArg(filename));
            };
            _delayedLootTimer.Start();
        }


        private static void OnChanged(FileSystemEventArgs e) {
            Logger.Debug("File: " + e.FullPath + " " + e.ChangeType);

            /*
                        // Specify what is done when a file is changed, created, or deleted.
                        if (e.FullPath.EndsWith(".gst") || e.FullPath.EndsWith(".gsh")) {
                            Logger.Debug("File: " + e.FullPath + " " + e.ChangeType);
                            using (TemporaryCopy copy = new TemporaryCopy(e.FullPath)) {
                                obj.LastSeenIsHardcore = GlobalSettings.IsHardcore(copy.Filename);

                                GDCryptoDataBuffer pCrypto = new GDCryptoDataBuffer(DataBuffer.ReadBytesFromDisk(copy.Filename));

                                Stash stash = new Stash();
                                if (stash.Read(pCrypto)) {
                                    obj.LastSeenModLabel = stash.ModLabel;
                                }
                            }
                        }
            */

            // Logger.InfoFormat("Detected a change");
        }

        private void OnRenamed(object source, RenamedEventArgs e) {
            // Specify what is done when a file is renamed.
            Logger.DebugFormat("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            if (!e.FullPath.EndsWith(".bak")) {
                if (File.Exists(e.FullPath)) {
                    if (GlobalSettings.PreviousStashStatus == StashAvailability.CRAFTING ||
                        GlobalSettings.StashStatus == StashAvailability.CRAFTING) {
                        Logger.Info("Detected an update to stash file, but ignoring due to crafting-safety-check");
                        // OBS: Can only do this if we've previously looted! CAnnot risk it containing unlooted items
                        if (StashManager._hasLootedItemsOnceThisSession) {
                            if (_delayedLootTimer == null) {
                                Logger.Info(
                                    "Items has already been looted this session, post-crafting safety measures required.");


                                StashManager.DeleteItemsInPageX(e.FullPath);
                            }
                            else {
                                Logger.Info("Player may have opened devotion screen before running away.. leaving items be..");
                            }
                        }
                        else {
                            Logger.Info("No items has been looted this session, ignoring safety measures.");
                        }
                    }
                    else {
                        Logger.Info("Detected an update to stash file, checking for loot..");

                        NotifyStashChangeDelayed(e.FullPath);
                    }
                }
                else {
                    Logger.Warn("Detected an update to stash file, but stash file does not appear to exist.");
                }
            }
        }
    }
}
