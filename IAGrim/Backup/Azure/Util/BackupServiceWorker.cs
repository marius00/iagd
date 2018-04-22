using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Azure.Service;
using log4net;

namespace IAGrim.Backup.Azure.Util {
    class BackupServiceWorker : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupServiceWorker));
        private BackgroundWorker _bw = new BackgroundWorker();
        private readonly BackupService _backupService;

        public BackupServiceWorker(BackupService backupService) {
            _backupService = backupService;

            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.RunWorkerAsync();
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "BackupServiceWorker";

            try {
                Logger.Debug("Backup service started, waiting for 10 seconds before initializing..");
                Thread.Sleep(15000);
                Logger.Debug("Backup initializing..");
            }
            catch (Exception ex) {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

            BackgroundWorker worker = sender as BackgroundWorker;
            while (!worker.CancellationPending) {
                try {
                    Thread.Sleep(1000);
                    _backupService.Execute();
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                }
            }
        }

        public void Dispose() {
            _bw?.CancelAsync();
            _bw = null;
        }
    }
}
