using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace IAGrim.Utilities {
    static class DependencyChecker {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DependencyChecker));

        // http://stackoverflow.com/questions/951856/is-there-an-easy-way-to-check-the-net-framework-version
        public static bool CheckNet452Installed() {
            Logger.Debug("Checking if .Net Framework v4.52 is installed");
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
                if (ndpKey == null)
                    return false;

                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return releaseKey >= 379893;
            }

        }

        public static bool CheckVs2013Installed() {
            Logger.Debug("Checking if VC++ 2013 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}")) {
                return registryKey != null;
            }
        }

        public static bool CheckVs2012Installed() {
            Logger.Debug("Checking if VC++ 2012 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}")) {
                return registryKey != null;
            }
        }

        public static bool CheckVs2010Installed() {
            Logger.Debug("Checking if VC++ 2010 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A")) {
                return registryKey != null;
            }
        }
    }
}
