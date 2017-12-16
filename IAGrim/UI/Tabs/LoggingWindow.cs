using log4net;
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
using IAGrim.Utilities;

namespace IAGrim.UI.Tabs {
    public partial class LoggingWindow : Form {
        private static ILog logger = LogManager.GetLogger(typeof(LoggingWindow));
        public LoggingWindow() {
            InitializeComponent();
        }

        private void LoggingWindow_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            // This actually activates the logging text... 
            // Since it only writes the backlog on the next event
            logger.Info("Logging window activated");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            
            System.Diagnostics.Process.Start(Path.Combine(GlobalPaths.CoreFolder, "log.txt"));
        }
    }
}
