using EvilsoftCommons.Exceptions;
using IAGrim.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI.Popups {
    public partial class UpdateModal : Form {
        private readonly string _version;
        private readonly SettingsService _settingsService;
        public UpdateModal(SettingsService settingsService, string version, bool forceUpdate) {
            InitializeComponent();
            _settingsService = settingsService;
            _version = version;
            this.DialogResult = DialogResult.None;
            if (forceUpdate) {
                // Practically warrants its own modal at this point..
                this.label1.Text = "Are you sure you wish to downgrade?";
                this.lnkRemindMeLater.Hide();
                this.lnkWhatHasChanged.Hide();
                this.btnUpdateNow.Text = "Downgrade now";
                this.Text = "Downgrade IAGD";
            }
        }

        private void lnkWhatHasChanged_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(new ProcessStartInfo { FileName = $"https://github.com/marius00/iagd/compare/{ExceptionReporter.VersionString}...{_version}", UseShellExecute = true });
        }

        private void lnkRemindMeLater_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _settingsService.GetPersistent().NextUpdateCheck = DateTime.UtcNow.AddDays(7);
            this.Close();
        }

        private void btnUpdateNow_Click(object sender, EventArgs e) {
            // And what if it's not a modal? eh?
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
