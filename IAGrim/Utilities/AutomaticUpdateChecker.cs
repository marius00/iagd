using EvilsoftCommons.Exceptions;
using IAGrim.Settings;
using IAGrim.UI.Popups;
using log4net;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Timer = System.Timers.Timer;

namespace IAGrim.Utilities {
    /// <summary>
    /// Flows:
    /// Auto => You have an update [only if IA has focus? check on get focus?]. Not modal/blocking
    /// Manual => You have an update[modal/blocking]
    /// Manual => No update available (popup)
    /// Downgrade => Start downgrading now
    /// 
    /// Download progress bar view
    /// </summary>
    class AutomaticUpdateChecker {
        private Timer? _timer;
        private DateTime _lastTimeNotMinimized = DateTime.UtcNow;
        private string _downloadUri = string.Empty;
        private string _installerPath = string.Empty;
        private readonly SettingsService _settings;
        private DownloadingUpdateModal? _progressModal = null;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AutomaticUpdateChecker));

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        private const string Url = "https://grimdawn.evilsoft.net/version.php";

        private static string GetUpdateXmlUri(bool requestLatest) {
            var version = ExceptionReporter.VersionString;

            if (requestLatest) {
                return $"{Url}?beta&version={version}&is64bit=true";
            }

            return $"{Url}?version={version}&is64bit=true";
        }

        public AutomaticUpdateChecker(SettingsService settings) {
            _settings = settings;
            int min = 1000 * 60;
            int hour = 60 * min;
            _timer = new Timer();
            _timer.Start();
            _timer.Elapsed += (a1, a2) => {
                if (Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = "CheckUpdatesThread";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                }

                bool hasBeenMinimizedRecently = (DateTime.UtcNow - _lastTimeNotMinimized).TotalHours < 38;
                if (GetTickCount64() > 5 * 60 * 1000 && hasBeenMinimizedRecently && ShouldCheckForUpdates()) {
                    CheckForUpdates();
                    _settings.GetPersistent().NextUpdateCheck = DateTime.UtcNow.AddDays(1);
                }
            };
            _timer.Interval = 12 * hour;
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void ResetLastMinimized() {
            _lastTimeNotMinimized = DateTime.UtcNow;
        }

        public bool ShouldCheckForUpdates() {
            return _settings.GetPersistent().NextUpdateCheck < DateTime.UtcNow;
        }

        public void CheckForUpdates(bool manualUpdate = false) {
            var uri = GetUpdateXmlUri(manualUpdate || _settings.GetPersistent().SubscribeExperimentalUpdates);
            CheckForUpdates(uri, false, manualUpdate);
        }

        private void CheckForUpdates(string uri, bool forceUpdate, bool userInitiated) {
            try {
                using WebClient client = new WebClient();
                var xmlContent = client.DownloadString(uri);

                XDocument doc;
                using StringReader reader = new StringReader(xmlContent);
                doc = XDocument.Load(reader);
                if (doc == null) {
                    if (userInitiated) {
                        MessageBox.Show("Something went wrong checking for updates", "Something went wrong");
                    }
                    Logger.Warn("Could not parse XML in version check");
                    return;
                }


                string version = doc.Root.Element("version")?.Value;
                _downloadUri = doc.Root.Element("url")?.Value;
                if (string.IsNullOrEmpty(version)) {
                    if (userInitiated) {
                        MessageBox.Show("Something went wrong checking for updates", "Something went wrong");
                    }
                    Logger.Warn("Could not check version");
                    return;
                }

                if (String.Compare(version, ExceptionReporter.VersionString) > 0 || forceUpdate) {
                    Logger.Info($"Latest version is {version}, local version is {ExceptionReporter.VersionString}, update available");
                    if (new UpdateModal(_settings, version, forceUpdate).ShowDialog() == DialogResult.OK) {
                        Download(version);
                        _progressModal = new DownloadingUpdateModal();
                        _progressModal.ShowDialog();
                    } else {
                        Logger.Info("User was made aware of a new update, chose not to update.");
                    }
                } else {
                    MessageBox.Show("You are on the latest version", "No new updates");
                }
            }
            catch (Exception ex) {
                Logger.Warn(ex);
                MessageBox.Show("Something went wrong checking for updates", "Something went wrong");
            }
        }

        private void Download(string version) {
            var downloadsFolder = GlobalPaths.DownloadsFolder ?? System.IO.Path.GetTempPath();
            _installerPath = Path.Combine(downloadsFolder, $"IAGD-{version}.exe");
            Logger.Info($"Downloading new update to {_installerPath}");

            WebClient client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted; ;
            client.DownloadFileAsync(new Uri(_downloadUri), _installerPath);
            _progressModal?.FormClosing += (_, __) => {
                client.CancelAsync();
            };
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            if (_progressModal == null) {
                return;
            }

            if (_progressModal.InvokeRequired) {
                _progressModal.Invoke((System.Windows.Forms.MethodInvoker)delegate {
                    _progressModal?.ProgressBar.Value = e.ProgressPercentage;
                });
            } else {
                _progressModal?.ProgressBar.Value = e.ProgressPercentage;
            }
        }

        private void Client_DownloadFileCompleted(object? sender, AsyncCompletedEventArgs e) {
            Logger.Info("Update download complete, initating installer and exiting IA");

            // Start installer and exit IAGD
            Process.Start(new ProcessStartInfo { FileName = "file://" + _installerPath, UseShellExecute = true });
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Downgrade to the version on the website, usually older and more stable.
        /// </summary>
        public void Downgrade() {
            CheckForUpdates(GetUpdateXmlUri(false) + "&downgrade", true, true);
        }

        public void Dispose() {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}