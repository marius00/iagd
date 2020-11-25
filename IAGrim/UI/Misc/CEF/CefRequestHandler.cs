using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using IAGrim.Backup.Azure.CefSharp.Events;

namespace IAGrim.UI.Misc.CEF {
    // https://github.com/cefsharp/CefSharp/blob/master/CefSharp.Example/RequestHandler.cs
    // Overrides links inside the embedded webview.
    public class CefRequestHandler : IRequestHandler {
        public event EventHandler OnAuthentication;

        private static string ExtractToken(string url) {
            var token = url.Replace("https://auth.iagd.dreamcrash.org/token/", "");
            if (token.Contains("#")) {
                return token.Substring(0, token.IndexOf("#"));
            }

            return token;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
            if (request.Url.StartsWith("https://auth.iagd.dreamcrash.org/token/") || request.Url.StartsWith("http://localhost:7071/api/Authenticate")) {
                var token = ExtractToken(request.Url);
                OnAuthentication?.Invoke(browser, new AuthResultEvent {Token = token});
                return true;
            }

            // Allow localdev
            else if (request.Url.StartsWith("http://localhost:3000")) {
                return false;
            }

            else if (request.Url.StartsWith("https://items.dreamcrash.org")) {
                return false;
            }

            else if (!request.Url.Contains("grimdawn.dreamcrash") && !frame.Url.Contains("dreamcrash.org") && (request.Url.StartsWith("http://") || request.Url.StartsWith("https://"))) {
                System.Diagnostics.Process.Start(request.Url);
                return true;
            }


            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser) {
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture) {
            return false;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) {
            return null;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
            return true;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl,
            ISslInfo sslInfo, IRequestCallback callback) {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath) {
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IRequestCallback callback) {
            return CefReturnValue.Continue;
        }

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port,
            string realm, string scheme, IAuthCallback callback) {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port,
            X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) {
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback) {
            return false;
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, ref string newUrl) {
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            ref string newUrl) {
        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url) {
            return false;
        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser) {
        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response) {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response) {
            return null;
        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, UrlRequestStatus status, long receivedContentLength) {
        }
    }
}