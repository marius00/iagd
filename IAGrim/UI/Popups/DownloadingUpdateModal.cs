using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;
using IAGrim.Utilities;

namespace IAGrim.UI.Popups {
    public partial class DownloadingUpdateModal : Form {
        public ProgressBar ProgressBar => progressBar1;
        public DownloadingUpdateModal() {
            InitializeComponent();
            LocalizationLoader.ApplyLanguage(Controls, RuntimeSettings.Language!);
        }

        private void DownloadingUpdateModal_Load(object sender, EventArgs e) {
        }
    }
}
