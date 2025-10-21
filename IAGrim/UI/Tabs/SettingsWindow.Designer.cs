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
            components = new System.ComponentModel.Container();
            linkLabel1 = new LinkLabel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            panelBox4 = new PanelBox();
            helpWhatIsDelayWhenSearching = new LinkLabel();
            cbDelaySearch = new FirefoxCheckBox();
            cbTransferAnyMod = new FirefoxCheckBox();
            cbAutoDismiss = new FirefoxCheckBox();
            cbDarkMode = new FirefoxCheckBox();
            cbStartMinimized = new FirefoxCheckBox();
            helpWhatIsUsingMultiplePc = new LinkLabel();
            cbMinimizeToTray = new FirefoxCheckBox();
            cbDualComputer = new FirefoxCheckBox();
            linkSourceCode = new LinkLabel();
            cbHideSkills = new FirefoxCheckBox();
            pbAutomaticUpdates = new PanelBox();
            linkDowngrade = new LinkLabel();
            linkCheckForUpdates = new LinkLabel();
            helpWhatIsExperimentalUpdates = new LinkLabel();
            helpWhatIsRegularUpdates = new LinkLabel();
            radioBeta = new FirefoxRadioButton();
            radioRelease = new FirefoxRadioButton();
            panelBox2 = new PanelBox();
            buttonPatreon = new Button();
            buttonPaypal = new Button();
            panelBox1 = new PanelBox();
            firefoxButton1 = new FirefoxButton();
            buttonAdvancedSettings = new FirefoxButton();
            buttonImportExport = new FirefoxButton();
            buttonLanguageSelect = new FirefoxButton();
            buttonViewBackups = new FirefoxButton();
            buttonViewLogs = new FirefoxButton();
            contextMenuStrip1.SuspendLayout();
            panelBox4.SuspendLayout();
            pbAutomaticUpdates.SuspendLayout();
            panelBox2.SuspendLayout();
            panelBox1.SuspendLayout();
            SuspendLayout();
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel1.ContextMenuStrip = contextMenuStrip1;
            linkLabel1.Font = new Font("Microsoft Sans Serif", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.Location = new Point(694, 528);
            linkLabel1.Margin = new Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(314, 38);
            linkLabel1.TabIndex = 20;
            linkLabel1.TabStop = true;
            linkLabel1.Tag = "iatag_ui_help";
            linkLabel1.Text = "Discord / Help";
            linkLabel1.TextAlign = ContentAlignment.MiddleRight;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(103, 26);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(102, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // panelBox4
            // 
            panelBox4.BackColor = Color.FromArgb(240, 240, 240);
            panelBox4.Controls.Add(helpWhatIsDelayWhenSearching);
            panelBox4.Controls.Add(cbDelaySearch);
            panelBox4.Controls.Add(cbTransferAnyMod);
            panelBox4.Controls.Add(cbAutoDismiss);
            panelBox4.Controls.Add(cbDarkMode);
            panelBox4.Controls.Add(cbStartMinimized);
            panelBox4.Controls.Add(helpWhatIsUsingMultiplePc);
            panelBox4.Controls.Add(cbMinimizeToTray);
            panelBox4.Controls.Add(cbDualComputer);
            panelBox4.Controls.Add(linkSourceCode);
            panelBox4.Controls.Add(cbHideSkills);
            panelBox4.Font = new Font("Segoe UI Semibold", 20F);
            panelBox4.ForeColor = Color.Black;
            panelBox4.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox4.HeaderHeight = 40;
            panelBox4.Location = new Point(580, 14);
            panelBox4.Margin = new Padding(4, 3, 4, 3);
            panelBox4.Name = "panelBox4";
            panelBox4.NoRounding = false;
            panelBox4.Size = new Size(335, 507);
            panelBox4.TabIndex = 8;
            panelBox4.Tag = "iatag_ui_settings_title";
            panelBox4.Text = "Settings";
            panelBox4.TextLocation = "8; 5";
            // 
            // helpWhatIsDelayWhenSearching
            // 
            helpWhatIsDelayWhenSearching.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpWhatIsDelayWhenSearching.AutoSize = true;
            helpWhatIsDelayWhenSearching.BackColor = Color.Transparent;
            helpWhatIsDelayWhenSearching.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpWhatIsDelayWhenSearching.Location = new Point(299, 324);
            helpWhatIsDelayWhenSearching.Margin = new Padding(4, 0, 4, 0);
            helpWhatIsDelayWhenSearching.Name = "helpWhatIsDelayWhenSearching";
            helpWhatIsDelayWhenSearching.Size = new Size(18, 13);
            helpWhatIsDelayWhenSearching.TabIndex = 31;
            helpWhatIsDelayWhenSearching.TabStop = true;
            helpWhatIsDelayWhenSearching.Tag = "iatag_ui_questionmark";
            helpWhatIsDelayWhenSearching.Text = " ? ";
            helpWhatIsDelayWhenSearching.LinkClicked += helpWhatIsDelayWhenSearching_LinkClicked;
            // 
            // cbDelaySearch
            // 
            cbDelaySearch.Bold = false;
            cbDelaySearch.EnabledCalc = true;
            cbDelaySearch.Font = new Font("Segoe UI", 10F);
            cbDelaySearch.ForeColor = Color.FromArgb(66, 78, 90);
            cbDelaySearch.IsDarkMode = false;
            cbDelaySearch.Location = new Point(4, 324);
            cbDelaySearch.Margin = new Padding(4, 3, 4, 3);
            cbDelaySearch.Name = "cbDelaySearch";
            cbDelaySearch.Size = new Size(313, 31);
            cbDelaySearch.TabIndex = 34;
            cbDelaySearch.Tag = "iatag_ui_searchdelay";
            cbDelaySearch.Text = "Delay when searching";
            cbDelaySearch.CheckedChanged += firefoxCheckBox1_CheckedChanged_1;
            // 
            // cbTransferAnyMod
            // 
            cbTransferAnyMod.Bold = false;
            cbTransferAnyMod.EnabledCalc = true;
            cbTransferAnyMod.Font = new Font("Segoe UI", 10F);
            cbTransferAnyMod.ForeColor = Color.FromArgb(66, 78, 90);
            cbTransferAnyMod.IsDarkMode = false;
            cbTransferAnyMod.Location = new Point(4, 287);
            cbTransferAnyMod.Margin = new Padding(4, 3, 4, 3);
            cbTransferAnyMod.Name = "cbTransferAnyMod";
            cbTransferAnyMod.Size = new Size(313, 31);
            cbTransferAnyMod.TabIndex = 33;
            cbTransferAnyMod.Tag = "iatag_ui_transferanymod";
            cbTransferAnyMod.Text = "Transfer to any mod";
            cbTransferAnyMod.CheckedChanged += firefoxCheckBox1_CheckedChanged;
            // 
            // cbAutoDismiss
            // 
            cbAutoDismiss.Bold = false;
            cbAutoDismiss.EnabledCalc = true;
            cbAutoDismiss.Font = new Font("Segoe UI", 10F);
            cbAutoDismiss.ForeColor = Color.FromArgb(66, 78, 90);
            cbAutoDismiss.IsDarkMode = false;
            cbAutoDismiss.Location = new Point(4, 249);
            cbAutoDismiss.Margin = new Padding(4, 3, 4, 3);
            cbAutoDismiss.Name = "cbAutoDismiss";
            cbAutoDismiss.Size = new Size(313, 31);
            cbAutoDismiss.TabIndex = 32;
            cbAutoDismiss.Tag = "iatag_ui_autodismiss";
            cbAutoDismiss.Text = "Auto dismiss notifications";
            cbAutoDismiss.CheckedChanged += cbAutoDismiss_CheckedChanged;
            // 
            // cbDarkMode
            // 
            cbDarkMode.Bold = false;
            cbDarkMode.EnabledCalc = true;
            cbDarkMode.Font = new Font("Segoe UI", 10F);
            cbDarkMode.ForeColor = Color.FromArgb(66, 78, 90);
            cbDarkMode.IsDarkMode = false;
            cbDarkMode.Location = new Point(4, 211);
            cbDarkMode.Margin = new Padding(4, 3, 4, 3);
            cbDarkMode.Name = "cbDarkMode";
            cbDarkMode.Size = new Size(313, 31);
            cbDarkMode.TabIndex = 31;
            cbDarkMode.Tag = "iatag_ui_darkmode";
            cbDarkMode.Text = "Dark mode";
            cbDarkMode.CheckedChanged += cbDarkMode_CheckedChanged;
            // 
            // cbStartMinimized
            // 
            cbStartMinimized.Bold = false;
            cbStartMinimized.EnabledCalc = true;
            cbStartMinimized.Font = new Font("Segoe UI", 10F);
            cbStartMinimized.ForeColor = Color.FromArgb(66, 78, 90);
            cbStartMinimized.IsDarkMode = false;
            cbStartMinimized.Location = new Point(4, 173);
            cbStartMinimized.Margin = new Padding(4, 3, 4, 3);
            cbStartMinimized.Name = "cbStartMinimized";
            cbStartMinimized.Size = new Size(313, 31);
            cbStartMinimized.TabIndex = 30;
            cbStartMinimized.Tag = "iatag_ui_startminimized";
            cbStartMinimized.Text = "Start minimized";
            cbStartMinimized.CheckedChanged += cbStartMinimized_CheckedChanged;
            // 
            // helpWhatIsUsingMultiplePc
            // 
            helpWhatIsUsingMultiplePc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpWhatIsUsingMultiplePc.AutoSize = true;
            helpWhatIsUsingMultiplePc.BackColor = Color.Transparent;
            helpWhatIsUsingMultiplePc.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpWhatIsUsingMultiplePc.Location = new Point(302, 105);
            helpWhatIsUsingMultiplePc.Margin = new Padding(4, 0, 4, 0);
            helpWhatIsUsingMultiplePc.Name = "helpWhatIsUsingMultiplePc";
            helpWhatIsUsingMultiplePc.Size = new Size(18, 13);
            helpWhatIsUsingMultiplePc.TabIndex = 28;
            helpWhatIsUsingMultiplePc.TabStop = true;
            helpWhatIsUsingMultiplePc.Tag = "iatag_ui_questionmark";
            helpWhatIsUsingMultiplePc.Text = " ? ";
            helpWhatIsUsingMultiplePc.LinkClicked += helpWhatIsUsingMultiplePc_LinkClicked;
            // 
            // cbMinimizeToTray
            // 
            cbMinimizeToTray.Bold = false;
            cbMinimizeToTray.EnabledCalc = true;
            cbMinimizeToTray.Font = new Font("Segoe UI", 10F);
            cbMinimizeToTray.ForeColor = Color.FromArgb(66, 78, 90);
            cbMinimizeToTray.IsDarkMode = false;
            cbMinimizeToTray.Location = new Point(4, 135);
            cbMinimizeToTray.Margin = new Padding(4, 3, 4, 3);
            cbMinimizeToTray.Name = "cbMinimizeToTray";
            cbMinimizeToTray.Size = new Size(313, 31);
            cbMinimizeToTray.TabIndex = 8;
            cbMinimizeToTray.Tag = "iatag_ui_minimizetotray";
            cbMinimizeToTray.Text = "Minimize to Tray";
            // 
            // cbDualComputer
            // 
            cbDualComputer.Bold = false;
            cbDualComputer.EnabledCalc = true;
            cbDualComputer.Font = new Font("Segoe UI", 10F);
            cbDualComputer.ForeColor = Color.FromArgb(66, 78, 90);
            cbDualComputer.IsDarkMode = false;
            cbDualComputer.Location = new Point(4, 97);
            cbDualComputer.Margin = new Padding(4, 3, 4, 3);
            cbDualComputer.Name = "cbDualComputer";
            cbDualComputer.Size = new Size(313, 31);
            cbDualComputer.TabIndex = 22;
            cbDualComputer.Tag = "iatag_ui_dualcomputer";
            cbDualComputer.Text = "Using IA on multiple PCs";
            cbDualComputer.CheckedChanged += cbDualComputer_CheckedChanged;
            // 
            // linkSourceCode
            // 
            linkSourceCode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkSourceCode.AutoSize = true;
            linkSourceCode.Font = new Font("Microsoft Sans Serif", 8.25F);
            linkSourceCode.Location = new Point(6, 481);
            linkSourceCode.Margin = new Padding(4, 0, 4, 0);
            linkSourceCode.Name = "linkSourceCode";
            linkSourceCode.Size = new Size(68, 13);
            linkSourceCode.TabIndex = 21;
            linkSourceCode.TabStop = true;
            linkSourceCode.Tag = "iatag_ui_source_code";
            linkSourceCode.Text = "Source code";
            linkSourceCode.LinkClicked += linkSourceCode_LinkClicked;
            // 
            // cbHideSkills
            // 
            cbHideSkills.Bold = false;
            cbHideSkills.EnabledCalc = true;
            cbHideSkills.Font = new Font("Segoe UI", 10F);
            cbHideSkills.ForeColor = Color.FromArgb(66, 78, 90);
            cbHideSkills.IsDarkMode = false;
            cbHideSkills.Location = new Point(4, 59);
            cbHideSkills.Margin = new Padding(4, 3, 4, 3);
            cbHideSkills.Name = "cbHideSkills";
            cbHideSkills.Size = new Size(313, 31);
            cbHideSkills.TabIndex = 17;
            cbHideSkills.Tag = "iatag_ui_hideskills";
            cbHideSkills.Text = "Hide Skills";
            cbHideSkills.CheckedChanged += cbDisplaySkills_CheckedChanged;
            // 
            // pbAutomaticUpdates
            // 
            pbAutomaticUpdates.BackColor = Color.FromArgb(240, 240, 240);
            pbAutomaticUpdates.Controls.Add(linkDowngrade);
            pbAutomaticUpdates.Controls.Add(linkCheckForUpdates);
            pbAutomaticUpdates.Controls.Add(helpWhatIsExperimentalUpdates);
            pbAutomaticUpdates.Controls.Add(helpWhatIsRegularUpdates);
            pbAutomaticUpdates.Controls.Add(radioBeta);
            pbAutomaticUpdates.Controls.Add(radioRelease);
            pbAutomaticUpdates.Font = new Font("Segoe UI Semibold", 20F);
            pbAutomaticUpdates.ForeColor = Color.Black;
            pbAutomaticUpdates.HeaderColor = Color.FromArgb(231, 231, 231);
            pbAutomaticUpdates.HeaderHeight = 40;
            pbAutomaticUpdates.Location = new Point(14, 367);
            pbAutomaticUpdates.Margin = new Padding(4, 3, 4, 3);
            pbAutomaticUpdates.Name = "pbAutomaticUpdates";
            pbAutomaticUpdates.NoRounding = false;
            pbAutomaticUpdates.Size = new Size(558, 153);
            pbAutomaticUpdates.TabIndex = 6;
            pbAutomaticUpdates.Tag = "iatag_ui_update_title";
            pbAutomaticUpdates.Text = "Automatic Updates";
            pbAutomaticUpdates.TextLocation = "8; 5";
            // 
            // linkDowngrade
            // 
            linkDowngrade.AutoSize = true;
            linkDowngrade.Font = new Font("Segoe UI", 8.25F);
            linkDowngrade.Location = new Point(436, 76);
            linkDowngrade.Margin = new Padding(4, 0, 4, 0);
            linkDowngrade.Name = "linkDowngrade";
            linkDowngrade.Size = new Size(68, 13);
            linkDowngrade.TabIndex = 32;
            linkDowngrade.TabStop = true;
            linkDowngrade.Tag = "iatag_ui_downgrade_ia";
            linkDowngrade.Text = "Downgrade";
            linkDowngrade.LinkClicked += linkDowngrade_LinkClicked;
            // 
            // linkCheckForUpdates
            // 
            linkCheckForUpdates.AutoSize = true;
            linkCheckForUpdates.Font = new Font("Segoe UI", 8.25F);
            linkCheckForUpdates.Location = new Point(436, 54);
            linkCheckForUpdates.Margin = new Padding(4, 0, 4, 0);
            linkCheckForUpdates.Name = "linkCheckForUpdates";
            linkCheckForUpdates.Size = new Size(101, 13);
            linkCheckForUpdates.TabIndex = 21;
            linkCheckForUpdates.TabStop = true;
            linkCheckForUpdates.Tag = "iatag_ui_checkforupdates";
            linkCheckForUpdates.Text = "Check for updates";
            linkCheckForUpdates.LinkClicked += linkLabel2_LinkClicked;
            // 
            // helpWhatIsExperimentalUpdates
            // 
            helpWhatIsExperimentalUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpWhatIsExperimentalUpdates.AutoSize = true;
            helpWhatIsExperimentalUpdates.BackColor = Color.Transparent;
            helpWhatIsExperimentalUpdates.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpWhatIsExperimentalUpdates.Location = new Point(198, 107);
            helpWhatIsExperimentalUpdates.Margin = new Padding(4, 0, 4, 0);
            helpWhatIsExperimentalUpdates.Name = "helpWhatIsExperimentalUpdates";
            helpWhatIsExperimentalUpdates.Size = new Size(18, 13);
            helpWhatIsExperimentalUpdates.TabIndex = 31;
            helpWhatIsExperimentalUpdates.TabStop = true;
            helpWhatIsExperimentalUpdates.Tag = "iatag_ui_questionmark";
            helpWhatIsExperimentalUpdates.Text = " ? ";
            helpWhatIsExperimentalUpdates.LinkClicked += helpWhatIsExperimentalUpdates_LinkClicked;
            // 
            // helpWhatIsRegularUpdates
            // 
            helpWhatIsRegularUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpWhatIsRegularUpdates.AutoSize = true;
            helpWhatIsRegularUpdates.BackColor = Color.Transparent;
            helpWhatIsRegularUpdates.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpWhatIsRegularUpdates.Location = new Point(198, 76);
            helpWhatIsRegularUpdates.Margin = new Padding(4, 0, 4, 0);
            helpWhatIsRegularUpdates.Name = "helpWhatIsRegularUpdates";
            helpWhatIsRegularUpdates.Size = new Size(18, 13);
            helpWhatIsRegularUpdates.TabIndex = 30;
            helpWhatIsRegularUpdates.TabStop = true;
            helpWhatIsRegularUpdates.Tag = "iatag_ui_questionmark";
            helpWhatIsRegularUpdates.Text = " ? ";
            helpWhatIsRegularUpdates.LinkClicked += helpWhatIsRegularUpdates_LinkClicked;
            // 
            // radioBeta
            // 
            radioBeta.Bold = false;
            radioBeta.Checked = false;
            radioBeta.EnabledCalc = true;
            radioBeta.Font = new Font("Segoe UI", 10F);
            radioBeta.ForeColor = Color.FromArgb(66, 78, 90);
            radioBeta.Location = new Point(10, 100);
            radioBeta.Margin = new Padding(4, 3, 4, 3);
            radioBeta.Name = "radioBeta";
            radioBeta.Size = new Size(530, 31);
            radioBeta.TabIndex = 1;
            radioBeta.Tag = "iatag_ui_experimentalupdates";
            radioBeta.Text = "Frequent Updates";
            radioBeta.CheckedChanged += radioBeta_CheckedChanged;
            // 
            // radioRelease
            // 
            radioRelease.Bold = false;
            radioRelease.Checked = false;
            radioRelease.EnabledCalc = true;
            radioRelease.Font = new Font("Segoe UI", 10F);
            radioRelease.ForeColor = Color.FromArgb(66, 78, 90);
            radioRelease.Location = new Point(10, 62);
            radioRelease.Margin = new Padding(4, 3, 4, 3);
            radioRelease.Name = "radioRelease";
            radioRelease.Size = new Size(530, 31);
            radioRelease.TabIndex = 0;
            radioRelease.Tag = "iatag_ui_regularupdates";
            radioRelease.Text = "Regular Updates";
            radioRelease.CheckedChanged += radioRelease_CheckedChanged;
            // 
            // panelBox2
            // 
            panelBox2.BackColor = Color.FromArgb(240, 240, 240);
            panelBox2.Controls.Add(buttonPatreon);
            panelBox2.Controls.Add(buttonPaypal);
            panelBox2.Font = new Font("Segoe UI Semibold", 20F);
            panelBox2.ForeColor = Color.Black;
            panelBox2.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox2.HeaderHeight = 40;
            panelBox2.Location = new Point(303, 14);
            panelBox2.Margin = new Padding(4, 3, 4, 3);
            panelBox2.Name = "panelBox2";
            panelBox2.NoRounding = false;
            panelBox2.Size = new Size(268, 346);
            panelBox2.TabIndex = 2;
            panelBox2.Tag = "iatag_ui_misc_title";
            panelBox2.Text = "Misc";
            panelBox2.TextLocation = "8; 5";
            // 
            // buttonPatreon
            // 
            buttonPatreon.BackgroundImage = Properties.Resources.Patreon_Navy;
            buttonPatreon.BackgroundImageLayout = ImageLayout.Stretch;
            buttonPatreon.Location = new Point(16, 128);
            buttonPatreon.Margin = new Padding(4, 3, 4, 3);
            buttonPatreon.Name = "buttonPatreon";
            buttonPatreon.Size = new Size(224, 54);
            buttonPatreon.TabIndex = 8;
            buttonPatreon.UseVisualStyleBackColor = true;
            buttonPatreon.Click += buttonPatreon_Click;
            // 
            // buttonPaypal
            // 
            buttonPaypal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonPaypal.BackgroundImage = Properties.Resources.donate;
            buttonPaypal.BackgroundImageLayout = ImageLayout.Stretch;
            buttonPaypal.Location = new Point(16, 59);
            buttonPaypal.Margin = new Padding(4, 3, 4, 3);
            buttonPaypal.Name = "buttonPaypal";
            buttonPaypal.Size = new Size(224, 63);
            buttonPaypal.TabIndex = 7;
            buttonPaypal.UseVisualStyleBackColor = true;
            buttonPaypal.Click += button1_Click;
            // 
            // panelBox1
            // 
            panelBox1.BackColor = Color.FromArgb(240, 240, 240);
            panelBox1.Controls.Add(firefoxButton1);
            panelBox1.Controls.Add(buttonAdvancedSettings);
            panelBox1.Controls.Add(buttonImportExport);
            panelBox1.Controls.Add(buttonLanguageSelect);
            panelBox1.Controls.Add(buttonViewBackups);
            panelBox1.Controls.Add(buttonViewLogs);
            panelBox1.Font = new Font("Segoe UI Semibold", 20F);
            panelBox1.ForeColor = Color.Black;
            panelBox1.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox1.HeaderHeight = 40;
            panelBox1.Location = new Point(14, 14);
            panelBox1.Margin = new Padding(4, 3, 4, 3);
            panelBox1.Name = "panelBox1";
            panelBox1.NoRounding = false;
            panelBox1.Size = new Size(268, 346);
            panelBox1.TabIndex = 1;
            panelBox1.Tag = "iatag_ui_actions_title";
            panelBox1.Text = "Actions";
            panelBox1.TextLocation = "8; 5";
            // 
            // firefoxButton1
            // 
            firefoxButton1.BackColor = Color.FromArgb(240, 240, 240);
            firefoxButton1.BackColorDefault = Color.FromArgb(212, 212, 212);
            firefoxButton1.BackColorOverride = Color.FromArgb(245, 245, 245);
            firefoxButton1.BorderColor = Color.FromArgb(193, 193, 193);
            firefoxButton1.EnabledCalc = true;
            firefoxButton1.Font = new Font("Segoe UI", 10F);
            firefoxButton1.ForeColor = Color.FromArgb(56, 68, 80);
            firefoxButton1.HoverColor = Color.FromArgb(232, 232, 232);
            firefoxButton1.HoverForeColor = Color.FromArgb(193, 193, 193);
            firefoxButton1.Location = new Point(22, 278);
            firefoxButton1.Margin = new Padding(4, 3, 4, 3);
            firefoxButton1.Name = "firefoxButton1";
            firefoxButton1.Size = new Size(224, 37);
            firefoxButton1.TabIndex = 9;
            firefoxButton1.Tag = "iatag_ui_configmode";
            firefoxButton1.Text = "Switch to modes";
            firefoxButton1.Click += firefoxButton1_Click;
            // 
            // buttonAdvancedSettings
            // 
            buttonAdvancedSettings.BackColor = Color.FromArgb(240, 240, 240);
            buttonAdvancedSettings.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonAdvancedSettings.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonAdvancedSettings.BorderColor = Color.FromArgb(193, 193, 193);
            buttonAdvancedSettings.EnabledCalc = true;
            buttonAdvancedSettings.Font = new Font("Segoe UI", 10F);
            buttonAdvancedSettings.ForeColor = Color.FromArgb(56, 68, 80);
            buttonAdvancedSettings.HoverColor = Color.FromArgb(232, 232, 232);
            buttonAdvancedSettings.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonAdvancedSettings.Location = new Point(22, 234);
            buttonAdvancedSettings.Margin = new Padding(4, 3, 4, 3);
            buttonAdvancedSettings.Name = "buttonAdvancedSettings";
            buttonAdvancedSettings.Size = new Size(224, 37);
            buttonAdvancedSettings.TabIndex = 8;
            buttonAdvancedSettings.Tag = "iatag_ui_advancedsettings";
            buttonAdvancedSettings.Text = "Reconfigure tabs";
            buttonAdvancedSettings.Click += buttonAdvancedSettings_Click;
            // 
            // buttonImportExport
            // 
            buttonImportExport.BackColor = Color.FromArgb(240, 240, 240);
            buttonImportExport.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonImportExport.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonImportExport.BorderColor = Color.FromArgb(193, 193, 193);
            buttonImportExport.EnabledCalc = true;
            buttonImportExport.Font = new Font("Segoe UI", 10F);
            buttonImportExport.ForeColor = Color.FromArgb(56, 68, 80);
            buttonImportExport.HoverColor = Color.FromArgb(232, 232, 232);
            buttonImportExport.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonImportExport.Location = new Point(22, 190);
            buttonImportExport.Margin = new Padding(4, 3, 4, 3);
            buttonImportExport.Name = "buttonImportExport";
            buttonImportExport.Size = new Size(224, 37);
            buttonImportExport.TabIndex = 7;
            buttonImportExport.Tag = "iatag_ui_importexport";
            buttonImportExport.Text = "Import/Export";
            buttonImportExport.Click += buttonImportExport_Click;
            // 
            // buttonLanguageSelect
            // 
            buttonLanguageSelect.BackColor = Color.FromArgb(240, 240, 240);
            buttonLanguageSelect.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonLanguageSelect.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonLanguageSelect.BorderColor = Color.FromArgb(193, 193, 193);
            buttonLanguageSelect.EnabledCalc = true;
            buttonLanguageSelect.Font = new Font("Segoe UI", 10F);
            buttonLanguageSelect.ForeColor = Color.FromArgb(56, 68, 80);
            buttonLanguageSelect.HoverColor = Color.FromArgb(232, 232, 232);
            buttonLanguageSelect.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonLanguageSelect.Location = new Point(22, 147);
            buttonLanguageSelect.Margin = new Padding(4, 3, 4, 3);
            buttonLanguageSelect.Name = "buttonLanguageSelect";
            buttonLanguageSelect.Size = new Size(224, 37);
            buttonLanguageSelect.TabIndex = 6;
            buttonLanguageSelect.Tag = "iatag_ui_language_select";
            buttonLanguageSelect.Text = "Language Select";
            buttonLanguageSelect.Click += buttonLanguageSelect_Click;
            // 
            // buttonViewBackups
            // 
            buttonViewBackups.BackColor = Color.FromArgb(240, 240, 240);
            buttonViewBackups.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonViewBackups.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonViewBackups.BorderColor = Color.FromArgb(193, 193, 193);
            buttonViewBackups.EnabledCalc = true;
            buttonViewBackups.Font = new Font("Segoe UI", 10F);
            buttonViewBackups.ForeColor = Color.FromArgb(56, 68, 80);
            buttonViewBackups.HoverColor = Color.FromArgb(232, 232, 232);
            buttonViewBackups.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonViewBackups.Location = new Point(22, 103);
            buttonViewBackups.Margin = new Padding(4, 3, 4, 3);
            buttonViewBackups.Name = "buttonViewBackups";
            buttonViewBackups.Size = new Size(224, 37);
            buttonViewBackups.TabIndex = 5;
            buttonViewBackups.Tag = "iatag_ui_viewbackups";
            buttonViewBackups.Text = "View Backups";
            buttonViewBackups.Click += buttonViewBackups_Click;
            // 
            // buttonViewLogs
            // 
            buttonViewLogs.BackColor = Color.FromArgb(240, 240, 240);
            buttonViewLogs.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonViewLogs.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonViewLogs.BorderColor = Color.FromArgb(193, 193, 193);
            buttonViewLogs.EnabledCalc = true;
            buttonViewLogs.Font = new Font("Segoe UI", 10F);
            buttonViewLogs.ForeColor = Color.FromArgb(56, 68, 80);
            buttonViewLogs.HoverColor = Color.FromArgb(232, 232, 232);
            buttonViewLogs.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonViewLogs.Location = new Point(22, 59);
            buttonViewLogs.Margin = new Padding(4, 3, 4, 3);
            buttonViewLogs.Name = "buttonViewLogs";
            buttonViewLogs.Size = new Size(224, 37);
            buttonViewLogs.TabIndex = 6;
            buttonViewLogs.Tag = "iatag_ui_viewlogs";
            buttonViewLogs.Text = "View Logs";
            buttonViewLogs.Click += buttonViewLogs_Click;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1022, 577);
            Controls.Add(panelBox4);
            Controls.Add(pbAutomaticUpdates);
            Controls.Add(panelBox2);
            Controls.Add(panelBox1);
            Controls.Add(linkLabel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Settings";
            Load += SettingsWindow_Load;
            contextMenuStrip1.ResumeLayout(false);
            panelBox4.ResumeLayout(false);
            panelBox4.PerformLayout();
            pbAutomaticUpdates.ResumeLayout(false);
            pbAutomaticUpdates.PerformLayout();
            panelBox2.ResumeLayout(false);
            panelBox1.ResumeLayout(false);
            ResumeLayout(false);

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
        private PanelBox panelBox4;
        private FirefoxButton buttonLanguageSelect;
        private FirefoxButton buttonImportExport;
        private FirefoxCheckBox cbHideSkills;
        private FirefoxButton buttonAdvancedSettings;
        private System.Windows.Forms.LinkLabel linkSourceCode;
        private FirefoxCheckBox cbDualComputer;
        private System.Windows.Forms.LinkLabel helpWhatIsUsingMultiplePc;
        private System.Windows.Forms.Button buttonPaypal;
        private System.Windows.Forms.Button buttonPatreon;
        private System.Windows.Forms.LinkLabel helpWhatIsExperimentalUpdates;
        private System.Windows.Forms.LinkLabel helpWhatIsRegularUpdates;
        private FirefoxCheckBox cbStartMinimized;
        private FirefoxCheckBox cbDarkMode;
        private System.Windows.Forms.LinkLabel linkCheckForUpdates;
        private FirefoxCheckBox cbAutoDismiss;
        private System.Windows.Forms.LinkLabel linkDowngrade;
        private FirefoxButton firefoxButton1;
        private FirefoxCheckBox cbTransferAnyMod;
        private FirefoxCheckBox cbDelaySearch;
        private LinkLabel helpWhatIsDelayWhenSearching;
    }
}