using IAGrim.Backup.FileWriter;
using IAGrim.Database.Interfaces;
using IAGrim.Utilities.HelperClasses;
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

namespace IAGrim.UI.Popups.ImportExport.Panels {
    public partial class ExportMode : Form {
        private readonly GDTransferFile[] _modSelection;
        private readonly IPlayerItemDao _playerItemDao;
        private string _filename;
        private bool _isGdstashFormat = false;

        public ExportMode(GDTransferFile[] modSelection, IPlayerItemDao playerItemDao) {
            InitializeComponent();
            this._modSelection = modSelection;
            this._playerItemDao = playerItemDao;
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            const int FilterIndexIas = 1;
            const int FilterIndexGds = 2;
            var diag = new OpenFileDialog {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "ias",
                Filter = "IA Stash exports (*.ias)|*.ias|GD Stash exports (*.gds)|*.gds",
                InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save"),
                Multiselect = false,
                Title = "Choose filename for export"
            };

            if (diag.ShowDialog() == DialogResult.OK) {
                buttonExport.Enabled = true;
                var idx = diag.FilterIndex;
                cbItemSelection.Visible = diag.FilterIndex == FilterIndexGds;
                this._isGdstashFormat = diag.FileName.EndsWith(".gds");
                this._filename = diag.FileName;
            }
        }

        private void ExportMode_Load(object sender, EventArgs e) {
            cbItemSelection.Items.Add("All items");
            cbItemSelection.Items.AddRange(_modSelection);
            cbItemSelection.SelectedIndex = 0;
            buttonExport.Enabled = false;
            cbItemSelection.Visible = false;
        }

        private void buttonExport_Click(object sender, EventArgs e) {
            if (buttonExport.Enabled) {
                if (_isGdstashFormat) {
                    var io = new GDFileExporter(_filename, false, string.Empty); // Params are not used for writing

                    GDTransferFile settings = cbItemSelection.SelectedItem as GDTransferFile;
                    if (settings == null) {
                        var items = _playerItemDao.ListAll();
                        io.Write(items);
                    }
                    else {
                        var items = _playerItemDao.ListAll()
                            .Where(item => item.IsHardcore == settings.IsHardcore/* && item.IsExpansion1 == settings.IsExpansion1*/);

                        if (string.IsNullOrEmpty(settings.Mod)) {
                            io.Write(items.Where(item => string.IsNullOrEmpty(item.Mod)).ToList());
                        }
                        else {
                            io.Write(items.Where(item => item.Mod == settings.Mod).ToList());
                        }
                    }
                }
                else {
                    var io = new IAFileExporter(_filename);
                    var items = _playerItemDao.ListAll();
                    io.Write(items);
                }

                MessageBox.Show("Items Exported!", "Items exported!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
