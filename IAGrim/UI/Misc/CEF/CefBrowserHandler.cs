using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using IAGrim.Backup.Cloud.CefSharp;
using IAGrim.Database.Model;
using IAGrim.Services;
using IAGrim.Services.ItemReplica;
using IAGrim.Settings;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc.Protocol;
using IAGrim.Utilities;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler : IDisposable, ICefBackupAuthentication, IUserFeedbackHandler, IBrowserCallbacks, IHelpService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private TabControl _tabControl; // TODO: UGh.. why?
        private readonly SettingsService _settings;

        public ChromiumWebBrowser BrowserControl { get; private set; }
        private readonly object _lockObj = new object();

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public CefBrowserHandler(SettingsService settings) {
            _settings = settings;
        }

        ~CefBrowserHandler() {
            Dispose();
        }

        private void SendMessage(IOMessage message) {
            if (BrowserControl != null && BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.message", JsonConvert.SerializeObject(message, _serializerSettings));
            }
            else {
                Logger.Warn("Attempted to communicate with the frontend, but CEF not yet initialized, discarded: " + JsonConvert.SerializeObject(message, _serializerSettings));
            }
        }

        public void ShowCharacterBackups() {
            SendMessage(new IOMessage { Type = IOMessageType.ShowCharacterBackups });
            SwitchToFrontendTab();
        }

        private void SwitchToFrontendTab() {
            if (_tabControl.InvokeRequired) {
                _tabControl.Invoke((MethodInvoker)delegate { _tabControl.SelectedIndex = 0; });
            }
            else {
                _tabControl.SelectedIndex = 0;
            }
        }

        public void ShowHelp(HelpService.HelpType type) {
            SendMessage(new IOMessage { Type = IOMessageType.ShowHelp, Data = type.ToString() });
            SwitchToFrontendTab();
        }

        public void Dispose() {
            try {
                lock (_lockObj) {
                    if (BrowserControl != null) {
                        CefSharpSettings.WcfTimeout = TimeSpan.Zero;
                        BrowserControl.Dispose();

                        Cef.Shutdown();
                        BrowserControl = null;
                    }
                }
            }
            catch (Exception ex) {
                // We're shutting down, doesn't matter. -- Rather not have these reported online.
                Logger.Warn(ex.Message, ex);
            }
        }

        public void ShowDevTools() {
            BrowserControl.ShowDevTools();
        }

        public void SetCollectionAggregateData(IList<CollectionItemAggregateRow> rows) {
            SendMessage(new IOMessage { Type = IOMessageType.SetAggregateItemData, Data = rows });
        }

        public void ShowLoadingAnimation(bool visible) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.IsLoading, Value = visible } });
        }

        public void ShowNoMoreInstantSyncWarning() {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.ShowNoMoreInstantSyncWarning, Value = true } });
        }

        /// <summary>
        /// Set the current batch of items
        /// </summary>
        /// <param name="items">The current batch</param>
        /// <param name="numItemsFound">The number of items found, total (eg 3000 found, but batch has 64)</param>
        public void SetItems(List<List<JsonItem>> items, int numItemsFound) {
            SendMessage(new IOMessage { 
                Type = IOMessageType.SetItems, 
                Data = new IOMessageSetItems {
                    NumItemsFound = numItemsFound,
                    Items = items,
                    ReplaceExistingItems = true,
                }
            });
        }

        public void SetCollectionItems(IList<CollectionItem> items) {
            SendMessage(new IOMessage {
                Type = IOMessageType.SetCollectionItems,
                Data = items
            });
        }

        public void AddItems(List<List<JsonItem>> items) {
            SendMessage(new IOMessage {
                Type = IOMessageType.SetItems,
                Data = new IOMessageSetItems {
                    Items = items,
                    ReplaceExistingItems = false,
                }
            });
        }

        public void SetDarkMode(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.DarkMode, Value = enabled } });
        }

        public void SetHideItemSkills(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.HideItemSkills, Value = enabled } });
        }

        public void SetIsGrimParsed(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.GrimDawnIsParsed, Value = enabled } });
        }

        public void SetIsFirstRun() {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.FirstRun, Value = true } });
        }

        public void SetEasterEggMode() {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.EasterEggMode, Value = true } });
        }

        public void SetGdSeasonMode() {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.GdSeasonError, Value = true } });
        }

        public void ShowModFilterWarning(int numOtherItems) {
            SendMessage(new IOMessage { Type = IOMessageType.ShowModFilterWarning, Data = numOtherItems });
        }

        public void SignalCloudIconChange(IList<long> playerItemIds) {
            SendMessage(new IOMessage { Type = IOMessageType.UpdateCloudIconStatus, Data = new IOMessageCloudIconStateChange { Ids = playerItemIds } });
        }
        public void SignalReplicaStatChange(long playerItemId, IList<ItemStatInfo> stats) {
            SendMessage(new IOMessage {
                Type = IOMessageType.UpdateItemStats,
                Data = new IOMessageSetReplicaStats { Id = playerItemId, ReplicaStats = stats }
            }
            );
        }



        public void SetOnlineBackupsEnabled(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.ShowCloudIcon, Value = enabled } });
        }


        private string GetSiteUri() {
#if DEBUG
            var client = new WebClient();

            try {
                Logger.Debug("Checking if NodeJS is running...");
                client.DownloadString("http://localhost:3000/");
                Logger.Debug("NodeJS running");
                return "http://localhost:3000/";
            }
            catch (System.Net.WebException) {
                Logger.Debug("NodeJS not running, defaulting to standard view");
            }
#endif
            return GlobalPaths.ItemsHtmlFile;
        }


        public void ShowMessage(string message, UserFeedbackLevel level = UserFeedbackLevel.Info, string helpUrl = null) {
            string levelLowercased = level.ToString().ToLowerInvariant();
            var m = message.Replace("\n", "\\n").Replace("'", "\\'");
            if (!string.IsNullOrEmpty(message)) {
                var autoDismissMessage = IsProgramActive.IsActive() || _settings.GetPersistent().AutoDismissNotifications;
                var ret = new Dictionary<string, string> {
                    {"message", m},
                    {"type", levelLowercased},
                    {"fade", autoDismissMessage ? "true" : "false"},
                };

                SendMessage(new IOMessage { Type = IOMessageType.ShowMessage, Data = ret });
            }
        }


        public void InitializeChromium(object bindable, EventHandler browserIsBrowserInitializedChanged, TabControl tabControl) {
            try {
                Logger.Info("Creating Chromium instance..");
                _tabControl = tabControl;

                var settings = new CefSettings();
                settings.CefCommandLineArgs.Add("disable-popup-blocking");
                Cef.Initialize(settings);



                // TODO: Read and analyze https://github.com/cefsharp/CefSharp/issues/2246 -- Is this the correct way to do things in the future?
                CefSharpSettings.WcfEnabled = true;
                BrowserControl = new ChromiumWebBrowser(GetSiteUri());

                // TODO: browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
                BrowserControl.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
                BrowserControl.JavascriptObjectRepository.Register("core", bindable, isAsync: false, options: BindingOptions.DefaultBinder);
                BrowserControl.IsBrowserInitializedChanged += browserIsBrowserInitializedChanged;
                BrowserControl.FrameLoadEnd += (sender, args) => browserIsBrowserInitializedChanged(this, args);



                var requestHandler = new CefRequestHandler();
                requestHandler.OnAuthentication += (sender, args) => OnAuthSuccess?.Invoke(sender, args);
                BrowserControl.RequestHandler = requestHandler;

                BrowserControl.LifeSpanHandler = new AzureOnClosePopupHijack();

                Logger.Info("Chromium created..");
            }
            catch (System.IO.FileNotFoundException ex) {
                MessageBox.Show("Error \"File Not Found\" loading Chromium, did you forget to install Visual C++ runtimes?\n\nvc_redist86 in the IA folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            }
            catch (IOException ex) {
                MessageBox.Show("Error loading Chromium, You may be lacking the proper Visual C++ runtimes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Process.Start("https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022");
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            }
            catch (Exception ex) {
                MessageBox.Show("Unknown error loading Chromium, please see log file for more information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            }
        }

        #region CefBackupAuthentication
        public void Open(string url) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                Logger.Debug("Opening IAGD login page..:");
                Logger.Debug($"window.open('{url}');");
                BrowserControl.ExecuteScriptAsync("window.open", url);
            }
            else {
                MessageBox.Show(
                    RuntimeSettings.Language.GetTag("iatag_ui_javascript_not_ready_body"),
                    RuntimeSettings.Language.GetTag("iatag_ui_javascript_not_ready_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        public bool IsReady() {
            return BrowserControl.CanExecuteJavascriptInMainFrame;
        }

        public event EventHandler OnAuthSuccess;
        #endregion CefBackupAuthentication
    }
}