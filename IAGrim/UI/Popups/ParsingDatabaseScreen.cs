using IAGrim.Database;
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
using System.Windows.Threading;
using IAGrim.Utilities;

namespace IAGrim.UI {
    /// <summary>
    /// Parses the arc/arz from Grim Dawn and creates a UI loading screen
    /// </summary>
    public partial class ParsingDatabaseScreen : Form {
        private ParsingUiBackgroundWorker _worker;
        public bool CanClose { get; set; }
        private string _grimdawnInstallPath;
        private readonly IDatabaseSettingDao _databaseSettingDao;

        public ParsingDatabaseScreen(
                IDatabaseSettingDao databaseSettingDao,
                ArzParser parser,
                string grimdawnInstallPath,
                string localizationFile,
                bool tagsOnly,
                bool expansionOnlyMod
            ) {

            InitializeComponent();
            CanClose = false;
            this._grimdawnInstallPath = grimdawnInstallPath;
            this._databaseSettingDao = databaseSettingDao;

            var arg = new IAGrim.Parsers.Arz.ParsingUiBackgroundWorker.ParsingUiBackgroundWorkerArgument {
                Path = grimdawnInstallPath,
                ExpansionOnlyMod = expansionOnlyMod,
                LocalizationFile = localizationFile,
                TagsOnly = tagsOnly
            };

            _worker = new ParsingUiBackgroundWorker(bw_RunWorkerCompleted, arg, parser);


            var tag = GlobalSettings.Language.GetTag("iatag_ui_popup_parsing_header");
            if (!string.IsNullOrEmpty(tag)) {
                this.firefoxH11.Text = tag;
            }
        }


        private void ParsingDatabaseScreen_Load(object sender, EventArgs e) {
            this.FormClosing += ParsingDatabaseScreen_FormClosing;
        }


        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            //timer.Stop();
            this.CanClose = true;
            this.Close();

            // Update the path to the database
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker)delegate {
                    _databaseSettingDao.UpdateCurrentDatabase(_grimdawnInstallPath);
                });
            }
            else {
                _databaseSettingDao.UpdateCurrentDatabase(_grimdawnInstallPath);
            }
        }

        

        void ParsingDatabaseScreen_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !CanClose;
            _worker.Dispose();
        }

        
    }
}
