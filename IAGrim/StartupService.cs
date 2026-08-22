using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Settings;
using IAGrim.UI;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;
using NHibernate;
using System.Diagnostics;
using System.Security.Principal;

namespace IAGrim {
    public class StartupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartupService));

        public void Init() {
            DateTime buildDate = ExceptionReporter.BuildDate;
            Logger.InfoFormat("Running version {0} from {1:dd/MM/yyyy}", ExceptionReporter.VersionString, buildDate);

            VerifyHookDllVersion();
        }

        /// <summary>
        /// Reports a hook DLL left behind by an update that could not overwrite it, which is the case where loot
        /// silently stops being captured.
        ///
        /// Everything here resolves against the install folder rather than the working directory. Both paths used
        /// to be relative and both throw rather than degrade when the working directory is not ours: the Windows
        /// shortcut sets it, so this never surfaced, but nothing guarantees it. A launcher script, a shell, or
        /// another process starting IAGD leaves it wherever it happened to be, and startup then died here, before
        /// the main window existed.
        /// </summary>
        private static void VerifyHookDllVersion() {
            var hookDll = Path.Combine(AppContext.BaseDirectory, "ItemAssistantHook_x64.dll");
            if (!File.Exists(hookDll)) {
                Logger.Error($"Could not find the hook DLL at \"{hookDll}\". Loot cannot be captured without it; IAGD needs to be reinstalled.");
                return;
            }

            FileVersionInfo dllVersion = FileVersionInfo.GetVersionInfo(hookDll);

            Logger.InfoFormat($"DLL version version {dllVersion.FileVersion}");
            LogOptionalDllVersion("Playtest", "ItemAssistantHook_playtest_x64.dll");

            // Numeric compare: dllver.txt is written from the DLL's ProductVersion (zero-padded revision) while
            // FileVersion is a numeric win32 resource that can't carry the padding, so the same version can be
            // spelled two ways. A string compare here read a stale DLL as up to date whenever the revision widths
            // differed, which is exactly the "updated while GD was running" case this check exists to catch.
            var versionFile = Path.Combine(AppContext.BaseDirectory, "dllver.txt");
            if (!File.Exists(versionFile)) {
                Logger.Warn($"Could not find \"{versionFile}\", skipping the hook DLL version check.");
                return;
            }

            var minimumDllVersion = File.ReadAllText(versionFile).Trim();
            if (VersionUtility.IsOlderThan(dllVersion.FileVersion, minimumDllVersion)) {
                Logger.Error($"The DLL version ({dllVersion.FileVersion}) is older than the required {minimumDllVersion}, did you perhaps run into a conflict while updating and clicked ignore?");
                Logger.Error("Item Assistant needs to be re-installed without GD running.");

                MessageBox.Show("IAGD install is corrupted.\nReinstall IAGD without GD running.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Logs the version of a hook DLL which may not be present in every install (GD v1.2 / playtest builds).
        /// </summary>
        private static void LogOptionalDllVersion(string label, string filename) {
            var path = Path.Combine(AppContext.BaseDirectory, filename);
            if (File.Exists(path)) {
                Logger.InfoFormat($"{label} DLL version {FileVersionInfo.GetVersionInfo(path).FileVersion}");
            }
            else {
                Logger.InfoFormat($"{label} DLL not present ({filename})");
            }
        }

        public static void PrintStartupInfo(SessionFactory factory, SettingsService settings) {
            try {
                Logger.Info(settings.GetLocal().StashToLootFrom == 0
                    ? "IA is configured to loot from the last stash page"
                    : $"IA is configured to loot from stash page #{settings.GetLocal().StashToLootFrom}");

                Logger.Info(settings.GetLocal().StashToDepositTo == 0
                    ? "IA is configured to deposit to the second-to-last stash page"
                    : $"IA is configured to deposit to stash page #{settings.GetLocal().StashToDepositTo}");

                using (ISession session = factory.OpenSession()) {
                    long numItemsStored = session.CreateCriteria<PlayerItem>()
                        .SetProjection(NHibernate.Criterion.Projections.RowCountInt64())
                        .UniqueResult<long>();

                    if (numItemsStored == 0)
                        Logger.Warn($"There are {numItemsStored} items stored in the database. <---- Unless you just installed IA, this is bad. No items.");
                    else
                        Logger.Info($"There are {numItemsStored} items stored in the database.");
                }


                Logger.Info("Transfer to any mod is " + (settings.GetPersistent().TransferAnyMod ? "enabled" : "disabled"));
                Logger.Info("Update check frequency is " + (settings.GetPersistent().CheckUpdatesDaily ? "daily" : "weekly"));
                Logger.Info((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator) ? "Running as administrator" : "Not running with low privileges");

                Logger.Info("There are items stored for the following mods:");

                foreach (ModSelection entry in new PlayerItemDaoImpl(factory, new DatabaseItemStatDaoImpl(factory))
                             .GetModSelection()) {
                    Logger.Info($"Mod: \"{entry.Mod}\", HC: {entry.IsHardcore}");
                }


                string gdPath = settings.GetLocal().CurrentGrimdawnLocation;
                Logger.Info(string.IsNullOrEmpty(gdPath)
                    ? "The path to Grim Dawn is unknown (not great)"
                    : $"The path to Grim Dawn is \"{gdPath}\"");

                Logger.Info($"Using IA on multiple PCs: {settings.GetPersistent().UsingDualComputer}");

                Logger.Info($"Logged into online backups: {!string.IsNullOrEmpty(settings.GetPersistent().CloudUser)}");
                Logger.Info($"Opted out of online backups: {settings.GetLocal().OptOutOfBackups}");



                using (ISession session = factory.OpenSession()) {
                    long num = session.CreateCriteria<DatabaseItem>()
                        .SetProjection(NHibernate.Criterion.Projections.RowCountInt64())
                        .UniqueResult<long>();

                    var isGdParsed = num > 0;
                    settings.GetLocal().IsGrimDawnParsed = isGdParsed;

                    if (isGdParsed) {
                        Logger.Info("The Grim Dawn database has been parsed");
                    }
                    else {
                        Logger.Warn("The Grim Dawn database has not been parsed");
                    }
                }

                Logger.Info("Startup data dump complete");
            }
            catch (Exception ex) {
                Logger.Error(ex.Message, ex);
                Logger.Error("IA may not function correctly");
            }
        }

        public static SettingsService LoadSettingsService() {
            return SettingsService.Load(GlobalPaths.SettingsFile);
        }

        /// <summary>
        /// Startup argument for resetting the settings that can leave IA impossible to find:
        /// a window position on a monitor that no longer exists, or a window hidden in the system tray.
        /// </summary>
        public const string SafeModeArgument = "--safe-mode";

        public static bool IsSafeMode(string[]? args) {
            return args?.Any(arg => SafeModeArgument.Equals(arg?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        /// <summary>
        /// Restores the window related settings to their defaults
        /// </summary>
        public static void ResetWindowSettings(SettingsService settings) {
            Logger.Info("Safe mode: resetting window position, start minimized and minimize to tray.");

            settings.GetLocal().WindowPositionSettings = null;
            settings.GetLocal().StartMinimized = false;
            settings.GetPersistent().MinimizeToTray = false;
        }

        /// <summary>
        /// Deletes the settings file and restarts IA, leaving the item database untouched.
        /// The process is killed rather than shut down cleanly: the in-memory settings are written back
        /// on exit (window position), which would recreate the file we just deleted.
        /// </summary>
        public static void ResetSettingsAndRestart() {
            try {
                Logger.Info($"Deleting {GlobalPaths.SettingsFile} on user request");
                File.Delete(GlobalPaths.SettingsFile);
            }
            catch (Exception ex) {
                Logger.Error($"Could not delete {GlobalPaths.SettingsFile}", ex);
                MessageBox.Show($"Could not delete the settings file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process.Start(new ProcessStartInfo { FileName = Application.ExecutablePath, UseShellExecute = true });
            LogManager.Shutdown();
            Environment.Exit(0);
        }

        public static void PerformGrimUpdateCheck(SettingsService settingsService) {
            string? location = settingsService.GetLocal().GrimDawnLocation?.FirstOrDefault();
            long lastParsed = settingsService.GetLocal().GrimDawnLocationLastModified;

            if (Directory.Exists(location)) {
                if (lastParsed > 0) {
                    long lastModified = ParsingService.GetHighestTimestamp(location);

                    if (lastModified > lastParsed) {
                        if (!settingsService.GetLocal().HasWarnedGrimDawnUpdate) {
                            Logger.Info("Grim Dawn appears to have been updated since last parse, notifying end user.");
                            string message = RuntimeSettings.Language?.GetTag("iatag_ui_database_modified_body") ?? string.Empty;
                            string title = RuntimeSettings.Language?.GetTag("iatag_ui_database_modified_title") ?? string.Empty;
                            settingsService.GetLocal().HasWarnedGrimDawnUpdate = true;
                            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else {
                            Logger.Debug("Grim Dawn appears to have been updated since last parse, end user previously notified.");
                        }
                    }
                    else {
                        Logger.Debug("Grim dawn appears unmodified since last run, database up to date.");
                    }
                }
                else {
                    Logger.Info("Last parsed entry for GD database is unset, skipping update check.");
                    settingsService.GetLocal().GrimDawnLocationLastModified = ParsingService.GetHighestTimestamp(location);
                }
            }
            else {
                Logger.Info("Grim dawn install is unset, skipping update check.");
            }
        }

        /// <summary>
        /// A record path that only exists in a given expansion's data, paired with the folder that
        /// tells us the user owns it. Add an entry when the next expansion ships.
        /// </summary>
        private static readonly (string Folder, string RecordPattern, string Name)[] ExpansionDataMarkers = {
            ("gdx3", "%ascendedrandomizers%", "Fangs of Asterkarn")
        };

        /// <summary>
        /// Parses the Grim Dawn database when we can tell it is missing the data for an expansion the
        /// user owns. Buying an expansion and never re-parsing is by far the most common support request:
        /// the items exist in the game but not in IA, and the fix is a database parse the user has no
        /// reason to know about.
        /// Returns true if a parse was performed.
        /// </summary>
        public static bool PerformMissingExpansionDataCheck(
            ParsingService parsingService,
            IDatabaseItemDao databaseItemDao,
            IPlayerItemDao playerItemDao,
            GrimDawnDetector grimDawnDetector,
            SettingsService settings
        ) {
            try {
                string gdPath = settings.GetLocal().CurrentGrimdawnLocation;

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    gdPath = grimDawnDetector.GetGrimLocations().FirstOrDefault() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    Logger.Warn("Could not find the Grim Dawn install location, skipping the expansion data check.");
                    return false;
                }

                var alreadyAttempted = settings.GetLocal().AutoParsedExpansions;

                foreach (var marker in ExpansionDataMarkers) {
                    if (!Directory.Exists(Path.Combine(gdPath, marker.Folder))) {
                        continue; // The user does not own this expansion.
                    }

                    if (alreadyAttempted.Contains(marker.Folder)) {
                        Logger.Debug($"An automatic parse for {marker.Name} has already been attempted, skipping.");
                        continue;
                    }

                    if (databaseItemDao.GetRowCountForRecordsLike(marker.RecordPattern) > 0) {
                        continue; // The expansion data is present, nothing to do.
                    }

                    Logger.Info($"The database has no items for {marker.Name} but the expansion is installed, parsing the game database.");

                    // Recorded before the parse rather than after: a parse that fails to produce the
                    // expected records (broken install, unsupported game version) must not re-trigger
                    // on every single startup.
                    settings.GetLocal().AutoParsedExpansions = new List<string>(alreadyAttempted) { marker.Folder };

                    AutoParseDatabase(parsingService, databaseItemDao, playerItemDao, settings, gdPath);
                    return true;
                }

                return false;
            }
            catch (Exception ex) {
                // Never block startup over this, the user can still parse manually.
                Logger.Warn("Error checking for missing expansion data", ex);
                return false;
            }
        }

        /// <summary>
        /// Parses the Grim Dawn database when the item names in it are not in the display language.
        /// Item names are taken from the games own Text_XX.arc and stored translated, so without this the
        /// stats and the UI switch language while every item name stays in the old one.
        /// Returns true if a parse was performed.
        /// </summary>
        public static bool PerformLanguageChangeCheck(
            ParsingService parsingService,
            IDatabaseItemDao databaseItemDao,
            IPlayerItemDao playerItemDao,
            GrimDawnDetector grimDawnDetector,
            SettingsService settings
        ) {
            try {
                var languageCode = settings.GetLocal().LanguageCode;
                var parsedLanguageCode = settings.GetLocal().ParsedLanguageCode;
                var isEnglish = languageCode.Equals("EN", StringComparison.OrdinalIgnoreCase);

                if (!settings.GetLocal().IsGrimDawnParsed) {
                    return false; // Nothing parsed yet, so nothing is in the wrong language either.
                }

                string reason;
                if (string.IsNullOrEmpty(parsedLanguageCode)) {
                    // Upgrading from a version that did not record this. Those versions ignored the games
                    // own tagItemNameOrder, which left every name in a gendered language (German, French,
                    // Russian, ..) cut down to its prefix. English was never affected, and a surprise full
                    // parse on first launch is not worth it for a database that is already correct.
                    if (isEnglish) {
                        settings.GetLocal().ParsedLanguageCode = languageCode;
                        return false;
                    }

                    reason = $"The item names were generated by a version that could not order {languageCode} names correctly";
                }
                else if (languageCode.Equals(parsedLanguageCode, StringComparison.OrdinalIgnoreCase)) {
                    return false;
                }
                else {
                    reason = $"The display language changed from {parsedLanguageCode} to {languageCode}";
                }

                string gdPath = settings.GetLocal().CurrentGrimdawnLocation;

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    gdPath = grimDawnDetector.GetGrimLocations().FirstOrDefault() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    Logger.Warn($"{reason}, but the Grim Dawn install location is unknown. The item names stay as they are until the database is parsed.");
                    return false;
                }

                Logger.Info($"{reason}, parsing the game database to regenerate the item names.");

                // Recorded before the parse: a parse that fails must not re-trigger on every startup.
                settings.GetLocal().ParsedLanguageCode = languageCode;

                AutoParseDatabase(parsingService, databaseItemDao, playerItemDao, settings, gdPath);
                return true;
            }
            catch (Exception ex) {
                // Never block startup over this, the user can still parse manually.
                Logger.Warn("Error checking for a display language change", ex);
                return false;
            }
        }

        /// <summary>
        /// Mirrors the "Load database" button: a clean slate, a full parse, then a player item stat refresh.
        /// </summary>
        private static void AutoParseDatabase(
            ParsingService parsingService,
            IDatabaseItemDao databaseItemDao,
            IPlayerItemDao playerItemDao,
            SettingsService settings,
            string gdPath
        ) {
            var modPath = settings.GetLocal().CurrentGrimdawnMod;

            if (!string.IsNullOrEmpty(modPath) && !Directory.Exists(modPath)) {
                Logger.Warn($"The previously parsed mod \"{modPath}\" no longer exists, parsing without it.");
                modPath = string.Empty;
            }

            databaseItemDao.Clean();
            parsingService.Update(gdPath, modPath);
            parsingService.Execute();

            using (var updatingPlayerItemsScreen = new UpdatingPlayerItemsScreen(playerItemDao)) {
                updatingPlayerItemsScreen.ShowDialog();
            }

            settings.GetLocal().CurrentGrimdawnLocation = gdPath;
            settings.GetLocal().GrimDawnLocationLastModified = ParsingService.GetHighestTimestamp(gdPath);
            settings.GetLocal().HasWarnedGrimDawnUpdate = false;
            settings.GetLocal().IsGrimDawnParsed = databaseItemDao.GetRowCount() > 0;
            settings.GetLocal().ParsedLanguageCode = settings.GetLocal().LanguageCode;

            // The item tags were dropped and rebuilt by the parse, but the language was loaded from the
            // old ones during startup. Without this every item name would render as a raw tag.
            RuntimeSettings.InitializeLanguage(settings.GetLocal().LanguageCode, databaseItemDao.GetTagDictionary());

            // A game update can add items whose icons have never been extracted, and the startup icon
            // check is a file-count heuristic that will not notice those.
            ArzParser.QueueIconExtraction(gdPath, modPath);
        }

        public void PerformIconCheck(GrimDawnDetector grimDawnDetector, SettingsService settings) {
            try {
                // Load the GD database (or mod, if any)
                string? gdPath = settings.GetLocal().CurrentGrimdawnLocation;

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    gdPath = grimDawnDetector.GetGrimLocations().FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {
                    int numFiles = Directory.GetFiles(GlobalPaths.StorageFolder).Length;
                    int numFilesExpected = 2100;
                    bool missingLokarrIcons = false;

                    if (Directory.Exists(Path.Combine(gdPath, "gdx3"))) {
                        // Fangs of Asterkarn. A complete icon extraction is ~717 more than
                        // base+gdx1+gdx2; kept slightly lower as a conservative floor.
                        numFilesExpected += 660;
                    }

                    if (Directory.Exists(Path.Combine(gdPath, "gdx2"))) {
                        numFilesExpected += 850;
                    }

                    if (Directory.Exists(Path.Combine(gdPath, "gdx1"))) {
                        numFilesExpected += 890;

                        // Lokarr boots. Need a re-parse if missing.
                        if (!File.Exists(Path.Combine(GlobalPaths.StorageFolder, "sign_f01a_dif.tex.png"))) {
                            missingLokarrIcons = true;
                        }
                    }

                    if (numFiles >= numFilesExpected && !missingLokarrIcons) {
                        return;
                    }

                    Logger.Debug($"Only found {numFiles} in storage, expected ~{numFilesExpected}+, parsing item icons.");
                    ArzParser.QueueIconExtraction(gdPath, null);
                }
                else {
                    Logger.Warn("Could not find the Grim Dawn install location");
                }
            }
            catch (Exception ex) {
                // Keep things moving, if icons are messed up its unfortunate, items should still be accessible.
                Logger.Warn("Error parsing icons", ex);
            }
        }
    }
}