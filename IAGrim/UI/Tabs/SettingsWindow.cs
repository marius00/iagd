﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using EvilsoftCommons;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Services;
using IAGrim.Settings;
using IAGrim.Settings.Dto;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Popups;
using IAGrim.Utilities.HelperClasses;

namespace IAGrim.UI.Tabs {
    partial class SettingsWindow : Form {
        private readonly ISettingsController _controller;
        private readonly TooltipHelper _tooltipHelper;

        private readonly Action _itemViewUpdateTrigger;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly GDTransferFile[] _modFilter;
        private readonly TransferStashService _transferStashService;
        private readonly CefBrowserHandler _cefBrowserHandler;
        private readonly LanguagePackPicker _languagePackPicker;
        private readonly SettingsService _settings;
        private readonly GrimDawnDetector _grimDawnDetector;

        public SettingsWindow(
            CefBrowserHandler cefBrowserHandler,
            TooltipHelper tooltipHelper, 
            Action itemViewUpdateTrigger, 
            IPlayerItemDao playerItemDao,
            GDTransferFile[] modFilter,
            TransferStashService transferStashService, 
            LanguagePackPicker languagePackPicker, 
            SettingsService settings, GrimDawnDetector grimDawnDetector) {            
            InitializeComponent();
            _controller = new SettingsController(settings);
            this._cefBrowserHandler = cefBrowserHandler;
            this._tooltipHelper = tooltipHelper;
            this._itemViewUpdateTrigger = itemViewUpdateTrigger;
            this._playerItemDao = playerItemDao;
            this._modFilter = modFilter;
            this._transferStashService = transferStashService;
            _languagePackPicker = languagePackPicker;
            _settings = settings;
            _grimDawnDetector = grimDawnDetector;

            _controller.BindCheckbox(cbMinimizeToTray);

            _controller.BindCheckbox(cbMergeDuplicates);
            _controller.BindCheckbox(cbTransferAnyMod);
            _controller.BindCheckbox(cbSecureTransfers);
            _controller.BindCheckbox(cbShowRecipesAsItems);
            _controller.BindCheckbox(cbAutoUpdateModSettings);
            _controller.BindCheckbox(cbDisplaySkills);
            _controller.LoadDefaults();
        }

        private void SettingsWindow_Load(object sender, EventArgs e) {
            this.Dock = DockStyle.Fill;
            
            
            radioBeta.Checked = _settings.GetPersistent().SubscribeExperimentalUpdates;
            radioRelease.Checked = !_settings.GetPersistent().SubscribeExperimentalUpdates;
            cbDualComputer.Checked = _settings.GetPersistent().UsingDualComputer;
            cbShowAugments.Checked = _settings.GetPersistent().ShowAugmentsAsItems;
            cbDeleteDuplicates.Checked = _settings.GetPersistent().DeleteDuplicates;
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
            _settings.GetPersistent().SubscribeExperimentalUpdates = false;
        }

        private void radioBeta_CheckedChanged(object sender, EventArgs e) {
            _settings.GetPersistent().SubscribeExperimentalUpdates = true;
        }

        // create bindings and stick these into its own settings class
        // unit testable
        

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                System.Diagnostics.Process.Start("https://discord.gg/vJHp4y");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText("https://discord.gg/vJHp4y");
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

        private void buttonLanguageSelect_Click(object sender, EventArgs e) {
            _languagePackPicker.Show(_grimDawnDetector.GetGrimLocations());
            _itemViewUpdateTrigger?.Invoke();
        }


        private void buttonImportExport_Click(object sender, EventArgs e) {
            new Popups.ImportExport.ImportExportContainer(_modFilter, _playerItemDao, _transferStashService).ShowDialog();
        }

        private void cbDisplaySkills_CheckedChanged(object sender, EventArgs e) {
            _itemViewUpdateTrigger?.Invoke();
        }

        private void buttonAdvancedSettings_Click(object sender, EventArgs e) {
            new StashTabPicker(_transferStashService.NumStashTabs, _settings).ShowDialog();
        }

        private void linkSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/marius00/iagd");
        }


        private void cbDualComputer_CheckedChanged(object sender, EventArgs e) {
            _settings.GetPersistent().UsingDualComputer = (sender as FirefoxCheckBox).Checked;
        }

        private void cbShowAugments_CheckedChanged(object sender, EventArgs e) {
            _settings.GetPersistent().ShowAugmentsAsItems = (sender as FirefoxCheckBox).Checked;
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
            _settings.GetPersistent().DeleteDuplicates = (sender as FirefoxCheckBox).Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            _controller.DonateNow();
        }

        private void buttonPatreon_Click(object sender, EventArgs e) {
            Process.Start("https://www.patreon.com/itemassistant");
        }
    }
}
