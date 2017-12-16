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
        private static ILog logger = LogManager.GetLogger(typeof(BackgroundTask));
        private BackgroundWorker bw = new BackgroundWorker();
        private bool disposed = false;

        public BackgroundTask(ICloudBackup handler) {
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync(handler);
        }



        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            try {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "Backup";

                BackgroundWorker worker = sender as BackgroundWorker;
                ICloudBackup b = e.Argument as ICloudBackup;
                while (!worker.CancellationPending) {
                    Thread.Sleep(10);
                    b.Update();
                }
            }
            catch (Exception ex) {
                logger.Fatal(ex.Message);
                logger.Fatal(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
                throw;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed && disposing) {

                if (bw != null) {
                    bw.CancelAsync();
                    bw = null;
                }
            }
            disposed = true;
        }


        public void Dispose() {
            Dispose(true);
        }
    }
}
