using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

namespace IAGrim.Utilities {
    static class DependencyChecker {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DependencyChecker));

        // http://stackoverflow.com/questions/951856/is-there-an-easy-way-to-check-the-net-framework-version
        public static bool CheckNet472Installed() {
            Logger.Debug("Checking if .Net Framework v4.7.2 is installed");
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
                if (ndpKey == null)
                    return false;

                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return releaseKey >= 461808; // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
            }
        }

        // Used by CefSharp
        public static bool CheckVs2015Installed() {
            Logger.Debug("Checking if VC++ 2015 Runtimes are installed");
            return IsVc2015X86Installed();
        }

        // https://stackoverflow.com/questions/53000475/how-to-check-if-microsoft-visual-c-2015-redistributable-installed-on-a-device
        private static bool IsVc2015X86Installed() {
            string dependenciesPath = @"SOFTWARE\Classes\Installer\Dependencies";

            using (RegistryKey dependencies = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(dependenciesPath)) {
                if (dependencies == null) return false;

                foreach (string subKeyName in dependencies.GetSubKeyNames().Where(n => !n.ToLower().Contains("dotnet") && !n.ToLower().Contains("microsoft"))) {
                    using (RegistryKey subDir = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(dependenciesPath + "\\" + subKeyName)) {
                        var value = subDir.GetValue("DisplayName")?.ToString() ?? null;
                        if (string.IsNullOrEmpty(value)) continue;

                        if (Regex.IsMatch(value, @"C\+\+ 2015.*\(x86\)")) //here u can specify your version.
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // TODO: What uses this? Anything? -- Might have been old chromium
        public static bool CheckVs2013Installed() {
            Logger.Debug("Checking if VC++ 2013 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}")) {
                return registryKey != null;
            }
        }

        // TODO: Unused?
        public static bool CheckVs2012Installed() {
            Logger.Debug("Checking if VC++ 2012 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}")) {
                return registryKey != null;
            }
        }

        // TODO: What uses this? Anything?
        public static bool CheckVs2010Installed() {
            Logger.Debug("Checking if VC++ 2010 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A")) {
                return registryKey != null;
            }
        }
    }
}