using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IAGrim.UI {
    /// <summary>
    /// Loading screen while parsing player item stats
    /// </summary>
    public partial class UpdatingPlayerItemsScreen : Form {
        private StatUpdateUIBackgroundWorker worker;
        public bool CanClose { get; set; }


        public UpdatingPlayerItemsScreen(IPlayerItemDao playerItemDao) {
            InitializeComponent();
            CanClose = false;

            worker = new StatUpdateUIBackgroundWorker(playerItemDao, bw_RunWorkerCompleted, bw_ProgressChanged);
        }

        /// <summary>
        /// Update progress bar safely in UI thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker)delegate {
                    bw_ProgressChanged(sender, e);
                });
            }
            else {
                if ((int)e.UserState == 1)
                    this.progressBar2.Maximum = e.ProgressPercentage;
                else
                    this.progressBar2.Value = e.ProgressPercentage;
            }
        }


        private void UpdatingPlayerItemsScreen_Load(object sender, EventArgs e) {
            this.FormClosing += ParsingDatabaseScreen_FormClosing;
        }


        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null)
                throw e.Error;

            this.CanClose = true;
            this.Close();
        }

        

        void ParsingDatabaseScreen_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !CanClose;
        }

    }
}
