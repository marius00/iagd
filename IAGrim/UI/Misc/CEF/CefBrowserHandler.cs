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
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler(SettingsService settings) : IUserFeedbackHandler, IBrowserCallbacks, IHelpService {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private TabControl? _tabControl; // TODO: UGh.. why?
        private readonly ConcurrentQueue<IOMessage> _initializationQueue = new ConcurrentQueue<IOMessage>();

        public WebView2? BrowserControl { get; private set; }

        private volatile bool _isReady;
        private volatile bool _isReadyUi;
        public bool IsReady { 
            get => _isReady; 
            set { 
                if (value && !_isReady && BrowserControl != null) {
                    BrowserControl.Source = new Uri(GetSiteUri());

                    if (_isReadyUi) {
                        Logger.Info("Both UI and browser are ready, processing queue..");
                        foreach (var item in _initializationQueue) {
                            Logger.Info($"Message: {JsonConvert.SerializeObject(item, _serializerSettings)}");
                            SendMessage(item);
                        }
                        _initializationQueue.Clear();
                    }
                    else {
                        Logger.Info("Browser signaled readiness, waiting for UI..");
                        _isReady = value;
                        /*var t = new Thread(new ThreadStart(() => {
                            Thread.Sleep(3500);
                            _isReady = value;
                            Logger.Warn("Did not receive any alert from the WebUI, sending messages anyways..");
                            foreach (var item in _initializationQueue) {
                                Logger.Info($"Message: {JsonConvert.SerializeObject(item, _serializerSettings)}");
                                SendMessage(item);
                            }

                            _initializationQueue.Clear();
                        }));
                        t.Start();*/
                    }

                    Logger.Info($"There are {_initializationQueue.Count} queued messages");
                }
            } 
        }

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private void SendMessage(IOMessage message) {
            if (BrowserControl?.Parent == null || !_isReadyUi) {
                if (message.IsLogHeavy) {
                    Logger.Warn("Attempted to communicate with the frontend, but browser not yet initialized, queued: " + message.Type.ToString());
                }
                else {
                    Logger.Warn("Attempted to communicate with the frontend, but browser not yet initialized, queued: " + message.Type.ToString() + " " + JsonConvert.SerializeObject(message, _serializerSettings));
                }

                _initializationQueue.Enqueue(message);
                return;
            }
            // window.message({'type':5, 'data':{'items': [], 'replaceExistingItems': true, 'numItemsFound': 0}})



            if (IsReady && _isReadyUi) {
                // Attempting to read the result or anything in a sync way will just stall.
                //Logger.Info("Exec: " + JsonConvert.SerializeObject(message, _serializerSettings));
                if (BrowserControl.Parent.InvokeRequired) {
                    BrowserControl.Parent.Invoke((MethodInvoker)delegate {
                        BrowserControl.ExecuteScriptAsync("window.message(" + JsonConvert.SerializeObject(message, _serializerSettings) + ")");
                    });
                }
                else {
                    BrowserControl.ExecuteScriptAsync("window.message(" + JsonConvert.SerializeObject(message, _serializerSettings) + ")");
                }

            }
            else {
                Logger.Warn("Attempting to interact with webview, but not yet ready.");
                _initializationQueue.Enqueue(message);
            }

        }

        public void ShowCharacterBackups() {
            SendMessage(new IOMessage { Type = IOMessageType.ShowCharacterBackups });
            SwitchToFrontendTab();
        }

        private void SwitchToFrontendTab() {
            if (_tabControl == null) {
                return;
            }
            
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


        private string GetSiteUri() {
#if DEBUG
            /*
            var client = new WebClient();

            try {
                Logger.Debug("Checking if NodeJS is running...");
                client.DownloadString("http://localhost:3000/");
                Logger.Debug("NodeJS running");
                return "http://localhost:3000/";
            }
            catch (System.Net.WebException) {
                Logger.Debug("NodeJS not running, defaulting to standard view");
            }*/
#endif
            return GlobalPaths.ItemsHtmlFile;
        }

        public void InitializeChromium(Microsoft.Web.WebView2.WinForms.WebView2 browserControlView2, JavascriptIntegration bindable, TabControl tabControl) {
            try {
                _tabControl = tabControl;
                this.BrowserControl = browserControlView2;

                BrowserControl.CoreWebView2.AddHostObjectToScript("core", bindable);
                bindable.OnSignalReadiness += (sender, args) => {
                    _isReadyUi = true;
                    Logger.Info("UI signalled readiness");

                    if (_isReady) {
                        Logger.Info("Both UI and browser are ready, processing queue..");
                        foreach (var item in _initializationQueue) {
                            Logger.Info($"Message: {item.Type.ToString() + (item.IsLogHeavy ? "" : " "+ JsonConvert.SerializeObject(item, _serializerSettings))}");
                            SendMessage(item);
                        }
                        _initializationQueue.Clear();
                    }
                };

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
        public void SetDarkMode(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.DarkMode, Value = enabled } });
        }

        public void SetHideItemSkills(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.HideItemSkills, Value = enabled } });
        }

        public void SetIsGrimParsed(bool enabled) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.GrimDawnIsParsed, Value = enabled } });
        }

        public void SetIsFirstRun(bool val) {
            SendMessage(new IOMessage { Type = IOMessageType.SetState, Data = new IOMessageStateChange { Type = IOMessageStateChangeType.FirstRun, Value = val } });
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
                var autoDismissMessage = IsProgramActive.IsActive() || settings.GetPersistent().AutoDismissNotifications;
                var ret = new Dictionary<string, string> {
                    {"message", m},
                    {"type", levelLowercased},
                    {"fade", autoDismissMessage ? "true" : "false"},
                };

                SendMessage(new IOMessage { Type = IOMessageType.ShowMessage, Data = ret });
            }
        }
    }
}