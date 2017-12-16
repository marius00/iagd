using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IAGrim.Parsers.Arz {
    class StatUpdateUIBackgroundWorker : IDisposable {
        private static ILog logger = LogManager.GetLogger(typeof(ParsingUiBackgroundWorker));
        private BackgroundWorker bw = new BackgroundWorker();
        private bool disposed = false;
        private readonly IPlayerItemDao playerItemDao;

        public StatUpdateUIBackgroundWorker(IPlayerItemDao playerItemDao, RunWorkerCompletedEventHandler completed, ProgressChangedEventHandler progress) {
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            this.playerItemDao = playerItemDao;
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerCompleted += completed;
            bw.ProgressChanged += progress;

            bw.RunWorkerAsync();
        }




        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            try {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "StatParserUI";

                logger.Info("Updating player stats");

                BackgroundWorker worker = sender as BackgroundWorker;


                // Update all stats
                playerItemDao.ClearAllItemStats();
                IList<PlayerItem> items = playerItemDao.ListAll();
                worker.ReportProgress(Math.Max(100, items.Count), 1);


                int total = 0;
                playerItemDao.UpdateAllItemStats(items, (p) => {
                    worker.ReportProgress(total++, 0);
                });
                logger.Info("Updated item stats");
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
            //GC.SuppressFinalize(this);
        }
    }
}
