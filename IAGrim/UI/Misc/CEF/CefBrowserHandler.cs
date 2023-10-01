using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using IAGrim.Backup.Cloud.CefSharp;
using IAGrim.Database.Model;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.UI.Controller.dto;
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

        public void ShowCharacterBackups() {
            if (BrowserControl != null && BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.showCharacterBackups", "");
                if (_tabControl.InvokeRequired) {
                    _tabControl.Invoke((MethodInvoker) delegate { _tabControl.SelectedIndex = 0; });
                }
                else {
                    _tabControl.SelectedIndex = 0;
                }
            }
            else {
                Logger.Warn("Attempted to show character backups but CEF not yet initialized.");
            }
        }

        public void ShowHelp(HelpService.HelpType type) {
            if (BrowserControl != null && BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.showHelp", type.ToString());
                if (_tabControl.InvokeRequired) {
                    _tabControl.Invoke((MethodInvoker) delegate { _tabControl.SelectedIndex = 0; });
                }
                else {
                    _tabControl.SelectedIndex = 0;
                }
            }
            else {
                Logger.Warn("Attempted to show help but CEF not yet initialized.");
            }
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
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setAggregateItemData", JsonConvert.SerializeObject(rows, _serializerSettings));
            }
            else {
                Logger.Warn("Attempted to update item aggregate but CEF not yet initialized.");
            }
        }

        public void ShowLoadingAnimation(bool visible) {
            if (BrowserControl != null && BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setIsLoading", visible);
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        /// <summary>
        /// Set the current batch of items
        /// </summary>
        /// <param name="items">The current batch</param>
        /// <param name="numItemsFound">The number of items found, total (eg 3000 found, but batch has 64)</param>
        public void SetItems(List<List<JsonItem>> items, int numItemsFound) {
            if (BrowserControl != null && BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setItems", JsonConvert.SerializeObject(items, _serializerSettings), numItemsFound);
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public void SetCollectionItems(IList<CollectionItem> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setCollectionItems", JsonConvert.SerializeObject(items, _serializerSettings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public void AddItems(List<List<JsonItem>> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.addItems", JsonConvert.SerializeObject(items, _serializerSettings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public bool SetDarkMode(bool enabled) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setIsDarkmode", enabled);
                return true;
            }
            else {
                Logger.Warn("Attempted to set dark/light mode but CEF not yet initialized.");
                return false;
            }
        }

        public bool SetHideItemSkills(bool enabled) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setHideItemSkills", enabled);
                return true;
            }
            else {
                Logger.Warn("Attempted to set hide item skilsl but CEF not yet initialized.");
                return false;
            }
        }

        public void SetIsGrimParsed(bool enabled) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setIsGrimParsed", enabled);
            }
            else {
                Logger.Warn("Attempted to set GD parsed but CEF not yet initialized.");
            }
        }

        public bool SetOnlineBackupsEnabled(bool enabled) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setOnlineBackupsEnabled", enabled);
                return true;
            } else {
                Logger.Warn("Attempted to toggle hide online backup icon but CEF not yet initialized.");
                return false;
            }
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


        public void ShowMessage(string message, UserFeedbackLevel level, string helpUrl = null) {
            string levelLowercased = level.ToString().ToLowerInvariant();
            var m = message.Replace("\n", "\\n").Replace("'", "\\'");
            if (!string.IsNullOrEmpty(message)) {
                if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                    var autoDismissMessage = IsProgramActive.IsActive() || _settings.GetPersistent().AutoDismissNotifications;
                    var ret = new Dictionary<string, string> {
                        {"message", m},
                        {"type", levelLowercased},
                        {"fade", autoDismissMessage ? "true" : "false"},
                    };

                    BrowserControl.ExecuteScriptAsync("window.showMessage", JsonConvert.SerializeObject(ret, _serializerSettings));
                }
                else {
                    Logger.Warn($"Attempted to display message \"{message}\" but browser is not yet initialized");
                }
            }
        }

        public void ShowMessage(string message) {
            ShowMessage(message, UserFeedbackLevel.Info);
        }


        public void InitializeChromium(object bindable, EventHandler browserIsBrowserInitializedChanged, TabControl tabControl) {
            try {
                Logger.Info("Creating Chromium instance..");
                _tabControl = tabControl;


                // TODO: Read and analyze https://github.com/cefsharp/CefSharp/issues/2246 -- Is this the correct way to do things in the future?
                CefSharpSettings.WcfEnabled = true;
                BrowserControl = new ChromiumWebBrowser(GetSiteUri());

                // TODO: browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
                BrowserControl.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
                BrowserControl.JavascriptObjectRepository.Register("core", bindable, isAsync: false, options: BindingOptions.DefaultBinder);
                BrowserControl.IsBrowserInitializedChanged += browserIsBrowserInitializedChanged;
                BrowserControl.FrameLoadEnd += (sender, args) => browserIsBrowserInitializedChanged(this, args);
                ;


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
                MessageBox.Show("Error loading Chromium, did you forget to install Visual C++ runtimes?\n\nvc_redist86 in the IA folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /* Start CefBackupAuthentication Start */
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

        public event EventHandler OnAuthSuccess;
        /* End CefBackupAuthentication End */
    }
}