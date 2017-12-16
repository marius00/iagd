using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Utilities.HelperClasses;
using System;
using System.Windows.Forms;

namespace IAGrim.UI.Popups.ImportExport.Panels {

    partial class ImportExportModePicker : Form {
        private readonly Control.ControlCollection parentContainer;
        private readonly IPlayerItemDao playerItemDao;
        private readonly GDTransferFile[] modFilter;
        private readonly StashManager sm;

        public ImportExportModePicker(GDTransferFile[] modFilter, IPlayerItemDao playerItemDao, Control.ControlCollection parentContainer, StashManager sm) {
            InitializeComponent();
            this.modFilter = modFilter;
            this.playerItemDao = playerItemDao;
            this.parentContainer = parentContainer;
            this.sm = sm;
        }

        private void buttonImport_Click(object sender, EventArgs e) {
            var form = new ImportMode(modFilter, playerItemDao, sm) { TopLevel = false };            
            parentContainer.Add(form);
            parentContainer.Remove(this);
            form.Show();
            this.Close();
        }

        private void buttonExport_Click(object sender, EventArgs e) {
            var form = new ExportMode(modFilter, playerItemDao) { TopLevel = false };
            parentContainer.Add(form);
            parentContainer.Remove(this);
            form.Show();
            this.Close();
        }

        private void ImportExportModePicker_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
        }
    }
}
