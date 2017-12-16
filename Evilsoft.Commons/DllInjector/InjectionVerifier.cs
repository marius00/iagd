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
        static ILog logger = LogManager.GetLogger(typeof(InjectionVerifier));


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
                logger.Warn("Error trying to create registry keys, this is not critical.");
                logger.Warn(ex.Message);
            }
        }

        public static bool VerifyInjection(long pid, string dll) {
            FixRegistryNagOnListDlls();

            logger.Info("Running Listdlls...");
            if (File.Exists("Listdlls.exe")) {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "Listdlls.exe";
                startInfo.Arguments = String.Format("-d {0}", dll);
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;

                startInfo.CreateNoWindow = true;

                Process processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.EnableRaisingEvents = true;
                try {
                    string spid = pid.ToString();
                    processTemp.Start();
                    while (!processTemp.StandardOutput.EndOfStream) {
                        string line = processTemp.StandardOutput.ReadLine();
                        if (line.EndsWith(spid))
                            return true;
                    }
                }
                catch (Exception ex) {
                    logger.Warn("Exception while attempting to verify injection.. " + ex.Message + ex.StackTrace);
                    ExceptionReporter.ReportException(ex);
                }
            }
            else {
                logger.Warn("Could not find Listdlls.exe, unable to verify successful injection.");
            }
            return false;
        }

    }
}
