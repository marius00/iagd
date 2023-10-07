using System;
using System.Drawing;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using log4net;

namespace IAGrim.UI.Popups {
    public partial class StashTabPicker : Form {
        private readonly SettingsService _settings;
        private readonly int _numStashTabs = 6;
        private readonly IHelpService _helpService;

        public StashTabPicker(SettingsService settings, IHelpService helpService) {
            InitializeComponent();
            _settings = settings;
            _helpService = helpService;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            if (_settings.GetLocal().StashToLootFrom == _settings.GetLocal().StashToDepositTo &&
                _settings.GetLocal().StashToLootFrom != 0) {
                MessageBox.Show(
                    "I cannot overstate what an incredibly bad experience it would be to use only one tab.",
                    "Yeah.. Nope!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
            else {
                this.Close();
            }
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
                        _settings.GetLocal().StashToDepositTo = p;
                    }
                };

                int y = 32 + 33 * i;
                var cb = CreateCheckbox($"moveto_tab_{i}", $"iatag_ui_tab_{i}", $"Tab {i}", new Point(6, y), callback);
                
                cb.Checked = _settings.GetLocal().StashToDepositTo == i;
                cb.Enabled = i <= _numStashTabs;
                cb.EnabledCalc = i <= _numStashTabs;
                this.gbMoveTo.Controls.Add(cb);
                helpWhyAreTheseDisabled.Visible = _numStashTabs <= 4;
            }


            for (int i = 1; i <= Math.Max(5, _numStashTabs); i++) {
                int p = i; // Don't reference out scope (mutated)
                FirefoxRadioButton.CheckedChangedEventHandler callback = (o, args) => {
                    if (p <= _numStashTabs) {
                        // Don't trust the "Firefox framework" to not trigger clicks on disabled buttons.
                        _settings.GetLocal().StashToLootFrom = p;
                    }
                };

                int y = 32 + 33 * i;
                var cb = CreateCheckbox($"lootfrom_tab_{i}", $"iatag_ui_tab_{i}", $"Tab {i}", new Point(6, y), callback);
                cb.Checked = _settings.GetLocal().StashToLootFrom == i;
                cb.Enabled = i <= _numStashTabs;
                cb.EnabledCalc = i <= _numStashTabs;
                this.gbLootFrom.Controls.Add(cb);
            }
            


            radioOutputSecondToLast.Checked = _settings.GetLocal().StashToDepositTo == 0;
            radioInputLast.Checked = _settings.GetLocal().StashToLootFrom == 0;

            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language);
        }

        private void radioOutputSecondToLast_CheckedChanged(object sender, EventArgs e) {
            _settings.GetLocal().StashToDepositTo = 0;
        }

        private void radioInputLast_CheckedChanged(object sender, EventArgs e) {
            _settings.GetLocal().StashToLootFrom = 0;
        }

        private void helpWhyAreTheseDisabled_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.NotEnoughStashTabs);
        }
    }
}