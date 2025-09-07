using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.Misc {

    public class BasicBackgroundWorker : IDisposable {
        private BackgroundWorker bw;
        Action<BackgroundWorker, DoWorkEventArgs> Execute;

        public BasicBackgroundWorker(Action<BackgroundWorker, DoWorkEventArgs> action, RunWorkerCompletedEventHandler completed, object args) {
            bw = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.Execute = action;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += completed;
            bw.RunWorkerAsync(args);
        }

        public BasicBackgroundWorker(Action<BackgroundWorker, DoWorkEventArgs> action, ProgressChangedEventHandler progress, object args) {
            bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            this.Execute = action;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.ProgressChanged += progress;
            bw.RunWorkerAsync(args);
        }

        public BasicBackgroundWorker(Action<BackgroundWorker, DoWorkEventArgs> action, RunWorkerCompletedEventHandler completed, ProgressChangedEventHandler progress, object args) {
            bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            this.Execute = action;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerCompleted += completed;
            bw.ProgressChanged += progress;
            bw.RunWorkerAsync(args);
        }

        public BasicBackgroundWorker(Action<BackgroundWorker, DoWorkEventArgs> action, object args) {
            bw = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.Execute = action;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync(args);
        }


        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            this.Execute(sender as BackgroundWorker, e);
        }

        public void Dispose() {
            if (bw != null) {
                bw.CancelAsync();
                bw = null;
            }
        }

        ~BasicBackgroundWorker() {
            Dispose();
        }

    }
}
