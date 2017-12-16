using CefSharp;
using CefSharp.WinForms;
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

namespace IAGrim.UI {
    public partial class HelpTab : Form {
        private static ILog logger = LogManager.GetLogger(typeof(HelpTab));
        public HelpTab() {
            InitializeComponent();
        }

        private void HelpTab_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            var f = @"resources\help.html";

            if (!File.Exists(f))
                logger.WarnFormat("Could not locate help file {0}", f);

            webBrowser1.Navigate(Path.Combine(Application.StartupPath, f));
        }
    }
}
