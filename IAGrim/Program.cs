using EvilsoftCommons.Exceptions;
using EvilsoftCommons.SingleInstance;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Migrations;
using IAGrim.Database.Synchronizer;
using IAGrim.Parsers.Arz;
using IAGrim.UI;
using IAGrim.UI.Misc;
using IAGrim.Utilities;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DataAccess;
using EvilsoftCommons;
using EvilsoftCommons.Exceptions.UUIDGenerator;
using Gameloop.Vdf;
using IAGrim.BuddyShare;
using IAGrim.Database.DAO;
using IAGrim.Parser.Arc;
using IAGrim.Services.Crafting;
using IAGrim.UI.Popups;
using IAGrim.Utilities.HelperClasses;


namespace IAGrim {

    internal class 
        Program {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static MainWindow _mw;

#if DEBUG

        private static void Test() {
            string bracket = "[";
            var splitted = bracket.Split('[');
            List<int> xl = new List<int>();
            xl.Add(1);
            xl.Add(5);
            xl.Add(2);
            var t = xl.Take(50).ToList();

            int x = 9;
            /*
            var json = "hello_world";
            using (var stream = new MemoryStream()) {
                using (GZipStream gz = new GZipStream(stream, CompressionMode.Compress, true)) {
                    var encoding = new ASCIIEncoding();
                    byte[] data123 = encoding.GetBytes(json);
                    gz.Write(data123, 0, data123.Length);
                }
                var c = Convert.ToBase64String(stream.ToArray());
                var s = new Synchronizer().UploadBuddyData($"json={c}", "http://grimdawn.dreamcrash.org/buddyitems/v2/test");
                
                int x123 = 9;
            }

            

            using (ThreadExecuter threadExecuter = new ThreadExecuter()) {
                var factory = new SessionFactory();
                
                
                var repo = new BuddyItemRepo(threadExecuter, factory);
                var allItems = repo.ListAll();
                var items = repo.ListItemsWithMissingRarity();
                repo.UpdateRarity(items);
                
                repo.UpdateNames(allItems);
                repo.UpdateLevelRequirements(allItems);
                
            }*/
            return;
            
            using (ThreadExecuter threadExecuter = new ThreadExecuter()) {
                var factory = new SessionFactory();
                string arcTagfile = @"D:\SteamLibrary\steamapps\common\Grim Dawn\resources\text_en.arc";
                var arcItemsFile = @"D:\SteamLibrary\steamapps\common\Grim Dawn\resources\items.arc";
                var databaseFile = @"D:\SteamLibrary\steamapps\common\Grim Dawn\database\database.arz";

                List<IItemTag> _tags = Parser.Arz.ArzParser.ParseArcFile(arcTagfile);
                var mappedTags = _tags.ToDictionary(item => item.Tag, item => item.Name);

                List<IItem> _items = Parser.Arz.ArzParser.LoadItemRecords(databaseFile, true); // TODO


                var skillParser = new ComplexItemParser(_items, mappedTags);
                skillParser.Generate();


                var _itemSkillDao = new ItemSkillRepo(threadExecuter, factory);

                //_itemSkillDao.Save(skillParser.Skills);
                //_itemSkillDao.Save(skillParser.SkillItemMapping);


                IPlayerItemDao playerItemDao = new PlayerItemRepo(threadExecuter, factory);
                IDatabaseItemDao databaseItemDao = new DatabaseItemRepo(threadExecuter, factory);
                IDatabaseItemStatDao databaseItemStatDao = new DatabaseItemStatRepo(threadExecuter, factory);
                /*
                var xys = databaseItemDao.GetValidClassItemTags();
                Parser.Arc.Decompress c = new Parser.Arc.Decompress(@"D:\SteamLibrary\steamapps\common\Grim Dawn\resources\Items.arc", true);
                c.decompress();
                */
            }
            
        }
#endif


        private static void LoadUuid(IDatabaseSettingDao dao) {
            string uuid = dao.GetUuid();
            if (string.IsNullOrEmpty(uuid)) {
                UuidGenerator g = Guid.NewGuid();
                uuid = g.ToString().Replace("-", "");
                dao.SetUuid(uuid);
            }

            GlobalSettings.Uuid = uuid;
            ExceptionReporter.Uuid = uuid;
            Logger.InfoFormat("Your user id is {0}, use this for any bug reports", GlobalSettings.Uuid);
        }


        [STAThread]
        private static void Main(string[] args) {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Main";

            Logger.Info("Starting IA:GD..");
            ExceptionReporter.UrlCrashreport = "http://ribbs.dreamcrash.org/iagd/crashreport.php"; 
            ExceptionReporter.UrlStats = "http://ribbs.dreamcrash.org/iagd/stats.php";
#if !DEBUG
            ExceptionReporter.LogExceptions = true;
#endif

            Logger.Info("Starting exception monitor for bug reports.."); // Phrased this way since people took it as a 'bad' thing.
            Logger.Debug("Crash reports can be seen at http://ribbs.dreamcrash.org/iagd/logs.html");
            ExceptionReporter.EnableLogUnhandledOnThread();


            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            Logger.InfoFormat("Running version {0}.{1}.{2}.{3} from {4}", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToString("dd/MM/yyyy"));


            if (!DependencyChecker.CheckNet452Installed()) {
                MessageBox.Show("It appears .Net Framework 4.5.2 is not installed.\nIA May not function correctly", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!DependencyChecker.CheckVS2013Installed()) {
                MessageBox.Show("It appears VS 2013 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (!DependencyChecker.CheckVS2010Installed()) {
                MessageBox.Show("It appears VS 2010 (x86) redistributable is not installed.\nPlease install it to continue using IA", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

#if DEBUG
            Test();
#endif

            // Prevent running in RELEASE mode by accident
            // And thus risking the live database
#if !DEBUG
            if (Debugger.IsAttached) {
                Logger.Fatal("Debugger attached, please run in DEBUG mode");
                return;
            }
#endif
            //ParsingUIBackgroundWorker tmp = new ParsingUIBackgroundWorker();

            Guid guid = new Guid("{F3693953-C090-4F93-86A2-B98AB96A9368}");
            using (SingleInstance singleInstance = new SingleInstance(guid)) {
                if (singleInstance.IsFirstInstance) {
                    Logger.Info("Calling run..");
                    using (ThreadExecuter threadExecuter = new ThreadExecuter()) {
                        Run(threadExecuter);
                    }
                } else {
                    singleInstance_ArgumentsReceived(null, null);
                }
            }
        }

        /// <summary>
        /// Upgrade any settings if required
        /// This happens for just about every compile
        /// </summary>
        private static void UpgradeSettings(IPlayerItemDao playerItemDao) {
            try {
                if (Properties.Settings.Default.CallUpgrade) {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.CallUpgrade = false;
                    Logger.Info("Settings upgraded..");

#if !DEBUG

                    // If we don't also update item stats, a lot of whining will ensue.
                    // This accounts for most database updates (new fields added that needs to get populated etc)
                    UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(playerItemDao);
                    x.ShowDialog();
#endif

                }
            } catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                ExceptionReporter.ReportException(ex);
            }

        }

        /// <summary>
        /// Attempting to run a second copy of the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void singleInstance_ArgumentsReceived(object _, ArgumentsReceivedEventArgs e) {
            try {
                if (_mw != null) {
                    Action<string[]> restoreWindow = arguments => {
                        _mw.WindowState = FormWindowState.Normal;
                        _mw.Activate();
                    };

                    _mw.Invoke(restoreWindow);
                }
            } catch (Exception ex) {
                ExceptionReporter.ReportException(ex, "singleInstance_ArgumentsReceived");
            }
        }

        private static void PrintStartupInfo(SessionFactory factory) {
            if (Properties.Settings.Default.StashToLootFrom == 0) {
                Logger.Info("IA is configured to loot from the last stash page");
            }
            else {
                Logger.Info($"IA is configured to loot from stash page #{Properties.Settings.Default.StashToLootFrom}");
            }
            if (Properties.Settings.Default.StashToDepositTo == 0) {
                Logger.Info("IA is configured to deposit to the second-to-last stash page");
            }
            else {
                Logger.Info($"IA is configured to deposit to stash page #{Properties.Settings.Default.StashToDepositTo}");
            }

            using (var session = factory.OpenSession()) {
                var numItemsStored = session.CreateCriteria<PlayerItem>()
                    .SetProjection(NHibernate.Criterion.Projections.RowCountInt64())
                    .UniqueResult<long>();

                Logger.Info($"There are {numItemsStored} items stored in the database.");
            }

            if (Properties.Settings.Default.BuddySyncEnabled)
                Logger.Info($"Buddy items is enabled with user id {Properties.Settings.Default.BuddySyncUserIdV2}");
            else
                Logger.Info("Buddy items is disabled");

            if (Properties.Settings.Default.ShowRecipesAsItems)
                Logger.Info("Show recipes as items is enabled");
            else
                Logger.Info("Show recipes as items is disabled");

            Logger.Info("Transfer to any mod is " + (Properties.Settings.Default.TransferAnyMod ? "enabled" : "disabled"));
            Logger.Info("Experimental updates is " + (Properties.Settings.Default.SubscribeExperimentalUpdates ? "enabled" : "disabled"));
            Logger.Info($"Instaloot is set to {Properties.Settings.Default.InstalootSetting} ({(InstalootSettingType)Properties.Settings.Default.InstalootSetting})");

            if (Properties.Settings.Default.UserNeverWantsBackups)
                Logger.Warn("You have opted out of backups");


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

        private static void Run(ThreadExecuter threadExecuter) {
            var factory = new SessionFactory();

            // Prohibited for now
            Properties.Settings.Default.InstaTransfer = false;
            Properties.Settings.Default.Save();

            new MigrationHandler(factory).Migrate();
            IDatabaseSettingDao databaseSettingDao = new DatabaseSettingRepo(threadExecuter, factory);
            LoadUuid(databaseSettingDao);
            IPlayerItemDao playerItemDao = new PlayerItemRepo(threadExecuter, factory);
            IDatabaseItemDao databaseItemDao = new DatabaseItemRepo(threadExecuter, factory);
            IDatabaseItemStatDao databaseItemStatDao = new DatabaseItemStatRepo(threadExecuter, factory);
            
            IBuddyItemDao buddyItemDao = new BuddyItemRepo(threadExecuter, factory);
            IBuddySubscriptionDao buddySubscriptionDao = new BuddySubscriptionRepo(threadExecuter, factory);
            IRecipeItemDao recipeItemDao = new RecipeItemRepo(threadExecuter, factory);
            IItemSkillDao itemSkillDao  = new ItemSkillRepo(threadExecuter, factory);
            ArzParser arzParser = new ArzParser(databaseItemDao, databaseItemStatDao, databaseSettingDao, itemSkillDao);

            PrintStartupInfo(factory);



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logger.Info("Visual styles enabled..");
            UpgradeSettings(playerItemDao);

            var language = GlobalSettings.Language as StatTranslator.EnglishLanguage;
            if (language != null) {
                foreach (var tag in databaseItemDao.GetClassItemTags()) {
                    language.SetTagIfMissing(tag.Tag, tag.Name);
                }
            }


            using (CefBrowserHandler browser = new CefBrowserHandler()) {
                _mw = new MainWindow(browser, 
                    databaseItemDao, 
                    databaseItemStatDao, 
                    playerItemDao, 
                    databaseSettingDao, 
                    buddyItemDao, 
                    buddySubscriptionDao, 
                    arzParser,
                    recipeItemDao,
                    itemSkillDao
                );

                Logger.Info("Checking for database updates..");
            

                // Load the GD database (or mod, if any)
                string GDPath = databaseSettingDao.GetCurrentDatabasePath();
                bool isVanilla;
                if (string.IsNullOrEmpty(GDPath) || !Directory.Exists(GDPath)) {
                    GDPath = GrimDawnDetector.GetGrimLocation();
                    isVanilla = true;
                } else {
                    isVanilla = GDPath.Equals(GrimDawnDetector.GetGrimLocation());
                }

                if (!string.IsNullOrEmpty(GDPath) && Directory.Exists(GDPath)) {
                    if (arzParser.NeedUpdate(GDPath)) {
                        
                        ParsingDatabaseScreen parserUI = new ParsingDatabaseScreen(
                            databaseSettingDao,
                            arzParser,
                            GDPath, 
                            Properties.Settings.Default.LocalizationFile, 
                            false, 
                            !isVanilla);
                        parserUI.ShowDialog();
                    }

                    if (playerItemDao.RequiresStatUpdate()) {
                        UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(playerItemDao);
                        x.ShowDialog();
                    }



                    var numFiles = Directory.GetFiles(GlobalPaths.StorageFolder).Length;
                    if (numFiles < 2000) {
                        Logger.Debug($"Only found {numFiles} in storage, expected ~3200+, parsing item icons.");
                        ThreadPool.QueueUserWorkItem((m) => ArzParser.LoadIconsOnly(GDPath));
                    }

                } else {
                    Logger.Warn("Could not find the Grim Dawn install location");
                }

                playerItemDao.UpdateHardcoreSettings();

                _mw.Visible = false;
                if (DonateNagScreen.CanNag)
                    Application.Run(new DonateNagScreen());

                Logger.Info("Running the main application..");


                Application.Run(_mw);
            }

            Logger.Info("Application ended.");
        }
    }
} 