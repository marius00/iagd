using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        private readonly IPlayerItemDao _playerItemDao;
        private readonly SettingsService _settings;
        private readonly IHelpService _helpService;

        public BackupSettings(IPlayerItemDao playerItemDao, SettingsService settings, IHelpService helpService) {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _settings = settings;
            _helpService = helpService;
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
            lbOpenCustomBackupFolder.Visible = cbCustom.Checked && IsCustomLocationValid();

            cbDropbox.CheckedChanged += cbDropbox_CheckedChanged;
            cbGoogle.CheckedChanged += cbGoogle_CheckedChanged;
            cbOneDrive.CheckedChanged += CbOneDriveCheckedChanged;
            cbCustom.CheckedChanged += cbCustom_CheckedChanged;
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
            lbOpenCustomBackupFolder.Visible = cbCustom.Checked && IsCustomLocationValid();

        }

        private void buttonCustom_Click(object sender, EventArgs e) {
            using (var folderBrowserDialog = new FolderBrowserDialog()) {

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                    DialogResult = DialogResult.None;
                    _settings.GetLocal().BackupCustomLocation = folderBrowserDialog.SelectedPath;
                    lbOpenCustomBackupFolder.Visible = cbCustom.Checked && IsCustomLocationValid();
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

        private void helpWhyGdriveDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyDropboxDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private void helpWhyOnedriveDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BackupAutodetectDisabled);
        }

        private bool IsCustomLocationValid() {
            return _settings.GetLocal().BackupCustomLocation != null && Directory.Exists(_settings.GetLocal().BackupCustomLocation);
        }
        private void lbOpenCustomBackupFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (!IsCustomLocationValid()) {
                MessageBox.Show("bla bla folder dont exist"); // TODO: Localize etc
            }
            else {
                Process.Start(_settings.GetLocal().BackupCustomLocation);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.RestoreBackup);
        }
    }
}