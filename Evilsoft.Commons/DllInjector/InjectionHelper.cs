using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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
        public const int STILL_RUNNING = 4;
        public const int INJECTION_ERROR_32BIT = 5;
        public const int PATH_ERROR = 6;
        public const int ABORTED = 7;
        public const int GD_SEASON = 8;
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
        public InjectionHelper(ProgressChangedEventHandler progressChanged, bool unloadOnExit, string windowName, string className, string dll) {
            if (string.IsNullOrEmpty(windowName) && string.IsNullOrEmpty(className)) {
                throw new ArgumentException("Either window or class name must be specified");
            }
            else if (string.IsNullOrEmpty(dll)) {
                throw new ArgumentException("DLL name must be specified");
            }

            this._registeredProgressCallback = progressChanged;
            _bw = new BackgroundWorker();
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerSupportsCancellation = true;
            _bw.WorkerReportsProgress = true;
            _bw.ProgressChanged += progressChanged;

            _exitArguments = new RunArguments {
                WindowName = windowName,
                ClassName = className,
                DllName = dll
            };
            _bw.RunWorkerAsync(_exitArguments);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "InjectionHelper";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            ExceptionReporter.EnableLogUnhandledOnThread();

            try {
                BackgroundWorker worker = sender as BackgroundWorker;

                Logger.Info("Generating the path to the IA DLL...");
                try {
                    Path.Combine(Directory.GetCurrentDirectory(), (e.Argument as RunArguments).DllName);
                }
                catch (Exception ex) {
                    Logger.Fatal("Error generating path to the IA dll, try installing IA in a different folder...");
                    Logger.Fatal(ex.Message, ex);
                    worker.ReportProgress(PATH_ERROR, null);
                }

                while (!worker.CancellationPending) {
                    if (!File.Exists("DllInjector64.exe")) {
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
        }

        private static IntPtr Inject64Bit(string exe, string dll, int method) {
            // Methods which tends to work: 1, 3, 5, 6
            return InjectXBit("DllInjector64.exe", exe, dll, method);
        }


        private static IntPtr InjectXBit(string injector, string exe, string dll, int method) {
            // 1, 3, 5 and 6 have been verified to work "at least on my PC"
            // Think its 4 that expects an export the DLL don't have.. 
            if (method != 1 && method != 3 && method != 5 && method != 6)
                throw new ArgumentException("Illegal argument", "method");

            method = 1;

            Logger.Info($"Running {injector}...");
            if (File.Exists(injector)) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = injector;
                startInfo.Arguments = $"-t {method} \"{exe}\" \"{dll}\"";
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
                            Logger.Info("Injection reported as successful.. (may or may not have loaded)");
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
            // Don't be so eager to inject, give the DLL some time to call back before retrying/verifying
            int sleep = 800;
            while (sleep > 0 && (!worker?.CancellationPending ?? false)) {
                System.Threading.Thread.Sleep(100);
                sleep -= 100;
            }

            if (System.Diagnostics.Process.GetProcessesByName("GDCommunityLauncher").Length > 0) {
                worker.ReportProgress(GD_SEASON, null);
                return;
            }

            HashSet<uint> pids = FindProcesses(arguments);


            if (pids.Count == 0 && _previouslyInjected.Count == 0) {
                worker.ReportProgress(NO_PROCESS_FOUND_ON_STARTUP, null);
            }
            else if (pids.Count == 0) {
                worker.ReportProgress(NO_PROCESS_FOUND, null);
            }





            string dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), arguments.DllName);
            if (!File.Exists(dll64Bit)) {
                Logger.FatalFormat("Could not find {1} at \"{0}\"", dll64Bit, arguments.DllName);
            }
            else {
                foreach (uint pid in pids) {
                    if (_previouslyInjected.Contains(pid)) {
                        worker.ReportProgress(STILL_RUNNING, null);
                        continue;
                    }

                    // Figure out the filename
                    try {
                        dll64Bit = GetFilenameForPid(pid, arguments.DllName);
                    } catch (Exception) {
                        worker.ReportProgress(INJECTION_ERROR_POSSIBLE_ACCESS_DENIED, null);
                        continue;
                    }
                    // TODO: if this throws, open help page.

                    // 32bit not supported
                    try {
                        if (!Is64Bit((int)pid, worker)) {
                            Logger.Fatal("This version of Item Assistant does not support 32bit Grim Dawn");
                            worker.ReportProgress(INJECTION_ERROR_32BIT, null);
                            continue;
                        }
                    }
                    catch (ArgumentException ex) {
                        // Probably just closed the game
                        Logger.Warn("Error checking 32/64 bit status", ex);
                        continue;
                    }
                    catch (InvalidOperationException ex) {
                        Logger.Warn("Error checking 32/64 bit status", ex);
                        continue;

                    }

                    // Actual injection
                    if (InjectionVerifier.VerifyInjection(pid, dll64Bit)) {
                        Logger.Info($"DLL already injected into target process, skipping injection into {pid}");
                        _dontLog.Add(pid);
                        _previouslyInjected.Add(pid);
                    }
                    else  {
                        Inject64Bit("Grim Dawn.exe", dll64Bit, 1);

                        if (!InjectionVerifier.VerifyInjection(pid, dll64Bit)) {
                            Logger.Error($"Error injecting DLL into Grim Dawn.");

                            worker.ReportProgress(INJECTION_ERROR, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This was previously: Path.Combine(Path.GetDirectoryName(GetWindowModuleFileName(pid)), "Game.dll");
        /// However, some users kept running into an invalid path exception inside Path.GetDirectoryName, despite the path existing and being valid.
        /// At least one of these scenarios is when GetWindowModuleFileName(..) has returned an empty string.
        /// </summary>
        /// <param name="path">Full path to Grim Dawn.exe</param>
        /// <returns>Full path to Game.dll</returns>
        private static string GetGameDllPath(string path) {
            var args = path.Split('\\');
            args[args.Count() - 1] = "Game.dll";
            return string.Join("\\", args);
        }

        private static Dictionary<string, bool> _isPlaytestCache = new Dictionary<string, bool>(1);
        private static string GetFilenameForPid(uint pid, string dllName) {
            string dll64Bit;
            try {
                var grimDawnExe = GetWindowModuleFileName(pid);
                if (grimDawnExe == string.Empty) {
                    throw new Exception("Could not determine Grim Dawn path from pid, most likely a permissions issue.");
                }

                // Figure out the filename
                var gdFilename = GetGameDllPath(GetWindowModuleFileName(pid));

                if (!_isPlaytestCache.ContainsKey(gdFilename)) {
                    _isPlaytestCache[gdFilename] = InjectionVerifier.IsPlaytest(gdFilename) && false; // No playtest dll at the moment, will probably be one again very soon.
                }

                if (_isPlaytestCache[gdFilename]) {
                    dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), dllName.Replace("_x64", "_playtest_x64"));
                    Logger.Info("Playtest detected, using DLL " + dll64Bit);

                    if (!File.Exists(dll64Bit)) {
                        Logger.Error("Could not find DLL");
                    }
                }
                else {
                    dll64Bit = Path.Combine(Directory.GetCurrentDirectory(), dllName);
                }

                return dll64Bit;
            } catch (Exception ex) {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(GetWindowModuleFileName(pid));
                var b64 = System.Convert.ToBase64String(plainTextBytes);

                Logger.Warn("The module filename in question is: " + GetWindowModuleFileName(pid));
                Logger.Warn("Base64: " + b64);
                Logger.Warn(ex);
                throw;
            }
        }

        // Copy-pasta from GrimDawnDetector: Remove once latest PlayTest is public

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        private static string GetWindowModuleFileName(uint pid) {
            const int nChars = 1024;
            StringBuilder filename = new StringBuilder(nChars);
            IntPtr hProcess = OpenProcess(1040, 0, pid);
            GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, nChars);
            CloseHandle(hProcess);
            return (filename.ToString());
        }
    }
}
