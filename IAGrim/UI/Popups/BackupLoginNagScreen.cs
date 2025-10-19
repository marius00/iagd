using EvilsoftCommons.SingleInstance;
using IAGrim.Backup.Cloud;
using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Settings;
using IAGrim.Utilities;
using Microsoft.Web.WebView2.Core;
using System;


namespace IAGrim.UI.Popups {
    public partial class BackupLoginNagScreen : Form {
        private readonly AuthService _authAuthService;
        private readonly SettingsService _settingsService;
        private int _closeTimerCounter = 5;
        private System.Windows.Forms.Timer? _closeTimer = null;
        public BackupLoginNagScreen(AuthService authAuthService, SettingsService settingsService) {
            _authAuthService = authAuthService;
            _settingsService = settingsService;
            InitializeComponent();
        }

        private void BackupLoginNagScreen_Load(object sender, EventArgs e) {
            Guid guid = new Guid("{071E8B2B-169A-4BE2-9539-548A1F3F1F1B}");
            using (SingleInstance singleInstance = new SingleInstance(guid)) {
                if (!singleInstance.IsFirstInstance) {
                    this.Close();
                    return;
                }
            }

            var pollingId = _authAuthService.Authenticate(true);
            _authAuthService.OnAuthCompletion += _authAuthService_OnAuthCompletion;

            var conf = CoreWebView2Environment.CreateAsync(null, GlobalPaths.EdgeCacheLocation).Result;
            webView21.EnsureCoreWebView2Async(conf);

            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            webView21.Source = new Uri(Uris.LoginPageUrl + $"?token={pollingId}&embedded=1");
            FormClosing += BackupLoginNagScreen_FormClosing;
        }

        private void _authAuthService_OnAuthCompletion(object? sender, EventArgs e) {
            if (this.InvokeRequired) {
                Invoke((System.Windows.Forms.MethodInvoker)delegate { _authAuthService_OnAuthCompletion(sender, e); });
                return;
            }
            var args = e as AuthResultEvent;
            if (args.IsAuthorized) {
                linkLabel1.Hide();
                linkLabel2.Hide();
                buttonClose.Show();

                _closeTimer = new System.Windows.Forms.Timer();
                _closeTimer.Tick += Timer_Tick; ;
                _closeTimer.Interval = 1000;
                _closeTimer.Start();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e) {
            buttonClose.Text = "Closing (" + _closeTimerCounter + ")";
            _closeTimerCounter--;

            if (_closeTimerCounter <= 0) {
                _closeTimer.Stop();
                this.Close();
            }
        }

        private void WebView21_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e) {
            if (!e.IsSuccess) {
                this.Close();
            }
        }

        private void BackupLoginNagScreen_FormClosing(object? sender, FormClosingEventArgs e) {
            _authAuthService.Dispose();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            this.Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _settingsService.GetLocal().OptOutOfBackups = true;
            this.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
