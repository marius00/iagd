using IAGrim.Database.Interfaces;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI.Model;
using IAGrim.UI.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Utilities;

namespace IAGrim.UI {
    public partial class ModsDatabaseConfig : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModsDatabaseConfig));

        private readonly Action _itemViewUpdateTrigger;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ParsingService _parsingService;
        private readonly DatabaseModSelectionService _databaseModSelectionService;

        private readonly GrimDawnDetector _grimDawnDetector;
        private readonly SettingsService _settingsService;
        private readonly IHelpService _helpService;
        private readonly IDatabaseItemDao _databaseItemDao;

        public ModsDatabaseConfig(
            Action itemViewUpdateTrigger,
            IPlayerItemDao playerItemDao,
            ParsingService parsingService,
            GrimDawnDetector grimDawnDetector,
            SettingsService settingsService,
            IHelpService helpService, IDatabaseItemDao databaseItemDao) {
            InitializeComponent();
            _itemViewUpdateTrigger = itemViewUpdateTrigger;
            _playerItemDao = playerItemDao;
            _parsingService = parsingService;
            _grimDawnDetector = grimDawnDetector;
            _settingsService = settingsService;
            _helpService = helpService;
            _databaseItemDao = databaseItemDao;
            _databaseModSelectionService = new DatabaseModSelectionService();
        }

        private void UpdateListView(IEnumerable<string> paths) {
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

            // Show help linklabel?
            helpFindGrimdawnInstall.Visible = listViewInstalls.Items.Count == 0;

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
            Dock = DockStyle.Fill;

            var paths = _grimDawnDetector.GetGrimLocations();

            // Ensure that we store all known paths.
            foreach (var path in paths) {
                _settingsService.GetLocal().AddGrimDawnLocation(path);
            }

            if (paths.Count == 0) {
                listViewInstalls.Enabled = false;
                buttonForceUpdate.Enabled = false;
            }
            else {
                UpdateListView(paths);
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
            var updatingPlayerItemsScreen = new UpdatingPlayerItemsScreen(_playerItemDao);

            updatingPlayerItemsScreen.ShowDialog();
            _itemViewUpdateTrigger?.Invoke();
        }

        private static ListViewEntry GetFirst(ListView lv) {
            foreach (ListViewItem lvi in lv.SelectedItems) {
                return lvi.Tag as ListViewEntry;
            }

            return null;
        }

        private void buttonForceUpdate_Click(object sender, EventArgs e) {
            _databaseItemDao.Clean();

            var isGdParsed2 = _databaseItemDao.GetRowCount() > 0;

            var mod = GetFirst(listViewMods);
            var entry = GetFirst(listViewInstalls);

            if (mod != null) {
                // Load selected mod icons
                ThreadPool.QueueUserWorkItem((m) => ArzParser.LoadSelectedModIcons(mod.Path));
            }

            ForceDatabaseUpdate(entry.Path, mod?.Path);
            _settingsService.GetLocal().CurrentGrimdawnLocation = entry.Path;

            // Store the loaded GD path, so we can poll it for updates later.
            //_settingsService.GetLocal().GrimDawnLocation = new List<string> { entry.Path }; // TODO: Wtf is this? Why overwrite any existing?
            _settingsService.GetLocal().GrimDawnLocationLastModified = ParsingService.GetHighestTimestamp(entry.Path);
            _settingsService.GetLocal().HasWarnedGrimDawnUpdate = false;

            var isGdParsed = _databaseItemDao.GetRowCount() > 0;
            _settingsService.GetLocal().IsGrimDawnParsed = isGdParsed;
            _helpService.SetIsGrimParsed(isGdParsed);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            buttonForceUpdate.Enabled = listViewInstalls.SelectedItems.Count > 0;
        }

        private void buttonUpdateItemStats_Click(object sender, EventArgs e) {
            var updatingPlayerItemsScreen = new UpdatingPlayerItemsScreen(_playerItemDao);

            updatingPlayerItemsScreen.ShowDialog();
            _itemViewUpdateTrigger?.Invoke();
        }

        private void helpFindGrimdawnInstall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _helpService.ShowHelp(HelpService.HelpType.CannotFindGrimdawn);
        }

        private void buttonClean_Click(object sender, EventArgs e) {
            _databaseItemDao.Clean();
            buttonUpdateItemStats_Click(sender, e);
            MessageBox.Show(RuntimeSettings.Language.GetTag("iatag_ui_clean_body"),
                RuntimeSettings.Language.GetTag("iatag_ui_clean_caption"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

            var isGdParsed = _databaseItemDao.GetRowCount() > 0;
            _settingsService.GetLocal().IsGrimDawnParsed = isGdParsed;
            _helpService.SetIsGrimParsed(isGdParsed);
        }

        private void buttonConfigure_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                    if (File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "Grim Dawn.exe"))) {
                        _settingsService.GetLocal().AddGrimDawnLocation(folderBrowserDialog.SelectedPath);
                        Logger.Info($"Added {folderBrowserDialog.SelectedPath} to the known Grim Dawn locations");
                        ModsDatabaseConfig_Load(sender, e);
                        // TODO: Kill the task that keeps looking for GD.
                    }
                    else {
                        var text = RuntimeSettings.Language.GetTag("iatag_ui_db_invalidlocation_body");
                        var title = RuntimeSettings.Language.GetTag("iatag_ui_db_invalidlocation_title");
                        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}