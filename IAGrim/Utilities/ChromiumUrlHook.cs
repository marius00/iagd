using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAGrim.Utilities {
    /// <summary>
    /// Helper class to hook URL requests for Chromium controls
    /// Derive from this class and overload OnBeforeBrowse
    /// </summary>
    public partial class ChromiumUrlHook : IRequestHandler {

        public virtual bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect) {
            throw new NotImplementedException();
        }

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
            return null;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback) {
            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) {
            return true;
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) {
            return true;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath) { }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url) {
            return true;
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) {
            callback.Continue(true);
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) { }
        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser) { }
        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) { }
        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl) { }
        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
            return false;
        }
    }
}
