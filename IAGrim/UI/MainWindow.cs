using CefSharp;
using EvilsoftCommons;
using EvilsoftCommons.Cloud;
using EvilsoftCommons.DllInjector;
using EvilsoftCommons.Exceptions;
using IAGrim.BuddyShare;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.Arz.dto;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Services;
using IAGrim.Services.MessageProcessor;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Tabs;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CefSharp.WinForms;
using DllInjector;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Backup.Cloud.Util;
using IAGrim.Parsers.TransferStash;
using IAGrim.Settings;

namespace IAGrim.UI {
    public partial class MainWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));
        private readonly CefBrowserHandler _cefBrowserHandler;
        private readonly ISettingsReadController _settingsController;
        private readonly ServiceProvider _serviceProvider;
        private readonly TooltipHelper _tooltipHelper = new TooltipHelper();
        private readonly DynamicPacker _dynamicPacker;
        private readonly UsageStatisticsReporter _usageStatisticsReporter = new UsageStatisticsReporter();
        private readonly AutomaticUpdateChecker _automaticUpdateChecker;
        private CharacterBackupService _charBackupService;

        private readonly List<IMessageProcessor> _messageProcessors = new List<IMessageProcessor>();

        private SplitSearchWindow _searchWindow;
        private TransferStashWorker _transferStashWorker;

        private StashFileMonitor _stashFileMonitor = new StashFileMonitor();
        private CsvFileMonitor _csvFileMonitor = new CsvFileMonitor();
        private ItemReplicaService _itemReplicaService;

        private Action<RegisterWindow.DataAndType> _registerWindowDelegate;
        private RegisterWindow _window;
        private InjectionHelper _injector;
        private ProgressChangedEventHandler _injectorCallbackDelegate;
        private CsvParsingService _csvParsingService;

        private BuddyItemsService _buddyItemsService;
        private BackgroundTask _backupBackgroundTask;
        private ItemTransferController _transferController;
        private readonly RecipeParser _recipeParser;
        private readonly ParsingService _parsingService;
        private AuthService _authService;
        private BackupServiceWorker _backupServiceWorker;
        private readonly UserFeedbackService _userFeedbackService;
        private MinimizeToTrayHandler _minimizeToTrayHandler;


        #region Stash Status

        // TODO: TEMPORARY FIX!
        private bool _hasShownStashErrorPage = false;

        /// <summary>
        /// Toolstrip callback for GDInjector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InjectorCallback(object sender, ProgressChangedEventArgs e) {
            if (InvokeRequired) {
                Invoke((MethodInvoker) delegate { InjectorCallback(sender, e); });
            }
            else {
                switch (e.ProgressPercentage) {
                    case InjectionHelper.INJECTION_ERROR: {
                        _itemReplicaService.SetIsGrimDawnRunning(false);
                        RuntimeSettings.StashStatus = StashAvailability.ERROR;
                        statusLabel.Text = e.UserState as string;
                        if (!_hasShownStashErrorPage) {
                            _cefBrowserHandler.ShowHelp(HelpService.HelpType.StashError);
                            _hasShownStashErrorPage = true;
                        }

                        break;
                    }
                    case InjectionHelper.INJECTION_ERROR_32BIT: {
                        _itemReplicaService.SetIsGrimDawnRunning(false);
                        RuntimeSettings.StashStatus = StashAvailability.NOT64BIT;
                        statusLabel.Text = e.UserState as string;
                        if (!_hasShownStashErrorPage) {
                            _cefBrowserHandler.ShowHelp(HelpService.HelpType.No32Bit);
                            _hasShownStashErrorPage = true;
                        }

                        break;
                    }
                    // No grim dawn client, so stash is closed!
                    case InjectionHelper.NO_PROCESS_FOUND_ON_STARTUP: {
                        _itemReplicaService.SetIsGrimDawnRunning(false);
                        if (RuntimeSettings.StashStatus == StashAvailability.UNKNOWN) {
                            RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                        }

                        break;
                    }
                    // No grim dawn client, so stash is closed!
                    case InjectionHelper.NO_PROCESS_FOUND:
                        RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                        _itemReplicaService.SetIsGrimDawnRunning(false);
                        break;
                    // Injection error
                    case InjectionHelper.INJECTION_ERROR_POSSIBLE_ACCESS_DENIED: {
                        _itemReplicaService.SetIsGrimDawnRunning(false);
                        RuntimeSettings.StashStatus = StashAvailability.ERROR;
                        if (!_hasShownStashErrorPage) {
                            _cefBrowserHandler.ShowHelp(HelpService.HelpType.StashError);
                            _hasShownStashErrorPage = true;
                        }

                        break;
                    }
                    case InjectionHelper.STILL_RUNNING:
                        _itemReplicaService.SetIsGrimDawnRunning(true);
                        break;
                }

                _charBackupService.SetIsActive(RuntimeSettings.StashStatus == StashAvailability.CLOSED);
            }
        }

        #endregion Stash Status


        /// <summary>
        /// Perform a search the moment were initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e) {
            var args = e as FrameLoadEndEventArgs;
            ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
            if (args != null && args.Frame.IsMain) {
                // https://github.com/cefsharp/CefSharp/issues/3021
                if (browser?.CanExecuteJavascriptInMainFrame ?? true) {
                    if (InvokeRequired) {
                        Invoke((MethodInvoker) delegate { Browser_IsBrowserInitializedChanged(sender, e); });
                    }
                    else {
                        _searchWindow?.UpdateListViewDelayed();

                        var isGdParsed = _serviceProvider.Get<IDatabaseItemDao>().GetRowCount() > 0;
                        var settingsService = _serviceProvider.Get<SettingsService>();
                        _cefBrowserHandler.SetDarkMode(settingsService.GetPersistent().DarkMode);
                        _cefBrowserHandler.SetHideItemSkills(settingsService.GetPersistent().HideSkills);
                        _cefBrowserHandler.SetIsGrimParsed(isGdParsed);
                        

                        _cefBrowserHandler.SetOnlineBackupsEnabled(!settingsService.GetLocal().OptOutOfBackups);
                    }
                }
            }
        }


        public MainWindow(
            ServiceProvider serviceProvider,
            CefBrowserHandler browser,
            ParsingService parsingService
        ) {
            this._serviceProvider = serviceProvider;
            _cefBrowserHandler = browser;
            InitializeComponent();
            FormClosing += MainWindow_FormClosing;

            _minimizeToTrayHandler = new MinimizeToTrayHandler(this, notifyIcon1, serviceProvider.Get<SettingsService>());

            var settingsService = _serviceProvider.Get<SettingsService>();
            _automaticUpdateChecker = new AutomaticUpdateChecker(settingsService);
            _settingsController = new SettingsController(settingsService);
            _dynamicPacker = new DynamicPacker(serviceProvider.Get<IDatabaseItemStatDao>());
            _recipeParser = new RecipeParser(serviceProvider.Get<IRecipeItemDao>());
            _parsingService = parsingService;
            _userFeedbackService = new UserFeedbackService(_cefBrowserHandler);
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

            _stashFileMonitor?.Dispose();
            _stashFileMonitor = null;

            _csvFileMonitor?.Dispose();
            _csvFileMonitor = null;

            _csvParsingService?.Dispose();
            _csvParsingService = null;

            _minimizeToTrayHandler?.Dispose();
            _minimizeToTrayHandler = null;

            _backupBackgroundTask?.Dispose();
            _usageStatisticsReporter.Dispose();
            _automaticUpdateChecker.Dispose();
            _transferController.Dispose();

            _tooltipHelper?.Dispose();

            _buddyItemsService?.Dispose();
            _buddyItemsService = null;

            _itemReplicaService.Dispose();

            _injector?.Dispose();
            _injector = null;

            _backupServiceWorker?.Dispose();
            _backupServiceWorker = null;

            _window?.Dispose();
            _window = null;

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
                Invoke((MethodInvoker) delegate { CustomWndProc(bt); });
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
                    Invoke((MethodInvoker) delegate { SetFeedback(feedback); });
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

        private void SetTooltipAtmouse(string message) {
            if (InvokeRequired) {
                Invoke((MethodInvoker) delegate { SetTooltipAtmouse(message); });
            }
            else {
                _tooltipHelper.ShowTooltipAtMouse(message, _cefBrowserHandler.BrowserControl);
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
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            if (Thread.CurrentThread.Name == null) {
                Thread.CurrentThread.Name = "UI";
            }

            Logger.Debug("Starting UI initialization");


            // Set version number
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
            statusLabel.Text = statusLabel.Text + $" - {version.Major}.{version.Minor}.{version.Build}.{version.Revision} from {buildDate.ToString("dd/MM/yyyy")}";


            var settingsService = _serviceProvider.Get<SettingsService>();
            ExceptionReporter.EnableLogUnhandledOnThread();
            SizeChanged += OnMinimizeWindow;


            // Chicken and the egg.. search controller needs browser, browser needs search controllers var.
            var databaseItemDao = _serviceProvider.Get<IDatabaseItemDao>();
            var searchController = _serviceProvider.Get<SearchController>();
            searchController.JsIntegration.OnRequestSetItemAssociations += (s, evvv) => { (evvv as GetSetItemAssociationsEventArgs).Elements = databaseItemDao.GetItemSetAssociations(); };

            _cefBrowserHandler.InitializeChromium(searchController.JsIntegration, Browser_IsBrowserInitializedChanged, tabControl1);
            searchController.Browser = _cefBrowserHandler;
            searchController.JsIntegration.OnClipboard += SetItemsClipboard;

            var playerItemDao = _serviceProvider.Get<IPlayerItemDao>();
            var cacher = _serviceProvider.Get<TransferStashServiceCache>();
            _parsingService.OnParseComplete += (o, args) => cacher.Refresh();


            var replicaItemDao = _serviceProvider.Get<IReplicaItemDao>();
            var stashWriter = new SafeTransferStashWriter(settingsService, _cefBrowserHandler);
            var transferStashService = new TransferStashService(_serviceProvider.Get<IDatabaseItemStatDao>(), settingsService, stashWriter);
            var transferStashService2 = new TransferStashService2(playerItemDao, cacher, transferStashService, stashWriter, settingsService, _cefBrowserHandler, replicaItemDao);
            _serviceProvider.Add(transferStashService2);

            _transferStashWorker = new TransferStashWorker(transferStashService2, _userFeedbackService);

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

            // Load recipes
            foreach (string file in GlobalPaths.FormulasFiles) {
                if (!string.IsNullOrEmpty(file)) {
                    bool isHardcore = file.EndsWith("gsh");
                    Logger.InfoFormat("Reading recipes at \"{0}\", IsHardcore={1}", file, isHardcore);
                    _recipeParser.UpdateFormulas(file, isHardcore);
                }
            }

            
            var buddyItemDao = _serviceProvider.Get<IBuddyItemDao>();
            var buddySubscriptionDao = _serviceProvider.Get<IBuddySubscriptionDao>();



            _authService = new AuthService(_cefBrowserHandler, new AuthenticationProvider(settingsService), playerItemDao);
            var backupSettings = new BackupSettings(playerItemDao, settingsService, _cefBrowserHandler);
            UIHelper.AddAndShow(backupSettings, backupPanel);

            var onlineSettings = new OnlineSettings(playerItemDao, _authService, settingsService, _cefBrowserHandler, buddyItemDao, buddySubscriptionDao);
            UIHelper.AddAndShow(onlineSettings, onlinePanel);
            _cefBrowserHandler.OnAuthSuccess += (_, __) => onlineSettings.UpdateUi();


            UIHelper.AddAndShow(new ModsDatabaseConfig(DatabaseLoadedTrigger, playerItemDao, _parsingService, grimDawnDetector, settingsService, _cefBrowserHandler, databaseItemDao), modsPanel);

            UIHelper.AddAndShow(new LoggingWindow(), panelLogging);

            var itemTagDao = _serviceProvider.Get<IItemTagDao>();
            var backupService = new BackupService(_authService, playerItemDao, settingsService);
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
                    transferStashService,
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
            new EasterEgg(settingsService).Activate(this);

            // Initialize the "stash packer" used to find item positions for transferring items ingame while the stash is open
            {
                _dynamicPacker.Initialize(8, 16);

                var transferFiles = GlobalPaths.GetTransferFiles(settingsService.GetPersistent().EnableDowngrades);
                if (transferFiles.Count > 0) {
                    var maxLastAccess = transferFiles.Max(m => m.LastAccess);
                    var file = transferFiles.First(x => x.LastAccess == maxLastAccess);

                    var stash = TransferStashService.GetStash(file.Filename);
                    if (stash != null) {
                        _dynamicPacker.Initialize(stash.Width, stash.Height);
                        if (stash.Tabs.Count >= 3) {
                            foreach (var item in stash.Tabs[2].Items) {
                                byte[] bx = BitConverter.GetBytes(item.XOffset);
                                uint x = (uint) BitConverter.ToSingle(bx, 0);

                                byte[] by = BitConverter.GetBytes(item.YOffset);
                                uint y = (uint) BitConverter.ToSingle(by, 0);

                                _dynamicPacker.Insert(item.BaseRecord, item.Seed, x, y);
                            }
                        }
                    }
                }
            }

            var itemReplicaProcessor = _serviceProvider.Get<ItemReplicaProcessor>();
            _messageProcessors.Add(new ItemPositionFinder(_dynamicPacker));
            _messageProcessors.Add(new PlayerPositionTracker(Debugger.IsAttached && false));
            _messageProcessors.Add(new StashStatusHandler());
            _messageProcessors.Add(new CloudDetectorProcessor(SetFeedback, _serviceProvider.Get<SettingsService>()));
            _messageProcessors.Add(new GenericErrorHandler());
            _messageProcessors.Add(itemReplicaProcessor);


            RuntimeSettings.StashStatusChanged += GlobalSettings_StashStatusChanged;

            _transferController = new ItemTransferController(
                _cefBrowserHandler,
                SetFeedback,
                SetTooltipAtmouse,
                _searchWindow,
                playerItemDao,
                transferStashService,
                _serviceProvider.Get<ItemStatService>(),
                settingsService
            );
            _transferController.Start();
            Application.AddMessageFilter(new MousewheelMessageFilter());


            var titleTag = RuntimeSettings.Language.GetTag("iatag_ui_itemassistant");
            if (!string.IsNullOrEmpty(titleTag)) {
                this.Text += $" - {titleTag}";
            }


            // Popup login diag
            if (_authService.CheckAuthentication() == AuthService.AccessStatus.Unauthorized && !settingsService.GetLocal().OptOutOfBackups) {
                var t = new System.Windows.Forms.Timer {Interval = 100};
                t.Tick += (o, args) => {
                    if (_cefBrowserHandler.BrowserControl.CanExecuteJavascriptInMainFrame) {
                        _authService.Authenticate();
                        t.Stop();
                    }
                };
                t.Start();
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


            _csvParsingService = new CsvParsingService(playerItemDao, replicaItemDao, _userFeedbackService, cacher, _transferController, transferStashService, settingsService);
            _csvFileMonitor.OnModified += (_, arg) => {
                var csvEvent = arg as CsvFileMonitor.CsvEvent;
                _csvParsingService.Queue(csvEvent.Filename, csvEvent.Cooldown);
            };
            _csvFileMonitor.StartMonitoring();
            _csvParsingService.Start();

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
            _injector = new InjectionHelper(new BackgroundWorker(), _injectorCallbackDelegate, false, "Grim Dawn", string.Empty, dllname);
        }


        private void GlobalSettings_StashStatusChanged(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke((MethodInvoker) delegate { GlobalSettings_StashStatusChanged(sender, e); });
                return;
            }

            var settingsService = _serviceProvider.Get<SettingsService>();
            tsStashStatus.Visible = settingsService.GetLocal().DisableInstaloot;

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
                Invoke((MethodInvoker) delegate { SetItemsClipboard(ignored, _args); });
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