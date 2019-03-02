using EvilsoftCommons.Cloud;
using IAGrim.Backup.Azure.Service;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using IAGrim.Utilities.Cloud;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IAGrim.UI
{
    public partial class BackupSettings : Form
    {
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AzureAuthService _authAuthService;


        public BackupSettings(IPlayerItemDao playerItemDao, AzureAuthService authAuthService)
        {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _authAuthService = authAuthService;
        }

        private void BackupSettings_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;

            var error = new Bitmap(Properties.Resources.error);
            var provider = new CloudWatcher();

            cbDropbox.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.DROPBOX);
            cbGoogle.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.GOOGLE_DRIVE);
            cbSkydrive.Enabled = provider.Providers.Any(m => m.Provider == CloudProviderEnum.ONEDRIVE);

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.DROPBOX))
            {
                pbDropbox.Image = error;
            }

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.GOOGLE_DRIVE))
            {
                pbGoogle.Image = error;
            }

            if (provider.Providers.All(m => m.Provider != CloudProviderEnum.ONEDRIVE))
            {
                pbSkydrive.Image = error;
            }

            pbDropbox.Enabled = cbDropbox.Enabled;
            pbGoogle.Enabled = cbGoogle.Enabled;
            pbSkydrive.Enabled = cbSkydrive.Enabled;

            cbDropbox.Checked = Properties.Settings.Default.BackupDropbox;
            cbGoogle.Checked = Properties.Settings.Default.BackupGoogle;
            cbSkydrive.Checked = Properties.Settings.Default.BackupOnedrive;
            cbCustom.Checked = Properties.Settings.Default.BackupCustom;
            cbDontWantBackups.Checked = Properties.Settings.Default.OptOutOfBackups;
            buttonLogin.Enabled = !Properties.Settings.Default.OptOutOfBackups;

            cbDropbox.CheckedChanged += cbDropbox_CheckedChanged;
            cbGoogle.CheckedChanged += cbGoogle_CheckedChanged;
            cbSkydrive.CheckedChanged += cbSkydrive_CheckedChanged;
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
            if (cbSkydrive.Enabled)
            {
                cbSkydrive.Checked = !cbSkydrive.Checked;
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

            Properties.Settings.Default.BackupGoogle = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbDropbox_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;

            Properties.Settings.Default.BackupDropbox = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbSkydrive_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;

            Properties.Settings.Default.BackupOnedrive = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void cbCustom_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as FirefoxCheckBox;

            Properties.Settings.Default.BackupCustom = cb.Checked;
            Properties.Settings.Default.Save();
        }

        private void buttonCustom_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                Properties.Settings.Default.BackupCustomLocation = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void buttonBackupNow_Click(object sender, EventArgs e)
        {
            var fileBackup = new FileBackup(_playerItemDao);

            if (fileBackup.Backup(true))
            {
                MessageBox.Show(
                    GlobalSettings.Language.GetTag("iatag_ui_backup_complete"), 
                    GlobalSettings.Language.GetTag("iatag_ui_backup_status"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    GlobalSettings.Language.GetTag("iatag_ui_backup_failed"),
                    GlobalSettings.Language.GetTag("iatag_ui_backup_status"), 
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
                        MessageBox.Show(GlobalSettings.Language.GetTag("iatag_ui_backup_service_error"));
                        break;
                    default: {
                        var alreadyLoggedIn = GlobalSettings.Language.GetTag("iatag_feedback_already_logged_in");

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
            Properties.Settings.Default.OptOutOfBackups = cbDontWantBackups.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
