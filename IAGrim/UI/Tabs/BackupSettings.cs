using EvilsoftCommons.Cloud;
using EvilsoftCommons.UI;
using IAGrim.Backup;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Misc;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IAGrim.Backup.Azure.Service;

namespace IAGrim.UI {
    public partial class BackupSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AzureAuthService _authAuthService;


        public BackupSettings(IPlayerItemDao playerItemDao, AzureAuthService authAuthService) {
            InitializeComponent();
            this._playerItemDao = playerItemDao;
            _authAuthService = authAuthService;
        }



        private void BackupSettings_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            Bitmap error = new Bitmap(IAGrim.Properties.Resources.error);

            CloudWatcher provider = new CloudWatcher();
            cbDropbox.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX);
            cbGoogle.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE);
            cbSkydrive.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE);

            if (!provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX))
                pbDropbox.Image = error;

            if (!provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE))
                pbGoogle.Image = error;

            if (!provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE))
                pbSkydrive.Image = error;

            pbDropbox.Enabled = cbDropbox.Enabled;
            pbGoogle.Enabled = cbGoogle.Enabled;
            pbSkydrive.Enabled = cbSkydrive.Enabled;


            cbDropbox.Checked = (bool)Properties.Settings.Default.BackupDropbox;
            cbGoogle.Checked = (bool)Properties.Settings.Default.BackupGoogle;
            cbSkydrive.Checked = (bool)Properties.Settings.Default.BackupOnedrive;
            cbCustom.Checked = (bool)Properties.Settings.Default.BackupCustom;

            cbDropbox.CheckedChanged += cbDropbox_CheckedChanged;
            cbGoogle.CheckedChanged += cbGoogle_CheckedChanged;
            cbSkydrive.CheckedChanged += cbSkydrive_CheckedChanged;
            cbCustom.CheckedChanged += cbCustom_CheckedChanged;
        }
        
        private void pictureBox2_Click(object sender, EventArgs e) {
            if (cbGoogle.Enabled)
                cbGoogle.Checked = !cbGoogle.Checked;
        }

        private void pbSkydrive_Click(object sender, EventArgs e) {
            if (cbSkydrive.Enabled)
                cbSkydrive.Checked = !cbSkydrive.Checked;
        }

        private void pbDropbox_Click(object sender, EventArgs e) {
            if (cbDropbox.Enabled)
                cbDropbox.Checked = !cbDropbox.Checked;
        }

        private void cbGoogle_CheckedChanged(object sender, EventArgs e) {
            FirefoxCheckBox cb = sender as FirefoxCheckBox;
            Properties.Settings.Default.BackupGoogle = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbDropbox_CheckedChanged(object sender, EventArgs e) {
            FirefoxCheckBox cb = sender as FirefoxCheckBox;
            Properties.Settings.Default.BackupDropbox = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbSkydrive_CheckedChanged(object sender, EventArgs e) {
            FirefoxCheckBox cb = sender as FirefoxCheckBox;
            Properties.Settings.Default.BackupOnedrive = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbCustom_CheckedChanged(object sender, EventArgs e) {
            FirefoxCheckBox cb = sender as FirefoxCheckBox;
            Properties.Settings.Default.BackupCustom = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void buttonCustom_Click(object sender, EventArgs e) {

            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK) {
                this.DialogResult = DialogResult.None;
                Properties.Settings.Default.BackupCustomLocation = diag.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void buttonBackupNow_Click(object sender, EventArgs e) {
            FileBackup b = new FileBackup(_playerItemDao);
            if (b.Backup(true)) {
                MessageBox.Show("Backup complete!", "Backup status", MessageBoxButtons.OK, MessageBoxIcon.Information); //  TODO:TRANSLATE
            }
            else {
                MessageBox.Show("Backup failed, see the log file for more detailed information.", "Backup status", MessageBoxButtons.OK, MessageBoxIcon.Error); //  TODO:TRANSLATE
            }
        }

        private void firefoxButton1_Click(object sender, EventArgs e) {

            if (!_authAuthService.IsAuthenticated()) {
                _authAuthService.Authenticate();
            }
            else {
                var alreadyLoggedIn = GlobalSettings.Language.GetTag("iatag_feedback_already_logged_in");
                MessageBox.Show(
                    alreadyLoggedIn,
                    alreadyLoggedIn,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
