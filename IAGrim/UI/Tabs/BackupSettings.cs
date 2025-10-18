using System.Diagnostics;
using EvilsoftCommons.Cloud;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;

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


            cbCustom.Checked = _settings.GetLocal().BackupCustom;
            lbOpenCustomBackupFolder.Visible = cbCustom.Checked && IsCustomLocationValid();

            cbCustom.CheckedChanged += cbCustom_CheckedChanged;
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