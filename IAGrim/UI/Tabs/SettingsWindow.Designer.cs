﻿namespace IAGrim.UI.Tabs
{
    partial class SettingsWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelBox4 = new PanelBox();
            this.helpWhatIsEnableDowngrades = new System.Windows.Forms.LinkLabel();
            this.cbEnableDowngrades = new FirefoxCheckBox();
            this.cbAutoDismiss = new FirefoxCheckBox();
            this.cbDarkMode = new FirefoxCheckBox();
            this.cbStartMinimized = new FirefoxCheckBox();
            this.helpWhatIsUsingMultiplePc = new System.Windows.Forms.LinkLabel();
            this.cbMinimizeToTray = new FirefoxCheckBox();
            this.helpWhatIsAugmentAsItem = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsRecipeAsItems = new System.Windows.Forms.LinkLabel();
            this.cbShowAugments = new FirefoxCheckBox();
            this.cbDualComputer = new FirefoxCheckBox();
            this.linkSourceCode = new System.Windows.Forms.LinkLabel();
            this.cbHideSkills = new FirefoxCheckBox();
            this.cbShowRecipesAsItems = new FirefoxCheckBox();
            this.pbAutomaticUpdates = new PanelBox();
            this.linkDowngrade = new System.Windows.Forms.LinkLabel();
            this.linkCheckForUpdates = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsExperimentalUpdates = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsRegularUpdates = new System.Windows.Forms.LinkLabel();
            this.radioBeta = new FirefoxRadioButton();
            this.radioRelease = new FirefoxRadioButton();
            this.panelBox2 = new PanelBox();
            this.buttonPatreon = new System.Windows.Forms.Button();
            this.buttonPaypal = new System.Windows.Forms.Button();
            this.buttonDevTools = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.firefoxButton1 = new FirefoxButton();
            this.buttonAdvancedSettings = new FirefoxButton();
            this.buttonImportExport = new FirefoxButton();
            this.buttonMigratePostgres = new FirefoxButton();
            this.buttonLanguageSelect = new FirefoxButton();
            this.buttonViewBackups = new FirefoxButton();
            this.buttonViewLogs = new FirefoxButton();
            this.contextMenuStrip1.SuspendLayout();
            this.panelBox4.SuspendLayout();
            this.pbAutomaticUpdates.SuspendLayout();
            this.panelBox2.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.ContextMenuStrip = this.contextMenuStrip1;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(595, 458);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(269, 33);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "iatag_ui_help";
            this.linkLabel1.Text = "Discord / Help";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // panelBox4
            // 
            this.panelBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox4.Controls.Add(this.helpWhatIsEnableDowngrades);
            this.panelBox4.Controls.Add(this.cbEnableDowngrades);
            this.panelBox4.Controls.Add(this.cbAutoDismiss);
            this.panelBox4.Controls.Add(this.cbDarkMode);
            this.panelBox4.Controls.Add(this.cbStartMinimized);
            this.panelBox4.Controls.Add(this.helpWhatIsUsingMultiplePc);
            this.panelBox4.Controls.Add(this.cbMinimizeToTray);
            this.panelBox4.Controls.Add(this.helpWhatIsAugmentAsItem);
            this.panelBox4.Controls.Add(this.helpWhatIsRecipeAsItems);
            this.panelBox4.Controls.Add(this.cbShowAugments);
            this.panelBox4.Controls.Add(this.cbDualComputer);
            this.panelBox4.Controls.Add(this.linkSourceCode);
            this.panelBox4.Controls.Add(this.cbHideSkills);
            this.panelBox4.Controls.Add(this.cbShowRecipesAsItems);
            this.panelBox4.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox4.ForeColor = System.Drawing.Color.Black;
            this.panelBox4.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox4.HeaderHeight = 40;
            this.panelBox4.Location = new System.Drawing.Point(496, 12);
            this.panelBox4.Name = "panelBox4";
            this.panelBox4.NoRounding = false;
            this.panelBox4.Size = new System.Drawing.Size(287, 439);
            this.panelBox4.TabIndex = 8;
            this.panelBox4.Tag = "iatag_ui_settings_title";
            this.panelBox4.Text = "Settings";
            this.panelBox4.TextLocation = "8; 5";
            // 
            // helpWhatIsEnableDowngrades
            // 
            this.helpWhatIsEnableDowngrades.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsEnableDowngrades.AutoSize = true;
            this.helpWhatIsEnableDowngrades.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsEnableDowngrades.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsEnableDowngrades.Location = new System.Drawing.Point(259, 325);
            this.helpWhatIsEnableDowngrades.Name = "helpWhatIsEnableDowngrades";
            this.helpWhatIsEnableDowngrades.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsEnableDowngrades.TabIndex = 35;
            this.helpWhatIsEnableDowngrades.TabStop = true;
            this.helpWhatIsEnableDowngrades.Tag = "iatag_ui_questionmark";
            this.helpWhatIsEnableDowngrades.Text = " ? ";
            this.helpWhatIsEnableDowngrades.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsEnableDowngrades_LinkClicked);
            // 
            // cbEnableDowngrades
            // 
            this.cbEnableDowngrades.Bold = false;
            this.cbEnableDowngrades.EnabledCalc = true;
            this.cbEnableDowngrades.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbEnableDowngrades.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbEnableDowngrades.IsDarkMode = false;
            this.cbEnableDowngrades.Location = new System.Drawing.Point(3, 318);
            this.cbEnableDowngrades.Name = "cbEnableDowngrades";
            this.cbEnableDowngrades.Size = new System.Drawing.Size(268, 27);
            this.cbEnableDowngrades.TabIndex = 34;
            this.cbEnableDowngrades.Tag = "iatag_ui_enabledowngrades";
            this.cbEnableDowngrades.Text = "Enable downgrades";
            this.cbEnableDowngrades.CheckedChanged += new System.EventHandler(this.cbEnableDowngrades_CheckedChanged);
            // 
            // cbAutoDismiss
            // 
            this.cbAutoDismiss.Bold = false;
            this.cbAutoDismiss.EnabledCalc = true;
            this.cbAutoDismiss.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbAutoDismiss.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbAutoDismiss.IsDarkMode = false;
            this.cbAutoDismiss.Location = new System.Drawing.Point(3, 285);
            this.cbAutoDismiss.Name = "cbAutoDismiss";
            this.cbAutoDismiss.Size = new System.Drawing.Size(268, 27);
            this.cbAutoDismiss.TabIndex = 32;
            this.cbAutoDismiss.Tag = "iatag_ui_autodismiss";
            this.cbAutoDismiss.Text = "Auto dismiss notifications";
            this.cbAutoDismiss.CheckedChanged += new System.EventHandler(this.cbAutoDismiss_CheckedChanged);
            // 
            // cbDarkMode
            // 
            this.cbDarkMode.Bold = false;
            this.cbDarkMode.EnabledCalc = true;
            this.cbDarkMode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDarkMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDarkMode.IsDarkMode = false;
            this.cbDarkMode.Location = new System.Drawing.Point(3, 252);
            this.cbDarkMode.Name = "cbDarkMode";
            this.cbDarkMode.Size = new System.Drawing.Size(268, 27);
            this.cbDarkMode.TabIndex = 31;
            this.cbDarkMode.Tag = "iatag_ui_darkmode";
            this.cbDarkMode.Text = "Dark mode";
            this.cbDarkMode.CheckedChanged += new System.EventHandler(this.cbDarkMode_CheckedChanged);
            // 
            // cbStartMinimized
            // 
            this.cbStartMinimized.Bold = false;
            this.cbStartMinimized.EnabledCalc = true;
            this.cbStartMinimized.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbStartMinimized.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbStartMinimized.IsDarkMode = false;
            this.cbStartMinimized.Location = new System.Drawing.Point(3, 219);
            this.cbStartMinimized.Name = "cbStartMinimized";
            this.cbStartMinimized.Size = new System.Drawing.Size(268, 27);
            this.cbStartMinimized.TabIndex = 30;
            this.cbStartMinimized.Tag = "iatag_ui_startminimized";
            this.cbStartMinimized.Text = "Start minimized";
            this.cbStartMinimized.CheckedChanged += new System.EventHandler(this.cbStartMinimized_CheckedChanged);
            // 
            // helpWhatIsUsingMultiplePc
            // 
            this.helpWhatIsUsingMultiplePc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsUsingMultiplePc.AutoSize = true;
            this.helpWhatIsUsingMultiplePc.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsUsingMultiplePc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsUsingMultiplePc.Location = new System.Drawing.Point(259, 160);
            this.helpWhatIsUsingMultiplePc.Name = "helpWhatIsUsingMultiplePc";
            this.helpWhatIsUsingMultiplePc.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsUsingMultiplePc.TabIndex = 28;
            this.helpWhatIsUsingMultiplePc.TabStop = true;
            this.helpWhatIsUsingMultiplePc.Tag = "iatag_ui_questionmark";
            this.helpWhatIsUsingMultiplePc.Text = " ? ";
            this.helpWhatIsUsingMultiplePc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsUsingMultiplePc_LinkClicked);
            // 
            // cbMinimizeToTray
            // 
            this.cbMinimizeToTray.Bold = false;
            this.cbMinimizeToTray.EnabledCalc = true;
            this.cbMinimizeToTray.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbMinimizeToTray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbMinimizeToTray.IsDarkMode = false;
            this.cbMinimizeToTray.Location = new System.Drawing.Point(3, 186);
            this.cbMinimizeToTray.Name = "cbMinimizeToTray";
            this.cbMinimizeToTray.Size = new System.Drawing.Size(268, 27);
            this.cbMinimizeToTray.TabIndex = 8;
            this.cbMinimizeToTray.Tag = "iatag_ui_minimizetotray";
            this.cbMinimizeToTray.Text = "Minimize to Tray";
            // 
            // helpWhatIsAugmentAsItem
            // 
            this.helpWhatIsAugmentAsItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsAugmentAsItem.AutoSize = true;
            this.helpWhatIsAugmentAsItem.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsAugmentAsItem.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsAugmentAsItem.Location = new System.Drawing.Point(259, 127);
            this.helpWhatIsAugmentAsItem.Name = "helpWhatIsAugmentAsItem";
            this.helpWhatIsAugmentAsItem.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsAugmentAsItem.TabIndex = 24;
            this.helpWhatIsAugmentAsItem.TabStop = true;
            this.helpWhatIsAugmentAsItem.Tag = "iatag_ui_questionmark";
            this.helpWhatIsAugmentAsItem.Text = " ? ";
            this.helpWhatIsAugmentAsItem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsAugmentAsItem_LinkClicked);
            // 
            // helpWhatIsRecipeAsItems
            // 
            this.helpWhatIsRecipeAsItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsRecipeAsItems.AutoSize = true;
            this.helpWhatIsRecipeAsItems.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsRecipeAsItems.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsRecipeAsItems.Location = new System.Drawing.Point(259, 61);
            this.helpWhatIsRecipeAsItems.Name = "helpWhatIsRecipeAsItems";
            this.helpWhatIsRecipeAsItems.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsRecipeAsItems.TabIndex = 21;
            this.helpWhatIsRecipeAsItems.TabStop = true;
            this.helpWhatIsRecipeAsItems.Tag = "iatag_ui_questionmark";
            this.helpWhatIsRecipeAsItems.Text = " ? ";
            this.helpWhatIsRecipeAsItems.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsRecipeAsItems_LinkClicked);
            // 
            // cbShowAugments
            // 
            this.cbShowAugments.Bold = false;
            this.cbShowAugments.EnabledCalc = true;
            this.cbShowAugments.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbShowAugments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbShowAugments.IsDarkMode = false;
            this.cbShowAugments.Location = new System.Drawing.Point(3, 120);
            this.cbShowAugments.Name = "cbShowAugments";
            this.cbShowAugments.Size = new System.Drawing.Size(268, 27);
            this.cbShowAugments.TabIndex = 23;
            this.cbShowAugments.Tag = "iatag_ui_showaugments";
            this.cbShowAugments.Text = "Show augments as Items";
            this.cbShowAugments.CheckedChanged += new System.EventHandler(this.cbShowAugments_CheckedChanged);
            // 
            // cbDualComputer
            // 
            this.cbDualComputer.Bold = false;
            this.cbDualComputer.EnabledCalc = true;
            this.cbDualComputer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDualComputer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDualComputer.IsDarkMode = false;
            this.cbDualComputer.Location = new System.Drawing.Point(3, 153);
            this.cbDualComputer.Name = "cbDualComputer";
            this.cbDualComputer.Size = new System.Drawing.Size(268, 27);
            this.cbDualComputer.TabIndex = 22;
            this.cbDualComputer.Tag = "iatag_ui_dualcomputer";
            this.cbDualComputer.Text = "Using IA on multiple PCs";
            this.cbDualComputer.CheckedChanged += new System.EventHandler(this.cbDualComputer_CheckedChanged);
            // 
            // linkSourceCode
            // 
            this.linkSourceCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkSourceCode.AutoSize = true;
            this.linkSourceCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkSourceCode.Location = new System.Drawing.Point(5, 417);
            this.linkSourceCode.Name = "linkSourceCode";
            this.linkSourceCode.Size = new System.Drawing.Size(68, 13);
            this.linkSourceCode.TabIndex = 21;
            this.linkSourceCode.TabStop = true;
            this.linkSourceCode.Tag = "iatag_ui_source_code";
            this.linkSourceCode.Text = "Source code";
            this.linkSourceCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSourceCode_LinkClicked);
            // 
            // cbHideSkills
            // 
            this.cbHideSkills.Bold = false;
            this.cbHideSkills.EnabledCalc = true;
            this.cbHideSkills.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbHideSkills.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbHideSkills.IsDarkMode = false;
            this.cbHideSkills.Location = new System.Drawing.Point(3, 87);
            this.cbHideSkills.Name = "cbHideSkills";
            this.cbHideSkills.Size = new System.Drawing.Size(268, 27);
            this.cbHideSkills.TabIndex = 17;
            this.cbHideSkills.Tag = "iatag_ui_hideskills";
            this.cbHideSkills.Text = "Hide Skills";
            this.cbHideSkills.CheckedChanged += new System.EventHandler(this.cbDisplaySkills_CheckedChanged);
            // 
            // cbShowRecipesAsItems
            // 
            this.cbShowRecipesAsItems.Bold = false;
            this.cbShowRecipesAsItems.EnabledCalc = true;
            this.cbShowRecipesAsItems.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbShowRecipesAsItems.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbShowRecipesAsItems.IsDarkMode = false;
            this.cbShowRecipesAsItems.Location = new System.Drawing.Point(3, 54);
            this.cbShowRecipesAsItems.Name = "cbShowRecipesAsItems";
            this.cbShowRecipesAsItems.Size = new System.Drawing.Size(268, 27);
            this.cbShowRecipesAsItems.TabIndex = 7;
            this.cbShowRecipesAsItems.Tag = "iatag_ui_showrecipesasitems";
            this.cbShowRecipesAsItems.Text = "Show recipes as items";
            this.cbShowRecipesAsItems.CheckedChanged += new System.EventHandler(this.cbShowRecipesAsItems_CheckedChanged);
            // 
            // pbAutomaticUpdates
            // 
            this.pbAutomaticUpdates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbAutomaticUpdates.Controls.Add(this.linkDowngrade);
            this.pbAutomaticUpdates.Controls.Add(this.linkCheckForUpdates);
            this.pbAutomaticUpdates.Controls.Add(this.helpWhatIsExperimentalUpdates);
            this.pbAutomaticUpdates.Controls.Add(this.helpWhatIsRegularUpdates);
            this.pbAutomaticUpdates.Controls.Add(this.radioBeta);
            this.pbAutomaticUpdates.Controls.Add(this.radioRelease);
            this.pbAutomaticUpdates.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.pbAutomaticUpdates.ForeColor = System.Drawing.Color.Black;
            this.pbAutomaticUpdates.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.pbAutomaticUpdates.HeaderHeight = 40;
            this.pbAutomaticUpdates.Location = new System.Drawing.Point(12, 318);
            this.pbAutomaticUpdates.Name = "pbAutomaticUpdates";
            this.pbAutomaticUpdates.NoRounding = false;
            this.pbAutomaticUpdates.Size = new System.Drawing.Size(478, 133);
            this.pbAutomaticUpdates.TabIndex = 6;
            this.pbAutomaticUpdates.Tag = "iatag_ui_update_title";
            this.pbAutomaticUpdates.Text = "Automatic Updates";
            this.pbAutomaticUpdates.TextLocation = "8; 5";
            // 
            // linkDowngrade
            // 
            this.linkDowngrade.AutoSize = true;
            this.linkDowngrade.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.linkDowngrade.Location = new System.Drawing.Point(374, 66);
            this.linkDowngrade.Name = "linkDowngrade";
            this.linkDowngrade.Size = new System.Drawing.Size(68, 13);
            this.linkDowngrade.TabIndex = 32;
            this.linkDowngrade.TabStop = true;
            this.linkDowngrade.Tag = "iatag_ui_downgrade_ia";
            this.linkDowngrade.Text = "Downgrade";
            this.linkDowngrade.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDowngrade_LinkClicked);
            // 
            // linkCheckForUpdates
            // 
            this.linkCheckForUpdates.AutoSize = true;
            this.linkCheckForUpdates.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.linkCheckForUpdates.Location = new System.Drawing.Point(374, 47);
            this.linkCheckForUpdates.Name = "linkCheckForUpdates";
            this.linkCheckForUpdates.Size = new System.Drawing.Size(101, 13);
            this.linkCheckForUpdates.TabIndex = 21;
            this.linkCheckForUpdates.TabStop = true;
            this.linkCheckForUpdates.Tag = "iatag_ui_checkforupdates";
            this.linkCheckForUpdates.Text = "Check for updates";
            this.linkCheckForUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // helpWhatIsExperimentalUpdates
            // 
            this.helpWhatIsExperimentalUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsExperimentalUpdates.AutoSize = true;
            this.helpWhatIsExperimentalUpdates.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsExperimentalUpdates.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsExperimentalUpdates.Location = new System.Drawing.Point(170, 93);
            this.helpWhatIsExperimentalUpdates.Name = "helpWhatIsExperimentalUpdates";
            this.helpWhatIsExperimentalUpdates.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsExperimentalUpdates.TabIndex = 31;
            this.helpWhatIsExperimentalUpdates.TabStop = true;
            this.helpWhatIsExperimentalUpdates.Tag = "iatag_ui_questionmark";
            this.helpWhatIsExperimentalUpdates.Text = " ? ";
            this.helpWhatIsExperimentalUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsExperimentalUpdates_LinkClicked);
            // 
            // helpWhatIsRegularUpdates
            // 
            this.helpWhatIsRegularUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsRegularUpdates.AutoSize = true;
            this.helpWhatIsRegularUpdates.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsRegularUpdates.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsRegularUpdates.Location = new System.Drawing.Point(170, 66);
            this.helpWhatIsRegularUpdates.Name = "helpWhatIsRegularUpdates";
            this.helpWhatIsRegularUpdates.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsRegularUpdates.TabIndex = 30;
            this.helpWhatIsRegularUpdates.TabStop = true;
            this.helpWhatIsRegularUpdates.Tag = "iatag_ui_questionmark";
            this.helpWhatIsRegularUpdates.Text = " ? ";
            this.helpWhatIsRegularUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsRegularUpdates_LinkClicked);
            // 
            // radioBeta
            // 
            this.radioBeta.Bold = false;
            this.radioBeta.Checked = false;
            this.radioBeta.EnabledCalc = true;
            this.radioBeta.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioBeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioBeta.Location = new System.Drawing.Point(9, 87);
            this.radioBeta.Name = "radioBeta";
            this.radioBeta.Size = new System.Drawing.Size(454, 27);
            this.radioBeta.TabIndex = 1;
            this.radioBeta.Tag = "iatag_ui_experimentalupdates";
            this.radioBeta.Text = "Frequent Updates";
            this.radioBeta.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioBeta_CheckedChanged);
            // 
            // radioRelease
            // 
            this.radioRelease.Bold = false;
            this.radioRelease.Checked = false;
            this.radioRelease.EnabledCalc = true;
            this.radioRelease.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioRelease.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioRelease.Location = new System.Drawing.Point(9, 54);
            this.radioRelease.Name = "radioRelease";
            this.radioRelease.Size = new System.Drawing.Size(454, 27);
            this.radioRelease.TabIndex = 0;
            this.radioRelease.Tag = "iatag_ui_regularupdates";
            this.radioRelease.Text = "Regular Updates";
            this.radioRelease.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioRelease_CheckedChanged);
            // 
            // panelBox2
            // 
            this.panelBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox2.Controls.Add(this.buttonPatreon);
            this.panelBox2.Controls.Add(this.buttonPaypal);
            this.panelBox2.Controls.Add(this.buttonDevTools);
            this.panelBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox2.ForeColor = System.Drawing.Color.Black;
            this.panelBox2.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox2.HeaderHeight = 40;
            this.panelBox2.Location = new System.Drawing.Point(260, 12);
            this.panelBox2.Name = "panelBox2";
            this.panelBox2.NoRounding = false;
            this.panelBox2.Size = new System.Drawing.Size(230, 300);
            this.panelBox2.TabIndex = 2;
            this.panelBox2.Tag = "iatag_ui_misc_title";
            this.panelBox2.Text = "Misc";
            this.panelBox2.TextLocation = "8; 5";
            // 
            // buttonPatreon
            // 
            this.buttonPatreon.BackgroundImage = global::IAGrim.Properties.Resources.Patreon_Navy;
            this.buttonPatreon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPatreon.Location = new System.Drawing.Point(14, 111);
            this.buttonPatreon.Name = "buttonPatreon";
            this.buttonPatreon.Size = new System.Drawing.Size(192, 47);
            this.buttonPatreon.TabIndex = 8;
            this.buttonPatreon.UseVisualStyleBackColor = true;
            this.buttonPatreon.Click += new System.EventHandler(this.buttonPatreon_Click);
            // 
            // buttonPaypal
            // 
            this.buttonPaypal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPaypal.BackgroundImage = global::IAGrim.Properties.Resources.donate;
            this.buttonPaypal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPaypal.Location = new System.Drawing.Point(14, 51);
            this.buttonPaypal.Name = "buttonPaypal";
            this.buttonPaypal.Size = new System.Drawing.Size(192, 55);
            this.buttonPaypal.TabIndex = 7;
            this.buttonPaypal.UseVisualStyleBackColor = true;
            this.buttonPaypal.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonDevTools
            // 
            this.buttonDevTools.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonDevTools.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonDevTools.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonDevTools.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonDevTools.EnabledCalc = true;
            this.buttonDevTools.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonDevTools.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonDevTools.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonDevTools.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonDevTools.Location = new System.Drawing.Point(14, 165);
            this.buttonDevTools.Name = "buttonDevTools";
            this.buttonDevTools.Size = new System.Drawing.Size(192, 32);
            this.buttonDevTools.TabIndex = 6;
            this.buttonDevTools.Tag = "iatag_ui_devtools";
            this.buttonDevTools.Text = "Devtools";
            this.buttonDevTools.Click += new System.EventHandler(this.buttonDevTools_Click);
            // 
            // panelBox1
            // 
            this.panelBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox1.Controls.Add(this.firefoxButton1);
            this.panelBox1.Controls.Add(this.buttonAdvancedSettings);
            this.panelBox1.Controls.Add(this.buttonImportExport);
            this.panelBox1.Controls.Add(this.buttonMigratePostgres);
            this.panelBox1.Controls.Add(this.buttonLanguageSelect);
            this.panelBox1.Controls.Add(this.buttonViewBackups);
            this.panelBox1.Controls.Add(this.buttonViewLogs);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.ForeColor = System.Drawing.Color.Black;
            this.panelBox1.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox1.HeaderHeight = 40;
            this.panelBox1.Location = new System.Drawing.Point(12, 12);
            this.panelBox1.Name = "panelBox1";
            this.panelBox1.NoRounding = false;
            this.panelBox1.Size = new System.Drawing.Size(230, 300);
            this.panelBox1.TabIndex = 1;
            this.panelBox1.Tag = "iatag_ui_actions_title";
            this.panelBox1.Text = "Actions";
            this.panelBox1.TextLocation = "8; 5";
            // 
            // firefoxButton1
            // 
            this.firefoxButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.firefoxButton1.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.firefoxButton1.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.firefoxButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.firefoxButton1.EnabledCalc = true;
            this.firefoxButton1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.firefoxButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.firefoxButton1.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.firefoxButton1.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.firefoxButton1.Location = new System.Drawing.Point(19, 241);
            this.firefoxButton1.Name = "firefoxButton1";
            this.firefoxButton1.Size = new System.Drawing.Size(192, 32);
            this.firefoxButton1.TabIndex = 9;
            this.firefoxButton1.Tag = "iatag_ui_configmode";
            this.firefoxButton1.Text = "Switch to modes";
            // 
            // buttonAdvancedSettings
            // 
            this.buttonAdvancedSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.buttonAdvancedSettings.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonAdvancedSettings.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonAdvancedSettings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonAdvancedSettings.EnabledCalc = true;
            this.buttonAdvancedSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonAdvancedSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonAdvancedSettings.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonAdvancedSettings.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonAdvancedSettings.Location = new System.Drawing.Point(19, 203);
            this.buttonAdvancedSettings.Name = "buttonAdvancedSettings";
            this.buttonAdvancedSettings.Size = new System.Drawing.Size(192, 32);
            this.buttonAdvancedSettings.TabIndex = 8;
            this.buttonAdvancedSettings.Tag = "iatag_ui_advancedsettings";
            this.buttonAdvancedSettings.Text = "Reconfigure tabs";
            this.buttonAdvancedSettings.Click += new System.EventHandler(this.buttonAdvancedSettings_Click);
            // 
            // buttonImportExport
            // 
            this.buttonImportExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.buttonImportExport.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonImportExport.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonImportExport.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonImportExport.EnabledCalc = true;
            this.buttonImportExport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonImportExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonImportExport.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonImportExport.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonImportExport.Location = new System.Drawing.Point(19, 165);
            this.buttonImportExport.Name = "buttonImportExport";
            this.buttonImportExport.Size = new System.Drawing.Size(192, 32);
            this.buttonImportExport.TabIndex = 7;
            this.buttonImportExport.Tag = "iatag_ui_importexport";
            this.buttonImportExport.Text = "Import/Export";
            this.buttonImportExport.Click += new System.EventHandler(this.buttonImportExport_Click);
            // 
            // buttonMigratePostgres
            // 
            this.buttonMigratePostgres.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonMigratePostgres.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonMigratePostgres.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonMigratePostgres.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonMigratePostgres.EnabledCalc = true;
            this.buttonMigratePostgres.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonMigratePostgres.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonMigratePostgres.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonMigratePostgres.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonMigratePostgres.Location = new System.Drawing.Point(0, 0);
            this.buttonMigratePostgres.Name = "buttonMigratePostgres";
            this.buttonMigratePostgres.Size = new System.Drawing.Size(0, 0);
            this.buttonMigratePostgres.TabIndex = 0;
            // 
            // buttonLanguageSelect
            // 
            this.buttonLanguageSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.buttonLanguageSelect.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonLanguageSelect.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonLanguageSelect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLanguageSelect.EnabledCalc = true;
            this.buttonLanguageSelect.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLanguageSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLanguageSelect.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonLanguageSelect.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLanguageSelect.Location = new System.Drawing.Point(19, 127);
            this.buttonLanguageSelect.Name = "buttonLanguageSelect";
            this.buttonLanguageSelect.Size = new System.Drawing.Size(192, 32);
            this.buttonLanguageSelect.TabIndex = 6;
            this.buttonLanguageSelect.Tag = "iatag_ui_language_select";
            this.buttonLanguageSelect.Text = "Language Select";
            this.buttonLanguageSelect.Click += new System.EventHandler(this.buttonLanguageSelect_Click);
            // 
            // buttonViewBackups
            // 
            this.buttonViewBackups.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.buttonViewBackups.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonViewBackups.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonViewBackups.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonViewBackups.EnabledCalc = true;
            this.buttonViewBackups.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonViewBackups.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonViewBackups.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonViewBackups.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonViewBackups.Location = new System.Drawing.Point(19, 89);
            this.buttonViewBackups.Name = "buttonViewBackups";
            this.buttonViewBackups.Size = new System.Drawing.Size(192, 32);
            this.buttonViewBackups.TabIndex = 5;
            this.buttonViewBackups.Tag = "iatag_ui_viewbackups";
            this.buttonViewBackups.Text = "View Backups";
            this.buttonViewBackups.Click += new System.EventHandler(this.buttonViewBackups_Click);
            // 
            // buttonViewLogs
            // 
            this.buttonViewLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.buttonViewLogs.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonViewLogs.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonViewLogs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonViewLogs.EnabledCalc = true;
            this.buttonViewLogs.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonViewLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonViewLogs.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonViewLogs.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonViewLogs.Location = new System.Drawing.Point(19, 51);
            this.buttonViewLogs.Name = "buttonViewLogs";
            this.buttonViewLogs.Size = new System.Drawing.Size(192, 32);
            this.buttonViewLogs.TabIndex = 6;
            this.buttonViewLogs.Tag = "iatag_ui_viewlogs";
            this.buttonViewLogs.Text = "View Logs";
            this.buttonViewLogs.Click += new System.EventHandler(this.buttonViewLogs_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 500);
            this.Controls.Add(this.panelBox4);
            this.Controls.Add(this.pbAutomaticUpdates);
            this.Controls.Add(this.panelBox2);
            this.Controls.Add(this.panelBox1);
            this.Controls.Add(this.linkLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panelBox4.ResumeLayout(false);
            this.panelBox4.PerformLayout();
            this.pbAutomaticUpdates.ResumeLayout(false);
            this.pbAutomaticUpdates.PerformLayout();
            this.panelBox2.ResumeLayout(false);
            this.panelBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox1;
        private PanelBox panelBox2;
        private FirefoxButton buttonViewBackups;
        private FirefoxButton buttonViewLogs;
        private PanelBox pbAutomaticUpdates;
        private FirefoxRadioButton radioBeta;
        private FirefoxRadioButton radioRelease;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private FirefoxCheckBox cbMinimizeToTray;
        private FirefoxCheckBox cbShowRecipesAsItems;
        private PanelBox panelBox4;
        private FirefoxButton buttonLanguageSelect;
        private FirefoxButton buttonMigratePostgres;
        private FirefoxButton buttonImportExport;
        private FirefoxCheckBox cbHideSkills;
        private FirefoxButton buttonAdvancedSettings;
        private System.Windows.Forms.LinkLabel linkSourceCode;
        private FirefoxCheckBox cbDualComputer;
        private FirefoxCheckBox cbShowAugments;
        private FirefoxButton buttonDevTools;
        private System.Windows.Forms.LinkLabel helpWhatIsRecipeAsItems;
        private System.Windows.Forms.LinkLabel helpWhatIsAugmentAsItem;
        private System.Windows.Forms.LinkLabel helpWhatIsUsingMultiplePc;
        private System.Windows.Forms.Button buttonPaypal;
        private System.Windows.Forms.Button buttonPatreon;
        private System.Windows.Forms.LinkLabel helpWhatIsExperimentalUpdates;
        private System.Windows.Forms.LinkLabel helpWhatIsRegularUpdates;
        private FirefoxCheckBox cbStartMinimized;
        private FirefoxCheckBox cbDarkMode;
        private System.Windows.Forms.LinkLabel linkCheckForUpdates;
        private FirefoxCheckBox cbAutoDismiss;
        private FirefoxCheckBox cbEnableDowngrades;
        private System.Windows.Forms.LinkLabel helpWhatIsEnableDowngrades;
        private System.Windows.Forms.LinkLabel linkDowngrade;
        private FirefoxButton firefoxButton1;
    }
}