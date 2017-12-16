using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Utilities {
    static class DependencyChecker {

        // http://stackoverflow.com/questions/951856/is-there-an-easy-way-to-check-the-net-framework-version
        public static bool CheckNet452Installed() {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
                if (ndpKey == null)
                    return false;

                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return releaseKey >= 379893;
            }

        }

        public static bool CheckVS2013Installed() {
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}")) {
                return registryKey != null;
            }
        }

        public static bool CheckVS2012Installed() {
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}")) {
                return registryKey != null;
            }
        }

        public static bool CheckVS2010Installed() {
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A")) {
                return registryKey != null;
            }
        }
    }
}
