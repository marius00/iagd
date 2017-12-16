using EvilsoftCommons.Exceptions;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using IAGrim.Database.Interfaces;

namespace IAGrim.BuddyShare {
    class BuddyBackgroundThread : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BuddyBackgroundThread));
        private BackgroundWorker bw = new BackgroundWorker();
        private bool _disposed = false;
        
        public const int ProgressStoreBuddydata = 1;
        public const int ProgressSetUid = 2;
        private List<long> _subscribers;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly ActionCooldown _cooldown;

        public BuddyBackgroundThread(
            ProgressChangedEventHandler progressCallback, 
            IPlayerItemDao playerItemDao,
            IBuddyItemDao buddyItemDao,
            List<long> subscribers,
            long cooldown
        ) {
            _playerItemDao = playerItemDao;
            _buddyItemDao = buddyItemDao;
            this._subscribers = subscribers;
            _cooldown = new ActionCooldown(cooldown);

            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += progressCallback;
            bw.RunWorkerAsync();
        }



        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "BuddyBackground";

            BackgroundWorker worker = sender as BackgroundWorker;
            Serializer serializer = new Serializer(_buddyItemDao, _playerItemDao);
            Synchronizer synchronizer = new Synchronizer();


            worker?.ReportProgress(ProgressSetUid, synchronizer.CreateUserId());

            while (!worker.CancellationPending) {
                try {
                    Thread.Sleep(10);

                    if (_cooldown.IsReady) {
                        // Sync up, if UI thread has sent us any data.
                        SerializedPlayerItems element = serializer.Serialize();
                        if (element != null) {
                            if (!synchronizer.UploadBuddyData(element)) {
                                Logger.Warn("Buddy items upload failed");
                            }
                        }
                        else {
                            Logger.Warn("Buddy Items upload NULL!?");
                        }



                        // Sync down, and pass data to UI thread
                        List<SerializedPlayerItems> syncdownData = synchronizer.DownloadBuddyItems(_subscribers);
                        if (syncdownData != null && syncdownData.Count > 0) {
                            Logger.InfoFormat("Downloaded itemdata for {0} buddies.", syncdownData.Count);

                            var deserializer = new Serializer(_buddyItemDao, _playerItemDao);
                            foreach (var item in syncdownData) {
                                if (item.UserId > 0) {
                                    deserializer.DeserializeAndSave(item);
                                }
                            }

                            worker.ReportProgress(ProgressStoreBuddydata);
                        }

                        _cooldown.Reset();
                    }
                }
                catch (NullReferenceException ex) {
                    Logger.Info("The following exception is logged, but can safely be ignored:");
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);                    
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                }
            }
        }

        void Dispose(bool disposing) {
            if (!_disposed && disposing) {

                if (bw != null) {
                    bw.CancelAsync();
                    bw = null;
                }
            }
            _disposed = true;
        }


        public void Dispose() {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }
}
