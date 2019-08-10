using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EvilsoftCommons.Cloud;
using IAGrim.Backup.Azure.Service;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.UI.Tabs {
    public partial class BackupSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AzureAuthService _authAuthService;
        private readonly SettingsService _settings;
        private readonly AzureSyncService _azureSyncService;

        public BackupSettings(IPlayerItemDao playerItemDao, AzureAuthService authAuthService, SettingsService settings) {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _authAuthService = authAuthService;
            _settings = settings;
            _azureSyncService = new AzureSyncService(authAuthService.GetRestService());
        }

        private void UpdateUi() {
            if (_authAuthService.CheckAuthentication() == AzureAuthService.AccessStatus.Authorized) {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_loggedinas", "emailhere");
                buttonLogin.Enabled = false;
            }
            else {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_notloggedin");
                buttonLogin.Enabled = true;
            }

            linkLogout.Enabled = !buttonLogin.Enabled;
            linkDeleteBackup.Enabled = !buttonLogin.Enabled;
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
        }

        private void buttonCustom_Click(object sender, EventArgs e) {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                DialogResult = DialogResult.None;
                _settings.GetLocal().BackupCustomLocation = folderBrowserDialog.SelectedPath;
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
                    case AzureAuthService.AccessStatus.Unauthorized:
                        _authAuthService.Authenticate();
                        break;
                    case AzureAuthService.AccessStatus.Unknown:
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
            HelpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyDropboxDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyOnedriveDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
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
                    if (_azureSyncService.DeleteAccount()) {
                        MessageBox.Show(
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_body"),
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_header"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );

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
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Logger.Info("Logging out of online backups.");
            _authAuthService.UnAuthenticate();
            MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_sucessful_body"), RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_sucessful_header"));
            UpdateUi();
        }

        private void btnRefreshBackupDetails_Click(object sender, EventArgs e) {
            UpdateUi();
        }
    }
}