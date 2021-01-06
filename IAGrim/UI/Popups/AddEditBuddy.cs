using System;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using IAGrim.Backup.Cloud;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;

namespace IAGrim.UI.Popups {
    public partial class AddEditBuddy : Form {
        private readonly IHelpService _helpService;
        private readonly RestService _restService;
        
        public long BuddyId {
            get {
                var t = tbBuddyId.Text;
                if (t.Length == 6) {
                    if (long.TryParse(t, out var r)) {
                        return r;
                    }
                }

                return -1;
            }
            set => tbBuddyId.Text = value.ToString();
        }

        public string Nickname => tbBuddyNickname.Text;

        public AddEditBuddy(IHelpService helpService, RestService restService) {
            _helpService = helpService;
            _restService = restService;
            InitializeComponent();
        }

        private void AddEditBuddy_Load(object sender, EventArgs e) {
            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);
            tbBuddyId.KeyPress += buddyId_KeyPress;
            tbBuddyNickname.KeyPress += nickname_KeyPress;

            if (tbBuddyId.Text.Length > 0) {
                tbBuddyId.Enabled = false;
                tbBuddyNickname.Focus();
            }
            else {
                tbBuddyId.Focus();
            }
        }

        private void lbHelpWhatisBuddyId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.WhatIsBuddyId);
        }

        private void lbHelpWhatisBuddyNickname_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.WhatIsBuddyNickname);
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            if (tbBuddyId.Text.Length != 6) {
                errorProvider1.SetError(tbBuddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_numeric_error_message"));
                errorProvider1.SetIconAlignment(tbBuddyId, ErrorIconAlignment.MiddleLeft);
            }

            else if (tbBuddyNickname.Text.Length <= 0) {
                errorProvider1.SetError(tbBuddyNickname, RuntimeSettings.Language.GetTag("iatag_ui_buddy_nickname_error_message"));
                errorProvider1.SetIconAlignment(tbBuddyNickname, ErrorIconAlignment.MiddleLeft);
            }

            else {
                if (Verify(tbBuddyId.Text)) {
                    DialogResult = DialogResult.OK;
                    Close();
                } else {
                    errorProvider1.SetError(tbBuddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_doesnotexist_error_message"));
                    errorProvider1.SetIconAlignment(tbBuddyId, ErrorIconAlignment.MiddleLeft);
                }
            }
        }

        void buddyId_KeyPress(object sender, KeyPressEventArgs e) {
            // Disallow more than 6 characters
            if (tbBuddyId.Text.Length >= 6 && !char.IsControl(e.KeyChar)) { // Non printable chars 0..31
                e.Handled = true;
                return;
            }

            // Allow numbers only
            if (char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar)) {
                e.Handled = false;
                errorProvider1.Clear();
            }
            else {
                e.Handled = true;
                errorProvider1.SetError(tbBuddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_numeric_error_message"));
                errorProvider1.SetIconAlignment(tbBuddyId, ErrorIconAlignment.MiddleLeft);
            }

            // Enter/Return key
            if (e.KeyChar == 13) {
                if (tbBuddyId.Text.Length != 6) {
                    errorProvider1.SetError(tbBuddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_numeric_error_message"));
                    errorProvider1.SetIconAlignment(tbBuddyId, ErrorIconAlignment.MiddleLeft);
                }
                else {
                    // Verify if ID exists
                    if (Verify(tbBuddyId.Text)) {
                        e.Handled = true;
                        tbBuddyNickname.Focus();
                    }
                    else {
                        errorProvider1.SetError(tbBuddyId, RuntimeSettings.Language.GetTag("iatag_ui_buddy_userid_doesnotexist_error_message"));
                        errorProvider1.SetIconAlignment(tbBuddyId, ErrorIconAlignment.MiddleLeft);
                    }
                }
            }
        }

        void nickname_KeyPress(object sender, KeyPressEventArgs e) {
            // Enter
            if (e.KeyChar == 13) {
                if (tbBuddyNickname.Text.Length >= 1) {
                    buttonAdd_Click(sender, e);
                }
                else {
                    errorProvider1.SetError(tbBuddyNickname, RuntimeSettings.Language.GetTag("iatag_ui_buddy_nickname_error_message"));
                    errorProvider1.SetIconAlignment(tbBuddyNickname, ErrorIconAlignment.MiddleLeft);
                }
                e.Handled = true;
            }
        }

        bool Verify(string buddyId) {
            var status = _restService.VerifyGet($"{Uris.BuddyItemsUrl}?id={buddyId}&ts=900000000000");
            return status == HttpStatusCode.OK;
        }
    }
}
