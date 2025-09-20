using IAGrim.Database;
using IAGrim.Database.DAO.Util;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace IAGrim {
    public class StartupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartupService));

        public void Init() {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
            Logger.InfoFormat("Running version {0}.{1}.{2}.{3} from {4:dd/MM/yyyy}",
                version.Major, version.Minor, version.Build, version.Revision, buildDate);

            FileVersionInfo dllVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Directory.GetCurrentDirectory(), "ItemAssistantHook_x64.dll"));
            FileVersionInfo playtestDllVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Directory.GetCurrentDirectory(), "ItemAssistantHook_playtest_x64.dll"));

            Logger.InfoFormat($"DLL version version {dllVersion.FileVersion}");
            Logger.InfoFormat($"Playtest DLL version version {playtestDllVersion.FileVersion}");

            var minimumDllVersion = File.ReadAllText("dllver.txt").Trim();
            if (dllVersion.FileVersion.CompareTo(minimumDllVersion) < 0) {
                Logger.Error("The DLL version is incompatible, did you perhaps run into a conflict while updating and clicked ignore?");
                Logger.Error("Item Assistant needs to be re-installed without GD running.");

                MessageBox.Show("IAGD install is corrupted.\nReinstall IAGD without GD running.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            if (!DependencyChecker.CheckVs2013Installed()) {
                MessageBox.Show("It appears VS 2013 (x86) redistributable is not installed.\nPlease install it to continue using IA",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!DependencyChecker.CheckVs2010Installed()) {
                MessageBox.Show("It appears VS 2010 (x86) redistributable is not installed.\nPlease install it to continue using IA",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        // TODO: This creates another session instance, should be executed inside the ThreadExecuter
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

                    if (numItemsStored < MainWindow.NumInstantSyncItemCount * 2.0/3.0) {
                        settings.GetLocal().PendingInstantSyncWarning = true;
                        Logger.Info($"The user has less than 2/3rd of {MainWindow.NumInstantSyncItemCount} items, queueing instant sync warning banner");
                    }

                    if (numItemsStored == 0)
                        Logger.Warn($"There are {numItemsStored} items stored in the database. <---- Unless you just installed IA, this is bad. No items.");
                    else
                        Logger.Info($"There are {numItemsStored} items stored in the database.");
                }


                Logger.Info("Transfer to any mod is " + (settings.GetPersistent().TransferAnyMod ? "enabled" : "disabled"));
                Logger.Info("Experimental updates is " + (settings.GetPersistent().SubscribeExperimentalUpdates ? "enabled" : "disabled"));
                Logger.Info((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator) ? "Running as administrator" : "Not running with low privileges");

                if (settings.GetLocal().PreferLegacyMode) {
                    Logger.Info("Running in legacy mode");
                    try {
                        List<GDTransferFile> mods = GlobalPaths.GetTransferFiles(true);

                        if (mods.Count == 0) {
                            Logger.Warn("No transfer files has been found");
                        }
                        else {
                            Logger.Info("The following transfer files has been found:");

                            foreach (GDTransferFile mod in mods) {
                                var stash = TransferStashService.GetStash(mod.Filename);
                                if (stash?.Tabs.Count < 2) {
                                    Logger.Warn($"\"{mod.Filename}\": Mod: \"{mod.Mod}\", HC: {mod.IsHardcore}, Downgrade: {mod.Downgrade}, Tabs: {stash?.Tabs.Count} <=======");
                                    Logger.Warn("Stash file does not have enough tabs");
                                }
                                else {
                                    Logger.Info($"\"{mod.Filename}\": Mod: \"{mod.Mod}\", HC: {mod.IsHardcore}, Downgrade: {mod.Downgrade}, Tabs: {stash?.Tabs.Count}");
                                }
                            }
                        }
                    }
                    catch (IOException ex) {
                        Logger.Fatal(ex.Message, ex);
                        Logger.Fatal("Error parsing transfer files. This is typically because the file is synced to OneDrive, but not available locally on the PC.");
                        MessageBox.Show("Error parsing transfer files.\nPossibly due to being located in OneDrive, but not synced to this PC.", "Fatal error", MessageBoxButtons.OK);
                        Process.Start("file://" + GlobalPaths.CoreFolder);
                        throw;
                    }
                } else {
                    Logger.Info("Running in regular mode, stash files ignored");
                }

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

        public static void PerformGrimUpdateCheck(SettingsService settingsService) {
            string location = settingsService.GetLocal().GrimDawnLocation?.FirstOrDefault();
            long lastParsed = settingsService.GetLocal().GrimDawnLocationLastModified;

            if (Directory.Exists(location)) {
                if (lastParsed > 0) {
                    long lastModified = ParsingService.GetHighestTimestamp(location);

                    if (lastModified > lastParsed) {
                        if (!settingsService.GetLocal().HasWarnedGrimDawnUpdate) {
                            Logger.Info("Grim Dawn appears to have been updated since last parse, notifying end user.");
                            string message = RuntimeSettings.Language.GetTag("iatag_ui_database_modified_body");
                            string title = RuntimeSettings.Language.GetTag("iatag_ui_database_modified_title");
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

        public void PerformIconCheck(GrimDawnDetector grimDawnDetector, SettingsService settings) {
            try {
                // Load the GD database (or mod, if any)
                string gdPath = settings.GetLocal().CurrentGrimdawnLocation;

                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    gdPath = grimDawnDetector.GetGrimLocations().FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {
                    int numFiles = Directory.GetFiles(GlobalPaths.StorageFolder).Length;
                    int numFilesExpected = 2100;
                    bool missingLokarrIcons = false;

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
                    ThreadPool.QueueUserWorkItem((m) => ArzParser.LoadIconsOnly(gdPath));
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