using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;
using IAGrim.UI.Tabs.Util;
using IAGrim.Utilities;
using IAGrim.Utilities.Registry;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;

namespace IAGrim.UI.Popups {
    public partial class StashTabPicker : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StashTabPicker));
        private readonly int _numStashTabs;

        public StashTabPicker(int numStashTabs) {
            InitializeComponent();
            _numStashTabs = numStashTabs;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            if (Properties.Settings.Default.StashToLootFrom == Properties.Settings.Default.StashToDepositTo &&
                Properties.Settings.Default.StashToLootFrom != 0) {
                MessageBox.Show(
                    "I cannot overstate what an incredibly bad experience it would be to use only one tab.",
                    "Yeah.. Nope!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation
                );
            }
            else {
                Properties.Settings.Default.Save();
                SaveStashSettingsToRegistry();
                this.Close();
            }
        }

        public void SaveStashSettingsToRegistry() {
            int stash = Properties.Settings.Default.StashToLootFrom;
            if (stash == 0) {
                stash = _numStashTabs;
            }
            RegistryHelper.Write(@"Software\EvilSoft\IAGD", "StashToLootFrom", stash);
        }

        private void StashTabPicker_Load(object sender, EventArgs e) {
            switch (Properties.Settings.Default.StashToDepositTo) {
                case 0:
                    radioOutputSecondToLast.Checked = true;
                    break;
                case 1:
                    radioOutput1.Checked = true;
                    break;
                case 2:
                    radioOutput2.Checked = true;
                    break;
                case 3:
                    radioOutput3.Checked = true;
                    break;
                case 4:
                    radioOutput4.Checked = true;
                    break;
                case 5:
                    radioOutput5.Checked = true;
                    break;
            }
            switch (Properties.Settings.Default.StashToLootFrom) {
                case 0:
                    radioInputLast.Checked = true;
                    break;
                case 1:
                    radioInput1.Checked = true;
                    break;
                case 2:
                    radioInput2.Checked = true;
                    break;
                case 3:
                    radioInput3.Checked = true;
                    break;
                case 4:
                    radioInput4.Checked = true;
                    break;
                case 5:
                    radioInput5.Checked = true;
                    break;
            }

            LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
        }

        private void radioOutputSecondToLast_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 0;
        }

        private void radioOutput1_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 1;
        }

        private void radioOutput2_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 2;
        }

        private void radioOutput3_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 3;
        }

        private void radioOutput4_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 4;
        }

        private void radioOutput5_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 5;
        }

        private void radioInputLast_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 0;
        }

        private void radioInput1_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 1;
        }

        private void radioInput2_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 2;
        }

        private void radioInput3_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 3;
        }

        private void radioInput4_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 4;
        }

        private void radioInput5_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 5;
        }
    }
}
