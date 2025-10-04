using IAGrim.Backup.Cloud;
using IAGrim.Backup.Cloud.CefSharp.Events;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Settings;


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
            var pollingId = _authAuthService.Authenticate(true);
            _authAuthService.OnAuthCompletion += _authAuthService_OnAuthCompletion;
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
            buttonClose.Text = "Closing (" + _closeTimer + ")";
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
