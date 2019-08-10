using CefSharp;
using EvilsoftCommons;
using EvilsoftCommons.Cloud;
using EvilsoftCommons.DllInjector;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Azure.Service;
using IAGrim.Backup.Azure.Util;
using IAGrim.BuddyShare;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Synchronizer;
using IAGrim.Parsers;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.Arz.dto;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Services;
using IAGrim.Services.MessageProcessor;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Misc.CEF.Dto;
using IAGrim.UI.Tabs;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using IAGrim.Utilities.HelperClasses;
using IAGrim.Utilities.RectanglePacker;
using log4net;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DllInjector;
using IAGrim.Parsers.TransferStash;
using IAGrim.Settings;

namespace IAGrim.UI
{
    public partial class MainWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));
        private readonly CefBrowserHandler _cefBrowserHandler;


        private readonly ISettingsReadController _settingsController;

        private FormWindowState _previousWindowState = FormWindowState.Normal;
        private readonly TooltipHelper _tooltipHelper = new TooltipHelper();
        private readonly DynamicPacker _dynamicPacker;
        private readonly UsageStatisticsReporter _usageStatisticsReporter = new UsageStatisticsReporter();
        private readonly AutomaticUpdateChecker _automaticUpdateChecker;
        private readonly List<IMessageProcessor> _messageProcessors = new List<IMessageProcessor>();

        private SplitSearchWindow _searchWindow;
        private TransferStashService _transferStashService;
        private TransferStashWorker _transferStashWorker;
        
        private BuddySettings _buddySettingsWindow;
        private StashFileMonitor _stashFileMonitor = new StashFileMonitor();

        private Action<RegisterWindow.DataAndType> _registerWindowDelegate;
        private RegisterWindow _window;
        private InjectionHelper _injector;
        private ProgressChangedEventHandler _injectorCallbackDelegate;

        private BuddyBackgroundThread _buddyBackgroundThread;
        private BackgroundTask _backupBackgroundTask;
        private ItemTransferController _transferController;

        private readonly IItemTagDao _itemTagDao;
        private readonly IDatabaseItemDao _databaseItemDao;
        private readonly IDatabaseItemStatDao _databaseItemStatDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IAzurePartitionDao _azurePartitionDao;
        [Obsolete]
        private readonly IDatabaseSettingDao _databaseSettingDao;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly IBuddySubscriptionDao _buddySubscriptionDao;
        private readonly RecipeParser _recipeParser;
        private readonly IItemSkillDao _itemSkillDao;
        private readonly ParsingService _parsingService;
        private readonly AugmentationItemRepo _augmentationItemRepo;
        private AzureAuthService _authAuthService;
        private BackupServiceWorker _backupServiceWorker;
        private readonly UserFeedbackService _userFeedbackService;
        private readonly SettingsService _settingsService;
        private readonly GrimDawnDetector _grimDawnDetector;


        #region Stash Status

        /// <summary>
        /// Toolstrip callback for GDInjector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InjectorCallback(object sender, ProgressChangedEventArgs e) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { InjectorCallback(sender, e); });
            } else {
                if (e.ProgressPercentage == InjectionHelper.INJECTION_ERROR) {
                    RuntimeSettings.StashStatus = StashAvailability.ERROR;
                    statusLabel.Text = e.UserState as string;
                }
                // No grim dawn client, so stash is closed!
                else if (e.ProgressPercentage == InjectionHelper.NO_PROCESS_FOUND_ON_STARTUP) {
                    if (RuntimeSettings.StashStatus == StashAvailability.UNKNOWN) {
                        RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                    }
                }
                // No grim dawn client, so stash is closed!
                else if (e.ProgressPercentage == InjectionHelper.NO_PROCESS_FOUND) {
                    RuntimeSettings.StashStatus = StashAvailability.CLOSED;
                }
            }
        }

