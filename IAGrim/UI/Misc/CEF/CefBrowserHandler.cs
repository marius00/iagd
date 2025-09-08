using System.Net;
using IAGrim.Database.Model;
using IAGrim.Services;
using IAGrim.Services.ItemReplica;
using IAGrim.Settings;
using IAGrim.UI.Controller.dto;
using IAGrim.UI.Misc.Protocol;
using IAGrim.Utilities;
using log4net;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler : ICefBackupAuthentication, IUserFeedbackHandler, IBrowserCallbacks, IHelpService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private TabControl _tabControl; // TODO: UGh.. why?
        private readonly SettingsService _settings;

        public Microsoft.Web.WebView2.WinForms.WebView2 BrowserControl { get; private set; }
        private readonly object _lockObj = new object();
        public bool IsReady { get; set; }

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public CefBrowserHandler(SettingsService settings, TabControl tabControl, WebView2 browserControl)
        {
            _settings = settings;
            _tabControl = tabControl;
            BrowserControl = browserControl;
        }

        private void SendMessage(IOMessage message) {
            if (IsReady) {
                BrowserControl.ExecuteScriptAsync("window.message(" + JsonConvert.SerializeObject(message, _serializerSettings) + ")");
            }
            else {
                Logger.Warn("Attempting to interact with webview, but not yet ready.");
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

        public void SetCollectionAggregateData(IList<CollectionItemAggregateRow> rows) {
            SendMessage(new IOMessage { Type = IOMessageType.SetAggregateItemData, Data = rows });
        }

        public void ShowLoadingAnimation(bool visible) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.IsLoading, Value = visible } });
        }

        public void ShowNoMoreInstantSyncWarning() {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.ShowNoMoreInstantSyncWarning, Value = true } });
        }

        bool IBrowserCallbacks.IsReady() {
            return IsReady;
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

        #region CefBackupAuthentication
        public void Open(string url) {
            Logger.Debug("Opening IAGD login page..:");
            Logger.Debug($"window.open('{url}');");
            BrowserControl.ExecuteScriptAsync("window.open('" + url + "')");
        }

        [Obsolete("Old login system via redirects")]
        public event EventHandler OnAuthSuccess;
        #endregion CefBackupAuthentication
    }
}