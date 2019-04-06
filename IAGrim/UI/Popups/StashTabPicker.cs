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

        private FirefoxRadioButton CreateCheckbox(string name, string label, string text, Point position, FirefoxRadioButton.CheckedChangedEventHandler callback) {
            FirefoxRadioButton checkbox = new FirefoxRadioButton();
            checkbox.Bold = false;
            checkbox.Checked = false;
            checkbox.EnabledCalc = true;
            checkbox.Font = new Font("Segoe UI", 10F);
            checkbox.ForeColor = Color.FromArgb(((int) (((byte) (66)))), ((int) (((byte) (78)))), ((int) (((byte) (90)))));
            checkbox.Location = position;
            checkbox.Name = name;
            checkbox.Size = new Size(188, 27);
            checkbox.TabIndex = 3;
            checkbox.Tag = label;
            checkbox.Text = text;
            checkbox.CheckedChanged += callback;
            return checkbox;
        }

        private void StashTabPicker_Load(object sender, EventArgs e) {
            // Calculate the height dynamically depending on how many stashes the user has
            Height = Math.Min(800, Math.Max(357, 202 + 31 * _numStashTabs));
            gbMoveTo.Height = Math.Max(248, 83 + 33 * _numStashTabs);
            gbLootFrom.Height = Math.Max(248, 83 + 33 * _numStashTabs);


            for (int i = 1; i <= Math.Max(5, _numStashTabs); i++) {
                int p = i; // Don't reference out scope (mutated)
                FirefoxRadioButton.CheckedChangedEventHandler callback = (o, args) => {
                    if (p <= _numStashTabs) {
                        // Don't trust the "Firefox framework" to not trigger clicks on disabled buttons.
                        Properties.Settings.Default.StashToDepositTo = p;
                    }
                };

                int y = 32 + 33 * i;
                var cb = CreateCheckbox($"moveto_tab_{i}", $"iatag_ui_tab_{i}", $"Tab {i}", new Point(6, y), callback);
                cb.Checked = Properties.Settings.Default.StashToDepositTo == i;
                cb.Enabled = i <= _numStashTabs;
                cb.EnabledCalc = i <= _numStashTabs;
                this.gbMoveTo.Controls.Add(cb);
            }


            for (int i = 1; i <= Math.Max(5, _numStashTabs); i++) {
                int p = i; // Don't reference out scope (mutated)
                FirefoxRadioButton.CheckedChangedEventHandler callback = (o, args) => {
                    if (p <= _numStashTabs) {
                        // Don't trust the "Firefox framework" to not trigger clicks on disabled buttons.
                        Properties.Settings.Default.StashToLootFrom = p;
                    }
                };

                int y = 32 + 33 * i;
                var cb = CreateCheckbox($"lootfrom_tab_{i}", $"iatag_ui_tab_{i}", $"Tab {i}", new Point(6, y), callback);
                cb.Checked = Properties.Settings.Default.StashToLootFrom == i;
                cb.Enabled = i <= _numStashTabs;
                cb.EnabledCalc = i <= _numStashTabs;
                this.gbLootFrom.Controls.Add(cb);
            }
            
            radioOutputSecondToLast.Checked = Properties.Settings.Default.StashToDepositTo == 0;
            radioInputLast.Checked = Properties.Settings.Default.StashToLootFrom == 0;

            LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
        }

        private void radioOutputSecondToLast_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToDepositTo = 0;
        }

        private void radioInputLast_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.StashToLootFrom = 0;
        }
    }
}