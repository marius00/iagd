using EvilsoftCommons.Exceptions;
using EvilsoftCommons.Exceptions.UUIDGenerator;
using EvilsoftCommons.SingleInstance;
using IAGrim.Backup.Azure.Constants;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Migrations;
using IAGrim.Database.Synchronizer;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace IAGrim
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static MainWindow _mw;
        private static readonly StartupService StartupService = new StartupService();

#if DEBUG

        private static void Test() {
            return;
        }
#endif

        private static void LoadUuid(IDatabaseSettingDao dao)
        {
            string uuid = dao.GetUuid();
            if (string.IsNullOrEmpty(uuid))
            {
                UuidGenerator g = Guid.NewGuid();
                uuid = g.ToString().Replace("-", "");
                dao.SetUuid(uuid);
            }

            RuntimeSettings.Uuid = uuid;
            ExceptionReporter.Uuid = uuid;
            Logger.InfoFormat("Your user id is {0}, use this for any bug reports", RuntimeSettings.Uuid);
        }

        public static MainWindow MainWindow => _mw;

        [STAThread]
        private static void Main(string[] args)
        {
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

#if DEBUG
            AzureUris.Initialize(AzureUris.EnvAzure);
            //AzureUris.Initialize(AzureUris.EnvDev);
#else
            AzureUris.Initialize(AzureUris.EnvAzure);
#endif

            StartupService.Init();

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

            ItemHtmlWriter.CopyMissingFiles();

            Guid guid = new Guid("{F3693953-C090-4F93-86A2-B98AB96A9368}");
            using (SingleInstance singleInstance = new SingleInstance(guid))
            {
                if (singleInstance.IsFirstInstance)
                {
                    Logger.Info("Calling run..");
                    singleInstance.ArgumentsReceived += singleInstance_ArgumentsReceived;
                    singleInstance.ListenForArgumentsFromSuccessiveInstances();
                    using (ThreadExecuter threadExecuter = new ThreadExecuter())
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Logger.Info("Visual styles enabled..");
                        Run(args, threadExecuter);
                    }
                }
                else
                {
                    if (args != null && args.Length > 0)
                    {
                        singleInstance.PassArgumentsToFirstInstance(args);
                    }
                    else
                    {
                        singleInstance.PassArgumentsToFirstInstance(new string[] { "--ignore" });
                    }
                }
            }

            Logger.Info("IA Exited");
        }


        /// <summary>
        /// Attempting to run a second copy of the program
        /// </summary>
        private static void singleInstance_ArgumentsReceived(object _, ArgumentsReceivedEventArgs e)
        {
            try
            {
                if (_mw != null)
                {
                    _mw.Invoke((MethodInvoker)delegate { _mw.notifyIcon1_MouseDoubleClick(null, null); });
                    _mw.Invoke((MethodInvoker)delegate { _mw.Activate(); });
                }
            }
            catch (Exception ex)
            {
                ExceptionReporter.ReportException(ex, "singleInstance_ArgumentsReceived");
            }
        }


        private static void DumpTranslationTemplate() {
            try {
                File.WriteAllText(Path.Combine(GlobalPaths.CoreFolder, "tags_ia.template.txt"), new EnglishLanguage(new Dictionary<string, string>()).Export());
            }
            catch (Exception ex) {
                Logger.Debug("Error dumping translation template", ex);
            }
        }

        private static void Run(string[] args, ThreadExecuter threadExecuter)
        {
            var factory = new SessionFactory();

            // Settings should be upgraded early, it contains the language pack etc and some services depends on settings.
            var settingsService = StartupService.LoadSettingsService();
            IPlayerItemDao playerItemDao = new PlayerItemRepo(threadExecuter, factory);
            StartupService.UpgradeSettings();
            
            // X
            IDatabaseItemDao databaseItemDao = new DatabaseItemRepo(threadExecuter, factory);
            RuntimeSettings.InitializeLanguage(settingsService.GetLocal().LocalizationFile, databaseItemDao.GetTagDictionary());
            DumpTranslationTemplate();

            threadExecuter.Execute(() => new MigrationHandler(factory).Migrate());

            IDatabaseSettingDao databaseSettingDao = new DatabaseSettingRepo(threadExecuter, factory);
            LoadUuid(databaseSettingDao);
            var azurePartitionDao = new AzurePartitionRepo(threadExecuter, factory);
            IDatabaseItemStatDao databaseItemStatDao = new DatabaseItemStatRepo(threadExecuter, factory);
            IItemTagDao itemTagDao = new ItemTagRepo(threadExecuter, factory);


#if !DEBUG
            if (statUpgradeNeeded) {

                // If we don't also update item stats, a lot of whining will ensue.
                // This accounts for most database updates (new fields added that needs to get populated etc)
                UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(playerItemDao);
                x.ShowDialog();
            }
#endif


            IBuddyItemDao buddyItemDao = new BuddyItemRepo(threadExecuter, factory);
            IBuddySubscriptionDao buddySubscriptionDao = new BuddySubscriptionRepo(threadExecuter, factory);
            IRecipeItemDao recipeItemDao = new RecipeItemRepo(threadExecuter, factory);
            IItemSkillDao itemSkillDao = new ItemSkillRepo(threadExecuter, factory);
            AugmentationItemRepo augmentationItemRepo = new AugmentationItemRepo(threadExecuter, factory, new DatabaseItemStatDaoImpl(factory));
            var grimDawnDetector = new GrimDawnDetector(settingsService);

            Logger.Debug("Updating augment state..");
            augmentationItemRepo.UpdateState();

            // TODO: GD Path has to be an input param, as does potentially mods.
            ParsingService parsingService = new ParsingService(itemTagDao, null, databaseItemDao, databaseItemStatDao, itemSkillDao, settingsService.GetLocal().LocalizationFile);
            StartupService.PrintStartupInfo(factory, settingsService);



            if (RuntimeSettings.Language is EnglishLanguage language)
            {
                foreach (var tag in itemTagDao.GetClassItemTags())
                {
                    language.SetTagIfMissing(tag.Tag, tag.Name);
                }
            }

            if (args != null && args.Any(m => m.Contains("-logout"))) {
                Logger.Info("Started with -logout specified, logging out of online backups.");
                settingsService.GetPersistent().AzureAuthToken = null;
            }

            using (CefBrowserHandler browser = new CefBrowserHandler())
            {
                _mw = new MainWindow(browser,
                    databaseItemDao,
                    databaseItemStatDao,
                    playerItemDao,
                    azurePartitionDao,
                    databaseSettingDao,
                    buddyItemDao,
                    buddySubscriptionDao,
                    recipeItemDao,
                    itemSkillDao,
                    itemTagDao,
                    parsingService,
                    augmentationItemRepo,
                    settingsService,
                    grimDawnDetector
                );

                Logger.Info("Checking for database updates..");

                StartupService.PerformIconCheck(databaseSettingDao, grimDawnDetector);

                _mw.Visible = false;
                if (new DonateNagScreen(settingsService).CanNag)
                    Application.Run(new DonateNagScreen(settingsService));

                Logger.Info("Running the main application..");


                Application.Run(_mw);
            }

            Logger.Info("Application ended.");
        }

    }
}