using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DllInjector.UI {
    public partial class AvastedWarning : Form {
        public AvastedWarning() {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start($"http://grimdawn.dreamcrash.org/ia/help.html?q=Avasted&r={DateTime.UtcNow.Ticks}");
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
