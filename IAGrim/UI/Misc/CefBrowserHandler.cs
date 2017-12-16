using CefSharp;
using CefSharp.WinForms;
using IAGrim.Database;
using IAGrim.Utilities;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.UI.Misc {
    public class CefBrowserHandler : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private ChromiumWebBrowser _browser;

        public Control BrowserControl {
            get {
                return _browser;
            }
        }

        private object lockObj = new object();

        ~CefBrowserHandler() {
            Dispose();
        }
        public void Dispose() {
            lock (lockObj) {
                if (_browser != null) {
                    CefSharpSettings.WcfTimeout = TimeSpan.Zero;
                    _browser.Dispose();

                    Cef.Shutdown();
                    _browser = null;
                }
            }
        }

        public void ShowDevTools() {
            _browser.ShowDevTools();
        }

        public void ShowLoadingAnimation() {
            if (_browser.IsBrowserInitialized)
                _browser.ExecuteScriptAsync("isLoading(true);");
        }
        public void RefreshItems() {
            if (_browser.IsBrowserInitialized) {
                _browser.ExecuteScriptAsync("refreshData();");
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        public void JsCallback(string method, string json) {
            if (_browser.IsBrowserInitialized) {
                _browser.ExecuteScriptAsync($"{method}({json});");
            }
            else {
                Logger.Warn("Attempted to execute a callback but CEF not yet initialized.");
            }
        }
        

        public void LoadItems() {
            if (_browser.IsBrowserInitialized) {
                _browser.ExecuteScriptAsync("addData();");
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }
        
        
        public void ShowMessage(string message, string level) {
            if (!string.IsNullOrEmpty(message)) {
                if (_browser.IsBrowserInitialized)
                    _browser.ExecuteScriptAsync("showMessage", new[] { message.Replace("\n", "\\n"), level, level.ToLower() });
                else
                    Logger.Warn($"Attempted to display message \"{message}\" but browser is not yet initialized");
            }
        }

        public void InitializeChromium(object bindeable, EventHandler<IsBrowserInitializedChangedEventArgs> Browser_IsBrowserInitializedChanged) {
            try {
                Logger.Info("Creating Chromium instance..");
                Cef.EnableHighDPISupport();

                //var settings = new CefSettings();
                //settings.CefCommandLineArgs.Add("disable-gpu", "1");
                if (!Cef.Initialize()) {
                    Logger.Fatal("Could not initialize Chromium");
                    MessageBox.Show("Fatal error - Could not initialize chromium", "Fatal error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                try {
                    string src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "item-kjs.html");
                    File.Copy(src, GlobalPaths.ItemsHtmlFile, true);
                } catch (Exception ex) {
                    // Probably doesn't matter, only relevant if its been updated
                    Logger.Warn(ex.Message);
                    Logger.Warn(ex.StackTrace);
                }

                _browser = new ChromiumWebBrowser(GlobalPaths.ItemsHtmlFile);
                _browser.RegisterJsObject("data", bindeable, false);
                _browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;

                //browser.RequestHandler = new TransferUrlHijack { TransferMethod = transferItem };
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
    }
}
