using EvilsoftCommons;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Popups;
using IAGrim.Utilities.HelperClasses;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using IAGrim.Services;

namespace IAGrim.UI {
    partial class SettingsWindow : Form {
        private readonly ISettingsController _controller = new SettingsController();
        private readonly TooltipHelper _tooltipHelper;

        private readonly Action _itemViewUpdateTrigger;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly GDTransferFile[] _modFilter;
        private readonly TransferStashService _transferStashService;
        private readonly CefBrowserHandler _cefBrowserHandler;
        private readonly LanguagePackPicker _languagePackPicker;

        public SettingsWindow(
            CefBrowserHandler cefBrowserHandler,
            TooltipHelper tooltipHelper, 
            Action itemViewUpdateTrigger, 
            IPlayerItemDao playerItemDao,
            GDTransferFile[] modFilter,
            TransferStashService transferStashService, 
            LanguagePackPicker languagePackPicker
            ) {            
            InitializeComponent();
            this._cefBrowserHandler = cefBrowserHandler;
            this._tooltipHelper = tooltipHelper;
            this._itemViewUpdateTrigger = itemViewUpdateTrigger;
            this._playerItemDao = playerItemDao;
            this._modFilter = modFilter;
            this._transferStashService = transferStashService;
            _languagePackPicker = languagePackPicker;

            _controller.BindCheckbox(cbMinimizeToTray);

            _controller.BindCheckbox(cbMergeDuplicates);
            _controller.BindCheckbox(cbTransferAnyMod);
            _controller.BindCheckbox(cbSecureTransfers);
            _controller.BindCheckbox(cbShowRecipesAsItems);
            _controller.BindCheckbox(cbAutoUpdateModSettings);
            _controller.BindCheckbox(cbAutoSearch);
            _controller.BindCheckbox(cbDisplaySkills);
            _controller.LoadDefaults();
        }

        private void SettingsWindow_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;

            radioBeta.Checked = Properties.Settings.Default.SubscribeExperimentalUpdates;
            radioRelease.Checked = !radioBeta.Checked;
            cbDualComputer.Checked = Properties.Settings.Default.UsingDualComputer;
            cbShowAugments.Checked = Properties.Settings.Default.ShowAugmentsAsItems;
            cbDeleteDuplicates.Checked = Properties.Settings.Default.DeleteDuplicates;
        }

        private void buttonViewBackups_Click(object sender, EventArgs e) {
            _controller.OpenDataFolder();
        }

        private void buttonViewLogs_Click(object sender, EventArgs e) {
            _controller.OpenLogFolder();
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
                System.Diagnostics.Process.Start("https://discord.gg/bKWuaG7");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText("https://discord.gg/bKWuaG7");
            _tooltipHelper.ShowTooltipForControl("Copied to clipboard", linkLabel1, TooltipHelper.TooltipLocation.TOP);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
            e.Cancel = false;
        }

        private void cbShowRecipesAsItems_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void cbMergeDuplicates_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void cbShowBaseStats_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void buttonLanguageSelect_Click(object sender, EventArgs e) {
            _languagePackPicker.Show(GrimDawnDetector.GetGrimLocations());
            _itemViewUpdateTrigger?.Invoke();
        }


        private void buttonImportExport_Click(object sender, EventArgs e) {
            new Popups.ImportExport.ImportExportContainer(_modFilter, _playerItemDao, _transferStashService).ShowDialog();
        }

        private void cbDisplaySkills_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void buttonAdvancedSettings_Click(object sender, EventArgs e) {
            new StashTabPicker(_transferStashService.NumStashTabs).ShowDialog();
        }

        private void linkSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/marius00/iagd");
        }


        private void cbDualComputer_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.UsingDualComputer = (sender as FirefoxCheckBox).Checked;
            Properties.Settings.Default.Save();
        }

        private void cbShowAugments_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.ShowAugmentsAsItems = (sender as FirefoxCheckBox).Checked;
            Properties.Settings.Default.Save();
        }

        private void buttonDevTools_Click(object sender, EventArgs e) {
            _cefBrowserHandler.ShowDevTools();
        }

        private void helpWhatIsRecipeAsItems_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.ShowRecipesAsItems);
        }

        private void helpWhatIsAugmentAsItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.ShowAugmentsAsItems);

        }

        private void helpWhatIsSecureTransfers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.SecureTransfers);
        }

        private void helpWhatIsTransferToAnyMod_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.TransferToAnyMod);
        }

        private void helpWhatIsUsingMultiplePc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.MultiplePcs);
        }

        private void helpWhatIsDeleteDuplicates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            HelpService.ShowHelp(HelpService.HelpType.DeleteDuplicates);
        }

        private void cbDeleteDuplicates_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.DeleteDuplicates = (sender as FirefoxCheckBox).Checked;
            Properties.Settings.Default.Save();
        }
    }
}
