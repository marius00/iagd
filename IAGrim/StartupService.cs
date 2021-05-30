using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using log4net;
using NHibernate.Util;

namespace IAGrim {
    public class StartupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartupService));
        public void Init() {

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            Logger.InfoFormat("Running version {0}.{1}.{2}.{3} from {4}", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToString("dd/MM/yyyy"));

            if (!DependencyChecker.CheckNet461Installed()) {
                MessageBox.Show("It appears .Net Framework 4.6.1 is not installed.\nIA May not function correctly", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            // TODO: Disabled due to false positives.. err negatives?
            /*if (!DependencyChecker.CheckVs2015Installed()) {
                MessageBox.Show("It appears VS 2015 (x86) redistributable may not be installed.\nInstall VS 2015 (x86) runtimes manually if you experience issues running IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/

            if (!DependencyChecker.CheckVs2013Installed()) {
                MessageBox.Show("It appears VS 2013 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!DependencyChecker.CheckVs2010Installed()) {
                MessageBox.Show("It appears VS 2010 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // TODO: This creates another session instance, should be executed inside the ThreadExecuter
        public static void PrintStartupInfo(SessionFactory factory, SettingsService settings, SqlDialect dialect) {

            if (settings.GetLocal().StashToLootFrom == 0) {
                Logger.Info("IA is configured to loot from the last stash page");
            }
            else {
                Logger.Info($"IA is configured to loot from stash page #{settings.GetLocal().StashToLootFrom}");
            }

            if (settings.GetLocal().StashToDepositTo == 0) {
                Logger.Info("IA is configured to deposit to the second-to-last stash page");
            }
            else {
                Logger.Info($"IA is configured to deposit to stash page #{settings.GetLocal().StashToDepositTo}");
            }

            using (var session = factory.OpenSession()) {
                var numItemsStored = session.CreateCriteria<PlayerItem>()
                    .SetProjection(NHibernate.Criterion.Projections.RowCountInt64())
                    .UniqueResult<long>();

                Logger.Info($"There are {numItemsStored} items stored in the database.");
            }


            if (settings.GetPersistent().ShowRecipesAsItems)
                Logger.Info("Show recipes as items is enabled");
            else
                Logger.Info("Show recipes as items is disabled");

            Logger.Info("Transfer to any mod is " + (settings.GetPersistent().TransferAnyMod ? "enabled" : "disabled"));
            Logger.Info("Experimental updates is " + (settings.GetPersistent().SubscribeExperimentalUpdates ? "enabled" : "disabled"));


            var mods = GlobalPaths.TransferFiles;
            if (mods.Count == 0) {
                Logger.Warn("No transfer files has been found");
            }
            else {
                Logger.Info("The following transfer files has been found:");
                foreach (var mod in mods) {
                    Logger.Info($"\"{mod.Filename}\": Mod: \"{mod.Mod}\", HC: {mod.IsHardcore}");
                }
            }

            Logger.Info("There are items stored for the following mods:");
            foreach (var entry in new PlayerItemDaoImpl(factory, new DatabaseItemStatDaoImpl(factory, dialect), dialect).GetModSelection()) {
                Logger.Info($"Mod: \"{entry.Mod}\", HC: {entry.IsHardcore}");
            }

            var gdPath = new DatabaseSettingDaoImpl(factory, dialect).GetCurrentDatabasePath();
            if (string.IsNullOrEmpty(gdPath)) {
                Logger.Info("The path to Grim Dawn is unknown (not great)");
            }
            else {
                Logger.Info($"The path to Grim Dawn is \"{gdPath}\"");
            }

            Logger.Info($"Using IA on multiple PCs: {settings.GetPersistent().UsingDualComputer}");

            Logger.Info("Startup data dump complete");
        }


        public static SettingsService LoadSettingsService() {
            return SettingsService.Load(GlobalPaths.SettingsFile);
        }

        public static void PerformGrimUpdateCheck(SettingsService settingsService) {
            var location = settingsService.GetLocal().GrimDawnLocation?.FirstOrNull()?.ToString();
            var lastParsed = settingsService.GetLocal().GrimDawnLocationLastModified;
            if (Directory.Exists(location)) {
                if (lastParsed > 0) {
                    var lastModified = ParsingService.GetHighestTimestamp(location);
                    if (lastModified > lastParsed) {
                        if (!settingsService.GetLocal().HasWarnedGrimDawnUpdate) {
                            Logger.Info("Grim Dawn appears to have been updated since last parse, notifying end user.");
                            var message = RuntimeSettings.Language.GetTag("iatag_ui_database_modified_body");
                            var title = RuntimeSettings.Language.GetTag("iatag_ui_database_modified_title");
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

        public void PerformIconCheck(IDatabaseSettingDao databaseSettingDao, GrimDawnDetector grimDawnDetector) {
            try {
                // Load the GD database (or mod, if any)
                string gdPath = databaseSettingDao.GetCurrentDatabasePath();
                if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                    gdPath = grimDawnDetector.GetGrimLocation();
                }

                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {

                    var numFiles = Directory.GetFiles(GlobalPaths.StorageFolder).Length;
                    int numFilesExpected = 2100;
                    if (Directory.Exists(Path.Combine(gdPath, "gdx2"))) {
                        numFilesExpected += 850;
                    }

                    if (Directory.Exists(Path.Combine(gdPath, "gdx1"))) {
                        numFilesExpected += 890;
                    }

                    if (numFiles < numFilesExpected) {
                        Logger.Debug($"Only found {numFiles} in storage, expected ~{numFilesExpected}+, parsing item icons.");
                        ThreadPool.QueueUserWorkItem((m) => ArzParser.LoadIconsOnly(gdPath));
                    }

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
