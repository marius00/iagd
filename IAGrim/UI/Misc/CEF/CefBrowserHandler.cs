using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Database.Model;
using IAGrim.Services;
using IAGrim.Services.ItemReplica;
using IAGrim.Settings;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc.Protocol;
using IAGrim.Utilities;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler : ICefBackupAuthentication, IUserFeedbackHandler, IBrowserCallbacks, IHelpService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private TabControl _tabControl; // TODO: UGh.. why?
        private readonly SettingsService _settings;

        public Microsoft.Web.WebView2.WinForms.WebView2 BrowserControl { get; private set; }

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public CefBrowserHandler(SettingsService settings) {
            _settings = settings;
        }

        private void SendMessage(IOMessage message) {
            if (BrowserControl?.Parent == null) {
                Logger.Warn("Attempted to communicate with the frontend, but CEF not yet initialized, discarded: " + JsonConvert.SerializeObject(message, _serializerSettings));
                return;
            }

            if (BrowserControl.Parent.InvokeRequired) {
                BrowserControl.Parent.Invoke((MethodInvoker)delegate { SendMessage(message); });
                return;
            }


            var script = "window.message(" + JsonConvert.SerializeObject(message, _serializerSettings) + ")";
            BrowserControl.ExecuteScriptAsync(script);
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


        public void InitializeChromium(Microsoft.Web.WebView2.WinForms.WebView2 browserControlView2, JavascriptIntegration bindable, TabControl tabControl) {
            try {
                _tabControl = tabControl;
                this.BrowserControl = browserControlView2;


                BrowserControl.CoreWebView2.AddHostObjectToScript("core", bindable);

                browserControlView2.NavigationStarting += BrowserControlView2_NavigationStarting;
                //browserControlView2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

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

        private void CoreWebView2_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e) {
            Microsoft.Web.WebView2.WinForms.WebView2 webView21 = new WebView2();
            ((System.ComponentModel.ISupportInitialize)(webView21)).BeginInit();
            webView21.AllowExternalDrop = true;
            webView21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                           | System.Windows.Forms.AnchorStyles.Left)
                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            webView21.Location = new System.Drawing.Point(3, 3);
            webView21.Name = "loginpopup";
            webView21.Size = new System.Drawing.Size(1100, 570);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            ((System.ComponentModel.ISupportInitialize)(webView21)).EndInit();
            webView21.NavigationStarting += BrowserControlView2_NavigationStarting;
            e.NewWindow = webView21.CoreWebView2;
        }

        private static string ExtractToken(string url) {
            return HttpUtility.ParseQueryString(url).Get("token");
        }
        private static string ExtractUser(string url) {
            return HttpUtility.ParseQueryString(url).Get("email");
        }
        private void BrowserControlView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e) {
            var url = e.Uri;
            // URL Hook for online backup login. (Binding a JS object to new windows proved to be too painful)
            if (url.StartsWith("https://token.iagd.evilsoft.net/")) {
                var token = ExtractToken(url);
                var user = ExtractUser(url);
                Logger.Info($"Got a login request for {user}");
                OnAuthSuccess?.Invoke(sender, new AuthResultEvent(user, token));
                e.Cancel = true;
            }

            // Allow localdev
            else if (url.StartsWith("http://localhost:3000")) {
                Logger.Debug($"URL Requested: {url}, Status: Allowed");
                return;
            }


            // TODO: Rewrite this, who the hell knows what this does.
            else if (!url.StartsWith("https://iagd.evilsoft.net") && !url.StartsWith("http://iagd.evilsoft.net") && !url.StartsWith("https://api.iagd.evilsoft.net") && !url.Contains("grimdawn.evilsoft.net") && !url.Contains("iagd.evilsoft.net") && (url.StartsWith("http://") || url.StartsWith("https://"))) {
                Logger.Debug($"URL Requested: {url}, Status: Deny, Action: DefaultBrowser");
                System.Diagnostics.Process.Start(url);
                e.Cancel = true;
            }


            Logger.Debug($"URL Requested: {url}, Status: Allowed");
        }

        #region CefBackupAuthentication
        public void Open(string url) {
            if (BrowserControl != null) {
                Logger.Debug("Opening IAGD login page..:");
                Logger.Debug($"window.open('{url}');");
                BrowserControl.ExecuteScriptAsync("window.open('" + url + "')");
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


        [Obsolete("Old login system via redirects")]
        public event EventHandler OnAuthSuccess;
        #endregion CefBackupAuthentication
    }
}