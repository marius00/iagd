using System;
using System.Diagnostics;
using System.Windows.Forms;
using EvilsoftCommons;
using IAGrim.Backup.Cloud;
using IAGrim.Backup.Cloud.Service;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.UI.Popups;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.UI.Tabs {
    public partial class OnlineSettings : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BackupSettings));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly AuthService _authAuthService;
        private readonly SettingsService _settings;
        private readonly IHelpService _helpService;
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly IBuddySubscriptionDao _buddySubscriptionDao;
        private TooltipHelper _tooltipHelper;

        public OnlineSettings(IPlayerItemDao playerItemDao, AuthService authAuthService, SettingsService settings, IHelpService helpService, IBuddyItemDao buddyItemDao, IBuddySubscriptionDao buddySubscriptionDao) {
            InitializeComponent();
            _playerItemDao = playerItemDao;
            _settings = settings;
            _helpService = helpService;
            _buddyItemDao = buddyItemDao;
            _buddySubscriptionDao = buddySubscriptionDao;

            _authAuthService = authAuthService;
            _tooltipHelper = new TooltipHelper();
        }

        private void UpdateUi() {
            var status = _authAuthService?.CheckAuthentication();
            if (status == AuthService.AccessStatus.Authorized) {
                labelStatus.Text = RuntimeSettings.Language.GetTag("iatag_ui_backup_loggedinas", _settings.GetPersistent().CloudUser);
                buttonLogin.Enabled = false;
                _settings.GetLocal().OptOutOfBackups = false;
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
            cbDontWantBackups.Visible = buttonLogin.Enabled; // Hide "I dont want backups" button if already logged in
            groupBoxBackupDetails.Visible = !cbDontWantBackups.Checked; // No point displaying info if user has opted for zero features
            pbBuddyItems.Visible = !cbDontWantBackups.Checked;
            btnAddBuddy.Enabled = !buttonLogin.Enabled;
            buddyList.Enabled = !buttonLogin.Enabled;
            linkViewOnline.Enabled = !buttonLogin.Enabled;
            if (buddyList.Enabled) UpdateBuddyList();


            var buddyId = _settings.GetPersistent().BuddySyncUserIdV3;
            if (buddyId.HasValue && buddyId > 0)
                labelBuddyId.Text = buddyId.ToString();
            else
                labelBuddyId.Text = "-";
        }


        private void BackupSettings_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;
            cbDontWantBackups.Checked = _settings.GetLocal().OptOutOfBackups;
            buttonLogin.Enabled = !_settings.GetLocal().OptOutOfBackups;


            // Allows 2nd column to auto-size to the width of the column heading
            // Source: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.columnheader.width
            buddyList.Columns[1].Width = -2;


            UpdateUi();
            this.FormClosing += (_, __) => {
                _tooltipHelper?.Dispose();
                _tooltipHelper = null;
            };
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
                    var restService = _authAuthService.GetRestService();
                    var cloudSyncService = new CloudSyncService(_authAuthService.GetRestService());
                    if (restService != null && cloudSyncService.DeleteAccount()) {
                        MessageBox.Show(
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_body"),
                            RuntimeSettings.Language.GetTag("iatag_ui_backup_deleteaccount_success_header"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                        );

                        _settings.GetPersistent().CloudUploadTimestamp = 0;
                        _authAuthService.UnAuthenticate();
                        _playerItemDao.ResetOnlineSyncState();
                        _settings.GetPersistent().BuddySyncUserIdV3 = null;
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
            _settings.GetPersistent().BuddySyncUserIdV3 = null;
            MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_successful_body"), RuntimeSettings.Language.GetTag("iatag_ui_backup_logout_successful_header"));
            UpdateUi();
        }

        private void btnRefreshBackupDetails_Click(object sender, EventArgs e) {
            UpdateUi();
        }


        /// <summary>
        /// Update the list of buddies
        /// </summary>
        public void UpdateBuddyList() {
            buddyList.Items.Clear();

            var subscriptions = _buddySubscriptionDao.ListAll();
            foreach (var subscription in subscriptions) {
                var label = subscription.Id.ToString();
                var stash = subscription.Nickname;

                if (stash != null) {
                    label = $"[{label}] {stash}";
                }

                var numItems = _buddyItemDao.GetNumItems(subscription.Id);

                var lvi = new ListViewItem(label);
                lvi.SubItems.Add(numItems.ToString());
                lvi.Tag = subscription.Id;
                buddyList.Items.Add(lvi);
            }
        }

        private void helpWhatIsThis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BuddyItems);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.OnlineBackups);
        }

        private void btnAddBuddy_Click(object sender, EventArgs e) {
            var diag = new AddEditBuddy(_helpService, _authAuthService.GetRestService());
            if (diag.ShowDialog() == DialogResult.OK) {
                bool isMyself = diag.BuddyId == _settings.GetPersistent().BuddySyncUserIdV3;
                if (diag.BuddyId > 0 && !isMyself) {
                    _buddySubscriptionDao.SaveOrUpdate(new BuddySubscription {Id = diag.BuddyId, Nickname = diag.Nickname});
                }

                UpdateBuddyList();
            }
        }

        private void buddyItemListContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in buddyList.SelectedItems) {
                if (item != null && long.TryParse(item.Tag.ToString(), out var id)) {
                    _buddyItemDao.RemoveBuddy(id);
                    UpdateBuddyList();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in buddyList.SelectedItems) {
                if (item != null && long.TryParse(item.Tag.ToString(), out var id)) {
                    var diag = new AddEditBuddy(_helpService, _authAuthService.GetRestService()) {BuddyId = id};
                    if (diag.ShowDialog() == DialogResult.OK) {
                        var entry = _buddySubscriptionDao.GetById(diag.BuddyId);
                        entry.Nickname = diag.Nickname;
                        _buddySubscriptionDao.Update(entry);
                    }

                    UpdateBuddyList();
                }
            }
        }

        private void linkViewOnline_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (_settings.GetPersistent().BuddySyncUserIdV3.HasValue && _settings.GetPersistent().BuddySyncUserIdV3 > 0) {
                Process.Start(Uris.OnlineItemsUrl + "?id=" + _settings.GetPersistent().BuddySyncUserIdV3);
            }
            else {
                MessageBox.Show("Unavailable - Not logged in");
            }
        }

        private void labelBuddyId_Click(object sender, EventArgs e) {
            if (_settings.GetPersistent().BuddySyncUserIdV3.HasValue && _settings.GetPersistent().BuddySyncUserIdV3 > 0) {
                _tooltipHelper.ShowTooltipForControl(RuntimeSettings.Language.GetTag("iatag_ui_copiedclipboard"), labelBuddyId);
                Clipboard.SetText(_settings.GetPersistent().BuddySyncUserIdV3.ToString());
            }
        }

        private void btnModifyBuddy_Click(object sender, EventArgs e) {
            editToolStripMenuItem_Click(sender, e);
        }

        private void btnDeleteBuddy_Click(object sender, EventArgs e) {
            deleteToolStripMenuItem_Click(sender, e);
        }
    }
}