using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI {
    public partial class StashPicker : Form {
        public StashPicker() {
            InitializeComponent();
        }

        public string Result {
            get;
            private set;
        }

        private void StashPicker_Load(object sender, EventArgs e) {
            int n = 0;
            foreach (var mod in GlobalPaths.TransferFiles) {
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
    }
}
