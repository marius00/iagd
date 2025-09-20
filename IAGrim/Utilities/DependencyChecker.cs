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

        // TODO: What uses this? Anything? -- Might have been old chromium
        public static bool CheckVs2013Installed() {
            Logger.Debug("Checking if VC++ 2013 Runtimes are installed");
            using (RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}")) {
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