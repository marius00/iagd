using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Drawing;
using System.Windows.Forms;
using IAGrim.Services;

namespace IAGrim.UI {
    public partial class StashPicker : Form {
        private readonly IHelpService _helpService;

        public StashPicker(IHelpService helpService) {
            _helpService = helpService;
            InitializeComponent();
        }

        public string Result {
            get;
            private set;
        }

        private void StashPicker_Load(object sender, EventArgs e) {
            int n = 0;
            foreach (var mod in GlobalPaths.GetTransferFiles(true)) {
                Control cb = new FirefoxRadioButton {
                    Location = new Point(10, 25 + n*33),
                    Text = mod.ToString(),
                    Tag = mod,
                    Enabled = !string.IsNullOrEmpty(mod.Filename),
                };

                cb.TabIndex = n;
                cb.TabStop = true;
                groupBox1.Controls.Add(cb);
                n++;
            }

            this.Height += n * 33;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Enter) {
                buttonTransfer_Click(null, null);

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonTransfer_Click(object sender, EventArgs e) {
            Result = string.Empty;

            foreach (Control c in groupBox1.Controls) {
                FirefoxRadioButton cb = c as FirefoxRadioButton;
                if (cb != null && cb.Checked) {
                    GDTransferFile mod = c.Tag as GDTransferFile;
                    if (mod != null) {
                        Result = mod.Filename;
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }


            this.Close();
        }

        private void helpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.TransferToAnyMod);
            this.Close();
        }
    }
}
