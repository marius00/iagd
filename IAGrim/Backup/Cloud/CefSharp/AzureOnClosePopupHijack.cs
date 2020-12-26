using CefSharp;

namespace IAGrim.Backup.Cloud.CefSharp {
    // TODO: What in the world did/does this class do?
    class AzureOnClosePopupHijack : ILifeSpanHandler {
        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser) {
            //Set newBrowser to null unless your attempting to host the popup in a new instance of ChromiumWebBrowser
            //This option is typically used in WPF. This example demos using IWindowInfo.SetAsChild
            //Older branches likely still have an example of this method if you choose to go down that path.
            newBrowser = null;
            return false;
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser) {
        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser) {
            //The default CEF behaviour (return false) will send a OS close notification (e.g. WM_CLOSE).
            //See the doc for this method for full details.    
            // Allow devtools to close
            if (browser.MainFrame.Url.Equals("chrome-devtools://devtools/inspector.html")) {
            }
            
            return false;
        }

        void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser) {
        }
    }
}
