namespace IAGrim.UI.Tabs {
    partial class OnlineSettings {
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
            this.buddyItemListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbBuddyItems = new PanelBox();
            this.buddyList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnToggleBuddyVisibility = new FirefoxButton();
            this.btnDeleteBuddy = new FirefoxButton();
            this.btnModifyBuddy = new FirefoxButton();
            this.helpWhatIsThis = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddBuddy = new FirefoxButton();
            this.onlineBackup = new PanelBox();
            this.lbWhatIsCloudBackup = new System.Windows.Forms.LinkLabel();
            this.groupBoxBackupDetails = new System.Windows.Forms.GroupBox();
            this.linkViewCharacters = new System.Windows.Forms.LinkLabel();
            this.labelBuddyId = new System.Windows.Forms.Label();
            this.lbBuddyId = new System.Windows.Forms.Label();
            this.btnRefreshBackupDetails = new System.Windows.Forms.Button();
            this.linkLogout = new System.Windows.Forms.LinkLabel();
            this.linkDeleteBackup = new System.Windows.Forms.LinkLabel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDontWantBackups = new FirefoxCheckBox();
            this.buttonLogin = new FirefoxButton();
            this.buddyItemListContextMenu.SuspendLayout();
            this.pbBuddyItems.SuspendLayout();
            this.onlineBackup.SuspendLayout();
            this.groupBoxBackupDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // buddyItemListContextMenu
            // 
            this.buddyItemListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.buddyItemListContextMenu.Name = "contextMenuStrip1";
            this.buddyItemListContextMenu.Size = new System.Drawing.Size(108, 92);
            this.buddyItemListContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.buddyItemListContextMenu_Opening);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // pbBuddyItems
            // 
            this.pbBuddyItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbBuddyItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbBuddyItems.Controls.Add(this.buddyList);
            this.pbBuddyItems.Controls.Add(this.btnToggleBuddyVisibility);
            this.pbBuddyItems.Controls.Add(this.btnDeleteBuddy);
            this.pbBuddyItems.Controls.Add(this.btnModifyBuddy);
            this.pbBuddyItems.Controls.Add(this.helpWhatIsThis);
            this.pbBuddyItems.Controls.Add(this.label2);
            this.pbBuddyItems.Controls.Add(this.btnAddBuddy);
            this.pbBuddyItems.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.pbBuddyItems.ForeColor = System.Drawing.Color.Black;
            this.pbBuddyItems.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.pbBuddyItems.HeaderHeight = 40;
            this.pbBuddyItems.Location = new System.Drawing.Point(12, 189);
            this.pbBuddyItems.Name = "pbBuddyItems";
            this.pbBuddyItems.NoRounding = false;
            this.pbBuddyItems.Size = new System.Drawing.Size(762, 251);
            this.pbBuddyItems.TabIndex = 19;
            this.pbBuddyItems.Tag = "iatag_ui_buddies";
            this.pbBuddyItems.Text = "Buddies";
            this.pbBuddyItems.TextLocation = "8; 5";
            // 
            // buddyList
            // 
            this.buddyList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buddyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.buddyList.ContextMenuStrip = this.buddyItemListContextMenu;
            this.buddyList.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.buddyList.FullRowSelect = true;
            this.buddyList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.buddyList.HideSelection = false;
            this.buddyList.Location = new System.Drawing.Point(25, 52);
            this.buddyList.MultiSelect = false;
            this.buddyList.Name = "buddyList";
            this.buddyList.Size = new System.Drawing.Size(730, 117);
            this.buddyList.TabIndex = 4;
            this.buddyList.UseCompatibleStateImageBehavior = false;
            this.buddyList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "iatag_ui_buddy_header_buddy";
            this.columnHeader1.Text = "Buddy";
            this.columnHeader1.Width = 308;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "iatag_ui_buddy_header_items";
            this.columnHeader2.Text = "Items";
            this.columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "iatag_ui_buddy_header_isvisible";
            this.columnHeader3.Text = "Visible/Active";
            this.columnHeader3.Width = 120;
            // 
            // btnToggleBuddyVisibility
            // 
            this.btnToggleBuddyVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleBuddyVisibility.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.btnToggleBuddyVisibility.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnToggleBuddyVisibility.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnToggleBuddyVisibility.EnabledCalc = true;
            this.btnToggleBuddyVisibility.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnToggleBuddyVisibility.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.btnToggleBuddyVisibility.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnToggleBuddyVisibility.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnToggleBuddyVisibility.Location = new System.Drawing.Point(521, 175);
            this.btnToggleBuddyVisibility.Name = "btnToggleBuddyVisibility";
            this.btnToggleBuddyVisibility.Size = new System.Drawing.Size(160, 25);
            this.btnToggleBuddyVisibility.TabIndex = 12;
            this.btnToggleBuddyVisibility.Tag = "iatag_ui_buddy_toggle";
            this.btnToggleBuddyVisibility.Text = "Toggle visibility";
            this.btnToggleBuddyVisibility.Click += new System.EventHandler(this.btnToggleBuddyVisibility_Click);
            // 
            // btnDeleteBuddy
            // 
            this.btnDeleteBuddy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteBuddy.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.btnDeleteBuddy.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnDeleteBuddy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnDeleteBuddy.EnabledCalc = true;
            this.btnDeleteBuddy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDeleteBuddy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.btnDeleteBuddy.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnDeleteBuddy.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnDeleteBuddy.Location = new System.Drawing.Point(355, 175);
            this.btnDeleteBuddy.Name = "btnDeleteBuddy";
            this.btnDeleteBuddy.Size = new System.Drawing.Size(160, 25);
            this.btnDeleteBuddy.TabIndex = 11;
            this.btnDeleteBuddy.Tag = "iatag_ui_buddy_delete";
            this.btnDeleteBuddy.Text = "Delete buddy";
            this.btnDeleteBuddy.Click += new System.EventHandler(this.btnDeleteBuddy_Click);
            // 
            // btnModifyBuddy
            // 
            this.btnModifyBuddy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnModifyBuddy.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.btnModifyBuddy.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnModifyBuddy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnModifyBuddy.EnabledCalc = true;
            this.btnModifyBuddy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnModifyBuddy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.btnModifyBuddy.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnModifyBuddy.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnModifyBuddy.Location = new System.Drawing.Point(189, 175);
            this.btnModifyBuddy.Name = "btnModifyBuddy";
            this.btnModifyBuddy.Size = new System.Drawing.Size(160, 25);
            this.btnModifyBuddy.TabIndex = 10;
            this.btnModifyBuddy.Tag = "iatag_ui_buddy_modify";
            this.btnModifyBuddy.Text = "Modify buddy";
            this.btnModifyBuddy.Click += new System.EventHandler(this.btnModifyBuddy_Click);
            // 
            // helpWhatIsThis
            // 
            this.helpWhatIsThis.AutoSize = true;
            this.helpWhatIsThis.BackColor = System.Drawing.Color.Transparent;
            this.helpWhatIsThis.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhatIsThis.Location = new System.Drawing.Point(119, 20);
            this.helpWhatIsThis.Name = "helpWhatIsThis";
            this.helpWhatIsThis.Size = new System.Drawing.Size(73, 13);
            this.helpWhatIsThis.TabIndex = 9;
            this.helpWhatIsThis.TabStop = true;
            this.helpWhatIsThis.Tag = "iatag_ui_whatisthis";
            this.helpWhatIsThis.Text = "What is this?";
            this.helpWhatIsThis.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhatIsThis_LinkClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label2.Location = new System.Drawing.Point(21, 216);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(350, 19);
            this.label2.TabIndex = 8;
            this.label2.Tag = "iatag_ui_buddy_descriptive_help_text";
            this.label2.Text = "Buddy items lets you see which items your friends have.";
            // 
            // btnAddBuddy
            // 
            this.btnAddBuddy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddBuddy.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.btnAddBuddy.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnAddBuddy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnAddBuddy.EnabledCalc = true;
            this.btnAddBuddy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnAddBuddy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.btnAddBuddy.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnAddBuddy.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.btnAddBuddy.Location = new System.Drawing.Point(25, 175);
            this.btnAddBuddy.Name = "btnAddBuddy";
            this.btnAddBuddy.Size = new System.Drawing.Size(160, 25);
            this.btnAddBuddy.TabIndex = 6;
            this.btnAddBuddy.Tag = "iatag_ui_buddy_add";
            this.btnAddBuddy.Text = "Add buddy";
            this.btnAddBuddy.Click += new System.EventHandler(this.btnAddBuddy_Click);
            // 
            // onlineBackup
            // 
            this.onlineBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onlineBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.onlineBackup.Controls.Add(this.lbWhatIsCloudBackup);
            this.onlineBackup.Controls.Add(this.groupBoxBackupDetails);
            this.onlineBackup.Controls.Add(this.cbDontWantBackups);
            this.onlineBackup.Controls.Add(this.buttonLogin);
            this.onlineBackup.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.onlineBackup.ForeColor = System.Drawing.Color.Black;
            this.onlineBackup.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.onlineBackup.HeaderHeight = 40;
            this.onlineBackup.Location = new System.Drawing.Point(12, 12);
            this.onlineBackup.Name = "onlineBackup";
            this.onlineBackup.NoRounding = false;
            this.onlineBackup.Size = new System.Drawing.Size(762, 174);
            this.onlineBackup.TabIndex = 12;
            this.onlineBackup.Tag = "iatag_ui_online_backup";
            this.onlineBackup.Text = "Cloud Backup";
            this.onlineBackup.TextLocation = "8; 5";
            // 
            // lbWhatIsCloudBackup
            // 
            this.lbWhatIsCloudBackup.AutoSize = true;
            this.lbWhatIsCloudBackup.BackColor = System.Drawing.Color.Transparent;
            this.lbWhatIsCloudBackup.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWhatIsCloudBackup.Location = new System.Drawing.Point(686, 0);
            this.lbWhatIsCloudBackup.Name = "lbWhatIsCloudBackup";
            this.lbWhatIsCloudBackup.Size = new System.Drawing.Size(73, 13);
            this.lbWhatIsCloudBackup.TabIndex = 10;
            this.lbWhatIsCloudBackup.TabStop = true;
            this.lbWhatIsCloudBackup.Tag = "iatag_ui_whatisthis";
            this.lbWhatIsCloudBackup.Text = "What is this?";
            this.lbWhatIsCloudBackup.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked_1);
            // 
            // groupBoxBackupDetails
            // 
            this.groupBoxBackupDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBackupDetails.Controls.Add(this.linkViewCharacters);
            this.groupBoxBackupDetails.Controls.Add(this.labelBuddyId);
            this.groupBoxBackupDetails.Controls.Add(this.lbBuddyId);
            this.groupBoxBackupDetails.Controls.Add(this.btnRefreshBackupDetails);
            this.groupBoxBackupDetails.Controls.Add(this.linkLogout);
            this.groupBoxBackupDetails.Controls.Add(this.linkDeleteBackup);
            this.groupBoxBackupDetails.Controls.Add(this.labelStatus);
            this.groupBoxBackupDetails.Controls.Add(this.label1);
            this.groupBoxBackupDetails.Location = new System.Drawing.Point(349, 46);
            this.groupBoxBackupDetails.Name = "groupBoxBackupDetails";
            this.groupBoxBackupDetails.Size = new System.Drawing.Size(410, 125);
            this.groupBoxBackupDetails.TabIndex = 17;
            this.groupBoxBackupDetails.TabStop = false;
            this.groupBoxBackupDetails.Tag = "iatag_ui_backup_details";
            this.groupBoxBackupDetails.Text = "Details";
            // 
            // linkViewCharacters
            // 
            this.linkViewCharacters.AutoSize = true;
            this.linkViewCharacters.BackColor = System.Drawing.Color.Transparent;
            this.linkViewCharacters.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkViewCharacters.Location = new System.Drawing.Point(6, 107);
            this.linkViewCharacters.Name = "linkViewCharacters";
            this.linkViewCharacters.Size = new System.Drawing.Size(89, 13);
            this.linkViewCharacters.TabIndex = 19;
            this.linkViewCharacters.TabStop = true;
            this.linkViewCharacters.Tag = "iatag_ui_backup_viewcharacters";
            this.linkViewCharacters.Text = "View Characters";
            this.linkViewCharacters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkViewCharacters_LinkClicked);
            // 
            // labelBuddyId
            // 
            this.labelBuddyId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBuddyId.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelBuddyId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelBuddyId.Location = new System.Drawing.Point(71, 62);
            this.labelBuddyId.Name = "labelBuddyId";
            this.labelBuddyId.Size = new System.Drawing.Size(197, 19);
            this.labelBuddyId.TabIndex = 6;
            this.labelBuddyId.Text = "-----";
            this.labelBuddyId.Click += new System.EventHandler(this.labelBuddyId_Click);
            // 
            // lbBuddyId
            // 
            this.lbBuddyId.AutoSize = true;
            this.lbBuddyId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lbBuddyId.Location = new System.Drawing.Point(6, 62);
            this.lbBuddyId.Name = "lbBuddyId";
            this.lbBuddyId.Size = new System.Drawing.Size(63, 19);
            this.lbBuddyId.TabIndex = 5;
            this.lbBuddyId.Tag = "iatag_ui_backup_buddy_id";
            this.lbBuddyId.Text = "BuddyId:";
            // 
            // btnRefreshBackupDetails
            // 
            this.btnRefreshBackupDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBackupDetails.BackgroundImage = global::IAGrim.Properties.Resources.refresh;
            this.btnRefreshBackupDetails.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRefreshBackupDetails.Location = new System.Drawing.Point(374, 24);
            this.btnRefreshBackupDetails.Name = "btnRefreshBackupDetails";
            this.btnRefreshBackupDetails.Size = new System.Drawing.Size(32, 32);
            this.btnRefreshBackupDetails.TabIndex = 4;
            this.btnRefreshBackupDetails.UseVisualStyleBackColor = true;
            this.btnRefreshBackupDetails.Click += new System.EventHandler(this.btnRefreshBackupDetails_Click);
            // 
            // linkLogout
            // 
            this.linkLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLogout.AutoSize = true;
            this.linkLogout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.linkLogout.Location = new System.Drawing.Point(351, 90);
            this.linkLogout.Name = "linkLogout";
            this.linkLogout.Size = new System.Drawing.Size(53, 19);
            this.linkLogout.TabIndex = 3;
            this.linkLogout.TabStop = true;
            this.linkLogout.Tag = "iatag_ui_backup_logout";
            this.linkLogout.Text = "Logout";
            this.linkLogout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkDeleteBackup
            // 
            this.linkDeleteBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkDeleteBackup.AutoSize = true;
            this.linkDeleteBackup.Font = new System.Drawing.Font("Segoe UI", 6F);
            this.linkDeleteBackup.Location = new System.Drawing.Point(347, 109);
            this.linkDeleteBackup.Name = "linkDeleteBackup";
            this.linkDeleteBackup.Size = new System.Drawing.Size(57, 11);
            this.linkDeleteBackup.TabIndex = 2;
            this.linkDeleteBackup.TabStop = true;
            this.linkDeleteBackup.Tag = "iatag_ui_backup_delete";
            this.linkDeleteBackup.Text = "Delete backup";
            this.linkDeleteBackup.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDeleteBackup_LinkClicked);
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelStatus.Location = new System.Drawing.Point(62, 42);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(342, 19);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "-----";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 19);
            this.label1.TabIndex = 0;
            this.label1.Tag = "iatag_ui_backup_status_label";
            this.label1.Text = "Status:";
            // 
            // cbDontWantBackups
            // 
            this.cbDontWantBackups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDontWantBackups.Bold = false;
            this.cbDontWantBackups.EnabledCalc = true;
            this.cbDontWantBackups.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDontWantBackups.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDontWantBackups.IsDarkMode = false;
            this.cbDontWantBackups.Location = new System.Drawing.Point(9, 84);
            this.cbDontWantBackups.Name = "cbDontWantBackups";
            this.cbDontWantBackups.Size = new System.Drawing.Size(351, 27);
            this.cbDontWantBackups.TabIndex = 16;
            this.cbDontWantBackups.Tag = "iatag_ui_dontwantbackups";
            this.cbDontWantBackups.Text = "I don\'t want online features, stop asking me!";
            this.cbDontWantBackups.UseVisualStyleBackColor = true;
            this.cbDontWantBackups.CheckedChanged += new System.EventHandler(this.cbDontWantBackups_CheckedChanged);
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonLogin.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonLogin.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLogin.EnabledCalc = true;
            this.buttonLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLogin.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonLogin.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonLogin.Location = new System.Drawing.Point(9, 54);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(245, 24);
            this.buttonLogin.TabIndex = 15;
            this.buttonLogin.Tag = "iatag_ui_login";
            this.buttonLogin.Text = "Login";
            this.buttonLogin.Click += new System.EventHandler(this.firefoxButton1_Click);
            // 
            // OnlineSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 458);
            this.Controls.Add(this.pbBuddyItems);
            this.Controls.Add(this.onlineBackup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OnlineSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Backup Settings";
            this.Load += new System.EventHandler(this.BackupSettings_Load);
            this.buddyItemListContextMenu.ResumeLayout(false);
            this.pbBuddyItems.ResumeLayout(false);
            this.pbBuddyItems.PerformLayout();
            this.onlineBackup.ResumeLayout(false);
            this.onlineBackup.PerformLayout();
            this.groupBoxBackupDetails.ResumeLayout(false);
            this.groupBoxBackupDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private PanelBox onlineBackup;
        private FirefoxButton buttonLogin;
        private FirefoxCheckBox cbDontWantBackups;
        private System.Windows.Forms.GroupBox groupBoxBackupDetails;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkDeleteBackup;
        private System.Windows.Forms.LinkLabel linkLogout;
        private System.Windows.Forms.Button btnRefreshBackupDetails;
        private PanelBox pbBuddyItems;
        private System.Windows.Forms.LinkLabel helpWhatIsThis;
        private System.Windows.Forms.Label label2;
        private FirefoxButton btnAddBuddy;
        private System.Windows.Forms.ListView buddyList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label lbBuddyId;
        private System.Windows.Forms.LinkLabel lbWhatIsCloudBackup;
        private System.Windows.Forms.Label labelBuddyId;
        private System.Windows.Forms.ContextMenuStrip buddyItemListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private FirefoxButton btnDeleteBuddy;
        private FirefoxButton btnModifyBuddy;
        private System.Windows.Forms.LinkLabel linkViewCharacters;
        private FirefoxButton btnToggleBuddyVisibility;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}