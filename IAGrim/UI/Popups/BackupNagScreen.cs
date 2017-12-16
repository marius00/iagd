using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;
using IAGrim.Utilities;

namespace IAGrim.UI.Popups {
    public partial class BackupNagScreen : Form {
        private static DateTime nextNag;
        public static bool ShouldNag {
            get {
                if (Properties.Settings.Default.BackupOnedrive || Properties.Settings.Default.BackupGoogle || Properties.Settings.Default.BackupDropbox)
                    return false;

                if (Properties.Settings.Default.OnlineBackupVerified && !string.IsNullOrEmpty(Properties.Settings.Default.OnlineBackupToken))
                    return false;

                if (Properties.Settings.Default.BackupCustom && !string.IsNullOrEmpty(Properties.Settings.Default.BackupCustomLocation))
                    return false;

                if (Properties.Settings.Default.UserNeverWantsBackups)
                    return false;

                if (DateTime.UtcNow > nextNag)
                    return true;

                return false;
            }
        }
        public BackupNagScreen() {
            InitializeComponent();
            nextNag = DateTime.UtcNow.AddHours(18);
        }

        public bool UserWantsBackups { get; set; }
        

        private void buttonThanks_Click(object sender, EventArgs e) {
            UserWantsBackups = true;
            this.Close();
        }

        private void buttonNeverRemindMe_Click(object sender, EventArgs e) {
            UserWantsBackups = false;
            Properties.Settings.Default.UserNeverWantsBackups = true;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void BackupNagScreen_Load(object sender, EventArgs e) {
            LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
        }
    }
}
