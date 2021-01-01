using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EvilsoftCommons.Cloud;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using IAGrim.Utilities.Detection;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.UI.Tabs {
    public partial class BackupSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AuthService _authAuthService;
        private readonly SettingsService _settings;
        private readonly CloudSyncService _cloudSyncService;
        private readonly IHelpService _helpService;

        public BackupSettings(IPlayerItemDao playerItemDao, AuthService authAuthService, SettingsService settings, IHelpService helpService) {
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

            var error = new Bitmap(Properties.Resources.error);
            var provider = new CloudWatcher();

            cbDropbox.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX);
            cbGoogle.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE);
            cbOneDrive.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE);
            helpWhyDropboxDisabled.Visible = !cbDropbox.Enabled;
            helpWhyGdriveDisabled.Visible = !cbGoogle.Enabled;
            helpWhyOnedriveDisabled.Visible = !cbOneDrive.Enabled;

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.DROPBOX)) {
                pbDropbox.Image = error;
            }

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.GOOGLE_DRIVE)) {
                pbGoogle.Image = error;
            }

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.ONEDRIVE)) {
                pbSkydrive.Image = error;
            }

            pbDropbox.Enabled = cbDropbox.Enabled;
            pbGoogle.Enabled = cbGoogle.Enabled;
            pbSkydrive.Enabled = cbOneDrive.Enabled;


            cbDropbox.Checked = _settings.GetLocal().BackupDropbox;
            cbGoogle.Checked = _settings.GetLocal().BackupGoogle;
            cbOneDrive.Checked = _settings.GetLocal().BackupOnedrive;
            cbCustom.Checked = _settings.GetLocal().BackupCustom;
            lbOpenCustomBackupFolder.Visible = cbCustom.Checked;

            cbDontWantBackups.Checked = _settings.GetLocal().OptOutOfBackups;
            buttonLogin.Enabled = !_settings.GetLocal().OptOutOfBackups;

            cbDropbox.CheckedChanged += cbDropbox_CheckedChanged;
            cbGoogle.CheckedChanged += cbGoogle_CheckedChanged;
            cbOneDrive.CheckedChanged += CbOneDriveCheckedChanged;
            cbCustom.CheckedChanged += cbCustom_CheckedChanged;

            UpdateUi();
        }

        private void pictureBox2_Click(object sender, EventArgs e) {
            if (cbGoogle.Enabled) {
                cbGoogle.Checked = !cbGoogle.Checked;
            }
        }

        private void pbSkydrive_Click(object sender, EventArgs e) {
            if (cbOneDrive.Enabled) {
                cbOneDrive.Checked = !cbOneDrive.Checked;
            }
        }

        private void pbDropbox_Click(object sender, EventArgs e) {
            if (cbDropbox.Enabled) {
                cbDropbox.Checked = !cbDropbox.Checked;
            }
        }

        private void cbGoogle_CheckedChanged(object sender, EventArgs e) {
            var cb = sender as FirefoxCheckBox;
            _settings.GetLocal().BackupGoogle = cb.Checked;
        }

        private void cbDropbox_CheckedChanged(object sender, EventArgs e) {
            var cb = sender as FirefoxCheckBox;
            _settings.GetLocal().BackupDropbox = cb.Checked;
        }

        private void CbOneDriveCheckedChanged(object sender, EventArgs e) {
            var cb = sender as FirefoxCheckBox;
            _settings.GetLocal().BackupOnedrive = cb.Checked;
        }

        private void cbCustom_CheckedChanged(object sender, EventArgs e) {
            var cb = sender as FirefoxCheckBox;
            _settings.GetLocal().BackupCustom = cb.Checked;
            lbOpenCustomBackupFolder.Visible = cb.Checked;
        }

        private void buttonCustom_Click(object sender, EventArgs e) {
            using (var folderBrowserDialog = new FolderBrowserDialog()) {

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                    DialogResult = DialogResult.None;
                    _settings.GetLocal().BackupCustomLocation = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void buttonBackupNow_Click(object sender, EventArgs e) {
            var fileBackup = new FileBackup(_playerItemDao, _settings);

            if (fileBackup.Backup(true)) {
                MessageBox.Show(
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_complete"),
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_status"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show(
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_failed"),
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_status"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

        private void helpWhyGdriveDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyDropboxDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyOnedriveDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
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

        private void lbOpenCustomBackupFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(_settings.GetLocal().BackupCustomLocation); // TODO: Should open the \evilsoft\iagd folder.. but not this class` responsibility to know that..
        }
    }
}