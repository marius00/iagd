using IAGrim.Utilities;
using log4net;
using System;
using System.IO;
using System.Windows.Forms;

namespace IAGrim.UI.Tabs
{
    public partial class LoggingWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LoggingWindow));

        public LoggingWindow() {
            InitializeComponent();
        }

        private void LoggingWindow_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;

            // This actually activates the logging text... 
            // Since it only writes the backlog on the next event
            Logger.Info("Logging window activated");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            
            System.Diagnostics.Process.Start("notepad", Path.Combine(GlobalPaths.CoreFolder, "log.txt"));
        }
    }
}
