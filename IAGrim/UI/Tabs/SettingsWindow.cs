using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IAGrim.Database;
using IAGrim.Parsers.Arz;
using log4net;
using EvilsoftCommons;
using IAGrim.UI.Controller;
using IAGrim.Utilities;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Popups;
using IAGrim.Utilities.HelperClasses;
// 
namespace IAGrim.UI {
    partial class SettingsWindow : Form {
        private ISettingsController _controller = new SettingsController();
        private TooltipHelper _tooltipHelper;

        private readonly Action _itemViewUpdateTrigger;
        private readonly IDatabaseSettingDao _settingsDao;
        private readonly IDatabaseItemDao _itemDao;
        private readonly ArzParser _parser;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly GDTransferFile[] _modFilter;
        private readonly StashManager _stashManager;

        public SettingsWindow(
            TooltipHelper tooltipHelper, 
            Action itemViewUpdateTrigger, 
            IDatabaseSettingDao settingsDao, 
            IDatabaseItemDao itemDao,
            IPlayerItemDao playerItemDao,
            ArzParser parser,
            GDTransferFile[] modFilter,
            StashManager stashManager
        ) {            
            InitializeComponent();
            this._tooltipHelper = tooltipHelper;
            this._itemViewUpdateTrigger = itemViewUpdateTrigger;
            this._settingsDao = settingsDao;
            this._itemDao = itemDao;
            this._playerItemDao = playerItemDao;
            this._parser = parser;
            this._modFilter = modFilter;
            this._stashManager = stashManager;

            _controller.BindCheckbox(cbMinimizeToTray);

            _controller.BindCheckbox(cbMergeDuplicates);
            _controller.BindCheckbox(cbTransferAnyMod);
            _controller.BindCheckbox(cbSecureTransfers);
            _controller.BindCheckbox(cbShowRecipesAsItems);
            _controller.BindCheckbox(cbAutoUpdateModSettings);
            //_controller.BindCheckbox(cbInstalootDisabled);
            _controller.BindCheckbox(cbInstaTransfer);
            _controller.BindCheckbox(cbAutoSearch);
            _controller.BindCheckbox(cbDisplaySkills);
            _controller.LoadDefaults();

            cbInstalootEnabled.Checked = (InstalootSettingType)Properties.Settings.Default.InstalootSetting == InstalootSettingType.Enabled;
        }

        private void SettingsWindow_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            

            this.labelNumItems.Text = string.Format("Number of items parsed from Grim Dawn database: {0}", _itemDao.GetRowCount());
            
            // TODO:
            string filename = GrimDawnDetector.GetGrimLocation();
            string databaseFile = Path.Combine(filename, "database", "database.arz");
            DateTime lastPatch = default(DateTime);
            if (File.Exists(databaseFile)) {
                lastPatch = System.IO.File.GetLastWriteTime(databaseFile);
                this.labelLastPatch.Text = string.Format("Last Grim Dawn patch: {0}", lastPatch.ToString("dd/MM/yyyy"));
            }
            else {
                this.labelLastPatch.Text = "Could not find Grim Dawn install folder";
            }

            DateTime lastUpdate = new DateTime(_settingsDao.GetLastDatabaseUpdate());
            this.labelLastUpdated.Text = string.Format("Grim Dawn database last updated: {0}", lastUpdate.ToString("dd/MM/yyyy"));
            if (lastUpdate < lastPatch)
                this.labelLastUpdated.ForeColor = Color.Red;


            radioBeta.Checked = (bool)Properties.Settings.Default.SubscribeExperimentalUpdates;
            radioRelease.Checked = !(bool)Properties.Settings.Default.SubscribeExperimentalUpdates;
            firefoxCheckBox1.Checked = Properties.Settings.Default.Hotfix1_0_4_0_active;
            //controller.LoadDefaults();

        }

        private void buttonViewBackups_Click(object sender, EventArgs e) {
            _controller.OpenDataFolder();
        }

        private void buttonViewLogs_Click(object sender, EventArgs e) {
            _controller.OpenLogFolder();
        }

        private void buttonDeveloper_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://www.grimdawn.com/forums/member.php?u=17888");
        }

        private void buttonForum_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://www.grimdawn.com/forums/showthread.php?t=35240");
        }


        private void radioRelease_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.SubscribeExperimentalUpdates = false;
            Properties.Settings.Default.Save();
        }

        private void radioBeta_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.SubscribeExperimentalUpdates = true;
            Properties.Settings.Default.Save();
        }

        // create bindings and stick these into its own settings class
        // unit testable


        private void buttonDonate_Click(object sender, EventArgs e) {
            _controller.DonateNow();
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                System.Diagnostics.Process.Start("https://discord.gg/PJ87Ewa");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText("https://discord.gg/PJ87Ewa");
            _tooltipHelper.ShowTooltipForControl("Copied to clipboard", linkLabel1, TooltipHelper.TooltipLocation.TOP);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            e.Cancel = false;
        }

        private void cbShowRecipesAsItems_CheckedChanged(object sender, EventArgs e) {
            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }

        private void cbMergeDuplicates_CheckedChanged(object sender, EventArgs e) {
            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }

        private void cbShowBaseStats_CheckedChanged(object sender, EventArgs e) {
            if (_itemViewUpdateTrigger != null)
                _itemViewUpdateTrigger();
        }

        private void buttonLanguageSelect_Click(object sender, EventArgs e) {
            new LanguagePackPicker(_itemDao, _settingsDao, _playerItemDao, _parser, GrimDawnDetector.GetGrimLocation())
                .ShowDialog();

            _itemViewUpdateTrigger?.Invoke();
        }


        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (e.Button == MouseButtons.Left)
                Process.Start("http://grimdawn.dreamcrash.org/ia/experimental.html#instaloot");
        }

        private void buttonImportExport_Click(object sender, EventArgs e) {
            new Popups.ImportExport.ImportExportContainer(_modFilter, _playerItemDao, _stashManager).ShowDialog();
        }

        private void cbDisplaySkills_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void buttonAdvancedSettings_Click(object sender, EventArgs e) {
            new StashTabPicker(_stashManager.NumStashTabs).ShowDialog();
        }

        private void linkSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/marius00/iagd");
        }

        private void cbInstalootDisabled_CheckedChanged(object sender, EventArgs e) {

            Properties.Settings.Default.InstalootSetting = (int)(cbInstalootEnabled.Checked ? InstalootSettingType.Enabled : InstalootSettingType.Disabled);
            Properties.Settings.Default.Save();

            var debuygggy = Properties.Settings.Default.InstalootSetting;
            var debuygggy2 = (InstalootSettingType)Properties.Settings.Default.InstalootSetting;
            int x = 9;
        }

        private void cbInstaTransfer_CheckedChanged(object sender, EventArgs e) {

        }

        private void firefoxCheckBox1_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.Hotfix1_0_4_0_active = firefoxCheckBox1.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
