using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Web;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.Service;
using log4net;

namespace IAGrim.Backup.Cloud.Util {
    class BackupServiceWorker : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupServiceWorker));
        private BackgroundWorker _bw = new BackgroundWorker();
        private readonly BackupService _backupService;
        private readonly CharacterBackupService _characterBackupService;

        public BackupServiceWorker(BackupService backupService, CharacterBackupService characterBackupService) {
            _backupService = backupService;
            _characterBackupService = characterBackupService;

            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.RunWorkerAsync();
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "BackupServiceWorker";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            ExceptionReporter.EnableLogUnhandledOnThread();

            try {
                Logger.Debug("Backup service started, waiting for 10 seconds before initializing..");
                Thread.Sleep(15000);
                Logger.Debug("Backup initializing..");
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
            }

            BackgroundWorker worker = sender as BackgroundWorker;
            while (!worker.CancellationPending) {
                try {
                    Thread.Sleep(1000);
                    _backupService.Execute();
                    _characterBackupService.Execute();
                }
                catch (HttpException ex) {
                    if (ex.WebEventCode == (int)HttpStatusCode.Unauthorized) {
                        _backupService.Logout();
                    }
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
            }
        }

        public void Dispose() {
            _bw?.CancelAsync();
            _bw = null;
        }
    }
}
