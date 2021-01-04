using System;

using System.Windows.Forms;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Utilities;
using IAGrim.Utilities.Detection;
using log4net;

namespace IAGrim.UI.Tabs {
    public partial class OnlineSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AuthService _authAuthService;
        private readonly SettingsService _settings;
        private readonly CloudSyncService _cloudSyncService;
        private readonly IHelpService _helpService;

        public OnlineSettings(IPlayerItemDao playerItemDao, AuthService authAuthService, SettingsService settings, IHelpService helpService) {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _settings = settings;
            _helpService = helpService;
            
            _authAuthService = authAuthService;
            _cloudSyncService = new CloudSyncService(authAuthService.GetRestService());

        }

        private void UpdateUi() {
            var status = _authAuthService?.CheckAuthentication();
            if (status == AuthService.AccessStatus.Authorized) {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_loggedinas", _settings.GetPersistent().CloudUser);
                buttonLogin.Enabled = false;
            }
            else if (status == AuthService.AccessStatus.Unknown) {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_statusunknown");
                buttonLogin.Enabled = false;
            }
            else {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_notloggedin");
                buttonLogin.Enabled = true;
            }

            linkLogout.Enabled = !buttonLogin.Enabled;
            linkDeleteBackup.Enabled = !buttonLogin.Enabled;
            buttonLogin.Visible = !BlockedLogsDetection.DreamcrashBlocked();
        }


        private void BackupSettings_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;
            cbDontWantBackups.Checked = _settings.GetLocal().OptOutOfBackups;
            buttonLogin.Enabled = !_settings.GetLocal().OptOutOfBackups;
            UpdateUi();
        }
        private void firefoxButton1_Click(object sender, EventArgs e) {
            if ((sender as FirefoxButton).EnabledCalc) {
                var access = _authAuthService.CheckAuthentication();

                switch (access) {
                    case AuthService.AccessStatus.Unauthorized:
                        _authAuthService.Authenticate();
                        break;
                    case AuthService.AccessStatus.Unknown:
                        MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_backup_service_error"));
                        break;
                    default: {
                        var alreadyLoggedIn = RuntimeSettings.Language.GetTag("iatag_feedback_already_logged_in");

                        MessageBox.Show(
                            alreadyLoggedIn,
                            alreadyLoggedIn,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        break;
                    }
                }
            }

            UpdateUi();
        }

        private void cbDontWantBackups_CheckedChanged(object sender, EventArgs e) {
            buttonLogin.Enabled = !cbDontWantBackups.Checked;
            _settings.GetLocal().OptOutOfBackups = cbDontWantBackups.Checked;
        }


        private void linkDeleteBackup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var caption = RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_header");
            var content = RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_body");
            if (MessageBox.Show(
                    content,
                    caption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
                try {
                    if (_cloudSyncService != null && _cloudSyncService.DeleteAccount()) {
                        MessageBox.Show(
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_body"),
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_header"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );
                        
                        _settings.GetPersistent().CloudUploadTimestamp = 0;
                        _authAuthService.UnAuthenticate();
                        _playerItemDao.ResetOnlineSyncState();
                    }
                    else {
                        MessageBox.Show(
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_failure_body"),
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_failure_header"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                catch (Exception ex) {
                    Logger.Warn("Error deleting account", ex);
                    MessageBox.Show(
                        RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_failure_body"),
                        RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_failure_header"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            
            UpdateUi();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Logger.Info("Logging out of online backups.");
            _authAuthService.Logout();
            _settings.GetPersistent().CloudUploadTimestamp = 0;
            _playerItemDao.ResetOnlineSyncState();
            MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_successful_body"), RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_successful_header"));
            UpdateUi();
        }

        private void btnRefreshBackupDetails_Click(object sender, EventArgs e) {
            UpdateUi();
        }

    }
}