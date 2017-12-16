using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.UI.Popups.ImportExport.Panels;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.UI.Popups.ImportExport {
    partial class ImportExportContainer : Form {
        private readonly GDTransferFile[] modFilter;
        private readonly IPlayerItemDao playerItemDao;
        private readonly StashManager sm;

        public ImportExportContainer(GDTransferFile[] modFilter, IPlayerItemDao playerItemDao, StashManager sm) {
            InitializeComponent();
            this.modFilter = modFilter;
            this.playerItemDao = playerItemDao;
            this.sm = sm;
        }

        private void ImportExportContainer_Load(object sender, EventArgs e) {
            UIHelper.AddAndShow(new ImportExportModePicker(modFilter, playerItemDao, contentPanel.Controls, sm), contentPanel);
        }
    }
}
