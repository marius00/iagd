using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IAGrim.Utilities;

namespace IAGrim.UI {
    public partial class ModsDatabaseConfig : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModsDatabaseConfig));
        private readonly IDatabaseSettingDao _databaseSettingDao;
        private Action _itemViewUpdateTrigger;
        private readonly ArzParser _arzParser;
        private readonly IPlayerItemDao _playerItemDao;

        public ModsDatabaseConfig(Action itemViewUpdateTrigger, IDatabaseSettingDao databaseSettingDao, ArzParser arzParser, IPlayerItemDao playerItemDao) {
            InitializeComponent();
            this._itemViewUpdateTrigger = itemViewUpdateTrigger;
            this._databaseSettingDao = databaseSettingDao;
            this._arzParser = arzParser;
            this._playerItemDao = playerItemDao;
        }

        class ListViewEntry {
            public string Path { get; set; }
            public bool IsVanilla { get; set; }
        }

        private void UpdateListview(IEnumerable<string> paths) {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            var tagVanilla = GlobalSettings.Language.GetTag("iatag_ui_vanilla");
            var tagVanillaXpac = GlobalSettings.Language.GetTag("iatag_ui_vanilla_xpac");
            var tagYes = GlobalSettings.Language.GetTag("iatag_ui_yes");
            var tagNo = GlobalSettings.Language.GetTag("iatag_ui_no");

            foreach (var gdPath in paths) {

                string currentDatabase = _databaseSettingDao.GetCurrentDatabasePath();
                if (!string.IsNullOrEmpty(gdPath) && Directory.Exists(gdPath)) {
                    bool hasExpansion = Directory.Exists(Path.Combine(gdPath, "gdx1"));

                    ListViewItem vanilla = new ListViewItem(hasExpansion ? tagVanillaXpac : tagVanilla);
                    vanilla.SubItems.Add(currentDatabase.Equals(gdPath) ? tagYes : tagNo);
                    vanilla.SubItems.Add(gdPath);

                    vanilla.Tag = new ListViewEntry {Path = gdPath, IsVanilla = true};

                    listView1.Items.Add(vanilla);
                }

                foreach (var modpath in new[] { Path.Combine(gdPath, "mods"), Path.Combine(gdPath, "gdx1", "mods") }) {
                    if (Directory.Exists(modpath)) {
                        foreach (var directory in Directory.EnumerateDirectories(modpath)) {
                            if (Directory.EnumerateFiles(directory, "*.arz", SearchOption.AllDirectories).Any()) {

                                var modName = Path.GetFileName(directory);
                                if (modName == "survivalmode") {
                                    modName = GlobalSettings.Language.GetTag("iatag_ui_survivalmode");
                                }

                                ListViewItem mod = new ListViewItem(modName);
                                mod.SubItems.Add(currentDatabase.Equals(directory) ? tagYes : tagNo);
                                mod.SubItems.Add(directory);
                                mod.Tag = new ListViewEntry {Path = directory, IsVanilla = false};
                                listView1.Items.Add(mod);
                            }
                        }
                    }
                }
            }
            listView1.EndUpdate();

        }


        private void ModsDatabaseConfig_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            var paths = GrimDawnDetector.GetGrimLocations();
            if (paths.Count == 0) {
                listView1.Enabled = false;
                buttonForceUpdate.Enabled = false;
            }
            else {
                UpdateListview(paths);
            }

            buttonForceUpdate.Enabled = listView1.SelectedItems.Count > 0;
        }


        /// <summary>
        /// Sets the "last database update" timestamp to 0 to force an update
        /// Queues a database update, followed by an item stat update.
        /// </summary>
        public void ForceDatabaseUpdate(string location, bool isVanilla) {
            // Override timestamp to force an update
            _databaseSettingDao.UpdateDatabaseTimestamp(0);
            
            var paths = GrimDawnDetector.GetGrimLocations();
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location) && _arzParser.NeedUpdate(location)) {

                ParsingDatabaseScreen parser = new ParsingDatabaseScreen(
                    _databaseSettingDao, 
                    _arzParser,
                    location, 
                    Properties.Settings.Default.LocalizationFile, 
                    false, 
                    !isVanilla);
                parser.ShowDialog();

                //databaseSettingDao.UpdateCurrentDatabase(location);
                UpdateListview(paths);
            }
            else {
                Logger.Warn("Could not find the Grim Dawn install location");
            }

            // Update item stats as well
            UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(_playerItemDao);
            x.ShowDialog();


            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e) {
            foreach (ListViewItem lvi in listView1.SelectedItems) {
                ListViewEntry entry = lvi.Tag as ListViewEntry;
                ForceDatabaseUpdate(entry.Path, entry.IsVanilla);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            buttonForceUpdate.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void buttonUpdateItemStats_Click(object sender, EventArgs e) {
            UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(_playerItemDao);
            x.ShowDialog();


            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }
    }
}
