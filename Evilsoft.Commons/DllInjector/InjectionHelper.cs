using log4net;
using EvilsoftCommons.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace EvilsoftCommons.DllInjector {
    public class InjectionHelper : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InjectionHelper));
        private BackgroundWorker _bw;
        private readonly HashSet<uint> _previouslyInjected = new HashSet<uint>();
        private readonly HashSet<uint> _dontLog = new HashSet<uint>();
        private readonly Dictionary<uint, IntPtr> _pidModuleHandleMap = new Dictionary<uint, IntPtr>();
        private readonly bool _unloadOnExit;
        private readonly RunArguments _exitArguments;

        public const int INJECTION_ERROR = 0;
        public const int NO_PROCESS_FOUND_ON_STARTUP = 1;
        public const int NO_PROCESS_FOUND = 2;

        private ProgressChangedEventHandler _registeredProgressCallback;

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
            this._unloadOnExit = unloadOnExit;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += progressChanged;
            //bw.ProgressChanged += bw_ProgressChanged;

            _exitArguments = new RunArguments {
                WindowName = windowName,
                ClassName = className,
                DllName = dll
            };
            bw.RunWorkerAsync(_exitArguments);
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            
        }

        public void Cancel() {
            if (_bw != null) {
                _bw.CancelAsync();
                _bw = null;
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            
            try {
                BackgroundWorker worker = sender as BackgroundWorker;

                if (Thread.CurrentThread.Name == null)
                    Thread.CurrentThread.Name = "InjectionHelper";
                
                while (!worker.CancellationPending) {
                    Process(worker, e.Argument as RunArguments);
                }
            }
            catch (Exception ex) {
                Logger.Fatal(ex.Message);
                Logger.Fatal(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
                throw;
            }
        }

        public void Dispose() {
            if (_bw != null) {
                _bw.ProgressChanged -= _registeredProgressCallback;
                _bw.CancelAsync();
                _bw = null;
            }

            if (_unloadOnExit) {
                // Unload the DLL from any still running instance
                HashSet<uint> pids = FindProcesses(_exitArguments);
                foreach (uint pid in _pidModuleHandleMap.Keys) {
                    if (pids.Contains(pid)) {
                        if (DllInjector.UnloadDll(pid, _pidModuleHandleMap[pid])) {
                            Logger.InfoFormat("Unloaded module from pid {0}", pid);
                        }
                        else {
                            Logger.InfoFormat("Failed to unload module from pid {0}", pid);
                        }
                    }
                }
            }
            else {
                Logger.Info("Exiting without unloading DLL (as per configuration)");
            }
        }


        private HashSet<uint> FindProcesses(RunArguments args) {

            if (!string.IsNullOrEmpty(args.WindowName)) {
                return DllInjector.FindProcessForWindow(args.WindowName);
            }
            else {
                throw new NotSupportedException("Class name provided instead of window name, not yet implemented.");
            }
        }

        private bool Is64Bit(int pid) {
            try {
                var p = System.Diagnostics.Process.GetProcessById(pid);
                return DllInjector.Is64BitProcess(p);
            }
            catch (ArgumentException) {
                return false;
            }
        }

        public static IntPtr Inject64Bit(long pid, string dll) {
            Logger.Info("Running 64 bit injector...");
            if (File.Exists("DllInjector64.exe")) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "DllInjector64.exe";
                startInfo.Arguments = $"{pid} \"{dll}\"";
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
                    ExceptionReporter.ReportException(ex);
                }
            }
            else {
                Logger.Warn("Could not find DllInjector64.exe, unable to inject into Grim Dawn 64bit.");
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


            // TODO:
            string dll32Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName);
            string dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName);
            /*
            string dll32Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName.Replace(".dll", "_x86.dll"));
            string dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName.Replace(".dll", "_x64.dll"));
            if (!File.Exists(dll32Bit)) {
                Logger.FatalFormat("Could not find {1} at \"{0}\"", dll32Bit, arguments.DllName);
            } else if (!File.Exists(dll64Bit)) {
                Logger.FatalFormat("Could not find {1} at \"{0}\"", dll64Bit, arguments.DllName);
            }
            else*/
            {
                foreach (uint pid in pids) {
                    if (!_previouslyInjected.Contains(pid)) {
                        string dllName;
                        if (Is64Bit((int)pid)) {
                            dllName = dll64Bit;

                            if (InjectionVerifier.VerifyInjection(pid, dllName)) {
                                Logger.Info($"DLL already injected into target process, skipping injection into {pid}");
                                _dontLog.Add(pid);
                            }
                        }
                        else {
                            dllName = dll32Bit;
                        }
                        var remoteModuleHandle = DllInjector.NewInject(pid, dllName);

                        if (remoteModuleHandle == IntPtr.Zero) {
                            if (!_dontLog.Contains(pid)) {
                                Logger.WarnFormat("Could not inject dll into process {0}, if this is a recurring issue, try running as administrator.", pid);
                                worker.ReportProgress(INJECTION_ERROR, "Could not inject dll into process " + pid);
                            }
                        }
                        else {
                            if (!_dontLog.Contains(pid))
                                Logger.Info("Injected dll into process " + pid);

                            if (!InjectionVerifier.VerifyInjection(pid, dllName)) {
                                if (!_dontLog.Contains(pid)) {
                                    Logger.Warn("InjectionVerifier reports injection failed.");
                                    ExceptionReporter.ReportIssue("InjectionVerifier reports injection failed into PID " + pid);
                                    worker.ReportProgress(INJECTION_ERROR, string.Format("InjectionVerifier reports injection failed into PID {0}, try running as administrator.", pid));
                                }


                                _dontLog.Add(pid);
                            }
                            else {
                                Logger.Info("InjectionVerifier reports injection succeeded.");
                                _previouslyInjected.Add(pid);
                                _pidModuleHandleMap[pid] = remoteModuleHandle;
                            }

                        }
                    }
                }
            }
        }
    }
}
