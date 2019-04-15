using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI.Model;
using IAGrim.UI.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using IAGrim.Database.Synchronizer;
using IAGrim.Services;
using IAGrim.Utilities;

namespace IAGrim.UI
{
    public partial class ModsDatabaseConfig : Form
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModsDatabaseConfig));

        private readonly Action _itemViewUpdateTrigger;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ParsingService _parsingService;
        private readonly DatabaseModSelectionService _databaseModSelectionService;
        private readonly IDatabaseSettingDao _databaseSettingRepo;

        public ModsDatabaseConfig(
            Action itemViewUpdateTrigger, 
            IPlayerItemDao playerItemDao, 
            ParsingService parsingService,
            IDatabaseSettingDao databaseSettingRepo
            )
        {
            InitializeComponent();
            _itemViewUpdateTrigger = itemViewUpdateTrigger;
            _playerItemDao = playerItemDao;
            _parsingService = parsingService;
            _databaseSettingRepo = databaseSettingRepo;
            _databaseModSelectionService = new DatabaseModSelectionService();
        }

        private void UpdateListView(IEnumerable<string> paths)
        {
            listViewInstalls.BeginUpdate();
            listViewInstalls.Items.Clear();

            var installs = _databaseModSelectionService.GetGrimDawnInstalls(paths);

            foreach (var grimDawnInstall in installs)
            {
                listViewInstalls.Items.Add(grimDawnInstall);
            }

            listViewInstalls.EndUpdate();

            if (listViewInstalls.Items.Count > 0) {
                listViewInstalls.Items[0].Selected = true;
            }

            // Show help linklabel?
            helpFindGrimdawnInstall.Visible = listViewInstalls.Items.Count == 0;

            listViewMods.BeginUpdate();
            listViewMods.Items.Clear();

            foreach (var grimDawnInstall in _databaseModSelectionService.GetInstalledMods(paths))
            {
                listViewMods.Items.Add(grimDawnInstall);
            }

            listViewMods.EndUpdate();

            if (listViewMods.Items.Count > 0)
            {
                listViewMods.Items[0].Selected = true;
            }

        }

        private void ModsDatabaseConfig_Load(object sender, EventArgs e)
        {
            Dock = DockStyle.Fill;

            var paths = GrimDawnDetector.GetGrimLocations();

            if (paths.Count == 0)
            {
                listViewInstalls.Enabled = false;
                buttonForceUpdate.Enabled = false;
            }
            else
            {
                UpdateListView(paths);
            }

            buttonForceUpdate.Enabled = listViewInstalls.SelectedItems.Count > 0;
        }


        /// <summary>
        /// Sets the "last database update" timestamp to 0 to force an update
        /// Queues a database update, followed by an item stat update.
        /// </summary>
        public void ForceDatabaseUpdate(string location, string modLocation)
        {
            if (!string.IsNullOrEmpty(location) && Directory.Exists(location))
            {
                _parsingService.Update(location, modLocation);
                _parsingService.Execute();
            }
            else
            {
                Logger.Warn("Could not find the Grim Dawn install location");
            }

            // Update item stats as well
            var updatingPlayerItemsScreen = new UpdatingPlayerItemsScreen(_playerItemDao);

            updatingPlayerItemsScreen.ShowDialog();
            _itemViewUpdateTrigger?.Invoke();
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e) {
            _databaseSettingRepo.Clean();
            foreach (ListViewItem lvi in listViewInstalls.SelectedItems) {
                var mod = listViewMods.SelectedItems[0].Tag as ListViewEntry;
                var entry = lvi.Tag as ListViewEntry;

                ForceDatabaseUpdate(entry.Path, mod?.Path);
                _databaseSettingRepo.UpdateCurrentDatabase(entry.Path);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            buttonForceUpdate.Enabled = listViewInstalls.SelectedItems.Count > 0;
        }

        private void buttonUpdateItemStats_Click(object sender, EventArgs e)
        {
            var updatingPlayerItemsScreen = new UpdatingPlayerItemsScreen(_playerItemDao);

            updatingPlayerItemsScreen.ShowDialog();
            _itemViewUpdateTrigger?.Invoke();
        }

        private void helpFindGrimdawnInstall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.CannotFindGrimdawn);
        }

        private void buttonClean_Click(object sender, EventArgs e) {
            _databaseSettingRepo.Clean();
            buttonUpdateItemStats_Click(sender, e);
            MessageBox.Show(GlobalSettings.Language.GetTag("iatag_ui_clean_body"), GlobalSettings.Language.GetTag("iatag_ui_clean_caption"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
