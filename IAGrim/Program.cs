using EvilsoftCommons.Exceptions;
using EvilsoftCommons.SingleInstance;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Migrations;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI;
using IAGrim.UI.Misc.CEF;
using IAGrim.Utilities;
using log4net;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using IAGrim.Backup.Cloud;
using IAGrim.Database.DAO.Util;
using IAGrim.Database.Synchronizer.Core;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Utilities.Detection;

namespace IAGrim {
    internal class Program {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static MainWindow _mw;
        private static readonly StartupService StartupService = new StartupService();

#if DEBUG
        private static void Test() {
            var testytest = HttpUtility.ParseQueryString("https://token.iagd.evilsoft.net?bug=1&email=itemassistant@gmail.com&token=00e449aa-634b-43b1-a9d5-9e503bd040aa");
            return;
        }
#endif

        private static void LoadUuid(IDatabaseSettingDao dao, SettingsService settings) {
            var uuid = settings.GetPersistent().UUID;
            if (string.IsNullOrEmpty(uuid)) {
                uuid = dao.GetUuid();
                settings.GetPersistent().UUID = uuid;
            }

            if (string.IsNullOrEmpty(uuid)) {
                uuid = Guid.NewGuid().ToString().Replace("-", "");
                settings.GetPersistent().UUID = uuid;
            }

            RuntimeSettings.Uuid = uuid;
            ExceptionReporter.Uuid = uuid;
            Logger.InfoFormat("Your user id is {0}, use this for any bug reports", RuntimeSettings.Uuid);
        }

        public static MainWindow MainWindow => _mw;


        [STAThread]
        private static void Main(string[] args) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "Main";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }


            Logger.Info("Starting IA:GD..");
            if (BlockedLogsDetection.DreamcrashBlocked()) {
                Logger.Debug("Crash report server blocked, disabling reporting");
            }
            else {
                ExceptionReporter.UrlCrashreport = "http://ribbs.dreamcrash.org/iagd/crashreport.php";
                ExceptionReporter.UrlStats = "http://ribbs.dreamcrash.org/iagd/stats.php";
#if !DEBUG
                ExceptionReporter.LogExceptions = true;
#endif
            }

            Logger.Info("Starting exception monitor for bug reports..");
            Logger.Debug("Crash reports can be seen at http://ribbs.dreamcrash.org/iagd/logs.html");
            ExceptionReporter.EnableLogUnhandledOnThread();

            Uris.Initialize(Uris.EnvCloud);
            StartupService.Init();

#if DEBUG
            Test();
            Uris.Initialize(Uris.EnvLocalDev);
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
            using (SingleInstance singleInstance = new SingleInstance(guid)) {
                if (singleInstance.IsFirstInstance) {
                    Logger.Info("Calling run..");
                    singleInstance.ArgumentsReceived += singleInstance_ArgumentsReceived;
                    singleInstance.ListenForArgumentsFromSuccessiveInstances();
                    using (ThreadExecuter threadExecuter = new ThreadExecuter()) {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Logger.Info("Visual styles enabled..");
                        Run(args, threadExecuter);
                    }
                }
                else {
                    if (args != null && args.Length > 0) {
                        singleInstance.PassArgumentsToFirstInstance(args);
                    }
                    else {
                        singleInstance.PassArgumentsToFirstInstance(new string[] {"--ignore"});
                    }
                }
            }

            Logger.Info("IA Exited");
            LogManager.Shutdown();
        }


        /// <summary>
        /// Attempting to run a second copy of the program
        /// </summary>
        private static void singleInstance_ArgumentsReceived(object _, ArgumentsReceivedEventArgs e) {
            try {
                if (_mw != null) {
                    _mw.Invoke((MethodInvoker) delegate { _mw.Activate(); });
                }
            }
            catch (Exception ex) {
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

        private static void Run(string[] args, ThreadExecuter threadExecuter) {
            var dialect = SqlDialect.Sqlite;
            var factory = new SessionFactory(dialect);
            var serviceProvider = ServiceProvider.Initialize(threadExecuter, dialect);

            var settingsService = serviceProvider.Get<SettingsService>();
            var databaseItemDao = serviceProvider.Get<IDatabaseItemDao>();
            var databaseSettingDao = serviceProvider.Get<IDatabaseSettingDao>();
            var augmentationItemRepo = serviceProvider.Get<IAugmentationItemDao>();
            RuntimeSettings.InitializeLanguage(settingsService.GetLocal().LocalizationFile, databaseItemDao.GetTagDictionary());
            DumpTranslationTemplate();

            Logger.Debug("Executing DB migrations..");
            threadExecuter.Execute(() => new MigrationHandler(factory).Migrate());

            Logger.Debug("Loading UUID");
            LoadUuid(databaseSettingDao, settingsService);

            Logger.Debug("Updating augment state..");
            augmentationItemRepo.UpdateState();

            var itemTagDao = serviceProvider.Get<IItemTagDao>();
            var databaseItemStatDao = serviceProvider.Get<IDatabaseItemStatDao>();
            var itemSkillDao = serviceProvider.Get<IItemSkillDao>();
            ParsingService parsingService = new ParsingService(itemTagDao, null, databaseItemDao, databaseItemStatDao, itemSkillDao, settingsService.GetLocal().LocalizationFile);
            StartupService.PrintStartupInfo(factory, settingsService, dialect);


            // TODO: Offload to the new language loader
            if (RuntimeSettings.Language is EnglishLanguage language) {
                foreach (var tag in itemTagDao.GetClassItemTags()) {
                    language.SetTagIfMissing(tag.Tag, tag.Name);
                }
            }

            using (CefBrowserHandler browser = new CefBrowserHandler()) {
                _mw = new MainWindow(
                    serviceProvider,
                    browser,
                    parsingService
                );

                Logger.Info("Checking for database updates..");

                var grimDawnDetector = serviceProvider.Get<GrimDawnDetector>();
                StartupService.PerformIconCheck(databaseSettingDao, grimDawnDetector);

                ItemStatCacheService itemStatCacheService = new ItemStatCacheService(
                    serviceProvider.Get<IPlayerItemDao>(),
                    serviceProvider.Get<ItemStatService>()
                );
                itemStatCacheService.Start();

                try {
                    var playerItemDao = serviceProvider.Get<IPlayerItemDao>();
                    playerItemDao.DeleteDuplicates();
                }
                catch (Exception ex) {
                    Logger.Warn("Something went terribly wrong trying to ensure no duplicate items are found, however we'll just ignore it instead of blocking you access to your items.. sigh..", ex);
                }

                _mw.Visible = false;
                if (new DonateNagScreen(settingsService).CanNag)
                    Application.Run(new DonateNagScreen(settingsService));

                Logger.Info("Running the main application..");

                StartupService.PerformGrimUpdateCheck(settingsService);
                Application.Run(_mw);

                itemStatCacheService.Dispose();
            }

            Logger.Info("Application ended.");
        }
    }
}