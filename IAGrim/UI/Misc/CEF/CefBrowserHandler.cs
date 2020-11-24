﻿using System;
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
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using NHibernate;

namespace IAGrim.UI.Misc.CEF {
    public class CefBrowserHandler : IDisposable, ICefBackupAuthentication, IUserFeedbackHandler {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefBrowserHandler));
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        private const string Dispatch = "data.globalStore.dispatch";

        public event EventHandler TransferSingleRequested;
        public event EventHandler TransferAllRequested;

        public ChromiumWebBrowser BrowserControl { get; private set; }

        private readonly object _lockObj = new object();

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

        /// <summary>
        /// Attempt to execute javascript code with 100ms retries until the browser is ready.
        /// Compensates for C# executing .js code before the page is fully loaded.
        /// Eg, the browser is loaded but the page is not.
        /// </summary>
        /// <param name="mustExist"></param>
        /// <param name="script"></param>
        private void SafeExecute(string mustExist, string script) {
            string js = @"
            function safeExecute(mustExist, func) {
                if (eval('typeof ' + mustExist) === 'undefined') {
                    setTimeout(() => safeExecute(mustExist, func), 100);
                } else {
                    func();
                }
            }
";

            js += "\n safeExecute({mustExist}, () => { {script}; });"
                .Replace("{mustExist}", mustExist)
                .Replace("{script}", script);
            
            BrowserControl.ExecuteScriptAsync(js);
        }

        public void SetItems(List<JsonItem> items) {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                BrowserControl.ExecuteScriptAsync("window.setItems", JsonConvert.SerializeObject(items, _settings));
            }
            else {
                Logger.Warn("Attempted to update items but CEF not yet initialized.");
            }
        }

        // TODO: Redo
        public void AddItems() {
            if (BrowserControl.CanExecuteJavascriptInMainFrame) {
                // TODO: This may not be available yet, better the .js ask us for data, and then we deliver it.
                BrowserControl.ExecuteScriptAsync($"addItemsFromGlobalItems();");
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
                    if (!string.IsNullOrEmpty(helpUrl)) {
                        BrowserControl.ExecuteScriptAsync($"{Dispatch}(data.showMessage('{m}', '{levelLowercased}', '{helpUrl}'));");
                    }
                    else {
                        BrowserControl.ExecuteScriptAsync($"{Dispatch}(data.showMessage('{m}', '{levelLowercased}', undefined));");
                    }
                }
                else {
                    Logger.Warn($"Attempted to display message \"{message}\" but browser is not yet initialized");
                }
            }
        }

        public void ShowMessage(string message) {
            ShowMessage(message, UserFeedbackLevel.Info);
        }


        public void InitializeChromium(object legacyBindeable, object bindable, EventHandler browserIsBrowserInitializedChanged) {
            try {
                Logger.Info("Creating Chromium instance..");

                // Useful: https://github.com/cefsharp/CefSharp/blob/cefsharp/79/CefSharp.Example/CefExample.cs#L208
                Cef.EnableHighDPISupport();


                // TODO: Read and analyze https://github.com/cefsharp/CefSharp/issues/2246 -- Is this the correct way to do things in the future?
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.WcfEnabled = true;
                BrowserControl = new ChromiumWebBrowser(GetSiteUri());

                // TODO: browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
                BrowserControl.JavascriptObjectRepository.Register("data", legacyBindeable, isAsync: false, options: BindingOptions.DefaultBinder);
                BrowserControl.JavascriptObjectRepository.Register("core", bindable, isAsync: true, options: BindingOptions.DefaultBinder);
                BrowserControl.IsBrowserInitializedChanged += browserIsBrowserInitializedChanged;

                var requestHandler = new CefRequestHandler();
                requestHandler.TransferSingleRequested += (sender, args) => this.TransferSingleRequested?.Invoke(sender, args);
                requestHandler.TransferAllRequested += (sender, args) => this.TransferAllRequested?.Invoke(sender, args);
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
