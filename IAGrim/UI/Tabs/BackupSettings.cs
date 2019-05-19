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

namespace IAGrim.UI.Tabs
{
    public partial class BackupSettings : Form
    {
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AzureAuthService _authAuthService;
        private readonly SettingsService _settings;


        public BackupSettings(IPlayerItemDao playerItemDao, AzureAuthService authAuthService, SettingsService settings)
        {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _authAuthService = authAuthService;
            _settings = settings;
        }

        private void BackupSettings_Load(object sender, EventArgs e)
        {
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
            


            cbDropbox.Checked = _settings.GetBool(LocalSetting.BackupDropbox);
            cbGoogle.Checked = _settings.GetBool(LocalSetting.BackupGoogle);
            cbOneDrive.Checked = _settings.GetBool(LocalSetting.BackupOnedrive);
            cbCustom.Checked = _settings.GetBool(LocalSetting.BackupCustom);
            cbDontWantBackups.Checked = _settings.GetBool(LocalSetting.OptOutOfBackups);
            buttonLogin.Enabled = !_settings.GetBool(LocalSetting.OptOutOfBackups);

            cbDropbox.CheckedChanged += cbDropbox_CheckedChanged;
            cbGoogle.CheckedChanged += cbGoogle_CheckedChanged;
            cbOneDrive.CheckedChanged += CbOneDriveCheckedChanged;
            cbCustom.CheckedChanged += cbCustom_CheckedChanged;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (cbGoogle.Enabled)
            {
                cbGoogle.Checked = !cbGoogle.Checked;
            }
        }

        private void pbSkydrive_Click(object sender, EventArgs e)
        {
            if (cbOneDrive.Enabled)
            {
                cbOneDrive.Checked = !cbOneDrive.Checked;
            }
        }

        private void pbDropbox_Click(object sender, EventArgs e)
        {
            if (cbDropbox.Enabled)
            {
                cbDropbox.Checked = !cbDropbox.Checked;
            }
        }

        private void cbGoogle_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;
            _settings.Save(LocalSetting.BackupGoogle, cb.Checked);
        }

        private void cbDropbox_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;
            _settings.Save(LocalSetting.BackupDropbox, cb.Checked);
        }

        private void CbOneDriveCheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;
            _settings.Save(LocalSetting.BackupOnedrive, cb.Checked);
        }

        private void cbCustom_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;
            _settings.Save(LocalSetting.BackupCustom, cb.Checked);
        }

        private void buttonCustom_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                _settings.Save(LocalSetting.BackupCustomLocation, folderBrowserDialog.SelectedPath);
            }
        }

        private void buttonBackupNow_Click(object sender, EventArgs e)
        {
            var fileBackup = new FileBackup(_playerItemDao, _settings);

            if (fileBackup.Backup(true))
            {
                MessageBox.Show(
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_complete"), 
                    RuntimeSettings.Language.GetTag("iatag_ui_backup_status"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            else
            {
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
        }

        private void cbDontWantBackups_CheckedChanged(object sender, EventArgs e) {
            buttonLogin.Enabled = !cbDontWantBackups.Checked;
            _settings.Save(LocalSetting.OptOutOfBackups, cbDontWantBackups.Checked);
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
    }
}
