using System;
using CefSharp;
using IAGrim.Backup.Azure.CefSharp.Events;
using IAGrim.UI.Misc.CEF.Dto;

namespace IAGrim.UI.Misc.CEF {
    // https://github.com/cefsharp/CefSharp/blob/master/CefSharp.Example/RequestHandler.cs
    public class CefRequestHandler : IRequestHandler {

        public event EventHandler TransferSingleRequested;
        public event EventHandler TransferAllRequested;
        public event EventHandler OnAuthentication;

        private static string ExtractToken(string url) {
            var token = url.Replace("https://auth.iagd.dreamcrash.org/token/", "");
            if (token.Contains("#")) {
                return token.Substring(0, token.IndexOf("#"));
            }

            return token;
        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect) {
            if (request.Url.StartsWith("https://auth.iagd.dreamcrash.org/token/")) {
                var token = ExtractToken(request.Url);
                OnAuthentication?.Invoke(browser, new AuthResultEvent { Token = token});
                return true;
            }

            else if (request.Url.StartsWith("transfer://single/")) {
                TransferSingleRequested?.Invoke(this, new ItemTransferEvent {
                    Request = request.Url.Replace("transfer://single/", "")
                });
                return true;
            }

            else if (request.Url.StartsWith("transfer://all/")) {
                TransferAllRequested?.Invoke(this, new ItemTransferEvent {
                    Request = request.Url.Replace("transfer://all/", "")
                });
                return true;
            }

            else if (!request.Url.Contains("grimdawn.dreamcrash") && (request.Url.StartsWith("http://") || request.Url.StartsWith("https://"))) {
                System.Diagnostics.Process.Start(request.Url);
                return true;
            }

            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture) {
            return false;
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

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) {

        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback) {
            return false;
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
