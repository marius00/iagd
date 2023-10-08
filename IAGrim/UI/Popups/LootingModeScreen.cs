using IAGrim.Settings;
using IAGrim.Utilities;
using System;
using System.Windows.Forms;

namespace IAGrim.UI.Popups {
    public partial class LootingModeScreen : Form {
        private readonly SettingsService _settings;
        private readonly bool _initialValue;

        public LootingModeScreen(SettingsService settings) {
            InitializeComponent();
            _settings = settings;
            _initialValue = _settings.GetLocal().PreferLegacyMode;
        }

        private void button1_Click(object sender, EventArgs e) {
            _settings.GetLocal().PreferLegacyMode = rbClassic.Checked;

            if (_settings.GetLocal().PreferLegacyMode != _initialValue) {
                MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_explain_classiclooting_restartia"));
            }

            this.Close();
        }

        private void LootingModeScreen_Load(object sender, EventArgs e) {
            rbInstant.Checked = !_settings.GetLocal().PreferLegacyMode;
            rbClassic.Checked = _settings.GetLocal().PreferLegacyMode;
        }
    }
}
