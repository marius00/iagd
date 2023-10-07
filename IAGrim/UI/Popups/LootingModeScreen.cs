using IAGrim.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.UI.Popups {
    public partial class LootingModeScreen : Form {
        private readonly SettingsService _settings;
        public LootingModeScreen(SettingsService settings) {
            InitializeComponent();
            _settings= settings;
        }

        private void button1_Click(object sender, EventArgs e) {
            _settings.GetLocal().PreferLegacyMode = rbClassic.Checked;
            this.Close();
        }

        private void LootingModeScreen_Load(object sender, EventArgs e) {
            rbInstant.Checked = !_settings.GetLocal().PreferLegacyMode;
            rbClassic.Checked = _settings.GetLocal().PreferLegacyMode;
        }
    }
}