#endregion Stash Status


        /// <summary>
        /// Perform a search the moment were initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e) {
            if (e.IsBrowserInitialized) {

                if (InvokeRequired) {
                    Invoke((MethodInvoker)delegate { _searchWindow?.UpdateListViewDelayed(); });
                } else {
                    _searchWindow?.UpdateListViewDelayed();
                }
            }
        }

        public MainWindow(
            CefBrowserHandler browser,
            IDatabaseItemDao databaseItemDao,
            IDatabaseItemStatDao databaseItemStatDao,
            IPlayerItemDao playerItemDao,
            IAzurePartitionDao azurePartitionDao,
            IDatabaseSettingDao databaseSettingDao,
            IBuddyItemDao buddyItemDao,
            IBuddySubscriptionDao buddySubscriptionDao,
            IRecipeItemDao recipeItemDao,
            IItemSkillDao itemSkillDao,
            IItemTagDao itemTagDao, 
            ParsingService parsingService, 
            AugmentationItemRepo augmentationItemRepo, 
            SettingsService settingsService, 
            GrimDawnDetector grimDawnDetector
            ) {
            _cefBrowserHandler = browser;
            InitializeComponent();
            FormClosing += MainWindow_FormClosing;
            _automaticUpdateChecker = new AutomaticUpdateChecker(settingsService);
            _settingsController = new SettingsController(settingsService);
            _dynamicPacker = new DynamicPacker(databaseItemStatDao);
            _databaseItemDao = databaseItemDao;
            _databaseItemStatDao = databaseItemStatDao;
            _playerItemDao = playerItemDao;
            _azurePartitionDao = azurePartitionDao;
            _databaseSettingDao = databaseSettingDao;
            _buddyItemDao = buddyItemDao;
            _buddySubscriptionDao = buddySubscriptionDao;
            _recipeParser = new RecipeParser(recipeItemDao);
            _itemSkillDao = itemSkillDao;
            _itemTagDao = itemTagDao;
            _parsingService = parsingService;
            _augmentationItemRepo = augmentationItemRepo;
            _userFeedbackService = new UserFeedbackService(_cefBrowserHandler);
            _settingsService = settingsService;
            _grimDawnDetector = grimDawnDetector;
        }

        /// <summary>
        /// Update the UI's language
        /// </summary>
        public void UpdateLanguage()
        {
            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);
            Text = RuntimeSettings.Language.GetTag(Tag.ToString());
            if (tsStashStatus.Tag is string)
            {
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
            _transferStashService = null;

            _backupBackgroundTask?.Dispose();
            _usageStatisticsReporter.Dispose();
            _automaticUpdateChecker.Dispose();

            _tooltipHelper?.Dispose();

            _buddyBackgroundThread?.Dispose();
            _buddyBackgroundThread = null;

            panelHelp.Controls.Clear();

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
                Invoke((MethodInvoker)delegate { CustomWndProc(bt); });
                return;
            }

            MessageType type = (MessageType)bt.Type;
            foreach (IMessageProcessor t in _messageProcessors) {
                t.Process(type, bt.Data);
            }

            switch (type) {
                case MessageType.TYPE_DetectedStashToLootFrom: {
                    int stashToLootFrom = IOHelper.GetInt(bt.Data, 0);
                        Logger.Info($"Grim Dawn hook reports it will be looting from stash tab: {stashToLootFrom}");
                    }
                break;

                case MessageType.TYPE_REPORT_WORKER_THREAD_LAUNCHED:
                    Logger.Info("Grim Dawn hook reports successful launch.");
                    break;

                case MessageType.TYPE_REPORT_WORKER_THREAD_EXPERIMENTAL_LAUNCHED:
                    Logger.Info("Grim Dawn exp-hook reports successful launch");
                    break;


                case MessageType.TYPE_GameInfo_IsHardcore:
                case MessageType.TYPE_GameInfo_IsHardcore_via_init:
                    Logger.Info($"TYPE_GameInfo_IsHardcore({bt.Data[0] > 0}, {type})");
                    if (_settingsController.AutoUpdateModSettings) {
                        _searchWindow.ModSelectionHandler.UpdateModSelection(bt.Data[0] > 0);
                    }

                    break;

                case MessageType.TYPE_GameInfo_IsHardcore_via_init_2:
                    Logger.Debug("GameInfo object created");
                    break;

                case MessageType.TYPE_GameInfo_SetModName:
                    Logger.InfoFormat("TYPE_GameInfo_SetModName({0})", IOHelper.GetPrefixString(bt.Data, 0));
                    if (_settingsController.AutoUpdateModSettings) {
                        _searchWindow.ModSelectionHandler.UpdateModSelection(IOHelper.GetPrefixString(bt.Data, 0));
                    }
                    break;

            }

            if (bt.Type == 1011) {
                Logger.Debug("dll_GameInfo_SetModNameWideString");
                
            }
            else if (bt.Type == 1010) {
                Logger.Debug($"Hooked_GameInfo_SetModName {IOHelper.GetPrefixString(bt.Data, 0)}");
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) {
                _searchWindow.ClearFilters();
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetFeedback(string level, string feedback, string helpUrl) {
            try {
                if (InvokeRequired) {
                    Invoke((MethodInvoker)delegate { SetFeedback(level, feedback, helpUrl); });
                }
                else {
                    statusLabel.Text = feedback.Replace("\\n", " - ");
                    _userFeedbackService.SetFeedback(feedback, level, helpUrl);
                }
            }
            catch (ObjectDisposedException) {
                Logger.Debug("Attempted to set feedback, but UI already disposed. (Probably shutting down)");
            }
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
            _tooltipHelper.ShowTooltipAtMouse(message, _cefBrowserHandler.BrowserControl);
        }


        private void TimerTickLookForGrimDawn(object sender, EventArgs e) {
            System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "DetectGrimDawnTimer";

            string gdPath = _grimDawnDetector.GetGrimLocation();
            if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {
                timer?.Stop();

                // Attempt to force a database update
                foreach (Control c in modsPanel.Controls) {
                    ModsDatabaseConfig config = c as ModsDatabaseConfig;
                    if (config != null) {
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

            ExceptionReporter.EnableLogUnhandledOnThread();
            SizeChanged += OnMinimizeWindow;
            
            
            var cacher = new TransferStashServiceCache(_databaseItemDao);
            _parsingService.OnParseComplete += (o, args) => cacher.Refresh();

            var stashWriter = new SafeTransferStashWriter(_settingsService);
            _transferStashService = new TransferStashService(_databaseItemStatDao, _settingsService, stashWriter);
            var transferStashService2 = new TransferStashService2(_playerItemDao, cacher, _transferStashService, stashWriter, _settingsService);
            _transferStashWorker = new TransferStashWorker(transferStashService2, _userFeedbackService);

            _stashFileMonitor.OnStashModified += (_, __) => {
                StashEventArg args = __ as StashEventArg;
                _transferStashWorker.Queue(args?.Filename);
            };

            if (!_stashFileMonitor.StartMonitorStashfile(GlobalPaths.SavePath)) {
                MessageBox.Show("Ooops!\nIt seems you are synchronizing your saves to steam cloud..\nThis tool is unfortunately not compatible.\n");
                HelpService.ShowHelp(HelpService.HelpType.CloudSavesEnabled);

                Logger.Warn("Shutting down IA, unable to monitor stash files.");

                if (!Debugger.IsAttached)
                    Close();
            }

            // Chicken and the egg..
            SearchController searchController = new SearchController(
                _databaseItemDao,
                _playerItemDao, 
                _databaseItemStatDao, 
                _itemSkillDao, 
                _buddyItemDao,
                _augmentationItemRepo,
                _settingsService
            );

            searchController.JsBind.SetItemSetAssociations(_databaseItemDao.GetItemSetAssociations());
            _cefBrowserHandler.InitializeChromium(searchController.JsBind, Browser_IsBrowserInitializedChanged);
            searchController.Browser = _cefBrowserHandler;
            searchController.JsBind.OnClipboard += SetItemsClipboard;

            // Load the grim database
            string gdPath = _grimDawnDetector.GetGrimLocation();
            if (!string.IsNullOrEmpty(gdPath)) {
            } else {
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

            var addAndShow = UIHelper.AddAndShow;

            // Create the tab contents
            _buddySettingsWindow = new BuddySettings(delegate (bool b) { BuddySyncEnabled = b; }, 
                _buddyItemDao, 
                _buddySubscriptionDao,
                _settingsService
                );

            addAndShow(_buddySettingsWindow, buddyPanel);
            
            _authAuthService = new AzureAuthService(_cefBrowserHandler, new AuthenticationProvider(_settingsService));
            var backupSettings = new BackupSettings(_playerItemDao, _authAuthService, _settingsService);
            addAndShow(backupSettings, backupPanel);
            addAndShow(new ModsDatabaseConfig(DatabaseLoadedTrigger, _playerItemDao, _parsingService, _databaseSettingDao, _grimDawnDetector, _settingsService), modsPanel);
            addAndShow(new HelpTab(), panelHelp);            
            addAndShow(new LoggingWindow(), panelLogging);
            var backupService = new BackupService(_authAuthService, _playerItemDao, _azurePartitionDao, () => _settingsService.GetPersistent().UsingDualComputer);
            _backupServiceWorker = new BackupServiceWorker(backupService);
            backupService.OnUploadComplete += (o, args) => _searchWindow.UpdateListView();
            searchController.OnSearch += (o, args) => backupService.OnSearch();

            _searchWindow = new SplitSearchWindow(_cefBrowserHandler.BrowserControl, SetFeedback, _playerItemDao, searchController, _itemTagDao, _settingsService);
            addAndShow(_searchWindow, searchPanel);

            transferStashService2.OnUpdate += (_, __) => {
                _searchWindow.UpdateListView();
            };

            var languagePackPicker = new LanguagePackPicker(_itemTagDao, _playerItemDao, _parsingService, _settingsService);

            addAndShow(
                new SettingsWindow(
                    _cefBrowserHandler,
                    _tooltipHelper,
                    ListviewUpdateTrigger,
                    _playerItemDao,
                    _searchWindow.ModSelectionHandler.GetAvailableModSelection(),
                    _transferStashService,
                    languagePackPicker,
                    _settingsService,
                    _grimDawnDetector
                ),
                settingsPanel);

#if !DEBUG
            ThreadPool.QueueUserWorkItem(m => ExceptionReporter.ReportUsage());
            _automaticUpdateChecker.CheckForUpdates();
#endif

            Shown += (_, __) => { StartInjector(); };

            //settingsController.Data.budd
            
            BuddySyncEnabled = _settingsService.GetPersistent().BuddySyncEnabled;

            // Start the backup task
            _backupBackgroundTask = new BackgroundTask(new FileBackup(_playerItemDao, _settingsService));

            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);
            new EasterEgg(_settingsService).Activate(this);

            // Initialize the "stash packer" used to find item positions for transferring items ingame while the stash is open
            {
                _dynamicPacker.Initialize(8, 16);

                var transferFiles = GlobalPaths.TransferFiles;
                if (transferFiles.Count > 0) {
                    var file = transferFiles.MaxBy(m => m.LastAccess);
                    var stash = TransferStashService.GetStash(file.Filename);
                    if (stash != null) {
                        _dynamicPacker.Initialize(stash.Width, stash.Height);
                        if (stash.Tabs.Count >= 3) {
                            foreach (var item in stash.Tabs[2].Items) {

                                byte[] bx = BitConverter.GetBytes(item.XOffset);
                                uint x = (uint)BitConverter.ToSingle(bx, 0);

                                byte[] by = BitConverter.GetBytes(item.YOffset);
                                uint y = (uint)BitConverter.ToSingle(by, 0);

                                _dynamicPacker.Insert(item.BaseRecord, item.Seed, x, y);
                            }
                        }
                    }
                }
            }

            _messageProcessors.Add(new ItemPositionFinder(_dynamicPacker));
            _messageProcessors.Add(new PlayerPositionTracker(Debugger.IsAttached && false));
            _messageProcessors.Add(new StashStatusHandler());
            _messageProcessors.Add(new ItemSpawnedProcessor());
            _messageProcessors.Add(new CloudDetectorProcessor(SetFeedback));
            _messageProcessors.Add(new GenericErrorHandler());
            //messageProcessors.Add(new LogMessageProcessor());
#if DEBUG
            _messageProcessors.Add(new DebugMessageProcessor());
#endif

            RuntimeSettings.StashStatusChanged += GlobalSettings_StashStatusChanged;

            _transferController = new ItemTransferController(
                _cefBrowserHandler, 
                SetFeedback, 
                SetTooltipAtmouse, 
                _settingsController, 
                _searchWindow, 
                _dynamicPacker,
                _playerItemDao,
                _transferStashService,
                new ItemStatService(_databaseItemStatDao, _itemSkillDao, _settingsService)
                );
            Application.AddMessageFilter(new MousewheelMessageFilter());
            

            var titleTag = RuntimeSettings.Language.GetTag("iatag_ui_itemassistant");
            if (!string.IsNullOrEmpty(titleTag)) {
                this.Text += $" - {titleTag}";
            }


            // Popup login diag
            
            if (_authAuthService.CheckAuthentication() == AzureAuthService.AccessStatus.Unauthorized && !_settingsService.GetLocal().OptOutOfBackups) {
                var t = new System.Windows.Forms.Timer {Interval = 100};
                t.Tick += (o, args) => {
                    if (_cefBrowserHandler.BrowserControl.IsBrowserInitialized) {
                        _authAuthService.Authenticate();
                        t.Stop();
                    }
                };
                t.Start();
            }


            _cefBrowserHandler.TransferSingleRequested += TransferSingleItem;
            _cefBrowserHandler.TransferAllRequested += TransferAllItems;
            new WindowSizeManager(this, _settingsService);


            // Suggest translation packs if available
            
            if (!string.IsNullOrEmpty(_settingsService.GetLocal().LocalizationFile) && !_settingsService.GetLocal().HasSuggestedLanguageChange) {
                if (LocalizationLoader.HasSupportedTranslations(_grimDawnDetector.GetGrimLocations())) {
                    Logger.Debug("A new language pack has been detected, informing end user..");
                    new LanguagePackPicker(_itemTagDao, _playerItemDao, _parsingService, _settingsService).Show(_grimDawnDetector.GetGrimLocations());

                    _settingsService.GetLocal().HasSuggestedLanguageChange = true;
                }
            }
            Logger.Debug("UI initialization complete");
        }

        void TransferSingleItem(object ignored, EventArgs args) {

            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate {
                    this.TransferSingleItem(ignored, args);
                });
            }
            else {
                ItemTransferEvent searchEvent = args as ItemTransferEvent;
                StashTransferEventArgs transferArgs = new StashTransferEventArgs {
                    Count = 1,
                    InternalId = searchEvent.Request.Split(';')
                };

                _transferController.TransferItem(ignored, transferArgs);
            }
        }

        void TransferAllItems(object ignored, EventArgs args) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate {
                    this.TransferAllItems(ignored, args);
                });
            }
            else {
                ItemTransferEvent searchEvent = args as ItemTransferEvent;
                StashTransferEventArgs transferArgs = new StashTransferEventArgs {
                    Count = int.MaxValue,
                    InternalId = searchEvent.Request.Split(';')
                };

                _transferController.TransferItem(ignored, transferArgs);
            }
        }

        private void StartInjector() {
            // Start looking for GD processes!
            _registerWindowDelegate = CustomWndProc;
            _window = new RegisterWindow("GDIAWindowClass", _registerWindowDelegate);

            // This prevents a implicit cast to new ProgressChangedEventHandler(func), which would hit the GC and before being used from another thread
            // Same happens when shutting down, fix unknown
            _injectorCallbackDelegate = InjectorCallback;

            var hasMods = _searchWindow.ModSelectionHandler.HasMods;
#if DEBUG
            hasMods = false; // TODO TODO TODO TODO
#endif
            // CBA dealing with this.
            string dllname = "ItemAssistantHook.dll";
            _injector = new InjectionHelper(new BackgroundWorker(), _injectorCallbackDelegate, false, "Grim Dawn", string.Empty, dllname);
        }

        void TransferItem(object ignored, EventArgs args) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate {
                    _transferController.TransferItem(ignored, args);
                });
            }
            else {
                _transferController.TransferItem(ignored, args);
            }
        }

        private void GlobalSettings_StashStatusChanged(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { GlobalSettings_StashStatusChanged(sender, e); });
                return;
            }

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
                case StashAvailability.SORTED:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = "iatag_stash_sorted";
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag(tsStashStatus.Tag.ToString());
                    break;
                default:
                    tsStashStatus.ForeColor = Color.FromArgb(255, 192, 0, 0);
                    tsStashStatus.Tag = null;
                    tsStashStatus.Text = RuntimeSettings.Language.GetTag("iatag_stash_") + RuntimeSettings.StashStatus;
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
            try {
                if (_settingsController.MinimizeToTray) {
                    if (WindowState == FormWindowState.Minimized) {
                        Hide();
                        notifyIcon1.Visible = true;
                    } else /*if (this.WindowState == FormWindowState.Normal)*/ {
                        notifyIcon1.Visible = false;
                        _previousWindowState = WindowState;
                    }
                }
            } catch (Exception ex) {
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
            }

            _usageStatisticsReporter.ResetLastMinimized();
            _automaticUpdateChecker.ResetLastMinimized();
        }

        public void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            Visible = true;
            notifyIcon1.Visible = false;
            WindowState = _previousWindowState;
        }

        private void trayContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            e.Cancel = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

