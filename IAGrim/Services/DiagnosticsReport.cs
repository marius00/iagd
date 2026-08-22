using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EvilsoftCommons.Exceptions;
using IAGrim.Utilities;
using IAGrim.Utilities.Detection;
using log4net;

namespace IAGrim.Services {
    /// <summary>
    /// One pasteable answer to "why is Item Assistant not picking up my loot".
    ///
    /// This exists because the failure modes that matter most cannot be reproduced by whoever has to fix them:
    /// they need a Wine prefix, a Proton install and a running copy of the game. Nearly all of them are, however,
    /// distinguishable from each other by state IAGD can simply read -- which path it resolved, whether the hook
    /// DLL is where it should be, whether the settings file the hook reads says what IAGD thinks it says, and
    /// whether the bridge has seen anything recently.
    ///
    /// Deliberately does not touch the database or any service: it runs before either exists, so that
    /// <c>--diagnose</c> still answers on an install too broken to start.
    /// </summary>
    internal static class DiagnosticsReport {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DiagnosticsReport));

        public const string Argument = "--diagnose";

        public static bool IsRequested(string[]? args) {
            return args?.Any(arg => Argument.Equals(arg?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Writes the report next to the settings file and opens it, for a user who was asked to produce one.
        /// Returns the path it was written to, or null if it could not be written.
        /// </summary>
        public static string? WriteAndOpen() {
            var report = Build();

            try {
                var path = Path.Combine(GlobalPaths.CoreFolder, "iagd-diagnostics.txt");
                File.WriteAllText(path, report);

                try {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch (Exception ex) {
                    // No file association, or no desktop session to open one in. The path still gets reported.
                    Logger.Warn($"Wrote the report but could not open it: {ex.Message}");
                }

                return path;
            }
            catch (Exception ex) {
                Logger.Error("Could not write the diagnostics report", ex);
                return null;
            }
        }

        /// <summary>
        /// Emits the report into the ordinary log at startup, so that every log file a user sends already carries
        /// it and nobody has to ask them to run anything.
        /// </summary>
        public static void LogAtStartup() {
            try {
                foreach (var line in Build().Split('\n')) {
                    Logger.Info("[diag] " + line.TrimEnd('\r'));
                }
            }
            catch (Exception ex) {
                Logger.Warn("Could not build the diagnostics report", ex);
            }
        }

        public static string Build() {
            var sb = new StringBuilder();

            void Section(string name) => sb.Append('\n').Append(name).Append('\n').Append(new string('-', name.Length)).Append('\n');
            void Item(string name, object? value) => sb.Append("  ").Append(name.PadRight(28)).Append(value ?? "(none)").Append('\n');

            sb.Append("Item Assistant diagnostics\n");
            sb.Append("==========================\n");

            Section("Application");
            Item("Version", ExceptionReporter.VersionString);
            Item("Built", $"{ExceptionReporter.BuildDate:yyyy-MM-dd}");
            Item("Install folder", AppContext.BaseDirectory);
            Item("Working directory", SafeGet(Directory.GetCurrentDirectory));
            Item("Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            Section("Environment");
            Item("OS", Environment.OSVersion);
            Item(".NET", Environment.Version);
            Item("Machine name", Environment.MachineName);
            Item("Running under Wine", WineDetector.IsRunningInWine() ? "yes" : "no");
            Item("Wine version", WineDetector.GetWineVersion());
            Item("Launched by Proton", ProtonPaths.IsRunningUnderProton ? "yes" : "no");
            Item("Steam root", ProtonPaths.SteamRoot);
            Item("Prefix (compatdata)", ProtonPaths.CompatData);
            Item("Game folder (Proton)", ProtonPaths.GameInstallDir);

            Section("WebView2");
            Item("Runtime version", WebView2Runtime.InstalledVersion);
            Item("Cache folder", SafeGet(() => GlobalPaths.EdgeCacheLocation));

            Section("Paths");
            Item("Settings file", SafeGet(() => GlobalPaths.SettingsFile));
            Item("Data folder", SafeGet(() => GlobalPaths.CoreFolder));
            Item("Item queue", SafeGet(() => GlobalPaths.CsvLocation));
            Item("Replica (to IA)", SafeGet(() => GlobalPaths.CsvReplicaReadLocation));
            Item("Replica (from IA)", SafeGet(() => GlobalPaths.CsvReplicaWriteLocation));
            Item("Wine bridge", SafeGet(() => GlobalPaths.LinuxHack));

            AppendHookFiles(sb, Section, Item);
            AppendGrimDawnLocations(sb, Section, Item);
            AppendBridgeState(sb, Section, Item);

            return sb.ToString();
        }

        /// <summary>The three files without which no loot is ever captured, and the versions they claim.</summary>
        private static void AppendHookFiles(StringBuilder sb, Action<string> section, Action<string, object?> item) {
            section("Hook and injector");

            foreach (var filename in new[] { "ItemAssistantHook_x64.dll", "DllInjector64.exe", "Listdlls.exe" }) {
                var path = Path.Combine(AppContext.BaseDirectory, filename);
                if (!File.Exists(path)) {
                    item(filename, "MISSING");
                    continue;
                }

                var version = SafeGet(() => System.Diagnostics.FileVersionInfo.GetVersionInfo(path).FileVersion);
                item(filename, string.IsNullOrEmpty(version) ? "present" : $"present, version {version}");
            }

            var versionFile = Path.Combine(AppContext.BaseDirectory, "dllver.txt");
            item("dllver.txt", File.Exists(versionFile) ? SafeGet(() => File.ReadAllText(versionFile).Trim()) : "MISSING");
        }

        private static void AppendGrimDawnLocations(StringBuilder sb, Action<string> section, Action<string, object?> item) {
            section("Grim Dawn");

            var locations = new HashSet<string>();
            try {
                GrimDawnDetector.AppendSteamPaths(locations);
            }
            catch (Exception ex) {
                item("Detection failed", ex.Message);
            }

            if (locations.Count == 0) {
                item("Installs found", "none -- IAGD cannot parse the item database without one");
                return;
            }

            var index = 0;
            foreach (var location in locations) {
                var arz = Path.Combine(location, "database", "database.arz");
                item($"Install #{++index}", $"{location} ({(File.Exists(arz) ? "database present" : "NO DATABASE")})");
            }
        }

        /// <summary>
        /// What the hook has actually been doing. The settings value is read back off disk rather than from the
        /// in-memory copy on purpose: that file is the hook's contract, and the symptom of the two disagreeing is
        /// loot quietly not arriving.
        /// </summary>
        private static void AppendBridgeState(StringBuilder sb, Action<string> section, Action<string, object?> item) {
            section("Bridge");

            var settingsFile = SafeGet(() => GlobalPaths.SettingsFile);
            if (settingsFile == null || !File.Exists(settingsFile)) {
                item("Settings file", "MISSING -- the hook reads this, and defaults to the Windows behaviour without it");
            }
            else {
                item("isRunningInWine", ReadSettingsFlag(settingsFile, "isRunningInWine"));
            }

            CountFiles(item, "Wine bridge messages", () => GlobalPaths.LinuxHack, "*.msg");
            CountFiles(item, "Injection markers", () => GlobalPaths.LinuxHack, "*.PID");
            CountFiles(item, "Aborted attaches", () => GlobalPaths.LinuxHack, "*.ABORTED");
            CountFiles(item, "Queued loot files", () => GlobalPaths.CsvLocationIngoing, "*.csv");
            CountFiles(item, "Queued transfers", () => GlobalPaths.CsvLocationOutgoing, "*.csv");
        }

        private static void CountFiles(Action<string, object?> item, string label, Func<string> folder, string pattern) {
            try {
                var files = Directory.GetFiles(folder(), pattern);
                if (files.Length == 0) {
                    item(label, "0");
                    return;
                }

                var newest = files.Max(File.GetLastWriteTime);
                item(label, $"{files.Length}, most recent {FormatAge(DateTime.Now - newest)} ago");
            }
            catch (Exception ex) {
                item(label, $"could not be read ({ex.Message})");
            }
        }

        private static string FormatAge(TimeSpan age) {
            if (age.TotalSeconds < 90) return $"{age.TotalSeconds:F0}s";
            if (age.TotalMinutes < 90) return $"{age.TotalMinutes:F0}m";
            if (age.TotalHours < 48) return $"{age.TotalHours:F0}h";
            return $"{age.TotalDays:F0}d";
        }

        /// <summary>
        /// Reads one persistent flag straight out of the settings JSON, the way the hook's own reader does, rather
        /// than through SettingsService -- which may not exist yet, and which would report what IAGD believes
        /// instead of what is on disk.
        /// </summary>
        private static string ReadSettingsFlag(string settingsFile, string key) {
            try {
                var json = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(settingsFile));
                var value = json["persistent"]?[key];
                return value == null ? "not set" : value.ToString();
            }
            catch (Exception ex) {
                return $"could not be read ({ex.Message})";
            }
        }

        private static string? SafeGet(Func<string?> get) {
            try {
                return get();
            }
            catch (Exception ex) {
                return $"could not be resolved ({ex.Message})";
            }
        }
    }
}
