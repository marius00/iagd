using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using CefSharp.DevTools.SystemInfo;
using EvilsoftCommons.Exceptions;
using log4net;

namespace IAGrim.Database.Synchronizer.Core {
    /// <summary>
    /// The thread executor ensures that all calls to SQLite runs on the same thread.
    /// SQLite has no support for access across multiple threads.
    /// </summary>
    public class ThreadExecuter : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThreadExecuter));
        private readonly ConcurrentDictionary<AutoResetEvent, object> _results = new ConcurrentDictionary<AutoResetEvent, object>();
        private readonly ConcurrentQueue<QueuedExecution> _queue = new ConcurrentQueue<QueuedExecution>();
        private Thread _thread;
        private volatile bool _isCancelled;
        private bool _exceptionOverride = false;
#if DEBUG
        public const int ThreadTimeout = 1000 * 60 * 20;
#else
        public const int ThreadTimeout = 1000 * 60 * 30;
#endif

        public ThreadExecuter() {
            _isCancelled = false;
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();

            while (!_thread.IsAlive) ;
        }

        private void Run() {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "SQL";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            ExceptionReporter.EnableLogUnhandledOnThread();

            while (!_isCancelled) {
                if (_queue.TryDequeue(out var elem)) {
                    try {
                        elem.IsStarted = true;
                        if (elem.IsLongRunning) {
                            _exceptionOverride = true;
                            Logger.Info("Initiating long running operation");
                        }

                        if (elem.Func != null)
                            _results[elem.Trigger] = elem.Func();
                        else
                            elem.Action();
                    }
                    catch (Exception ex) {
                        _results[elem.Trigger] = ex;
                    }

                    elem.Trigger.Set();

                    if (_exceptionOverride) {
                        _exceptionOverride = false;
                        Logger.Info("Ending long running operation");
                    }
                }

                try {
                    Thread.Sleep(1);
                }
                catch (Exception) {
                }
            }
        }

        private void AwaitTimeout(AutoResetEvent ev, int timeout, QueuedExecution item) {
            if (ev.WaitOne(timeout, true)) return;


            // We're in a expected long running operation. Just wait.
            while (_exceptionOverride) {
                Logger.Info("Timed out during long running operation, awaiting..");
                if (ev.WaitOne(timeout / 10, true)) {
                    return; // Success
                }
            }

            throw new Exception($"Operation never terminated: Started: {item.IsStarted}");
        }

        public void Execute(Action func, int timeout = ThreadTimeout, bool expectLongOperation = false) {
            if (_thread == null)
                throw new InvalidOperationException("Object has been disposed");
            AutoResetEvent ev = new AutoResetEvent(false);

            var item = new QueuedExecution {
                Action = () => func(),
                Trigger = ev,
                IsLongRunning = expectLongOperation
            };
            _queue.Enqueue(item);

            AwaitTimeout(ev, timeout, item);

            if (_results.ContainsKey(ev)) {
                object val;
                if (_results.TryRemove(ev, out val)) {
                    var ex = val as Exception;
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        public T Execute<T>(Func<T> func) {
            return Execute(func, ThreadTimeout);
        }

        private T Execute<T>(Func<T> func, int timeout) {
            if (_thread == null)
                throw new InvalidOperationException("Object has been disposed");
            AutoResetEvent ev = new AutoResetEvent(false);

            var item = new QueuedExecution {
                Func = () => func(),
                Trigger = ev
            };
            _queue.Enqueue(item);


            AwaitTimeout(ev, timeout, item);


            object result;

            if (_results.TryRemove(ev, out result)) {
                Exception ex = result as Exception;
                if (ex != null) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                    ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                }
            }

            return (T) result;
        }

        ~ThreadExecuter() {
            Dispose();
        }

        public void Dispose() {
            _isCancelled = true;
            _exceptionOverride = false;
            _thread = null;
        }


        class QueuedExecution {
            public Func<object> Func { get; set; }
            public Action Action { get; set; }
            public AutoResetEvent Trigger { get; set; }

            /// <summary>
            /// Helps track down which operation stalled.
            /// Multiple operations can be queued and time out, but only one of them will have started.
            /// </summary>
            public volatile bool IsStarted = false;

            public bool IsLongRunning { get; set; }= false;
        }
    }
}