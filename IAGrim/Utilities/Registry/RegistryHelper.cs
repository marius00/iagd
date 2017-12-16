using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;

namespace IAGrim.Utilities.Registry {
    class RegistryHelper {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RegistryHelper));

        public static void Write(string path, string key, string value) {
            var registryKey = OpenOrCreate(path);
            Logger.Debug($"Updating value of {key} to {value}");
            registryKey?.SetValue(key, value, RegistryValueKind.String);
            registryKey?.Dispose();
        }

        public static void Write(string path, string key, int value) {
            var registryKey = OpenOrCreate(path);
            Logger.Debug($"Updating value of {key} to {value}");
            registryKey?.SetValue(key, value, RegistryValueKind.DWord);
            registryKey?.Dispose();
        }

        private static RegistryKey OpenOrCreate(string path) {
            var root = Microsoft.Win32.Registry.CurrentUser;

            Logger.Debug($"Looking for registry key \"{path}\"..");
            RegistryKey registryKey = root.OpenSubKey(path, true);

            if (registryKey == null) {
                Logger.Debug("Registry key not found, creating key..");
                registryKey = root.CreateSubKey(path);
            }

            return registryKey;
        }

    }
}
