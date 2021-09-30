namespace IAGrim.UI.Tabs
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
            this.cbDarkMode = new FirefoxCheckBox();
            this.cbStartMinimized = new FirefoxCheckBox();
            this.helpWhatIsDeleteDuplicates = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsUsingMultiplePc = new System.Windows.Forms.LinkLabel();
            this.cbDeleteDuplicates = new FirefoxCheckBox();
            this.helpWhatIsTransferToAnyMod = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsSecureTransfers = new System.Windows.Forms.LinkLabel();
            this.cbMinimizeToTray = new FirefoxCheckBox();
            this.helpWhatIsAugmentAsItem = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsRecipeAsItems = new System.Windows.Forms.LinkLabel();
            this.cbShowAugments = new FirefoxCheckBox();
            this.cbDualComputer = new FirefoxCheckBox();
            this.linkSourceCode = new System.Windows.Forms.LinkLabel();
            this.cbHideSkills = new FirefoxCheckBox();
            this.cbAutoUpdateModSettings = new FirefoxCheckBox();
            this.cbTransferAnyMod = new FirefoxCheckBox();
            this.cbShowRecipesAsItems = new FirefoxCheckBox();
            this.cbSecureTransfers = new FirefoxCheckBox();
            this.panelBox3 = new PanelBox();
            this.helpWhatIsExperimentalUpdates = new System.Windows.Forms.LinkLabel();
            this.helpWhatIsRegularUpdates = new System.Windows.Forms.LinkLabel();
            this.radioBeta = new FirefoxRadioButton();
            this.radioRelease = new FirefoxRadioButton();
            this.panelBox2 = new PanelBox();
            this.buttonLootManually = new FirefoxButton();
            this.buttonPatreon = new System.Windows.Forms.Button();
            this.buttonPaypal = new System.Windows.Forms.Button();
            this.buttonDevTools = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.buttonAdvancedSettings = new FirefoxButton();
            this.buttonImportExport = new FirefoxButton();
            this.buttonMigratePostgres = new FirefoxButton();
            this.buttonLanguageSelect = new FirefoxButton();
            this.buttonViewBackups = new FirefoxButton();
            this.buttonViewLogs = new FirefoxButton();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.panelBox4.SuspendLayout();
            this.panelBox3.SuspendLayout();
            this.panelBox2.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.ContextMenuStrip = this.contextMenuStrip1;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(595, 469);
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
            this.panelBox4.Controls.Add(this.cbDarkMode);
            this.panelBox4.Controls.Add(this.cbStartMinimized);
            this.panelBox4.Controls.Add(this.helpWhatIsDeleteDuplicates);
            this.panelBox4.Controls.Add(this.helpWhatIsUsingMultiplePc);
            this.panelBox4.Controls.Add(this.cbDeleteDuplicates);
            this.panelBox4.Controls.Add(this.helpWhatIsTransferToAnyMod);
            this.panelBox4.Controls.Add(this.helpWhatIsSecureTransfers);
            this.panelBox4.Controls.Add(this.cbMinimizeToTray);
            this.panelBox4.Controls.Add(this.helpWhatIsAugmentAsItem);
            this.panelBox4.Controls.Add(this.helpWhatIsRecipeAsItems);
            this.panelBox4.Controls.Add(this.cbShowAugments);
            this.panelBox4.Controls.Add(this.cbDualComputer);
            this.panelBox4.Controls.Add(this.linkSourceCode);
            this.panelBox4.Controls.Add(this.cbHideSkills);
            this.panelBox4.Controls.Add(this.cbAutoUpdateModSettings);
            this.panelBox4.Controls.Add(this.cbTransferAnyMod);
            this.panelBox4.Controls.Add(this.cbShowRecipesAsItems);
            this.panelBox4.Controls.Add(this.cbSecureTransfers);
            this.panelBox4.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox4.ForeColor = System.Drawing.Color.Black;
            this.panelBox4.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox4.HeaderHeight = 40;
            this.panelBox4.Location = new System.Drawing.Point(496, 12);
            this.panelBox4.Name = "panelBox4";
            this.panelBox4.NoRounding = false;
            this.panelBox4.Size = new System.Drawing.Size(287, 490);
            this.panelBox4.TabIndex = 8;
            this.panelBox4.Tag = "iatag_ui_settings_title";
            this.panelBox4.Text = "Settings";
            this.panelBox4.TextLocation = "8; 5";
            // 
            // cbDarkMode
            // 
            this.cbDarkMode.Bold = false;
            this.cbDarkMode.EnabledCalc = true;
            this.cbDarkMode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDarkMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDarkMode.IsDarkMode = false;
            this.cbDarkMode.Location = new System.Drawing.Point(3, 351);
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
            this.cbStartMinimized.Location = new System.Drawing.Point(3, 318);
            this.cbStartMinimized.Name = "cbStartMinimized";
            this.cbStartMinimized.Size = new System.Drawing.Size(268, 27);
            this.cbStartMinimized.TabIndex = 30;
            this.cbStartMinimized.Tag = "iatag_ui_startminimized";
            this.cbStartMinimized.Text = "Start minimized";
            this.cbStartMinimized.CheckedChanged += new System.EventHandler(this.cbStartMinimized_CheckedChanged);
            // 
            // helpWhatIsDeleteDuplicates
            // 
            this.helpWhatIsDeleteDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsDeleteDuplicates.AutoSize = true;
            this.helpWhatIsDeleteDuplicates.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsDeleteDuplicates.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsDeleteDuplicates.Location = new System.Drawing.Point(259, 259);
            this.helpWhatIsDeleteDuplicates.Name = "helpWhatIsDeleteDuplicates";
            this.helpWhatIsDeleteDuplicates.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsDeleteDuplicates.TabIndex = 29;
            this.helpWhatIsDeleteDuplicates.TabStop = true;
            this.helpWhatIsDeleteDuplicates.Tag = "iatag_ui_questionmark";
            this.helpWhatIsDeleteDuplicates.Text = " ? ";
            this.helpWhatIsDeleteDuplicates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsDeleteDuplicates_LinkClicked);
            // 
            // helpWhatIsUsingMultiplePc
            // 
            this.helpWhatIsUsingMultiplePc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsUsingMultiplePc.AutoSize = true;
            this.helpWhatIsUsingMultiplePc.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsUsingMultiplePc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsUsingMultiplePc.Location = new System.Drawing.Point(259, 226);
            this.helpWhatIsUsingMultiplePc.Name = "helpWhatIsUsingMultiplePc";
            this.helpWhatIsUsingMultiplePc.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsUsingMultiplePc.TabIndex = 28;
            this.helpWhatIsUsingMultiplePc.TabStop = true;
            this.helpWhatIsUsingMultiplePc.Tag = "iatag_ui_questionmark";
            this.helpWhatIsUsingMultiplePc.Text = " ? ";
            this.helpWhatIsUsingMultiplePc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsUsingMultiplePc_LinkClicked);
            // 
            // cbDeleteDuplicates
            // 
            this.cbDeleteDuplicates.Bold = false;
            this.cbDeleteDuplicates.EnabledCalc = true;
            this.cbDeleteDuplicates.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDeleteDuplicates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDeleteDuplicates.IsDarkMode = false;
            this.cbDeleteDuplicates.Location = new System.Drawing.Point(3, 252);
            this.cbDeleteDuplicates.Name = "cbDeleteDuplicates";
            this.cbDeleteDuplicates.Size = new System.Drawing.Size(268, 27);
            this.cbDeleteDuplicates.TabIndex = 27;
            this.cbDeleteDuplicates.Tag = "iatag_ui_deleteduplicates";
            this.cbDeleteDuplicates.Text = "Delete bugged duplicates";
            this.cbDeleteDuplicates.CheckedChanged += new System.EventHandler(this.cbDeleteDuplicates_CheckedChanged);
            // 
            // helpWhatIsTransferToAnyMod
            // 
            this.helpWhatIsTransferToAnyMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsTransferToAnyMod.AutoSize = true;
            this.helpWhatIsTransferToAnyMod.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsTransferToAnyMod.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsTransferToAnyMod.Location = new System.Drawing.Point(259, 94);
            this.helpWhatIsTransferToAnyMod.Name = "helpWhatIsTransferToAnyMod";
            this.helpWhatIsTransferToAnyMod.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsTransferToAnyMod.TabIndex = 26;
            this.helpWhatIsTransferToAnyMod.TabStop = true;
            this.helpWhatIsTransferToAnyMod.Tag = "iatag_ui_questionmark";
            this.helpWhatIsTransferToAnyMod.Text = " ? ";
            this.helpWhatIsTransferToAnyMod.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsTransferToAnyMod_LinkClicked);
            // 
            // helpWhatIsSecureTransfers
            // 
            this.helpWhatIsSecureTransfers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpWhatIsSecureTransfers.AutoSize = true;
            this.helpWhatIsSecureTransfers.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsSecureTransfers.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsSecureTransfers.Location = new System.Drawing.Point(259, 127);
            this.helpWhatIsSecureTransfers.Name = "helpWhatIsSecureTransfers";
            this.helpWhatIsSecureTransfers.Size = new System.Drawing.Size(18, 13);
            this.helpWhatIsSecureTransfers.TabIndex = 25;
            this.helpWhatIsSecureTransfers.TabStop = true;
            this.helpWhatIsSecureTransfers.Tag = "iatag_ui_questionmark";
            this.helpWhatIsSecureTransfers.Text = " ? ";
            this.helpWhatIsSecureTransfers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsSecureTransfers_LinkClicked);
            // 
            // cbMinimizeToTray
            // 
            this.cbMinimizeToTray.Bold = false;
            this.cbMinimizeToTray.EnabledCalc = true;
            this.cbMinimizeToTray.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbMinimizeToTray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbMinimizeToTray.IsDarkMode = false;
            this.cbMinimizeToTray.Location = new System.Drawing.Point(3, 285);
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
            this.helpWhatIsAugmentAsItem.Location = new System.Drawing.Point(259, 193);
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
            this.cbShowAugments.Location = new System.Drawing.Point(3, 186);
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
            this.cbDualComputer.Location = new System.Drawing.Point(3, 219);
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
            this.linkSourceCode.Location = new System.Drawing.Point(5, 468);
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
            this.cbHideSkills.Location = new System.Drawing.Point(3, 153);
            this.cbHideSkills.Name = "cbHideSkills";
            this.cbHideSkills.Size = new System.Drawing.Size(268, 27);
            this.cbHideSkills.TabIndex = 17;
            this.cbHideSkills.Tag = "iatag_ui_hideskills";
            this.cbHideSkills.Text = "Hide Skills";
            this.cbHideSkills.CheckedChanged += new System.EventHandler(this.cbDisplaySkills_CheckedChanged);
            // 
            // cbAutoUpdateModSettings
            // 
            this.cbAutoUpdateModSettings.Bold = false;
            this.cbAutoUpdateModSettings.Enabled = false;
            this.cbAutoUpdateModSettings.EnabledCalc = true;
            this.cbAutoUpdateModSettings.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoUpdateModSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbAutoUpdateModSettings.IsDarkMode = false;
            this.cbAutoUpdateModSettings.Location = new System.Drawing.Point(3, 436);
            this.cbAutoUpdateModSettings.Name = "cbAutoUpdateModSettings";
            this.cbAutoUpdateModSettings.Size = new System.Drawing.Size(268, 27);
            this.cbAutoUpdateModSettings.TabIndex = 15;
            this.cbAutoUpdateModSettings.Tag = "iatag_ui_autoselectmod";
            this.cbAutoUpdateModSettings.Text = "Auto Select Mod";
            this.cbAutoUpdateModSettings.UseVisualStyleBackColor = true;
            this.cbAutoUpdateModSettings.Visible = false;
            // 
            // cbTransferAnyMod
            // 
            this.cbTransferAnyMod.Bold = false;
            this.cbTransferAnyMod.EnabledCalc = true;
            this.cbTransferAnyMod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbTransferAnyMod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbTransferAnyMod.IsDarkMode = false;
            this.cbTransferAnyMod.Location = new System.Drawing.Point(3, 87);
            this.cbTransferAnyMod.Name = "cbTransferAnyMod";
            this.cbTransferAnyMod.Size = new System.Drawing.Size(268, 27);
            this.cbTransferAnyMod.TabIndex = 14;
            this.cbTransferAnyMod.Tag = "iatag_ui_transferanymod";
            this.cbTransferAnyMod.Text = "Transfer to any mod";
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
            // cbSecureTransfers
            // 
            this.cbSecureTransfers.Bold = false;
            this.cbSecureTransfers.EnabledCalc = true;
            this.cbSecureTransfers.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbSecureTransfers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbSecureTransfers.IsDarkMode = false;
            this.cbSecureTransfers.Location = new System.Drawing.Point(3, 120);
            this.cbSecureTransfers.Name = "cbSecureTransfers";
            this.cbSecureTransfers.Size = new System.Drawing.Size(268, 27);
            this.cbSecureTransfers.TabIndex = 12;
            this.cbSecureTransfers.Tag = "iatag_ui_securetransfers";
            this.cbSecureTransfers.Text = "Secure Transfers";
            // 
            // panelBox3
            // 
            this.panelBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox3.Controls.Add(this.linkLabel2);
            this.panelBox3.Controls.Add(this.helpWhatIsExperimentalUpdates);
            this.panelBox3.Controls.Add(this.helpWhatIsRegularUpdates);
            this.panelBox3.Controls.Add(this.radioBeta);
            this.panelBox3.Controls.Add(this.radioRelease);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox3.ForeColor = System.Drawing.Color.Black;
            this.panelBox3.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox3.HeaderHeight = 40;
            this.panelBox3.Location = new System.Drawing.Point(12, 264);
            this.panelBox3.Name = "panelBox3";
            this.panelBox3.NoRounding = false;
            this.panelBox3.Size = new System.Drawing.Size(478, 133);
            this.panelBox3.TabIndex = 6;
            this.panelBox3.Tag = "iatag_ui_update_title";
            this.panelBox3.Text = "Automatic Updates";
            this.panelBox3.TextLocation = "8; 5";
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
            this.panelBox2.Controls.Add(this.buttonLootManually);
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
            this.panelBox2.Size = new System.Drawing.Size(230, 246);
            this.panelBox2.TabIndex = 2;
            this.panelBox2.Tag = "iatag_ui_misc_title";
            this.panelBox2.Text = "Misc";
            this.panelBox2.TextLocation = "8; 5";
            // 
            // buttonLootManually
            // 
            this.buttonLootManually.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonLootManually.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonLootManually.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonLootManually.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLootManually.EnabledCalc = true;
            this.buttonLootManually.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLootManually.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLootManually.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonLootManually.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLootManually.Location = new System.Drawing.Point(14, 203);
            this.buttonLootManually.Name = "buttonLootManually";
            this.buttonLootManually.Size = new System.Drawing.Size(192, 32);
            this.buttonLootManually.TabIndex = 9;
            this.buttonLootManually.Tag = "iatag_ui_lootmanually";
            this.buttonLootManually.Text = "Loot Manually";
            this.buttonLootManually.Visible = false;
            this.buttonLootManually.Click += new System.EventHandler(this.buttonLootManually_Click);
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
            this.panelBox1.Size = new System.Drawing.Size(230, 246);
            this.panelBox1.TabIndex = 1;
            this.panelBox1.Tag = "iatag_ui_actions_title";
            this.panelBox1.Text = "Actions";
            this.panelBox1.TextLocation = "8; 5";
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
            this.buttonAdvancedSettings.Text = "Advanced";
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
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.linkLabel2.Location = new System.Drawing.Point(374, 47);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(101, 13);
            this.linkLabel2.TabIndex = 21;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Tag = "iatag_ui_checkforupdates";
            this.linkLabel2.Text = "Check for updates";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 511);
            this.Controls.Add(this.panelBox4);
            this.Controls.Add(this.panelBox3);
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
            this.panelBox3.ResumeLayout(false);
            this.panelBox3.PerformLayout();
            this.panelBox2.ResumeLayout(false);
            this.panelBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox1;
        private PanelBox panelBox2;
        private FirefoxButton buttonViewBackups;
        private FirefoxButton buttonViewLogs;
        private PanelBox panelBox3;
        private FirefoxRadioButton radioBeta;
        private FirefoxRadioButton radioRelease;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private FirefoxCheckBox cbMinimizeToTray;
        private FirefoxCheckBox cbSecureTransfers;
        private FirefoxCheckBox cbShowRecipesAsItems;
        private FirefoxCheckBox cbTransferAnyMod;
        private FirefoxCheckBox cbAutoUpdateModSettings;
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
        private System.Windows.Forms.LinkLabel helpWhatIsSecureTransfers;
        private System.Windows.Forms.LinkLabel helpWhatIsTransferToAnyMod;
        private FirefoxCheckBox cbDeleteDuplicates;
        private System.Windows.Forms.LinkLabel helpWhatIsDeleteDuplicates;
        private System.Windows.Forms.LinkLabel helpWhatIsUsingMultiplePc;
        private System.Windows.Forms.Button buttonPaypal;
        private System.Windows.Forms.Button buttonPatreon;
        private System.Windows.Forms.LinkLabel helpWhatIsExperimentalUpdates;
        private System.Windows.Forms.LinkLabel helpWhatIsRegularUpdates;
        private FirefoxButton buttonLootManually;
        private FirefoxCheckBox cbStartMinimized;
        private FirefoxCheckBox cbDarkMode;
        private System.Windows.Forms.LinkLabel linkLabel2;
    }
}