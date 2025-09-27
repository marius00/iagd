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
using IAGrim.Parsers.TransferStash;

namespace IAGrim.UI.Popups.ImportExport {
    partial class ImportExportContainer : Form {
        private readonly GDTransferFile[] _modFilter;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly TransferStashService2 _sm;

        public ImportExportContainer(GDTransferFile[] modFilter, IPlayerItemDao playerItemDao, TransferStashService2 sm) {
            InitializeComponent();
            this._modFilter = modFilter;
            this._playerItemDao = playerItemDao;
            this._sm = sm;
        }

        private void ImportExportContainer_Load(object sender, EventArgs e) {
            UIHelper.AddAndShow(new ImportExportModePicker(
                _modFilter, 
                _playerItemDao, 
                contentPanel.Controls, 
                _sm,
                () => this.Close()
                ), contentPanel);
        }
    }
}
