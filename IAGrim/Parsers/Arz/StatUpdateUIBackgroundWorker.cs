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
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StatUpdateUIBackgroundWorker));
        private BackgroundWorker _bw = new BackgroundWorker();
        private bool _disposed = false;
        private readonly IPlayerItemDao _playerItemDao;

        public StatUpdateUIBackgroundWorker(IPlayerItemDao playerItemDao, RunWorkerCompletedEventHandler completed, ProgressChangedEventHandler progress) {
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            this._playerItemDao = playerItemDao;
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.RunWorkerCompleted += completed;
            _bw.ProgressChanged += progress;

            _bw.RunWorkerAsync();
        }




        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            try {
                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "StatParserUI";
                ExceptionReporter.EnableLogUnhandledOnThread();

                Logger.Info("Updating player stats");

                BackgroundWorker worker = sender as BackgroundWorker;


                // Update all stats
                IList<PlayerItem> items = _playerItemDao.ListAll();
                worker.ReportProgress(Math.Max(100, items.Count * 3), 1);


                int total = 0;
                _playerItemDao.UpdateAllItemStats(items, (p) => {
                    worker.ReportProgress(total++, 0);
                });
                Logger.Info("Updated item stats");
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
            //GC.SuppressFinalize(this);
        }
    }
}
