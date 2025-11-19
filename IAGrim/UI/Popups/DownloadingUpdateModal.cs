using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI.Popups {
    public partial class DownloadingUpdateModal : Form {
        public ProgressBar ProgressBar => progressBar1;
        public DownloadingUpdateModal() {
            InitializeComponent();
        }

        private void DownloadingUpdateModal_Load(object sender, EventArgs e) {
        }
    }
}
