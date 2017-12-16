using IAGrim.Backup.FileWriter;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.UI.Controller;
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
    partial class ImportMode : Form {
        private readonly GDTransferFile[] modSelection;
        private readonly IPlayerItemDao playerItemDao;
        private string filename;
        private readonly StashManager sm;

        public ImportMode(GDTransferFile[] modSelection, IPlayerItemDao playerItemDao, StashManager sm) {
            InitializeComponent();
            this.modSelection = modSelection;
            this.playerItemDao = playerItemDao;
            this.sm = sm;
        }

        private void ImportMode_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            this.buttonImport.Enabled = false;
            cbItemSelection.Visible = false;
            cbItemSelection.Enabled = false;

            cbItemSelection.Items.AddRange(modSelection);
        }

        private void radioIAStash_CheckedChanged(object sender, EventArgs e) {
            cbItemSelection.Visible = !(sender as Control).Enabled;
        }

        private void radioGDStash_CheckedChanged(object sender, EventArgs e) {
            cbItemSelection.Visible = (sender as Control).Enabled;
        }
        private void radioGameStash_CheckedChanged(object sender, EventArgs e) {
            cbItemSelection.Visible = !(sender as Control).Enabled;
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            if (buttonBrowse.Enabled) {
                OpenFileDialog diag = new OpenFileDialog {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Grim Dawn", "Save"),
                    Multiselect = false,
                    Title = "Select the file to import"
                };

                if (radioGDStash.Checked) {
                    diag.DefaultExt = "gds";
                    diag.Filter = "GD Stash exports (*.gds)|*.gds";
                }
                else if (radioIAStash.Checked) {
                    diag.DefaultExt = "ias";
                    diag.Filter = "IA Stash exports (*.ias)|*.ias";
                }
                else {
                    diag.DefaultExt = "gst";
                    diag.Filter = "Grim Dawn Stash files (*.gst)|*.gst";
                    diag.FileName = "transfer.gst";
                }

                if (diag.ShowDialog() == DialogResult.OK) {
                    radioGDStash.Enabled = false;
                    radioIAStash.Enabled = false;
                    buttonImport.Enabled = true;
                    cbItemSelection.Enabled = true;
                    this.filename = diag.FileName;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e) {
            if (buttonImport.Enabled) {
                FileExporter io;

                if (radioIAStash.Checked) {
                    io = new IAFileExporter(filename);
                }
                else if (radioGDStash.Checked) {
                    GDTransferFile settings = cbItemSelection.SelectedItem as GDTransferFile;
                    if (settings == null) {
                        io = new GDFileExporter(filename, false, string.Empty);
                    }
                    else {
                        io = new GDFileExporter(filename, settings.IsExpansion1, settings.Mod);
                    }
                }
                else {
                    io = null;
                    playerItemDao.Save(sm.EmptyStash(filename));
                    MessageBox.Show("Items imported", "Items imported!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var items = io.Read();
                playerItemDao.Import(items);

                if (items.Any(m => m.OnlineId.HasValue && m.OnlineId > 0)) {
                    MessageBox.Show("Items imported\nSome items may have been skipped to avoid duplicating items.", "Items imported!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    MessageBox.Show("Items imported\nIf you already had items, you may have gotten duplicates.", "Items imported!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    }
}
