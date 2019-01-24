using log4net;
using System;
using System.IO;
using System.Windows.Forms;

namespace IAGrim.UI
{
    public partial class HelpTab : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HelpTab));

        public HelpTab() {
            InitializeComponent();
        }

        private void HelpTab_Load(object sender, EventArgs e) {
            Dock = DockStyle.Fill;
            const string file = @"resources\help.html";

            if (!File.Exists(file))
            {
                Logger.WarnFormat($"Could not locate help file {file}");
            }

            webBrowser1.Navigate(Path.Combine(Application.StartupPath, file));
        }
    }
}
