using System;
using IAGrim.Services;
using log4net;
using Microsoft.Web.WebView2.Core;

namespace IAGrim.Utilities {
    /// <summary>
    /// Whether the Microsoft Edge WebView2 runtime -- which renders the entire item grid -- is actually installed,
    /// and what to tell the user when it is not.
    ///
    /// This used to be found out the hard way. <c>CoreWebView2Environment.CreateAsync</c> throws when the runtime
    /// is missing, the call was unguarded, and it happens while the main window is still being built: so a missing
    /// runtime took IAGD down during startup, and none of the WebView2 error handling further in ever ran. The
    /// user saw a crash rather than the message explaining what to install.
    ///
    /// Checking up front costs one registry lookup and turns that into an explanation.
    /// </summary>
    internal static class WebView2Runtime {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebView2Runtime));

        private const string DownloadPage = "https://developer.microsoft.com/microsoft-edge/webview2/";

        private static readonly Lazy<string?> Version = new Lazy<string?>(DetectVersion);

        /// <summary>The installed runtime version, or null when there is none.</summary>
        public static string? InstalledVersion => Version.Value;

        public static bool IsInstalled => !string.IsNullOrEmpty(Version.Value);

        private static string? DetectVersion() {
            try {
                // Returns null when nothing is installed, and throws when the runtime is present but unusable.
                var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                if (string.IsNullOrEmpty(version)) {
                    Logger.Warn("No Microsoft Edge WebView2 runtime is installed.");
                    return null;
                }

                Logger.Info($"Microsoft Edge WebView2 runtime version {version}");
                return version;
            }
            catch (WebView2RuntimeNotFoundException) {
                Logger.Warn("No Microsoft Edge WebView2 runtime is installed.");
                return null;
            }
            catch (Exception ex) {
                // Unexpected, so do not claim it is missing -- let the real initialisation produce the error.
                Logger.Warn($"Could not determine the WebView2 runtime version: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// Creates the shared WebView2 environment, or returns null with a message describing why it could not be.
        ///
        /// Never throws, and that is the whole point: both callers construct it while a window is being built, so
        /// an exception escaping here takes IAGD down before there is anywhere to show the error. Losing the
        /// browser costs the item grid; everything else -- the hook, loot capture, settings, the tray -- carries on.
        /// </summary>
        public static CoreWebView2Environment? TryCreateEnvironment(out string? error) {
            if (!IsInstalled) {
                error = DescribeMissingRuntime();
                return null;
            }

            try {
                // WINE PATCH: the Chromium sandbox and GPU/renderer subprocesses fail to spawn reliably under
                // Wine (especially while Grim Dawn is running), so WebView2 navigation never completes and the
                // item grid stays blank. --no-sandbox + --disable-gpu + --single-process avoids the failing
                // subprocess/sandbox path so the page renders regardless of Grim Dawn. Applied ONLY under Wine
                // so Windows keeps its default (sandboxed, GPU-accelerated, multi-process) behaviour untouched.
                error = null;
                if (WineDetector.IsRunningInWine()) {
                    var envOptions = new CoreWebView2EnvironmentOptions {
                        AdditionalBrowserArguments = "--no-sandbox --disable-gpu --disable-gpu-compositing --single-process"
                    };
                    return CoreWebView2Environment.CreateAsync(null, GlobalPaths.EdgeCacheLocation, envOptions).Result;
                }

                return CoreWebView2Environment.CreateAsync(null, GlobalPaths.EdgeCacheLocation).Result;
            }
            catch (Exception ex) {
                // .Result wraps whatever CreateAsync threw; the inner exception is the informative one.
                var cause = (ex as AggregateException)?.InnerException ?? ex;
                Logger.Error($"Could not create the WebView2 environment (runtime {InstalledVersion}, cache \"{GlobalPaths.EdgeCacheLocation}\"): {cause.Message}", cause);

                error =
                    "Item Assistant could not start the Microsoft Edge WebView2 runtime, so your items cannot be displayed.\n\n" +
                    $"Installed runtime: {InstalledVersion ?? "unknown"}\n" +
                    $"Cache folder: {GlobalPaths.EdgeCacheLocation}\n" +
                    $"Error: {cause.Message}\n\n" +
                    "Deleting the cache folder above and restarting sometimes clears this.";

                return null;
            }
        }

        /// <summary>
        /// What to show the user when the runtime is missing. The Wine wording is different because the mistake
        /// available there is different, and it is the one people make: installing the runtime on the Linux side,
        /// or into some other prefix, where IAGD cannot see it.
        /// </summary>
        public static string DescribeMissingRuntime() {
            if (!WineDetector.IsRunningInWine()) {
                return
                    "Item Assistant needs the Microsoft Edge WebView2 runtime to display your items, and it does not appear to be installed.\n\n" +
                    "Download the \"Evergreen Standalone Installer\" (x64) from:\n" +
                    DownloadPage + "\n\n" +
                    "Install it, then start Item Assistant again.";
            }

            return
                "Item Assistant needs the Microsoft Edge WebView2 runtime to display your items, and it is not installed in this Wine prefix.\n\n" +
                "Download the \"Evergreen Standalone Installer\" (x64) from:\n" +
                DownloadPage + "\n\n" +
                "It has to be installed into the same prefix Item Assistant is running in -- installing it on the Linux side, or into a different prefix, will not work.\n\n" +
                "If Item Assistant is running in Grim Dawn's own Proton prefix, that is:\n" +
                "    protontricks-launch --appid 219990 MicrosoftEdgeWebView2Setup.exe\n\n" +
                "Install it, then start Item Assistant again.";
        }
    }
}
