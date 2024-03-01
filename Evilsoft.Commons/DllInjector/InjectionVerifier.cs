using log4net;
using EvilsoftCommons.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.DllInjector {
    /// <summary>
    /// Runs the Microsoft "Listdlls.exe" to verify that the DLL injection was successful.
    /// Sometimes the injection reports as successful, but the DLL does not persist. (unloaded by anti virus?)
    /// </summary>
    public class InjectionVerifier {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InjectionVerifier));


        /// <summary>
        /// Remove nag screens on running ListDLLs
        /// </summary>
        public static void FixRegistryNagOnListDlls() {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);


                key.CreateSubKey("Sysinternals");
                key = key.OpenSubKey("Sysinternals", true);

                key.CreateSubKey("ListDLLs");
                key = key.OpenSubKey("ListDLLs", true);

                key.SetValue("EulaAccepted", 1);

                key.Close();
            }
            catch (Exception ex) {
                Logger.Warn("Error trying to create registry keys, this is not critical.");
                Logger.Warn(ex.Message);
            }
        }

        public static bool VerifyInjection(long pid, string dll) {
            FixRegistryNagOnListDlls();

            Logger.Info("Running Listdlls...");
            List<string> output = new List<string>();
            if (File.Exists("Listdlls.exe")) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = "Listdlls.exe",
                    Arguments = $"{pid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.EnableRaisingEvents = true;
                try {
                    string spid = pid.ToString();
                    processTemp.Start();
                    processTemp.WaitForExit(3000);


                    while (!processTemp.StandardOutput.EndOfStream) {
                        string line = processTemp.StandardOutput.ReadLine();
                        output.Add(line);
                        if (line.Contains(dll))
                            return true;
                    }
                }
                catch (Exception ex) {
                    Logger.Warn("Exception while attempting to verify injection.. " + ex.Message + ex.StackTrace);
                }
            }
            else {
                Logger.Warn("Could not find Listdlls.exe, unable to verify successful injection.");
            }
            return false;
        }

        public static bool IsPlaytest(string dll) {
            FixRegistryNagOnListDlls();

            Logger.Info("Running dumpbin...");
            string exportOnlyInPlaytest = "??0GameTextLine@GAME@@QEAA@W4GameTextClass@1@AEBV?$basic_string@GU?$char_traits@G@std@@V?$allocator@G@2@@std@@_NPEBVGraphicsTexture@1@M@Z";

            List<string> output = new List<string>();
            if (File.Exists("dumpbin.exe")) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = "dumpbin.exe",
                    Arguments = $"/exports \"{dll}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.EnableRaisingEvents = true;
                try {
                    processTemp.Start();
                    processTemp.WaitForExit(3000);


                    while (!processTemp.StandardOutput.EndOfStream) {
                        string line = processTemp.StandardOutput.ReadLine();
                        output.Add(line);

                        if (line.Contains(exportOnlyInPlaytest)) { // Export only exists in v1.2
                            return true;
                        }
                    }

                    if (processTemp.ExitCode != 0) {
                        Logger.Fatal("Could not determine if running GD v1.1 or v1.2, will most likely crash the game.");
                        Logger.Fatal("Halting IA");
                        System.Environment.Exit(1);
                    }
                }
                catch (Exception ex) {
                    Logger.Warn("Exception while attempting to verify isPlaytest.. " + ex.Message + ex.StackTrace);
                }
            }
            else {
                Logger.Warn("Could not find dumpbin.exe, unable to verify isPlaytest.");
            }
            return false;
        }

    }
}
