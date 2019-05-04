using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using EvilsoftCommons.Exceptions;
using IAGrim.UI.Misc.CEF;
using log4net;

namespace IAGrim.Parsers.TransferStash {
    class TransferStashWorker : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TransferStashWorker));
        private BackgroundWorker _bw = new BackgroundWorker();
        private readonly TransferStashService2 _transferStashService;
        private readonly ConcurrentQueue<string> _queuedTransferFiles = new ConcurrentQueue<string>();

        public TransferStashWorker(TransferStashService2 transferStashService) {
            _transferStashService = transferStashService;

            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.RunWorkerAsync();
        }

        public void Queue(string transferFile) {
            if (_queuedTransferFiles.Count < 1 && !string.IsNullOrEmpty(transferFile)) {
                _queuedTransferFiles.Enqueue(transferFile);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "TransferStashWorker";

            BackgroundWorker worker = sender as BackgroundWorker;
            while (!worker.CancellationPending) {
                try {
                    Thread.Sleep(10);
                }
                catch (ThreadInterruptedException) {
                }

                // Nothing queued for looting
                if (_queuedTransferFiles.Count == 0) {
                    continue;
                }
                List<UserFeedback> feedback;
                try {
                    var (isLootable, lootableFeedback) = _transferStashService.IsTransferStashLootable();
                    if (isLootable) {
                        feedback = Execute();
                    } else {
                        feedback = lootableFeedback;
                    }
                }
                catch (NullReferenceException ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    feedback = UserFeedback.FromTagSingleton("iatag_feedback_unable_to_loot_stash");
                }
                catch (IOException ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    Logger.Info("Exception not reported, IOExceptions are bound to happen.");
                    feedback = UserFeedback.FromTagSingleton("iatag_feedback_unable_to_loot_stash");
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionReporter.ReportException(ex, "EmptyPageX??");
                    feedback = UserFeedback.FromTagSingleton("iatag_feedback_unable_to_loot_stash");
                }

                foreach (var message in feedback) {
                    Logger.Warn($"No feedback service yet, so this is it: {message.Message}");
                }
                // TODO: Handle feedback
                // TODO: Handle errors, some kind of ErrorService? -- feedback service will be responsible to cutting out spammed messages. Somehow..
            }
        }

        private List<UserFeedback> Execute() {
            if (_queuedTransferFiles.TryDequeue(out string transferFile)) {
                return _transferStashService.Loot(transferFile);
            }
            else {
                return new List<UserFeedback>(0);
            }
        }

        public void Dispose() {
            _bw?.CancelAsync();
            _bw = null;
        }
    }
}