using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using CefSharp;
using IAGrim.Backup.Cloud.CefSharp.Events;
using log4net;

namespace IAGrim.UI.Misc.CEF {
    // https://github.com/cefsharp/CefSharp/blob/master/CefSharp.Example/RequestHandler.cs
    // Overrides links inside the embedded webview.
    public class CefRequestHandler : IRequestHandler {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CefRequestHandler));
        public event EventHandler OnAuthentication;

        private static string ExtractToken(string url) {
            return HttpUtility.ParseQueryString(url).Get("token");
        }
        private static string ExtractUser(string url) {
            return HttpUtility.ParseQueryString(url).Get("email");
        }

        /// <summary>
        /// Navigation/redirect request hook.
        /// This ensures that only whitelisted URLs can be opened inside IA.
        /// Any URL not whitelisted will be opened using the system default browser instead.
        /// </summary>
        /// <param name="chromiumWebBrowser"></param>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="request"></param>
        /// <param name="userGesture"></param>
        /// <param name="isRedirect"></param>
        /// <returns>True to ABORT any request/redirect, False to ALLOW them.</returns>
        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
            // URL Hook for online backup login. (Binding a JS object to new windows proved to be too painful)
            if (request.Url.StartsWith("https://token.iagd.evilsoft.net/")) {
                var token = ExtractToken(request.Url);
                var user = ExtractUser(request.Url);
                Logger.Info($"Got a login request for {user}");
                OnAuthentication?.Invoke(browser, new AuthResultEvent(user, token));
                return true;
            }

            // Allow localdev
            else if (request.Url.StartsWith("http://localhost:3000")) {
                Logger.Debug($"URL Requested: {request.Url}, Status: Allowed");
                return false;
            }

            else if (request.Url.StartsWith("https://items.dreamcrash.org")) {
                Logger.Debug($"URL Requested: {request.Url}, Status: Allowed");
                return false;
            }

            // TODO: Rewrite this, who the hell knows what this does.
            else if (!request.Url.Contains("iagd.evilsoft.net") && !request.Url.Contains("grimdawn.dreamcrash") && !frame.Url.Contains("dreamcrash.org") && !frame.Url.Contains("iagd.evilsoft.net") && (request.Url.StartsWith("http://") || request.Url.StartsWith("https://"))) {
                Logger.Debug($"URL Requested: {request.Url}, Status: Deny, Action: DefaultBrowser");
                System.Diagnostics.Process.Start(request.Url);
                return true;
            }

            Logger.Debug($"URL Requested: {request.Url}, Status: Allowed");
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser) {
            
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) {
            return false;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
            return null;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
            return true; // TODO: We got 2 old ones.. true AND false.. O.o
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, string originUrl, long newSize, IRequestCallback callback) {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, string pluginPath) {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser) {
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, CefTerminationStatus status) {
        }
    }
}