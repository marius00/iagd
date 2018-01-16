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
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI.Model;
using IAGrim.UI.Service;
using IAGrim.Utilities;
using NHibernate.Util;

namespace IAGrim.UI {
    public partial class ModsDatabaseConfig : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModsDatabaseConfig));
        private readonly IDatabaseSettingDao _databaseSettingDao;
        private Action _itemViewUpdateTrigger;
        private readonly ArzParser _arzParser;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ParsingService _parsingService;
        private readonly DatabaseModSelectionService _databaseModSelectionService;

        public ModsDatabaseConfig(Action itemViewUpdateTrigger, IDatabaseSettingDao databaseSettingDao, ArzParser arzParser, IPlayerItemDao playerItemDao, ParsingService parsingService) {
            InitializeComponent();
            this._itemViewUpdateTrigger = itemViewUpdateTrigger;
            this._databaseSettingDao = databaseSettingDao;
            this._arzParser = arzParser;
            this._playerItemDao = playerItemDao;
            _parsingService = parsingService;
            _databaseModSelectionService = new DatabaseModSelectionService();
        }


        private void UpdateListview(IEnumerable<string> paths) {
            listViewInstalls.BeginUpdate();
            listViewInstalls.Items.Clear();
            var installs = _databaseModSelectionService.GetGrimDawnInstalls(paths);
            foreach (var grimDawnInstall in installs) {
                listViewInstalls.Items.Add(grimDawnInstall);
            }
            listViewInstalls.EndUpdate();
            if (listViewInstalls.Items.Count > 0) {
                listViewInstalls.Items[0].Selected = true;
            }


            listViewMods.BeginUpdate();
            listViewMods.Items.Clear();
            foreach (var grimDawnInstall in _databaseModSelectionService.GetInstalledMods(paths)) {
                listViewMods.Items.Add(grimDawnInstall);
            }
            listViewMods.EndUpdate();
            if (listViewMods.Items.Count > 0) {
                listViewMods.Items[0].Selected = true;
            }

        }


        private void ModsDatabaseConfig_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            var paths = GrimDawnDetector.GetGrimLocations();
            if (paths.Count == 0) {
                listViewInstalls.Enabled = false;
                buttonForceUpdate.Enabled = false;
            }
            else {
                UpdateListview(paths);
            }

            buttonForceUpdate.Enabled = listViewInstalls.SelectedItems.Count > 0;
        }


        /// <summary>
        /// Sets the "last database update" timestamp to 0 to force an update
        /// Queues a database update, followed by an item stat update.
        /// </summary>
        public void ForceDatabaseUpdate(string location, string modLocation) {
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location)) {
                _parsingService.Update(location, modLocation);
                _parsingService.Execute();
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
            foreach (ListViewItem lvi in listViewInstalls.SelectedItems) {
                ListViewEntry mod = listViewMods.SelectedItems[0].Tag as ListViewEntry;
                ListViewEntry entry = lvi.Tag as ListViewEntry;
                ForceDatabaseUpdate(entry.Path, mod?.Path);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            buttonForceUpdate.Enabled = listViewInstalls.SelectedItems.Count > 0;
        }

        private void buttonUpdateItemStats_Click(object sender, EventArgs e) {
            UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(_playerItemDao);
            x.ShowDialog();


            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }
    }
}
