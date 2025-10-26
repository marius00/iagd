using DllInjector;
using EvilsoftCommons;
using EvilsoftCommons.Cloud;
using EvilsoftCommons.DllInjector;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Backup.Cloud.Util;
using IAGrim.BuddyShare;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.Arz.dto;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Parsers.TransferStash;
using IAGrim.Services;
using IAGrim.Services.ItemReplica;
using IAGrim.Services.MessageProcessor;
using IAGrim.Settings;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Popups;
using IAGrim.UI.Tabs;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using IAGrim.Utilities.HelperClasses;
using log4net;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace IAGrim.UI {
    public partial class MainWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));
        private readonly CefBrowserHandler _cefBrowserHandler;
        private readonly ISettingsReadController _settingsController;
        private readonly ServiceProvider _serviceProvider;
        private readonly TooltipHelper _tooltipHelper = new TooltipHelper();
        private readonly UsageStatisticsReporter _usageStatisticsReporter = new UsageStatisticsReporter();
        private readonly AutomaticUpdateChecker _automaticUpdateChecker;
        private CharacterBackupService _charBackupService;

        private readonly List<IMessageProcessor> _messageProcessors = new List<IMessageProcessor>();

        private SplitSearchWindow _searchWindow;
        private TransferStashWorker _transferStashWorker;

        private StashFileMonitor _stashFileMonitor = new StashFileMonitor();
        private CsvFileMonitor _csvFileMonitor = new CsvFileMonitor();
        private CsvFileMonitor _replicaCsvFileMonitor = new CsvFileMonitor();
        private ItemReplicaService _itemReplicaService;

        private Action<RegisterWindow.DataAndType> _registerWindowDelegate;
        private RegisterWindow _window;
        private InjectionHelper _injector;
        private ProgressChangedEventHandler _injectorCallbackDelegate;
        private CsvParsingService _csvParsingService;
        private ItemReplicaParser _itemReplicaParser;

        private BuddyItemsService _buddyItemsService;
        private BackgroundTask _backupBackgroundTask;
        private ItemTransferController _transferController;
        private readonly ParsingService _parsingService;
        private AuthService _authService;
        private BackupServiceWorker _backupServiceWorker;
        private readonly UserFeedbackService _userFeedbackService;
        private MinimizeToTrayHandler _minimizeToTrayHandler;
        private ModsDatabaseConfig _modsDatabaseConfigTab;
        public static int NumInstantSyncItemCount = 300;


        #region Stash Status

        // TODO: TEMPORARY FIX!
        private bool _hasShownStashErrorPage = false;
        private bool _hasShownSeasonErrorPage = false;
        private bool _hasShownPathErrorPage = false;
        private bool _hasShown32bitErrorPage = false;

        /// <summary>
        /// Toolstrip callback for GDInjector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InjectorCallback(object sender, ProgressChangedEventArgs e) {
            if (InvokeRequired) {
                Invoke((System.Windows.Forms.MethodInvoker) delegate { InjectorCallback(sender, e); });
            }
            else {
                switch (e.ProgressPercentage) {
                    case InjectionHelper.ABORTED:
                        RuntimeSettings.StashStatus = StashAvailability.MENU;
                        break;


                    case InjectionHelper.INJECTION_ERROR: {
                            if (RuntimeSettings.StashStatus == StashAvailability.MENU) {
                                // False positive, injection failed because we intentionally aborted.
                                break;
                            }

                            RuntimeSettings.StashStatus = StashAvailability.ERROR;
                            statusLabel.Text = e.UserState as string;
                            if (!_hasShownStashErrorPage) {
                                _cefBrowserHandler.ShowHelp(HelpService.HelpType.StashError);
                                _hasShownStashErrorPage = true;
                            }

                            break;
                        }


                    case InjectionHelper.GD_SEASON: {
                            if (!_hasShownSeasonErrorPage) {
                                _cefBrowserHandler.SetGdSeasonMode();
                                _hasShownSeasonErrorPage = true;
                            }

                            break;
                        }

                    case InjectionHelper.PATH_ERROR: {
                            RuntimeSettings.StashStatus = StashAvailability.ERROR;
                            if (!_hasShownPathErrorPage) {
                                _cefBrowserHandler.ShowHelp(HelpService.HelpType.PathError);
                                _hasShownPathErrorPage = true;
                            }

                            break;
                        }

                    case InjectionHelper.INJECTION_ERROR_32BIT: {
                        RuntimeSettings.StashStatus = StashAvailability.NOT64BIT;
                        statusLabel.Text = e.UserState as string;
                        if (!_hasShown32bitErrorPage) {
                            _cefBrowserHandler.ShowHelp(HelpService.HelpType.No32Bit);
                            _hasShown32bitErrorPage = true;
                        }

                        break;
                    }

                    // No grim dawn client, so stash is closed!
                    case InjectionHelper.NO_PROCESS_FOUND_ON_STARTUP: {
                            if (RuntimeSettings.StashStatus == StashAvailability.UNKNOWN) {
                            RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                        }

                        break;
                    }

                    // No grim dawn client, so stash is closed!
                    case InjectionHelper.NO_PROCESS_FOUND:
                        RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                        break;

                    // Injection error
                    case InjectionHelper.INJECTION_ERROR_POSSIBLE_ACCESS_DENIED: {
                        RuntimeSettings.StashStatus = StashAvailability.ERROR;
                        if (!_hasShownStashErrorPage) {
                            _cefBrowserHandler.ShowHelp(HelpService.HelpType.StashError);
                            _hasShownStashErrorPage = true;
                        }

                        break;
                    }
                    case InjectionHelper.STILL_RUNNING:
                        break;
                }

                _charBackupService.SetIsActive(RuntimeSettings.StashStatus == StashAvailability.CLOSED);
            }
        }

        #endregion Stash Status

        public MainWindow(
            ServiceProvider serviceProvider,
            ParsingService parsingService
        ) {
            this._serviceProvider = serviceProvider;
            var settingsService = _serviceProvider.Get<SettingsService>();
            _cefBrowserHandler = new CefBrowserHandler(settingsService);
            InitializeComponent();
            FormClosing += MainWindow_FormClosing;

            _minimizeToTrayHandler = new MinimizeToTrayHandler(this, notifyIcon1, serviceProvider.Get<SettingsService>());

            _automaticUpdateChecker = new AutomaticUpdateChecker(settingsService);
            _settingsController = new SettingsController(settingsService);
            _parsingService = parsingService;
            _userFeedbackService = new UserFeedbackService(_cefBrowserHandler);
        }

        private void Browser_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs args)
        {
            var browser = (sender as Microsoft.Web.WebView2.WinForms.WebView2);
            if (browser == null) {
                browser = (sender as CefBrowserHandler).BrowserControl;
            }
            if (args != null) {
                if (InvokeRequired) {
                    Invoke((System.Windows.Forms.MethodInvoker)delegate { Browser_CoreWebView2InitializationCompleted(sender, args); });
                }
                else {
                    var searchController = _serviceProvider.Get<SearchController>();
                    _cefBrowserHandler.InitializeChromium(browser, searchController.JsIntegration, tabControl1);
                    _cefBrowserHandler.IsReady = true;

                    _searchWindow?.UpdateListViewDelayed();

                    var isGdParsed = _serviceProvider.Get<IDatabaseItemDao>().GetRowCount() > 0;
                    var settingsService = _serviceProvider.Get<SettingsService>();
                    _cefBrowserHandler.SetDarkMode(settingsService.GetPersistent().DarkMode);
                    _cefBrowserHandler.SetHideItemSkills(settingsService.GetPersistent().HideSkills);
                    _cefBrowserHandler.SetIsGrimParsed(isGdParsed);


                    _cefBrowserHandler.SetOnlineBackupsEnabled(!settingsService.GetLocal().OptOutOfBackups);
                    _cefBrowserHandler.SetIsFirstRun(_serviceProvider.Get<IPlayerItemDao>().GetNumItems() == 0);
                    if (_serviceProvider.Get<IPlayerItemDao>().GetNumItems() == 0) {
                    } else if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) {
                        if (settingsService.GetLocal().EasterPrank) {
                            _cefBrowserHandler.SetEasterEggMode();
                            settingsService.GetLocal().EasterPrank = false;
                        }
                    }
                    else {
                        settingsService.GetLocal().EasterPrank = true;
                    }
                }
            }
        }

        /// <summary>
        /// Update the UI's language
        /// </summary>
        public void UpdateLanguage() {
            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);
            Text = RuntimeSettings.Language.GetTag(Tag.ToString());
            if (tsStashStatus.Tag is string) {
                tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
            }

            Refresh();
        }


        private void IterAndCloseForms(Control.ControlCollection controls) {
            foreach (Control c in controls) {
                Form f = c as Form;
                if (f != null)
                    f.Close();

                IterAndCloseForms(c.Controls);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            // No idea which of these are triggering on rare occasions, perhaps Deactivate, sizechanged or filterWindow.
            FormClosing -= MainWindow_FormClosing;
            SizeChanged -= OnMinimizeWindow;

            _authService?.Dispose();
            _authService = null;

            _stashFileMonitor?.Dispose();
            _stashFileMonitor = null;

            _csvFileMonitor?.Dispose();
            _csvFileMonitor = null;

            _replicaCsvFileMonitor?.Dispose();
            _replicaCsvFileMonitor = null;

            _csvParsingService?.Dispose();
            _csvParsingService = null;

            _itemReplicaService?.Dispose();
            _itemReplicaService = null;

            _minimizeToTrayHandler?.Dispose();
            _minimizeToTrayHandler = null;

            _backupBackgroundTask?.Dispose();
            _usageStatisticsReporter.Dispose();
            _automaticUpdateChecker.Dispose();

            _tooltipHelper?.Dispose();

            _buddyItemsService?.Dispose();
            _buddyItemsService = null;

            _injector?.Dispose();
            _injector = null;

            _backupServiceWorker?.Dispose();
            _backupServiceWorker = null;

            _window?.Dispose();
            _window = null;

            _itemReplicaParser?.Dispose();
            _itemReplicaParser = null;

            IterAndCloseForms(Controls);
        }


        /// <summary>
        /// Callback called when the Grim Dawn hook sends messages to IA
        /// </summary>
        /// <returns></returns>
        private void CustomWndProc(RegisterWindow.DataAndType bt) {
            // Most if not all actions may interact with SQL
            // SQL is done on the UI thread.
            if (InvokeRequired) {
                Invoke((System.Windows.Forms.MethodInvoker) delegate { CustomWndProc(bt); });
                return;
            }

            MessageType type = (MessageType) bt.Type;
            foreach (IMessageProcessor t in _messageProcessors) {
                t.Process(type, bt.Data, bt.StringData);
            }

            switch (type) {

                case MessageType.TYPE_REPORT_WORKER_THREAD_LAUNCHED:
                    Logger.Info("Grim Dawn hook reports successful launch.");
                    break;


                case MessageType.TYPE_GameInfo_IsHardcore:
                case MessageType.TYPE_GameInfo_IsHardcore_via_init:
                    Logger.Info($"TYPE_GameInfo_IsHardcore({bt.Data[0] > 0}, {type})");
                    if (_settingsController.AutoUpdateModSettings) {
                        _searchWindow.ModSelectionHandler.UpdateModSelection(bt.Data[0] > 0);
                    }

                    break;

                case MessageType.TYPE_GameInfo_SetModName:
                    Logger.InfoFormat("TYPE_GameInfo_SetModName({0})", IOHelper.GetPrefixString(bt.Data, 0));
                    if (_settingsController.AutoUpdateModSettings) {
                        _searchWindow.ModSelectionHandler.UpdateModSelection(IOHelper.GetPrefixString(bt.Data, 0));
                    }

                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) {
                _searchWindow.ClearFilters();
                return true; // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetFeedback(string feedback) {
            try {
                if (InvokeRequired) {
                    Invoke((System.Windows.Forms.MethodInvoker)delegate { SetFeedback(feedback); });
                }
                else {
                    statusLabel.Text = feedback.Replace("\\n", " - ");
                    _userFeedbackService.SetFeedback(feedback);
                }
            }
            catch (ObjectDisposedException) {
                Logger.Debug("Attempted to set feedback, but UI already disposed. (Probably shutting down)");
            }
        }

        private void SetInjectionAbortedStatus() {
            try {
                if (InvokeRequired) {
                    Invoke((System.Windows.Forms.MethodInvoker)SetInjectionAbortedStatus);
                }
                else {
                    InjectorCallback(null, new ProgressChangedEventArgs(InjectionHelper.ABORTED, null));
                    
                }
            }
            catch (ObjectDisposedException ex) {
                Logger.Warn(ex.ToString());
            }
        }


        private void TimerTickLookForGrimDawn(object sender, EventArgs e) {
            System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "DetectGrimDawnTimer";
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            var grimDawnDetector = _serviceProvider.Get<GrimDawnDetector>();
            if (grimDawnDetector.GetGrimLocations().Count > 0) {
                timer?.Stop();
                var xyx = grimDawnDetector.GetGrimLocations();
                var gdPath = grimDawnDetector.GetGrimLocations().First();

                // Attempt to force a database update
                foreach (Control c in modsPanel.Controls) {
                    if (c is ModsDatabaseConfig config) {
                        config.ForceDatabaseUpdate(gdPath, string.Empty);
                        break;
                    }
                }

                Logger.InfoFormat("Found Grim Dawn at {0}", gdPath);
            }
        }

        /// <summary>
        /// We've looted some items, so make sure the listview is up to date!
        /// Otherwise people freak out.
        ///
        /// The first ~1700 users did not notice at all, but past that seems its the end of days if items don't appear immediately.
        /// </summary>
        private void ListviewUpdateTrigger() {
            _searchWindow?.UpdateListViewDelayed();
        }

        private void DatabaseLoadedTrigger() {
            _searchWindow.UpdateInterface();
            _searchWindow?.UpdateListViewDelayed();
            _itemReplicaService.Reset();
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "UI";
            }

            Logger.Debug("Starting UI initialization");


            // Set version number
            DateTime buildDate = ExceptionReporter.BuildDate;
            statusLabel.Text = statusLabel.Text + $" - {ExceptionReporter.VersionString} from {buildDate.ToString("dd/MM/yyyy")}";
            tsVersionNumber.Text = ExceptionReporter.VersionString;


            var settingsService = _serviceProvider.Get<SettingsService>();
            ExceptionReporter.EnableLogUnhandledOnThread();
            SizeChanged += OnMinimizeWindow;


            // Chicken and the egg.. search controller needs browser, browser needs search controllers var.
            var databaseItemDao = _serviceProvider.Get<IDatabaseItemDao>();
            var searchController = _serviceProvider.Get<SearchController>();
            searchController.JsIntegration.OnRequestSetItemAssociations += (s, evvv) => { (evvv as GetSetItemAssociationsEventArgs).Elements = databaseItemDao.GetItemSetAssociations(); };

            searchController.Browser = _cefBrowserHandler;
            searchController.JsIntegration.OnClipboard += SetItemsClipboard;

            var playerItemDao = _serviceProvider.Get<IPlayerItemDao>();
            var cacher = _serviceProvider.Get<TransferStashServiceCache>();
            _parsingService.OnParseComplete += (o, args) => cacher.Refresh();


            var replicaItemDao = _serviceProvider.Get<IReplicaItemDao>();
            var stashWriter = new SafeTransferStashWriter(settingsService, _cefBrowserHandler);
            var transferStashService = new TransferStashService(settingsService);
            var transferStashService2 = new TransferStashService2(playerItemDao, cacher, transferStashService, stashWriter, _cefBrowserHandler);
            _serviceProvider.Add(transferStashService2);

            _transferStashWorker = new TransferStashWorker(transferStashService2, _userFeedbackService);

            if (settingsService.GetLocal().PreferLegacyMode) {
                _stashFileMonitor.OnStashModified += (_, __) => {
                    StashEventArg args = __ as StashEventArg;
                    _transferStashWorker.Queue(args?.Filename);

                    // May not minimize, but 
                    _usageStatisticsReporter.ResetLastMinimized();
                    _automaticUpdateChecker.ResetLastMinimized();
                };

                if (!_stashFileMonitor.StartMonitoring(GlobalPaths.SavePath)) {

                    MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_cloudsync_mb"));
                    _cefBrowserHandler.ShowHelp(HelpService.HelpType.CloudSavesEnabled);

                    Logger.Warn("Shutting down IA, unable to monitor stash files.");

                    if (!Debugger.IsAttached)
                        Close();
                }
            }

                

            // Load the grim database
            var grimDawnDetector = _serviceProvider.Get<GrimDawnDetector>();
            if (grimDawnDetector.GetGrimLocations().Count == 0) {
                Logger.Warn("Could not find the Grim Dawn install location");
                statusLabel.Text = "Could not find the Grim Dawn install location";

                var timer = new System.Windows.Forms.Timer();
                timer.Tick += TimerTickLookForGrimDawn;
                timer.Interval = 10000;
                timer.Start();
            }

            
            var buddyItemDao = _serviceProvider.Get<IBuddyItemDao>();
            var buddySubscriptionDao = _serviceProvider.Get<IBuddySubscriptionDao>();



            _authService = new AuthService(new AuthenticationProvider(settingsService), playerItemDao);
            var onlineSettings = new OnlineSettings(playerItemDao, settingsService, _cefBrowserHandler, buddyItemDao, buddySubscriptionDao);
            UIHelper.AddAndShow(onlineSettings, onlinePanel);
            _authService.OnAuthCompletion += (sender, args_) => {
                if (((args_ as AuthResultEvent)!).IsAuthorized) {
                    onlineSettings.UpdateUi();
                }
                else {
                    int x = 9;
                }
            };


            _modsDatabaseConfigTab = new ModsDatabaseConfig(DatabaseLoadedTrigger, playerItemDao, _parsingService, grimDawnDetector, settingsService, _cefBrowserHandler, databaseItemDao, replicaItemDao);
            UIHelper.AddAndShow(_modsDatabaseConfigTab, modsPanel);

            UIHelper.AddAndShow(new LoggingWindow(), panelLogging);

            var itemTagDao = _serviceProvider.Get<IItemTagDao>();
            var backupService = new BackupService(_authService, playerItemDao, settingsService, _cefBrowserHandler);
            _charBackupService = new CharacterBackupService(settingsService, _authService);
            _backupServiceWorker = new BackupServiceWorker(backupService, _charBackupService);
            searchController.JsIntegration.OnRequestBackedUpCharacterList += (_, args) => {
                RequestCharacterListEventArg a = args as RequestCharacterListEventArg;
                a.Characters = _charBackupService.ListBackedUpCharacters();
            };
            searchController.JsIntegration.OnRequestCharacterDownloadUrl += (_, args) => {
                RequestCharacterDownloadUrlEventArg a = args as RequestCharacterDownloadUrlEventArg;
                a.Url = _charBackupService.GetDownloadUrl(a.Character);
            };

            searchController.OnSearch += (o, args) => backupService.OnSearch();

            _searchWindow = new SplitSearchWindow(_cefBrowserHandler.BrowserControl, SetFeedback, playerItemDao, searchController, itemTagDao, settingsService);
            UIHelper.AddAndShow(_searchWindow, searchPanel);


            var browser = _searchWindow.Browser;
            browser.CoreWebView2InitializationCompleted += Browser_CoreWebView2InitializationCompleted;

            searchPanel.Height = searchPanel.Parent.Height;
            searchPanel.Width = searchPanel.Parent.Width;

            transferStashService2.OnUpdate += (_, __) => { _searchWindow.UpdateListView(); };

            var languagePackPicker = new LanguagePackPicker(itemTagDao, playerItemDao, _parsingService, settingsService);


            var dm = new DarkMode(this);
            UIHelper.AddAndShow(
                new SettingsWindow(
                    _cefBrowserHandler,
                    _tooltipHelper,
                    ListviewUpdateTrigger,
                    playerItemDao,
                    _searchWindow.ModSelectionHandler.GetAvailableModSelection(),
                    transferStashService2,
                    languagePackPicker,
                    settingsService,
                    grimDawnDetector,
                    dm,
                    _automaticUpdateChecker
                ),
                settingsPanel);

            
            _itemReplicaService = _serviceProvider.Get<ItemReplicaService>();
            _itemReplicaService.Start();
            

#if !DEBUG
            _automaticUpdateChecker.CheckForUpdates();
#endif

            Shown += (_, __) => { StartInjector(); };
            _buddyItemsService = new BuddyItemsService(
                buddyItemDao,
                3 * 60 * 1000,
                settingsService,
                _authService,
                buddySubscriptionDao
            );

            // Start the backup task
            _backupBackgroundTask = new BackgroundTask(new FileBackup(playerItemDao, settingsService));

            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);

            if (settingsService.GetLocal().PreferLegacyMode) {
                _messageProcessors.Add(new PlayerPositionTracker(Debugger.IsAttached && false));
                _messageProcessors.Add(new StashStatusHandler());
                _messageProcessors.Add(new CloudDetectorProcessor(SetFeedback, _serviceProvider.Get<SettingsService>()));
            }
            _messageProcessors.Add(new GenericErrorHandler());
            _messageProcessors.Add(new InjectionAbortedProcessor(SetInjectionAbortedStatus));


            RuntimeSettings.StashStatusChanged += GlobalSettings_StashStatusChanged;

            _transferController = new ItemTransferController(
                _cefBrowserHandler,
                SetFeedback,
                playerItemDao,
                transferStashService,
                settingsService
            );
            Application.AddMessageFilter(new MousewheelMessageFilter());


            var titleTag = RuntimeSettings.Language.GetTag("iatag_ui_itemassistant");
            if (!string.IsNullOrEmpty(titleTag)) {
                this.Text += $" - {titleTag}";
            }

            if (_authService.CheckAuthentication() == AuthService.AccessStatus.Unauthorized && !settingsService.GetLocal().OptOutOfBackups && playerItemDao.GetNumItems() > 100) {
                var authService = new AuthService(new AuthenticationProvider(settingsService), _serviceProvider.Get<IPlayerItemDao>());
                new BackupLoginNagScreen(authService, settingsService).Show();
            }

            searchController.JsIntegration.ItemTransferEvent += TransferItem;
            new WindowSizeManager(this, settingsService);


            // Suggest translation packs if available
            if (string.IsNullOrEmpty(settingsService.GetLocal().LocalizationFile) && !settingsService.GetLocal().HasSuggestedLanguageChange) {
                if (LocalizationLoader.HasSupportedTranslations(grimDawnDetector.GetGrimLocations())) {
                    Logger.Debug("A new language pack has been detected, informing end user..");
                    new LanguagePackPicker(itemTagDao, playerItemDao, _parsingService, settingsService).Show(grimDawnDetector.GetGrimLocations());

                    settingsService.GetLocal().HasSuggestedLanguageChange = true;
                }
            }


            if (settingsService.GetPersistent().DarkMode) {
                dm.Activate(); // Needs a lot more work before its ready, for example custom components uses Draw and does not respect coloring.
                _cefBrowserHandler.SetDarkMode(settingsService.GetPersistent().DarkMode);
            }

            settingsService.GetLocal().OnMutate += delegate(object o, EventArgs args) { _cefBrowserHandler.SetOnlineBackupsEnabled(!settingsService.GetLocal().OptOutOfBackups); };


            _csvParsingService = new CsvParsingService(playerItemDao, _userFeedbackService, cacher, transferStashService);
            _csvFileMonitor.OnModified += (_, arg) => {
                var csvEvent = arg as CsvFileMonitor.CsvEvent;
                _csvParsingService.Queue(csvEvent.Filename, csvEvent.Cooldown);
            };

            _itemReplicaParser = new ItemReplicaParser(replicaItemDao, playerItemDao, _cefBrowserHandler);
            _replicaCsvFileMonitor.OnModified += (_, arg) => {
                _itemReplicaParser.Enqueue(arg);
            };
            _itemReplicaParser.Start();

            // Typically new users expect that items just magically appear, which is a terrible user experience when you have thousands of items. But works fine for a small set of items.
            // Maybe you should get a help screen when crossing "the magic threshold"? Explaining how it changes..
            var shouldAutoSearchOnNewItems = playerItemDao.GetNumItems() < NumInstantSyncItemCount;

            _csvParsingService.OnItemLooted += (_, arg) => {
                _searchWindow.SelectModFilterIfNotSelected();

                if (shouldAutoSearchOnNewItems) {
                    _searchWindow.UpdateListView();

                    if (playerItemDao.GetNumItems() > NumInstantSyncItemCount) {
                        settingsService.GetLocal().PendingInstantSyncWarning = false;
                        shouldAutoSearchOnNewItems = false;
                        _cefBrowserHandler.ShowNoMoreInstantSyncWarning();
                        Logger.Info($"Item count has reached {NumInstantSyncItemCount}, showing warning and disabling instant sync");
                    }
                }

            };

            _csvFileMonitor.StartMonitoring(GlobalPaths.CsvLocationIngoing, "*.csv");
            _replicaCsvFileMonitor.StartMonitoring(GlobalPaths.CsvReplicaReadLocation, "*.json");
            _csvParsingService.Start();


            var preloadThread= new Thread(_itemReplicaParser.Preload);
            preloadThread.Start();

            tsStashStatus.Visible = settingsService.GetLocal().PreferLegacyMode;

            Logger.Debug("UI initialization complete");
        }

        void TransferItem(object ignored, EventArgs args) {
            _transferController.TransferItem(args as StashTransferEventArgs);
        }


        private void StartInjector() {
            // Start looking for GD processes!
            _registerWindowDelegate = CustomWndProc;
            _window = new RegisterWindow("GDIAWindowClass", _registerWindowDelegate);

            // This prevents a implicit cast to new ProgressChangedEventHandler(func), which would hit the GC and before being used from another thread
            // Same happens when shutting down, fix unknown
            _injectorCallbackDelegate = InjectorCallback;


            string dllname = "ItemAssistantHook_x64.dll";
            _injector = new InjectionHelper(_injectorCallbackDelegate, false, "Grim Dawn", string.Empty, dllname);
        }


        private void GlobalSettings_StashStatusChanged(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke((System.Windows.Forms.MethodInvoker) delegate { GlobalSettings_StashStatusChanged(sender, e); });
                return;
            }

            var settingsService = _serviceProvider.Get<SettingsService>();
            tsStashStatus.Visible = settingsService.GetLocal().PreferLegacyMode;

            switch (RuntimeSettings.StashStatus) {
                case StashAvailability.OPEN:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_open";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    break;
                case StashAvailability.CRAFTING:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_crafting";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    break;
                case StashAvailability.CLOSED:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 0, 142, 0);
                    tsStashStatus.Tag = "iatag_stash_closed";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    break;
                case StashAvailability.ERROR:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_error";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    tsStashStatus.Visible = true;
                    break;
                case StashAvailability.UNKNOWN:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_unknown";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    break;
                case StashAvailability.NOT64BIT:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_not64bit";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    tsStashStatus.Visible = true;
                    break;
                case StashAvailability.MENU:
                    tsStashStatus.Visible = settingsService.GetLocal().PreferLegacyMode;
                    break;
                default:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = null;
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag("iatag_stash_") + RuntimeSettings.StashStatus.ToString().ToLowerInvariant();
                    break;
            }
        }

        #region Tray and Menu

        /// <summary>
        /// Minimize to tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object sender, EventArgs e) {
            _usageStatisticsReporter.ResetLastMinimized();
            _automaticUpdateChecker.ResetLastMinimized();
        }


        private void trayContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            e.Cancel = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        #endregion Tray and Menu


        private void SetItemsClipboard(object ignored, EventArgs _args) {
            if (InvokeRequired) {
                Invoke((System.Windows.Forms.MethodInvoker) delegate { SetItemsClipboard(ignored, _args); });
            }
            else {
                if (_args is ClipboardEventArg args) {
                    Clipboard.SetText(args.Text);
                }

                _tooltipHelper.ShowTooltipAtMouse(RuntimeSettings.Language.GetTag("iatag_copied_clipboard"), _cefBrowserHandler.BrowserControl);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            _minimizeToTrayHandler.notifyIcon_MouseDoubleClick(sender, null);
        }

    } // CLASS
}