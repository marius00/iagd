using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using IAGrim.Backup.Azure.CefSharp;
using IAGrim.Database.Model;
using IAGrim.UI.Controller.dto;
using IAGrim.Utilities;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler : IDisposable, ICefBackupAuthentication, IUserFeedbackHandler, IBrowserCallbacks {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        public ChromiumWebBrowser BrowserControl { get; private set; }
        private readonly object _lockObj = new object();
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        ~CefBrowserHandler() {
            Dispose();
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

        public void ShowLoadingAnimation() {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setIsLoading", true);
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }

        }
        
        public void SetItems(List<JsonItem> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setItems", JsonConvert.SerializeObject(items, _settings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public void SetCollectionItems(IList<CollectionItem> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setCollectionItems", JsonConvert.SerializeObject(items, _settings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public void AddItems(List<JsonItem> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.addItems", JsonConvert.SerializeObject(items, _settings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
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
                    var ret = new Dictionary<string, string> {
                        {"message", m},
                        {"type", levelLowercased}
                    };

                    BrowserControl.ExecuteScriptAsync("window.showMessage", JsonConvert.SerializeObject(ret, _settings));
                }
                else {
                    Logger.Warn($"Attempted to display message \"{message}\" but browser is not yet initialized");
                }
            }
        }

        public void ShowMessage(string message) {
            ShowMessage(message, UserFeedbackLevel.Info);
        }


        public void InitializeChromium(object bindable, EventHandler browserIsBrowserInitializedChanged) {
            try {
                Logger.Info("Creating Chromium instance..");

                // Useful: https://github.com/cefsharp/CefSharp/blob/cefsharp/79/CefSharp.Example/CefExample.cs#L208
                Cef.EnableHighDPISupport();


                // TODO: Read and analyze https://github.com/cefsharp/CefSharp/issues/2246 -- Is this the correct way to do things in the future?
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.WcfEnabled = true;
                BrowserControl = new ChromiumWebBrowser(GetSiteUri());

                // TODO: browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
                BrowserControl.JavascriptObjectRepository.Register("core", bindable, isAsync: false, options: BindingOptions.DefaultBinder);
                BrowserControl.IsBrowserInitializedChanged += browserIsBrowserInitializedChanged;
                BrowserControl.FrameLoadEnd += (sender, args) => browserIsBrowserInitializedChanged(this, args);
                ;


                var requestHandler = new CefRequestHandler();
                requestHandler.OnAuthentication += (sender, args) => OnSuccess?.Invoke(sender, args);
                BrowserControl.RequestHandler = requestHandler;
                
                BrowserControl.LifeSpanHandler = new AzureOnClosePopupHijack();
                
                Logger.Info("Chromium created..");
            } catch (System.IO.FileNotFoundException ex) {
                MessageBox.Show("Error \"File Not Found\" loading Chromium, did you forget to install Visual C++ runtimes?\n\nvc_redist86 in the IA folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            } catch (IOException ex) {
                MessageBox.Show("Error loading Chromium, did you forget to install Visual C++ runtimes?\n\nvc_redist86 in the IA folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            } catch (Exception ex) {
                MessageBox.Show("Unknown error loading Chromium, please see log file for more information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.Warn(ex.Message);
                Logger.Warn(ex.StackTrace);
                throw;
            }
        }

        /* Start CefBackupAuthentication Start */
        public void Open(string url) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync($"window.open('{url}');");
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

        public event EventHandler OnSuccess;
        /* End CefBackupAuthentication End */
    }
}
