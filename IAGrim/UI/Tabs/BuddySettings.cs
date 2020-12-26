using IAGrim.Database;
using IAGrim.Database.DAO.Table;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using NHibernate.Criterion;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using EvilsoftCommons.Exceptions;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Settings.Dto;

namespace IAGrim.UI {
    public partial class BuddySettings : Form {
        private readonly IBuddyItemDao _buddyItemDao;
        private readonly IBuddySubscriptionDao _buddySubscriptionDao;
        private readonly Action<bool> _enableCallback;
        private readonly SettingsService _settingsService;
        private readonly IHelpService _helpService;

        private Timer _delayedTextChangedTimer;

        public long UID {
            set {
                var userId = RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_none");
                if (value > 0) {
                    userId = value.ToString();
                }

                useridLabel.Text = $"{RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid")}{userId}";
            }
        }

        public BuddySettings(Action<bool> enableCallback, IBuddyItemDao buddyItemDao, IBuddySubscriptionDao buddySubscriptionDao, SettingsService settingsService, IHelpService helpService) {
            InitializeComponent();
            _enableCallback = enableCallback;
            _buddyItemDao = buddyItemDao;
            _buddySubscriptionDao = buddySubscriptionDao;
            _settingsService = settingsService;
            _helpService = helpService;
        }

        private void BuddySettings_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;
            UpdateBuddyList();


            buddySyncEnabled.Checked = _settingsService.GetPersistent().BuddySyncEnabled;
            UpdateEnabledStatus();

            UID = _settingsService.GetPersistent().BuddySyncUserIdV2 ?? 0L;

            tbDescription.Text = _settingsService.GetPersistent().BuddySyncDescription;
            descriptionLabel.Text = $"Name: {tbDescription.Text}";

            buddyId.KeyPress += buddyId_KeyPress;
            tbDescription.KeyPress += tbDescription_KeyPress;
            tbDescription.LostFocus += tbDescription_LostFocus;
            buddySyncEnabled.CheckedChanged += buddySyncEnabled_CheckedChanged;

            // Allows 2nd column to auto-size to the width of the column heading
            // Source: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.columnheader.width
            buddyList.Columns[1].Width = -2;
        }

        void tbDescription_LostFocus(object sender, EventArgs e) {
            tbDescription.Visible = false;
            descriptionLabel.Visible = true;
        }

        void tbDescription_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13) {
                tbDescription.Visible = false;
                descriptionLabel.Visible = true;
            }
        }

        void buddyId_KeyPress(object sender, KeyPressEventArgs e) {
            //if (char.IsLetter(e.KeyChar) || char.IsSymbol(e.KeyChar))
            if (char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)) {
                e.Handled = false;
                errorProvider1.Clear();
            }
            else {
                e.Handled = true;

                errorProvider1.SetError(buddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_numeric_error_message"));
                errorProvider1.SetIconAlignment(buddyId, ErrorIconAlignment.MiddleLeft);
            }


            if (e.KeyChar == 13) {
                firefoxButton2_Click(null, null);
            }
        }

        /// <summary>
        /// Delete all items and subscriptions for a buddy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (ListViewItem item in buddyList.SelectedItems) {
                if (item != null && long.TryParse(item.Tag.ToString(), out var id)) {
                    _buddyItemDao.RemoveBuddy(id);
                    UpdateBuddyList();
                }
            }
        }

        /// <summary>
        /// Update the list of buddies
        /// </summary>
        public void UpdateBuddyList() {
            buddyList.Items.Clear();

            // TODO: Wrap in a DAO
            var subscriptions = _buddySubscriptionDao.ListAll();
            foreach (var subscription in subscriptions) {
                var label = subscription.Id.ToString();
                var stash = _buddyItemDao.GetBySubscriptionId(subscription.Id);

                if (stash != null) {
                    label = $"[{label}] {stash.Description}";
                }

                var numItems = _buddyItemDao.GetNumItems(subscription.Id);

                var lvi = new ListViewItem(label);
                lvi.SubItems.Add(numItems.ToString());
                lvi.Tag = subscription.Id;
                buddyList.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Add another buddy subscription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void firefoxButton2_Click(object sender, EventArgs e) {
            if (long.TryParse(buddyId.Text, out var id)) {
                if (id > 0 && id != _settingsService.GetPersistent().BuddySyncUserIdV2) {
                    _buddySubscriptionDao.SaveOrUpdate(new BuddySubscription {Id = id});
                }

                UpdateBuddyList();
                _enableCallback(true);
            }
            else {
                errorProvider1.SetError(buddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_numeric_error_message"));
                errorProvider1.SetIconAlignment(buddyId, ErrorIconAlignment.MiddleLeft);
            }

            buddyId.Text = "";
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            e.Cancel = false;
        }

        /// <summary>
        /// Enable / disable buddy sync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buddySyncEnabled_CheckedChanged(object sender, EventArgs e) {
            _delayedTextChangedTimer?.Stop();

            _delayedTextChangedTimer = new Timer();
            _delayedTextChangedTimer.Tick += new EventHandler(DelayedActivate);
            _delayedTextChangedTimer.Interval = 1200;
            _delayedTextChangedTimer.Start();

            UpdateEnabledStatus();
        }

        private void UpdateEnabledStatus() {
            buttonSyncNow.Enabled = buddySyncEnabled.Checked;
            buddyId.Enabled = buddySyncEnabled.Checked;
            firefoxButton2.Enabled = buddySyncEnabled.Checked;
            buddyList.Enabled = buddySyncEnabled.Checked;
            descriptionLabel.Enabled = buddySyncEnabled.Checked;
            tbDescription.Enabled = buddySyncEnabled.Checked;
            contextMenuStripDescription.Enabled = buddySyncEnabled.Checked;
            useridLabel.Enabled = buddySyncEnabled.Checked;
        }

        private void DelayedActivate(object sender, EventArgs e) {
            ExceptionReporter.EnableLogUnhandledOnThread();
            if (_delayedTextChangedTimer != null) {
                _delayedTextChangedTimer.Stop();
                _delayedTextChangedTimer = null;
            }

            _settingsService.GetPersistent().BuddySyncEnabled = buddySyncEnabled.Checked;
            _enableCallback(buddySyncEnabled.Checked);
        }


        private void buttonSyncNow_Click(object sender, EventArgs e) {
            if (buddySyncEnabled.Checked) {
                _enableCallback(true);
            }
        }

        private void tbDescription_TextChanged(object sender, EventArgs e) {
            var tb = sender as TextBox;

            _settingsService.GetPersistent().BuddySyncDescription = tb.Text;
            descriptionLabel.Text = $"{RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_name")}{tb.Text}";
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            tbDescription.Visible = true;
            descriptionLabel.Visible = false;
            tbDescription.Focus();
        }

        private void contextMenuStripDescription_Opening(object sender, CancelEventArgs e) {
            e.Cancel = !buddySyncEnabled.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            editToolStripMenuItem_Click(sender, e);
        }

        private void helpWhatIsThis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.BuddyItems);
        }
    }
}