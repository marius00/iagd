using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Drawing;
using System.Windows.Forms;
using IAGrim.Services;
using IAGrim.Database.Interfaces;
using IAGrim.Database.Dto;

namespace IAGrim.UI {
    public partial class StashPicker : Form {
        private readonly IHelpService _helpService;
        private readonly IPlayerItemDao _playerItemDao;

        public StashPicker(IHelpService helpService, IPlayerItemDao playerItemDao) {
            _helpService = helpService;
            _playerItemDao = playerItemDao;
            InitializeComponent();
        }


        private void StashPicker_Load(object sender, EventArgs e) {
            int n = 0;
            foreach (var mod in _playerItemDao.GetModSelection()) {
                Control cb = new FirefoxRadioButton {
                    Location = new Point(10, 25 + n*33),
                    Text = mod.Mod + " (" + (mod.IsHardcore ? "hc" : "sc") + ")",
                    Tag = mod,
                };

                cb.TabIndex = n;
                cb.TabStop = true;
                groupBox1.Controls.Add(cb);
                n++;
            }

            this.Height += n * 33;
        }

        public StashPickerResult Result { get; private set; }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Enter) {
                buttonTransfer_Click(null, null);

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonTransfer_Click(object sender, EventArgs e) {

            foreach (Control c in groupBox1.Controls) {
                FirefoxRadioButton cb = c as FirefoxRadioButton;
                if (cb != null && cb.Checked) {
                    ModSelection mod = c.Tag as ModSelection;
                    if (mod != null) {
                        Result = new StashPickerResult {
                            Mod = mod.Mod,
                            IsHardcore = mod.IsHardcore
                        };
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

        public class StashPickerResult {
            public string Mod {
                get;
                set;
            }

            public bool IsHardcore {
                get;
                set;
            }
        }
    }

}