#endregion Tray and Menu

#region BuddySync
        
        /// <summary>
        /// Enable / Disable the buddy sync feature
        /// </summary>
        private bool BuddySyncEnabled {
            get {
                return _buddyBackgroundThread != null;
            }
            set {
                if (value) {
                    // Reset timers etc first
                    BuddySyncEnabled = false;

                    List<long> buddies = new List<long>(_buddySubscriptionDao.ListAll().Select(m => m.Id));
                    _buddyBackgroundThread = new BuddyBackgroundThread(BuddyItemsCallback, _playerItemDao, _buddyItemDao, buddies, 3 * 60 * 1000, _settingsService);
                } else {
                    if (_buddyBackgroundThread != null) {
                        _buddyBackgroundThread.Dispose();
                        _buddyBackgroundThread = null;
                    }
                }
            }
        }

        /// <summary>
        /// BuddyShare callback to store data on UI thread (SQL is here)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuddyItemsCallback(object sender, ProgressChangedEventArgs e) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate {
                    BuddyItemsCallback(sender, e);
                });
            } else {
                if (e.ProgressPercentage == BuddyBackgroundThread.ProgressStoreBuddydata) {

                    _buddySettingsWindow.UpdateBuddyList();
                } else if (e.ProgressPercentage == BuddyBackgroundThread.ProgressSetUid) {
                    _buddySettingsWindow.UID = (long)e.UserState;
                }
            }
        }

#endregion BuddySync

        private void SetItemsClipboard(object ignored, EventArgs _args) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { SetItemsClipboard(ignored, _args); });
            } else {
                if (_args is ClipboardEventArg args) {
                    Clipboard.SetText(args.Text);
                }

                _tooltipHelper.ShowTooltipAtMouse(RuntimeSettings.Language.GetTag("iatag_copied_clipboard"), _cefBrowserHandler.BrowserControl);
            }
        }
        
    } // CLASS
}