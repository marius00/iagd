namespace IAGrim.UI {
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
            this.panelBox6 = new PanelBox();
            this.firefoxCheckBox1 = new FirefoxCheckBox();
            this.cbInstaTransfer = new FirefoxCheckBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.cbInstalootEnabled = new FirefoxCheckBox();
            this.panelBox5 = new PanelBox();
            this.labelNumItems = new System.Windows.Forms.Label();
            this.labelLastUpdated = new System.Windows.Forms.Label();
            this.labelLastPatch = new System.Windows.Forms.Label();
            this.panelBox4 = new PanelBox();
            this.linkSourceCode = new System.Windows.Forms.LinkLabel();
            this.cbDisplaySkills = new FirefoxCheckBox();
            this.cbAutoUpdateModSettings = new FirefoxCheckBox();
            this.cbAutoSearch = new FirefoxCheckBox();
            this.cbTransferAnyMod = new FirefoxCheckBox();
            this.cbShowRecipesAsItems = new FirefoxCheckBox();
            this.cbSecureTransfers = new FirefoxCheckBox();
            this.cbMergeDuplicates = new FirefoxCheckBox();
            this.cbMinimizeToTray = new FirefoxCheckBox();
            this.panelBox3 = new PanelBox();
            this.radioBeta = new FirefoxRadioButton();
            this.radioRelease = new FirefoxRadioButton();
            this.panelBox2 = new PanelBox();
            this.buttonDonate = new FirefoxButton();
            this.buttonForum = new FirefoxButton();
            this.buttonDeveloper = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.buttonAdvancedSettings = new FirefoxButton();
            this.buttonImportExport = new FirefoxButton();
            this.buttonMigratePostgres = new FirefoxButton();
            this.buttonLanguageSelect = new FirefoxButton();
            this.buttonViewBackups = new FirefoxButton();
            this.buttonViewLogs = new FirefoxButton();
            this.contextMenuStrip1.SuspendLayout();
            this.panelBox6.SuspendLayout();
            this.panelBox5.SuspendLayout();
            this.panelBox4.SuspendLayout();
            this.panelBox3.SuspendLayout();
            this.panelBox2.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.ContextMenuStrip = this.contextMenuStrip1;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(853, 469);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(75, 33);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
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
            // panelBox6
            // 
            this.panelBox6.Controls.Add(this.firefoxCheckBox1);
            this.panelBox6.Controls.Add(this.cbInstaTransfer);
            this.panelBox6.Controls.Add(this.linkLabel2);
            this.panelBox6.Controls.Add(this.cbInstalootEnabled);
            this.panelBox6.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox6.HeaderHeight = 40;
            this.panelBox6.Location = new System.Drawing.Point(700, 12);
            this.panelBox6.Name = "panelBox6";
            this.panelBox6.NoRounding = false;
            this.panelBox6.Size = new System.Drawing.Size(206, 385);
            this.panelBox6.TabIndex = 6;
            this.panelBox6.Tag = "iatag_ui_experimental";
            this.panelBox6.Text = "Experimental";
            this.panelBox6.TextLocation = "8; 5";
            // 
            // firefoxCheckBox1
            // 
            this.firefoxCheckBox1.Bold = false;
            this.firefoxCheckBox1.EnabledCalc = true;
            this.firefoxCheckBox1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.firefoxCheckBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.firefoxCheckBox1.Location = new System.Drawing.Point(3, 120);
            this.firefoxCheckBox1.Name = "firefoxCheckBox1";
            this.firefoxCheckBox1.Size = new System.Drawing.Size(178, 27);
            this.firefoxCheckBox1.TabIndex = 19;
            this.firefoxCheckBox1.Tag = "iatag_ui_v1_0_40_0_hotfix";
            this.firefoxCheckBox1.Text = "v1.0.40.0 hotfix";
            this.firefoxCheckBox1.Visible = false;
            this.firefoxCheckBox1.CheckedChanged += new System.EventHandler(this.firefoxCheckBox1_CheckedChanged);
            // 
            // cbInstaTransfer
            // 
            this.cbInstaTransfer.Bold = false;
            this.cbInstaTransfer.EnabledCalc = true;
            this.cbInstaTransfer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbInstaTransfer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbInstaTransfer.Location = new System.Drawing.Point(3, 87);
            this.cbInstaTransfer.Name = "cbInstaTransfer";
            this.cbInstaTransfer.Size = new System.Drawing.Size(178, 27);
            this.cbInstaTransfer.TabIndex = 18;
            this.cbInstaTransfer.Tag = "iatag_ui_instatransfer";
            this.cbInstaTransfer.Text = "InstaTransfer";
            this.cbInstaTransfer.Visible = false;
            this.cbInstaTransfer.CheckedChanged += new System.EventHandler(this.cbInstaTransfer_CheckedChanged);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkLabel2.Location = new System.Drawing.Point(147, 61);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(45, 13);
            this.linkLabel2.TabIndex = 17;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Tag = "iatag_ui_details";
            this.linkLabel2.Text = "Details..";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // cbInstalootEnabled
            // 
            this.cbInstalootEnabled.Bold = false;
            this.cbInstalootEnabled.EnabledCalc = true;
            this.cbInstalootEnabled.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbInstalootEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbInstalootEnabled.Location = new System.Drawing.Point(3, 54);
            this.cbInstalootEnabled.Name = "cbInstalootEnabled";
            this.cbInstalootEnabled.Size = new System.Drawing.Size(178, 27);
            this.cbInstalootEnabled.TabIndex = 16;
            this.cbInstalootEnabled.Tag = "iatag_ui_instaloot";
            this.cbInstalootEnabled.Text = "Instaloot";
            this.cbInstalootEnabled.CheckedChanged += new System.EventHandler(this.cbInstalootDisabled_CheckedChanged);
            // 
            // panelBox5
            // 
            this.panelBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelBox5.Controls.Add(this.labelNumItems);
            this.panelBox5.Controls.Add(this.labelLastUpdated);
            this.panelBox5.Controls.Add(this.labelLastPatch);
            this.panelBox5.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox5.HeaderHeight = 40;
            this.panelBox5.Location = new System.Drawing.Point(12, 407);
            this.panelBox5.Name = "panelBox5";
            this.panelBox5.NoRounding = false;
            this.panelBox5.Size = new System.Drawing.Size(478, 95);
            this.panelBox5.TabIndex = 9;
            this.panelBox5.Tag = "iatag_ui_gddb_title";
            this.panelBox5.Text = "Grim Dawn Database";
            this.panelBox5.TextLocation = "8; 5";
            // 
            // labelNumItems
            // 
            this.labelNumItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelNumItems.AutoSize = true;
            this.labelNumItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelNumItems.Location = new System.Drawing.Point(3, 48);
            this.labelNumItems.Name = "labelNumItems";
            this.labelNumItems.Size = new System.Drawing.Size(35, 13);
            this.labelNumItems.TabIndex = 4;
            this.labelNumItems.Text = "label2";
            // 
            // labelLastUpdated
            // 
            this.labelLastUpdated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastUpdated.AutoSize = true;
            this.labelLastUpdated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelLastUpdated.Location = new System.Drawing.Point(3, 61);
            this.labelLastUpdated.Name = "labelLastUpdated";
            this.labelLastUpdated.Size = new System.Drawing.Size(35, 13);
            this.labelLastUpdated.TabIndex = 3;
            this.labelLastUpdated.Text = "label1";
            // 
            // labelLastPatch
            // 
            this.labelLastPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelLastPatch.AutoSize = true;
            this.labelLastPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelLastPatch.Location = new System.Drawing.Point(3, 74);
            this.labelLastPatch.Name = "labelLastPatch";
            this.labelLastPatch.Size = new System.Drawing.Size(35, 13);
            this.labelLastPatch.TabIndex = 5;
            this.labelLastPatch.Text = "label1";
            // 
            // panelBox4
            // 
            this.panelBox4.Controls.Add(this.linkSourceCode);
            this.panelBox4.Controls.Add(this.cbDisplaySkills);
            this.panelBox4.Controls.Add(this.cbAutoUpdateModSettings);
            this.panelBox4.Controls.Add(this.cbAutoSearch);
            this.panelBox4.Controls.Add(this.cbTransferAnyMod);
            this.panelBox4.Controls.Add(this.cbShowRecipesAsItems);
            this.panelBox4.Controls.Add(this.cbSecureTransfers);
            this.panelBox4.Controls.Add(this.cbMergeDuplicates);
            this.panelBox4.Controls.Add(this.cbMinimizeToTray);
            this.panelBox4.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox4.HeaderHeight = 40;
            this.panelBox4.Location = new System.Drawing.Point(496, 12);
            this.panelBox4.Name = "panelBox4";
            this.panelBox4.NoRounding = false;
            this.panelBox4.Size = new System.Drawing.Size(198, 385);
            this.panelBox4.TabIndex = 8;
            this.panelBox4.Tag = "iatag_ui_settings_title";
            this.panelBox4.Text = "Settings";
            this.panelBox4.TextLocation = "8; 5";
            // 
            // linkSourceCode
            // 
            this.linkSourceCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkSourceCode.AutoSize = true;
            this.linkSourceCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.linkSourceCode.Location = new System.Drawing.Point(5, 363);
            this.linkSourceCode.Name = "linkSourceCode";
            this.linkSourceCode.Size = new System.Drawing.Size(68, 13);
            this.linkSourceCode.TabIndex = 21;
            this.linkSourceCode.TabStop = true;
            this.linkSourceCode.Text = "Source code";
            this.linkSourceCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSourceCode_LinkClicked);
            // 
            // cbDisplaySkills
            // 
            this.cbDisplaySkills.Bold = false;
            this.cbDisplaySkills.EnabledCalc = true;
            this.cbDisplaySkills.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDisplaySkills.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDisplaySkills.Location = new System.Drawing.Point(3, 219);
            this.cbDisplaySkills.Name = "cbDisplaySkills";
            this.cbDisplaySkills.Size = new System.Drawing.Size(160, 27);
            this.cbDisplaySkills.TabIndex = 17;
            this.cbDisplaySkills.Tag = "iatag_ui_showskills";
            this.cbDisplaySkills.Text = "Show Skills";
            this.cbDisplaySkills.CheckedChanged += new System.EventHandler(this.cbDisplaySkills_CheckedChanged);
            // 
            // cbAutoUpdateModSettings
            // 
            this.cbAutoUpdateModSettings.Bold = false;
            this.cbAutoUpdateModSettings.Enabled = false;
            this.cbAutoUpdateModSettings.EnabledCalc = true;
            this.cbAutoUpdateModSettings.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoUpdateModSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbAutoUpdateModSettings.Location = new System.Drawing.Point(3, 339);
            this.cbAutoUpdateModSettings.Name = "cbAutoUpdateModSettings";
            this.cbAutoUpdateModSettings.Size = new System.Drawing.Size(132, 27);
            this.cbAutoUpdateModSettings.TabIndex = 15;
            this.cbAutoUpdateModSettings.Tag = "iatag_ui_autoselectmod";
            this.cbAutoUpdateModSettings.Text = "Auto Select Mod";
            this.cbAutoUpdateModSettings.UseVisualStyleBackColor = true;
            this.cbAutoUpdateModSettings.Visible = false;
            // 
            // cbAutoSearch
            // 
            this.cbAutoSearch.Bold = false;
            this.cbAutoSearch.Enabled = false;
            this.cbAutoSearch.EnabledCalc = true;
            this.cbAutoSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbAutoSearch.Location = new System.Drawing.Point(3, 186);
            this.cbAutoSearch.Name = "cbAutoSearch";
            this.cbAutoSearch.Size = new System.Drawing.Size(178, 27);
            this.cbAutoSearch.TabIndex = 16;
            this.cbAutoSearch.Tag = "iatag_ui_autosearch";
            this.cbAutoSearch.Text = "Auto Search";
            this.cbAutoSearch.UseVisualStyleBackColor = true;
            // 
            // cbTransferAnyMod
            // 
            this.cbTransferAnyMod.Bold = false;
            this.cbTransferAnyMod.EnabledCalc = true;
            this.cbTransferAnyMod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbTransferAnyMod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbTransferAnyMod.Location = new System.Drawing.Point(3, 120);
            this.cbTransferAnyMod.Name = "cbTransferAnyMod";
            this.cbTransferAnyMod.Size = new System.Drawing.Size(178, 27);
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
            this.cbShowRecipesAsItems.Location = new System.Drawing.Point(3, 54);
            this.cbShowRecipesAsItems.Name = "cbShowRecipesAsItems";
            this.cbShowRecipesAsItems.Size = new System.Drawing.Size(178, 27);
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
            this.cbSecureTransfers.Location = new System.Drawing.Point(3, 153);
            this.cbSecureTransfers.Name = "cbSecureTransfers";
            this.cbSecureTransfers.Size = new System.Drawing.Size(147, 27);
            this.cbSecureTransfers.TabIndex = 12;
            this.cbSecureTransfers.Tag = "iatag_ui_securetransfers";
            this.cbSecureTransfers.Text = "Secure Transfers";
            // 
            // cbMergeDuplicates
            // 
            this.cbMergeDuplicates.Bold = false;
            this.cbMergeDuplicates.EnabledCalc = true;
            this.cbMergeDuplicates.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbMergeDuplicates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbMergeDuplicates.Location = new System.Drawing.Point(3, 306);
            this.cbMergeDuplicates.Name = "cbMergeDuplicates";
            this.cbMergeDuplicates.Size = new System.Drawing.Size(160, 27);
            this.cbMergeDuplicates.TabIndex = 9;
            this.cbMergeDuplicates.Tag = "iatag_ui_mergeduplicates";
            this.cbMergeDuplicates.Text = "Merge Duplicates";
            this.cbMergeDuplicates.Visible = false;
            this.cbMergeDuplicates.CheckedChanged += new System.EventHandler(this.cbMergeDuplicates_CheckedChanged);
            // 
            // cbMinimizeToTray
            // 
            this.cbMinimizeToTray.Bold = false;
            this.cbMinimizeToTray.EnabledCalc = true;
            this.cbMinimizeToTray.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbMinimizeToTray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbMinimizeToTray.Location = new System.Drawing.Point(3, 87);
            this.cbMinimizeToTray.Name = "cbMinimizeToTray";
            this.cbMinimizeToTray.Size = new System.Drawing.Size(160, 27);
            this.cbMinimizeToTray.TabIndex = 8;
            this.cbMinimizeToTray.Tag = "iatag_ui_minimizetotray";
            this.cbMinimizeToTray.Text = "Minimize to Tray";
            // 
            // panelBox3
            // 
            this.panelBox3.Controls.Add(this.radioBeta);
            this.panelBox3.Controls.Add(this.radioRelease);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            // radioBeta
            // 
            this.radioBeta.Bold = false;
            this.radioBeta.Checked = false;
            this.radioBeta.EnabledCalc = true;
            this.radioBeta.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioBeta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioBeta.Location = new System.Drawing.Point(9, 87);
            this.radioBeta.Name = "radioBeta";
            this.radioBeta.Size = new System.Drawing.Size(188, 27);
            this.radioBeta.TabIndex = 1;
            this.radioBeta.Tag = "iatag_ui_experimentalupdates";
            this.radioBeta.Text = "Experimental Features";
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
            this.radioRelease.Size = new System.Drawing.Size(188, 27);
            this.radioRelease.TabIndex = 0;
            this.radioRelease.Tag = "iatag_ui_regularupdates";
            this.radioRelease.Text = "Regular Updates";
            this.radioRelease.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioRelease_CheckedChanged);
            // 
            // panelBox2
            // 
            this.panelBox2.Controls.Add(this.buttonDonate);
            this.panelBox2.Controls.Add(this.buttonForum);
            this.panelBox2.Controls.Add(this.buttonDeveloper);
            this.panelBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            // buttonDonate
            // 
            this.buttonDonate.EnabledCalc = true;
            this.buttonDonate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonDonate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonDonate.Location = new System.Drawing.Point(14, 127);
            this.buttonDonate.Name = "buttonDonate";
            this.buttonDonate.Size = new System.Drawing.Size(192, 32);
            this.buttonDonate.TabIndex = 5;
            this.buttonDonate.Tag = "iatag_ui_donatenow";
            this.buttonDonate.Text = "Donate Now!";
            this.buttonDonate.Click += new System.EventHandler(this.buttonDonate_Click);
            // 
            // buttonForum
            // 
            this.buttonForum.EnabledCalc = true;
            this.buttonForum.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonForum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonForum.Location = new System.Drawing.Point(14, 89);
            this.buttonForum.Name = "buttonForum";
            this.buttonForum.Size = new System.Drawing.Size(192, 32);
            this.buttonForum.TabIndex = 4;
            this.buttonForum.Tag = "iatag_ui_openforum";
            this.buttonForum.Text = "Open Forum";
            this.buttonForum.Click += new System.EventHandler(this.buttonForum_Click);
            // 
            // buttonDeveloper
            // 
            this.buttonDeveloper.EnabledCalc = true;
            this.buttonDeveloper.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonDeveloper.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonDeveloper.Location = new System.Drawing.Point(14, 51);
            this.buttonDeveloper.Name = "buttonDeveloper";
            this.buttonDeveloper.Size = new System.Drawing.Size(192, 32);
            this.buttonDeveloper.TabIndex = 3;
            this.buttonDeveloper.Tag = "iatag_ui_contactdev";
            this.buttonDeveloper.Text = "Contact Developer";
            this.buttonDeveloper.Click += new System.EventHandler(this.buttonDeveloper_Click);
            // 
            // panelBox1
            // 
            this.panelBox1.Controls.Add(this.buttonAdvancedSettings);
            this.panelBox1.Controls.Add(this.buttonImportExport);
            this.panelBox1.Controls.Add(this.buttonMigratePostgres);
            this.panelBox1.Controls.Add(this.buttonLanguageSelect);
            this.panelBox1.Controls.Add(this.buttonViewBackups);
            this.panelBox1.Controls.Add(this.buttonViewLogs);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            this.buttonAdvancedSettings.EnabledCalc = true;
            this.buttonAdvancedSettings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonAdvancedSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.buttonImportExport.EnabledCalc = true;
            this.buttonImportExport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonImportExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.buttonMigratePostgres.EnabledCalc = true;
            this.buttonMigratePostgres.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonMigratePostgres.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonMigratePostgres.Location = new System.Drawing.Point(0, 0);
            this.buttonMigratePostgres.Name = "buttonMigratePostgres";
            this.buttonMigratePostgres.Size = new System.Drawing.Size(0, 0);
            this.buttonMigratePostgres.TabIndex = 0;
            // 
            // buttonLanguageSelect
            // 
            this.buttonLanguageSelect.EnabledCalc = true;
            this.buttonLanguageSelect.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLanguageSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLanguageSelect.Location = new System.Drawing.Point(19, 127);
            this.buttonLanguageSelect.Name = "buttonLanguageSelect";
            this.buttonLanguageSelect.Size = new System.Drawing.Size(192, 32);
            this.buttonLanguageSelect.TabIndex = 6;
            this.buttonLanguageSelect.Text = "Language Select";
            this.buttonLanguageSelect.Click += new System.EventHandler(this.buttonLanguageSelect_Click);
            // 
            // buttonViewBackups
            // 
            this.buttonViewBackups.EnabledCalc = true;
            this.buttonViewBackups.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonViewBackups.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.buttonViewLogs.EnabledCalc = true;
            this.buttonViewLogs.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonViewLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.ClientSize = new System.Drawing.Size(940, 511);
            this.Controls.Add(this.panelBox6);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.panelBox5);
            this.Controls.Add(this.panelBox4);
            this.Controls.Add(this.panelBox3);
            this.Controls.Add(this.panelBox2);
            this.Controls.Add(this.panelBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panelBox6.ResumeLayout(false);
            this.panelBox6.PerformLayout();
            this.panelBox5.ResumeLayout(false);
            this.panelBox5.PerformLayout();
            this.panelBox4.ResumeLayout(false);
            this.panelBox4.PerformLayout();
            this.panelBox3.ResumeLayout(false);
            this.panelBox2.ResumeLayout(false);
            this.panelBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PanelBox panelBox1;
        private PanelBox panelBox2;
        private FirefoxButton buttonDeveloper;
        private FirefoxButton buttonViewBackups;
        private FirefoxButton buttonViewLogs;
        private FirefoxButton buttonForum;
        private System.Windows.Forms.Label labelLastUpdated;
        private System.Windows.Forms.Label labelNumItems;
        private System.Windows.Forms.Label labelLastPatch;
        private PanelBox panelBox3;
        private FirefoxRadioButton radioBeta;
        private FirefoxRadioButton radioRelease;
        private PanelBox panelBox5;
        private FirefoxButton buttonDonate;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private FirefoxCheckBox cbMinimizeToTray;
        private FirefoxCheckBox cbMergeDuplicates;
        private FirefoxCheckBox cbSecureTransfers;
        private FirefoxCheckBox cbShowRecipesAsItems;
        private FirefoxCheckBox cbTransferAnyMod;
        private FirefoxCheckBox cbAutoUpdateModSettings;
        private PanelBox panelBox4;
        private FirefoxButton buttonLanguageSelect;
        private PanelBox panelBox6;
        private FirefoxButton buttonMigratePostgres;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private FirefoxCheckBox cbInstalootEnabled;
        private FirefoxCheckBox cbInstaTransfer;
        private FirefoxCheckBox cbAutoSearch;
        private FirefoxButton buttonImportExport;
        private FirefoxCheckBox cbDisplaySkills;
        private FirefoxButton buttonAdvancedSettings;
        private System.Windows.Forms.LinkLabel linkSourceCode;
        private FirefoxCheckBox firefoxCheckBox1;
    }
}