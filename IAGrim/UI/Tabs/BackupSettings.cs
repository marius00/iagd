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

namespace IAGrim.UI {
    public partial class BackupSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly Action<bool> _enableOnlineBackups;
        private readonly IPlayerItemDao _playerItemDao;


        public BackupSettings(Action<bool> enableOnlineBackups, IPlayerItemDao playerItemDao) {
            InitializeComponent();
            this._enableOnlineBackups = enableOnlineBackups;
            this._playerItemDao = playerItemDao;
        }

        private bool _onlineBackupsActive;
        private bool OnlineBackupsActive {
            get {
                return _onlineBackupsActive;
            }
            set {
                _onlineBackupsActive = value;
                logoutThisComputer.Visible = value;
                logoutAllComputers.Visible = value;
                buttonLogin.Visible = !value;
                if (!value) {
                    labelItemSyncFeedback.Text = GlobalSettings.Language.GetTag("iatag_ui_online_backups_disabled");
                } else {
                    BackupSettings_GotFocus();
                    backupEmailLabel.Text = GlobalSettings.Language.GetTag("iatag_ui_online_backup_email")
                        .Replace("{OnlineBackupEmail}", Properties.Settings.Default.OnlineBackupEmail);
                }
            }
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


            var b = string.IsNullOrEmpty(Properties.Settings.Default.OnlineBackupToken) || !Properties.Settings.Default.OnlineBackupVerified;
            buttonLogin.Visible = b;
            logoutThisComputer.Visible = !b;
            logoutAllComputers.Visible = !b;
            OnlineBackupsActive = !b;

            UpdateInformativeLabel(0, 0, 0);
        }

        public void BackupSettings_GotFocus() {
            var numItems = _playerItemDao.GetNumItems();
            var numUnsynchronizedItems = _playerItemDao.GetNumUnsynchronizedItems();
            var numSynchronized = numItems - numUnsynchronizedItems;

            UpdateInformativeLabel(numItems, numSynchronized, numUnsynchronizedItems);
        }

        private void UpdateInformativeLabel(long numItems, long numSynchronized, long numUnsynchronizedItems) {
            if (OnlineBackupsActive) {
                if (numSynchronized == numItems) {
                    labelItemSyncFeedback.Text = GlobalSettings.Language.GetTag("iatag_ui_all_items_backed_up");
                }
                else {
                    var tag = GlobalSettings.Language.GetTag("iatag_ui_x_items_backed_up")
                        .Replace("{numSynchronized}", numSynchronized.ToString())
                        .Replace("{numItems}", numItems.ToString())
                        .Replace("{numUnsynchronizedItems}", numUnsynchronizedItems.ToString());
                    labelItemSyncFeedback.Text = tag;
                }
            }
            else {
                labelItemSyncFeedback.Text = GlobalSettings.Language.GetTag("iatag_ui_online_backups_disabled");
            }
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
            CloudBackup b = new CloudBackup(_playerItemDao);
            if (b.Backup(true)) {
                MessageBox.Show("Backup complete!", "Backup status", MessageBoxButtons.OK, MessageBoxIcon.Information); //  TODO:TRANSLATE
            }
            else {
                MessageBox.Show("Backup failed, see the log file for more detailed information.", "Backup status", MessageBoxButtons.OK, MessageBoxIcon.Error); //  TODO:TRANSLATE
            }
        }

        private void firefoxButton1_Click(object sender, EventArgs e) {
            new UI.Popups.OnlineBackupLogin(_playerItemDao).ShowDialog();


            var loginFailed = string.IsNullOrEmpty(Properties.Settings.Default.OnlineBackupToken) || !Properties.Settings.Default.OnlineBackupVerified;
            OnlineBackupsActive = !loginFailed;
            if (!loginFailed) {
                _enableOnlineBackups(true);
                backupEmailLabel.Text = GlobalSettings.Language.GetTag("iatag_ui_online_backup_email")
                    .Replace("{OnlineBackupEmail}", Properties.Settings.Default.OnlineBackupEmail);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var s = new ItemSynchronizer(_playerItemDao, Properties.Settings.Default.OnlineBackupToken, GlobalSettings.RemoteBackupServer, null);
            s.Logout();
            backupEmailLabel.Text = "";
            Properties.Settings.Default.LastOnlineBackup = 0;
            Properties.Settings.Default.OnlineBackupVerified = false;
            Properties.Settings.Default.OnlineBackupToken = string.Empty;
            Properties.Settings.Default.Save();
            _enableOnlineBackups(false);
            OnlineBackupsActive = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var s = new ItemSynchronizer(_playerItemDao, Properties.Settings.Default.OnlineBackupToken, GlobalSettings.RemoteBackupServer, null);
            s.LogoutAll();
            backupEmailLabel.Text = "";

            Properties.Settings.Default.LastOnlineBackup = 0;
            Properties.Settings.Default.OnlineBackupVerified = false;
            Properties.Settings.Default.OnlineBackupToken = string.Empty;
            Properties.Settings.Default.Save();
            _enableOnlineBackups(false);
            OnlineBackupsActive = false;
        }
    }
}
