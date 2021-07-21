using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DllInjector.UI;
using EvilsoftCommons.DllInjector;
using EvilsoftCommons.Exceptions;
using log4net;

namespace DllInjector {
    public class InjectionHelper : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InjectionHelper));
        private BackgroundWorker _bw;
        private readonly HashSet<uint> _previouslyInjected = new HashSet<uint>();
        private readonly HashSet<uint> _dontLog = new HashSet<uint>();
        private readonly RunArguments _exitArguments;
        private bool _hasWarnedPermissionError = false;

        public const int INJECTION_ERROR = 0;
        public const int NO_PROCESS_FOUND_ON_STARTUP = 1;
        public const int NO_PROCESS_FOUND = 2;
        public const int INJECTION_ERROR_POSSIBLE_ACCESS_DENIED = 3;

        private readonly ProgressChangedEventHandler _registeredProgressCallback;

        class RunArguments {
            public string WindowName;
            public string ClassName;
            public string DllName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressChanged">Callback for errors and notifications</param>
        /// <param name="unloadOnExit">Whetever this DLL should get unloaded on exit</param>
        /// <param name="windowName">Name of the window you wish to inject to</param>
        /// <param name="className">Name of the class you wish to inject to (IFF window name is empty/null)</param>
        /// <param name="dll">Name of the DLL you wish to inject</param>
        public InjectionHelper(BackgroundWorker bw, ProgressChangedEventHandler progressChanged, bool unloadOnExit, string windowName, string className, string dll) {
            if (string.IsNullOrEmpty(windowName) && string.IsNullOrEmpty(className)) {
                throw new ArgumentException("Either window or class name must be specified");
            }
            else if (string.IsNullOrEmpty(dll)) {
                throw new ArgumentException("DLL name must be specified");
            }

            this._registeredProgressCallback = progressChanged;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += progressChanged;

            _exitArguments = new RunArguments {
                WindowName = windowName,
                ClassName = className,
                DllName = dll
            };
            bw.RunWorkerAsync(_exitArguments);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "InjectionHelper";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            ExceptionReporter.EnableLogUnhandledOnThread();

            try {
                BackgroundWorker worker = sender as BackgroundWorker;
                
                while (!worker.CancellationPending) {
                    if (!File.Exists("DllInjector64.exe") || !File.Exists("DllInjector32.exe")) {
                        new AvastedWarning().ShowDialog();
                        Logger.Fatal("Shutting down injection helper. End user has been avasted and IA is now inoperational until reinstalled.");
                        return;
                    }
                    else {
                        Process(worker, e.Argument as RunArguments);
                    }
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
                throw;
            }
        }

        public void Dispose() {
            if (_bw != null) {
                _bw.ProgressChanged -= _registeredProgressCallback;
                _bw.CancelAsync();
                _bw = null;
            }
        }


        private HashSet<uint> FindProcesses(RunArguments args) {

            if (!string.IsNullOrEmpty(args.WindowName)) {
                return EvilsoftCommons.DllInjector.DllInjector.FindProcessForWindow(args.WindowName);
            }
            else {
                throw new NotSupportedException("Class name provided instead of window name, not yet implemented.");
            }
        }

        private bool Is64Bit(int pid, BackgroundWorker worker) {
            try {
                var p = System.Diagnostics.Process.GetProcessById(pid);
                return EvilsoftCommons.DllInjector.DllInjector.Is64BitProcess(p);
            }
            catch (Win32Exception ex) {
                Logger.Warn("Error checking 32/64bit status of GD, if this was an access denied error, try running as administrator.");
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                if (!_hasWarnedPermissionError) {
                    worker.ReportProgress(INJECTION_ERROR_POSSIBLE_ACCESS_DENIED, null);
                    _hasWarnedPermissionError = true;
                }

                return false;
            }
            catch (ArgumentException) {
                return false;
            }
        }

        private static IntPtr Inject64Bit(string exe, string dll) {
            return InjectXBit("DllInjector64.exe", exe, dll);
        }

        private static IntPtr Inject32Bit(string exe, string dll) {
            return InjectXBit("DllInjector32.exe", exe, dll);
        }

        private static IntPtr InjectXBit(string injector, string exe, string dll) {
            Logger.Info($"Running {injector}...");
            if (File.Exists(injector)) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = injector;
                startInfo.Arguments = $"-t 1 \"{exe}\" \"{dll}\"";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;

                startInfo.CreateNoWindow = true;

                Process processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.EnableRaisingEvents = true;
                try {
                    int timeout = 1000;
                    processTemp.Start();
                    processTemp.WaitForExit(timeout);
                    if (!processTemp.HasExited) {
                        Logger.Warn($"Injector did not finish in {timeout}ms, discarding result");
                        return IntPtr.Zero;
                    }

                    
                    while (!processTemp.StandardOutput.EndOfStream) {
                        string output = processTemp.StandardOutput.ReadLine();
                        if (processTemp.ExitCode != 0) {
                            Logger.Warn($"Injector returned status code {processTemp.ExitCode} with error: {output}");
                            return IntPtr.Zero;
                        }
                        else {
                            Logger.Info("Injection successful, storing mock point due to pointer space being outside of 32bit range.");
                            return new IntPtr(0xBADF00D);
                        }
                    }
                }
                catch (Exception ex) {
                    Logger.Warn($"Exception while attempting to verify injection.. {ex.Message}", ex);
                }
            }
            else {
                Logger.Warn($"Could not find {injector}, unable to inject into Grim Dawn.");
            }
            return IntPtr.Zero;
        }

        private void Process(BackgroundWorker worker, RunArguments arguments) {
            System.Threading.Thread.Sleep(1200);

            HashSet<uint> pids = FindProcesses(arguments);
            
            
            if (pids.Count == 0 && _previouslyInjected.Count == 0)
                worker.ReportProgress(NO_PROCESS_FOUND_ON_STARTUP, null);
            else if (pids.Count == 0)
                worker.ReportProgress(NO_PROCESS_FOUND, null);



            string dll32Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName.Replace(".dll", "_x86.dll"));
            string dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName.Replace(".dll", "_x64.dll"));
            if (!File.Exists(dll32Bit)) {
                Logger.FatalFormat("Could not find {1} at \"{0}\"", dll32Bit, arguments.DllName);
            } else if (!File.Exists(dll64Bit)) {
                Logger.FatalFormat("Could not find {1} at \"{0}\"", dll64Bit, arguments.DllName);
            }
            else {
                foreach (uint pid in pids) {
                    if (!_previouslyInjected.Contains(pid)) {
                        if (Is64Bit((int)pid, worker)) {
                            if (InjectionVerifier.VerifyInjection(pid, dll64Bit)) {
                                Logger.Info($"DLL already injected into target process, skipping injection into {pid}");
                                _dontLog.Add(pid);
                                _previouslyInjected.Add(pid);
                            }
                            else {
                                Inject64Bit("Grim Dawn.exe", dll64Bit);

                                if (!InjectionVerifier.VerifyInjection(pid, dll64Bit)) {
                                    worker.ReportProgress(INJECTION_ERROR, null);
                                }
                            }
                        }
                        else {
                            if (InjectionVerifier.VerifyInjection(pid, dll32Bit)) {
                                Logger.Info($"DLL already injected into target process, skipping injection into {pid}");
                                _dontLog.Add(pid);
                                _previouslyInjected.Add(pid);
                            }
                            else {
                                //DllInjector.NewInject(pid, dll32Bit);
                                Inject32Bit("Grim Dawn.exe", dll32Bit);


                                if (!InjectionVerifier.VerifyInjection(pid, dll32Bit)) {
                                    worker.ReportProgress(INJECTION_ERROR, null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
