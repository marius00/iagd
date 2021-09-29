using System;
using System.Collections.Generic;
using System.IO;
using Gameloop.Vdf;
using log4net;
using Microsoft.Win32;

namespace IAGrim.Utilities.Detection {
    class SteamDetection {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SteamDetection));
        private const string VALVE_32BIT_PATH = @"Software\Valve\Steam";
        private const string VALVE_64BIT_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";

        /// <summary>
        /// Checks if a directory contains the "config/config.vdf" file, which typically indicates the Steam install directory.
        /// </summary>
        /// <param name="path">Path to check</param>
        private static bool IsSteamDirectory(string path) {
            return !String.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, "config", "config.vdf"));
        }

        public static string GetSteamDirectory() {
            var path = GetSteamDirectoryFromValveRegistry();
            if (string.Empty == path) {
                return GetSteamDirectoryFromShellRegistry();
            }

            return path;
        }

        private static string GetSteamDirectoryFromValveRegistry() {
            foreach (string candidate in new[] {VALVE_32BIT_PATH, VALVE_64BIT_PATH}) {
                using (RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")) {
                    Logger.Debug("Looking for steam registry key..");
                    if (registryKey != null) {
                        string location = (string) registryKey.GetValue("SteamPath");
                        if (IsSteamDirectory(location)) {
                            Logger.Info("Steam config location located");
                            return location;
                        }
                    }

                    Logger.Debug($"Steam registry key not found for {candidate}");
                }
            }

            return string.Empty;
        }


        private static string GetSteamDirectoryFromShellRegistry() {
            // Attempt to locate steam via the shell command, registry entry "Computer\HKEY_CLASSES_ROOT\steam\Shell\Open\Command"
            using (RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(@"steam\Shell\Open\Command")) {
                string content = (string) registryKey?.GetValue("");
                if (!String.IsNullOrEmpty(content) && content.Contains(".exe")) {
                    var sub = content.Substring(0, 4 + content.IndexOf(".exe", StringComparison.InvariantCultureIgnoreCase)).Replace("\"", "");
                    var exe = Path.GetFullPath(sub);
                    if (File.Exists(exe)) {
                        var candidate = Directory.GetParent(exe).FullName;
                        if (IsSteamDirectory(candidate)) {
                            return candidate;
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Attempts to locate all the steam install folders from the steam VDF config file.
        /// Note: This does NOT include the actual steam install folder.
        /// 
        /// The result of this operation should be appended along with the "{steamFolder}\steamapps\steamapps" folder.
        /// </summary>
        /// <param name="vdf"></param>
        /// <returns></returns>
        public static List<string> ExtractSteamLibraryPaths(string vdf) {
            List<string> paths = new List<string>();
            if (File.Exists(vdf)) {
                dynamic config = VdfConvert.Deserialize(File.ReadAllText(vdf));
                var root = config.Value;

                for (int i = 1; i < 8; i++) {
                    try {
                        paths.Add(root[$"{i}"].path.ToString());
                    }
                    catch (KeyNotFoundException) {
                        Logger.Debug("Key #1 not found, stopping parse of steam config");
                        return paths;
                    }
                }
            }

            return paths;
        }

        public static List<string> GetGrimFolderFromSteamLibrary(List<string> libraryPaths) {
            List<string> validPaths = new List<string>();
            foreach (var path in libraryPaths) {
                var subPath = Path.Combine(path, "steamapps", "common", "Grim Dawn");
                if (Directory.Exists(subPath)) {
                    if (File.Exists(Path.Combine(subPath, "database", "database.arz"))) {
                        validPaths.Add(subPath);
                    }
                }
            }

            return validPaths;
        }
    }
}