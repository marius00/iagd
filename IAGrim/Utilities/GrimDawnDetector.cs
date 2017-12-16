using EvilsoftCommons.Exceptions;
using IAGrim.Utilities;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using Gameloop.Vdf;

namespace IAGrim {
    class GrimDawnDetector {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GrimDawnDetector));

        public static string GetSteamDirectory() {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")) {
                Logger.Debug("Looking for steam registry key..");
                if (registryKey != null) {
                    string location = (string)registryKey.GetValue("SteamPath");
                    if (!string.IsNullOrEmpty(location) && File.Exists(Path.Combine(location, "config", "config.vdf"))) {
                        Logger.Info("Steam config location located");
                        return location;
                    }
                }
                Logger.Debug("Steam registry key not found");
            }

            return string.Empty;
        }

        
        public static List<string> ExtractSteamLibraryPaths(string vdf) {
            List<string> paths = new List<string>();
            if (File.Exists(vdf)) {
                dynamic config = VdfConvert.Deserialize(File.ReadAllText(vdf));
                var root = config.Value.Software.Valve.Steam;

                try {
                    paths.Add(root.BaseInstallFolder_1.Value.ToString());
                }
                catch (KeyNotFoundException) {
                    Logger.Debug("Key #1 not found, stopping parse of steam config");
                    return paths;
                }

                try {
                    paths.Add(root.BaseInstallFolder_2.Value.ToString());
                }
                catch (KeyNotFoundException) {
                    Logger.Debug("Key #2 not found, stopping parse of steam config");
                    return paths;
                }

                try {
                    paths.Add(root.BaseInstallFolder_3.Value.ToString());
                }
                catch (KeyNotFoundException) {
                    Logger.Debug("Key #3 not found, stopping parse of steam config");
                    return paths;
                }

                try {
                    paths.Add(root.BaseInstallFolder_4.Value.ToString());
                }
                catch (KeyNotFoundException) {
                    Logger.Debug("Key #4 not found, stopping parse of steam config");
                    return paths;
                }

                try {
                    paths.Add(root.BaseInstallFolder_5.Value.ToString());
                }
                catch (KeyNotFoundException) {
                    Logger.Debug("Key #5 not found, stopping parse of steam config");
                    return paths;
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

        /// <summary>
        /// Get the location of Grim Dawn using the steam app id or steam client from the registry
        /// </summary>
        /// <returns></returns>
        private static string FindGogByRegistry() {

            // Hopefully this is where GOG stores all its games
            // Works for detecting Prison Architect
            string gog = @"Software\Wow6432Node\GOG.com\Games";
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(gog)) {
                Logger.Debug("Looking for Grim Dawn GOG install..");
                if (registryKey != null) {
                    foreach (string s in registryKey.GetSubKeyNames()) {
                        using (RegistryKey gameKey = Registry.LocalMachine.OpenSubKey(Path.Combine(gog, s))) {
                            if (gameKey != null) {
                                string exe = (string)gameKey.GetValue("EXEFILE");
                                string location = (string)gameKey.GetValue("PATH");
                                if (!string.IsNullOrEmpty(exe) && !string.IsNullOrEmpty(location)) {
                                    bool isGrimDawn = "grim dawn.exe".Equals(Path.GetFileName(exe).ToLower());
                                    bool exists = File.Exists(Path.Combine(location, exe));
                                    if (isGrimDawn) {
                                        if (exists) {
                                            Logger.InfoFormat("Found GOG Grim Dawn at {0}", location);
                                            return location;
                                        }
                                        else {
                                            Logger.InfoFormat("Found registry entry for GOG Grim Dawn at {0}, but executable not found.", location);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Logger.Debug("Grim Dawn GOG install not found..");
            }


            return string.Empty;
        }


        /// <summary>
        /// Get the location of Grim Dawn using the steam app id or steam client from the registry
        /// </summary>
        /// <returns></returns>
        private static string FindSteamUserdata() {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 219990")) {
                if (registryKey != null) {
                    string location = (string)registryKey.GetValue("InstallLocation");
                    if (!string.IsNullOrEmpty(location)) {


                        location = Path.Combine(Directory.GetParent(location).Parent.Parent.FullName, "userdata");
                        if (Directory.Exists(location)) {
                            Logger.Info("Grim Dawn userdata location located using App Id");
                            return location;
                        }
                    }
                }
            }
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")) {
                if (registryKey != null) {
                    string location = (string)registryKey.GetValue("SourceModInstallPath");
                    if (!string.IsNullOrEmpty(location)) {
                        string p = Path.Combine(Directory.GetParent(location).Parent.FullName, "userdata");
                        if (Directory.Exists(p)) {
                            Logger.Info("Grim Dawn install location located using Source Mod Path");
                            return p;
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the path to the steam "cloud save" folder for Grim Dawn
        /// </summary>
        /// <returns></returns>
        public static string FindSteamGrimDawnUserdata() {
            string userdataPath = FindSteamUserdata();
            if (!string.IsNullOrEmpty(userdataPath)) {
                foreach (string userid in Directory.GetDirectories(userdataPath)) {
                    string fullpath = Path.Combine(userdataPath, userid, "219990", "remote", "save");
                    if (File.Exists(Path.Combine(fullpath, "transfer.gst")))
                        return fullpath;
                    else if (File.Exists(Path.Combine(fullpath, "transfer.gsh")))
                        return fullpath;
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// Find the path to Grim Dawn by searching for the HWND it creates
        /// </summary>
        /// <returns></returns>
        private static string FindByWindow() {
            HashSet<string> possibles;

            try {
                possibles = FindProcessForWindow("Grim Dawn");
            }
            catch (Exception) {
                possibles = FindViaManagmentSearch();
            }


            foreach (string possible in possibles) {
                var p = Path.Combine(Path.GetDirectoryName(possible), "database", "database.arz");
                if (File.Exists(p))
                    return Path.GetDirectoryName(possible);
            }

            return string.Empty;
        }


        /// <summary>
        /// Get the location of Grim Dawn
        /// </summary>
        /// <returns></returns>
        public static string GetGrimLocation() {
            string location = Properties.Settings.Default.GrimDawnLocation as string;
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location) && !location.Contains(".arz"))
                return location;

            try {
                try {
                    var steamPath = GetSteamDirectory();
                    var locations =
                        GetGrimFolderFromSteamLibrary(
                            ExtractSteamLibraryPaths(Path.Combine(steamPath, "config", "config.vdf")));
                    if (locations.Count > 0)
                        location = locations[0];
                }
                catch (Exception ex) {
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                }

                location = FindGogByRegistry();
                if (!string.IsNullOrEmpty(location))
                    return location;

                location = FindByWindow();
                if (!string.IsNullOrEmpty(location))
                    return location;
            }

            // Cache the location
            finally {
                Properties.Settings.Default.GrimDawnLocation = location;
                Properties.Settings.Default.Save();
            }

            return string.Empty;
        }
        public static ISet<string> GetGrimLocations() {
            var locations = new HashSet<string>();

            string location = Properties.Settings.Default.GrimDawnLocation as string;
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location) && !location.Contains(".arz")) {
                locations.Add(location);
            }


            try {
                var steamPath = GetSteamDirectory();
                GetGrimFolderFromSteamLibrary(ExtractSteamLibraryPaths(Path.Combine(steamPath, "config", "config.vdf")))
                    .ForEach(loc => locations.Add(loc));
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            location = FindGogByRegistry();
            if (!string.IsNullOrEmpty(location)) {
                locations.Add(location);
            }

            location = FindByWindow();
            if (!string.IsNullOrEmpty(location)) {
                locations.Add(location);
            }

            return locations;
        }


        public static HashSet<string> FindViaManagmentSearch() {
            HashSet<string> paths = new HashSet<string>();
            try {

                var searcher = new ManagementObjectSearcher("Select * From Win32_Process");
                var processList = searcher.Get();
                foreach (var process in processList) {
                    if ("grim dawn.exe".Equals(process["Name"].ToString().ToLower())) {
                        if (File.Exists(process["ExecutablePath"].ToString()))
                            paths.Add(process["ExecutablePath"].ToString());
                    }
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            return paths;
        }


        /// <summary>
        /// Attempt to get the path for a process
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static string GetProcessPath(uint pid) {
            try {
                Process proc = Process.GetProcessById((int)pid);
                return proc.MainModule.FileName.ToString();
            }	
            catch (ArgumentException ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                return string.Empty;
            }
            catch (System.ComponentModel.Win32Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                string message = $"GetProcessPath: NativeErrorCode={ex.NativeErrorCode}, ErrorCode={ex.ErrorCode}";
                if (ex.NativeErrorCode == 5) {
                    Logger.Fatal("ACCESS_DENIED obtaining the Grim Dawn process -- IA will run in limited usability mode, and will not function for GoG installs.");
                    Logger.Info("Running IA as administrator may solve this issue.");
                }
                //ExceptionReporter.ReportException(ex, message);
                throw;
            }
/*
            catch (Exception ex) {
                logger.Warn(ex.Message);
                logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex, "GetProcessPath");
                throw;
            }*/
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        /// <summary>
        /// Find all processes for a given window class (HWND class)
        /// </summary>
        /// <param name="windowname"></param>
        /// <returns></returns>
        public static HashSet<string> FindProcessForWindow(string windowname) {
            // Find the windows
            HashSet<string> clients = new HashSet<string>();
            uint pid;
            IntPtr prevWindow = IntPtr.Zero;
            do {
                prevWindow = FindWindowEx(IntPtr.Zero, prevWindow, windowname, null);
                if (prevWindow != null && prevWindow != IntPtr.Zero) {
                    GetWindowThreadProcessId(prevWindow, out pid);
                    string path = GetProcessPath(pid);
                    if (!string.IsNullOrEmpty(path))
                        clients.Add(path);
                    else {
                        if (!string.IsNullOrEmpty(path))
                            clients.Add(path);
                    }
                }
            }

            while (prevWindow != IntPtr.Zero);


            return clients;
        }
    }
}
