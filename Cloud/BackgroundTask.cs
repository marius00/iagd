using log4net;
using EvilsoftCommons.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace EvilsoftCommons.Cloud {
    public class BackgroundTask : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackgroundTask));
        private BackgroundWorker? _bw = new BackgroundWorker();
        private bool _disposed = false;

        public BackgroundTask(ICloudBackup handler) {
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.RunWorkerAsync(handler);
        }



        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            try {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "Backup";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }
                ExceptionReporter.EnableLogUnhandledOnThread();

                BackgroundWorker worker = sender as BackgroundWorker;
                ICloudBackup b = e.Argument as ICloudBackup;
                while (!worker.CancellationPending) {
                    Thread.Sleep(10);
                    b.Update();
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
                throw;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed && disposing) {

                if (_bw != null) {
                    _bw.CancelAsync();
                    _bw = null;
                }
            }
            _disposed = true;
        }


        public void Dispose() {
            Dispose(true);
        }
    }
}
