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
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using log4net;

namespace IAGrim {
    public class StartupService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StartupService));
        public void Init() {

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            Logger.InfoFormat("Running version {0}.{1}.{2}.{3} from {4}", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToString("dd/MM/yyyy"));

            if (!DependencyChecker.CheckNet452Installed()) {
                MessageBox.Show("It appears .Net Framework 4.5.2 is not installed.\nIA May not function correctly", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!DependencyChecker.CheckVs2013Installed()) {
                MessageBox.Show("It appears VS 2013 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!DependencyChecker.CheckVs2010Installed()) {
                MessageBox.Show("It appears VS 2010 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Upgrade any settings if required
        /// This happens for just about every compile
        /// </summary>
        public static bool UpgradeSettings() {
            try {
                if (Properties.Settings.Default.CallUpgrade) {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.CallUpgrade = false;
                    Logger.Info("Settings upgraded..");

                    return true;
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

            return false;
        }


        // TODO: This creates another session instance, should be executed inside the ThreadExecuter
        public static void PrintStartupInfo(SessionFactory factory, SettingsService settings) {

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


            if (settings.GetPersistent().BuddySyncEnabled) {
                Logger.Info($"Buddy items is enabled with user id {settings.GetPersistent().BuddySyncUserIdV2}");
            }
            else {
                Logger.Info("Buddy items is disabled");
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
            foreach (var entry in new PlayerItemDaoImpl(factory, new DatabaseItemStatDaoImpl(factory)).GetModSelection()) {
                Logger.Info($"Mod: \"{entry.Mod}\", HC: {entry.IsHardcore}");
            }

            var gdPath = new DatabaseSettingDaoImpl(factory).GetCurrentDatabasePath();
            if (string.IsNullOrEmpty(gdPath)) {
                Logger.Info("The path to Grim Dawn is unknown (not great)");
            }
            else {
                Logger.Info($"The path to Grim Dawn is \"{gdPath}\"");
            }

            Logger.Info("Startup data dump complete");
        }


        public static SettingsService LoadSettingsService() {
            string settingsFile = GlobalPaths.SettingsFile;
            if (File.Exists(settingsFile)) {
                return SettingsService.Load(settingsFile);
            }
            else {
                Logger.Info("No settings file found, creating new settings file from legacy settings.");
                var p = Properties.Settings.Default;
                var service = SettingsService.Load(settingsFile);
                service.GetLocal().GrimDawnLocation = new List<string> { p.GrimDawnLocation };
                service.GetLocal().BackupNumber = p.BackupNumber;
                service.GetLocal().HasSuggestedLanguageChange = p.HasSuggestedLanguageChange;


                service.GetPersistent().BuddySyncUserIdV2 = p.BuddySyncUserIdV2;
                service.GetPersistent().BuddySyncDescription = p.BuddySyncDescription;
                service.GetPersistent().BuddySyncEnabled = p.BuddySyncEnabled;

                service.GetPersistent().SubscribeExperimentalUpdates = p.SubscribeExperimentalUpdates;
                service.GetPersistent().ShowRecipesAsItems = p.ShowRecipesAsItems;
                service.GetLocal().LastNagTimestamp = p.LastNagTimestamp;
                service.GetPersistent().MinimizeToTray = p.MinimizeToTray;
                service.GetPersistent().UsingDualComputer = p.UsingDualComputer;
                service.GetPersistent().ShowAugmentsAsItems = p.ShowAugmentsAsItems;
                service.GetPersistent().MergeDuplicates = p.MergeDuplicates;
                service.GetPersistent().TransferAnyMod = p.TransferAnyMod;
                service.GetLocal().SecureTransfers = p.SecureTransfers;
                service.GetPersistent().AutoUpdateModSettings = p.AutoUpdateModSettings;
                service.GetPersistent().DisplaySkills = p.DisplaySkills;
                service.GetLocal().StashToDepositTo = p.StashToDepositTo;
                service.GetLocal().StashToLootFrom = p.StashToLootFrom;
                service.GetLocal().LocalizationFile = p.LocalizationFile;

                service.GetLocal().BackupDropbox = p.BackupDropbox;
                service.GetLocal().BackupGoogle = p.BackupGoogle;
                service.GetLocal().BackupOnedrive = p.BackupOnedrive;
                service.GetLocal().BackupCustom = p.BackupCustom;
                service.GetLocal().BackupCustomLocation = p.BackupCustomLocation;

                service.GetPersistent().AzureAuthToken = p.AzureAuthToken;

                service.GetLocal().WindowPositionSettings = null;

                return service;
            }
        }

        public void PerformIconCheck(IDatabaseSettingDao databaseSettingDao, GrimDawnDetector grimDawnDetector) {
            // Load the GD database (or mod, if any)
            string gdPath = databaseSettingDao.GetCurrentDatabasePath();
            if (string.IsNullOrEmpty(gdPath) || !Directory.Exists(gdPath)) {
                gdPath = grimDawnDetector.GetGrimLocation();
            }

            if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {

                var numFiles = Directory.GetFiles(GlobalPaths.StorageFolder).Length;
                int numFilesExpected = 2100;
                if (Directory.Exists(Path.Combine(gdPath, "gdx2"))) {
                    numFilesExpected += 580;
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
    }
}
