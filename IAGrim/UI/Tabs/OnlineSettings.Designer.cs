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
            components = new System.ComponentModel.Container();
            buddyItemListContextMenu = new ContextMenuStrip(components);
            showToolStripMenuItem = new ToolStripMenuItem();
            hideToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            pbBuddyItems = new PanelBox();
            buddyList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            btnToggleBuddyVisibility = new FirefoxButton();
            btnDeleteBuddy = new FirefoxButton();
            btnModifyBuddy = new FirefoxButton();
            helpWhatIsThis = new LinkLabel();
            label2 = new Label();
            btnAddBuddy = new FirefoxButton();
            onlineBackup = new PanelBox();
            lbWhatIsCloudBackup = new LinkLabel();
            groupBoxBackupDetails = new GroupBox();
            linkViewCharacters = new LinkLabel();
            labelBuddyId = new Label();
            lbBuddyId = new Label();
            btnRefreshBackupDetails = new Button();
            linkLogout = new LinkLabel();
            linkDeleteBackup = new LinkLabel();
            labelStatus = new Label();
            label1 = new Label();
            cbDontWantBackups = new FirefoxCheckBox();
            buttonLogin = new FirefoxButton();
            buddyItemListContextMenu.SuspendLayout();
            pbBuddyItems.SuspendLayout();
            onlineBackup.SuspendLayout();
            groupBoxBackupDetails.SuspendLayout();
            SuspendLayout();
            // 
            // buddyItemListContextMenu
            // 
            buddyItemListContextMenu.Items.AddRange(new ToolStripItem[] { showToolStripMenuItem, hideToolStripMenuItem, editToolStripMenuItem, deleteToolStripMenuItem });
            buddyItemListContextMenu.Name = "contextMenuStrip1";
            buddyItemListContextMenu.Size = new Size(108, 92);
            buddyItemListContextMenu.Opening += buddyItemListContextMenu_Opening;
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(107, 22);
            showToolStripMenuItem.Text = "Show";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // hideToolStripMenuItem
            // 
            hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            hideToolStripMenuItem.Size = new Size(107, 22);
            hideToolStripMenuItem.Text = "Hide";
            hideToolStripMenuItem.Click += hideToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(107, 22);
            editToolStripMenuItem.Text = "Edit";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(107, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // pbBuddyItems
            // 
            pbBuddyItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbBuddyItems.BackColor = Color.FromArgb(240, 240, 240);
            pbBuddyItems.Controls.Add(buddyList);
            pbBuddyItems.Controls.Add(btnToggleBuddyVisibility);
            pbBuddyItems.Controls.Add(btnDeleteBuddy);
            pbBuddyItems.Controls.Add(btnModifyBuddy);
            pbBuddyItems.Controls.Add(helpWhatIsThis);
            pbBuddyItems.Controls.Add(label2);
            pbBuddyItems.Controls.Add(btnAddBuddy);
            pbBuddyItems.Font = new Font("Segoe UI Semibold", 20F);
            pbBuddyItems.ForeColor = Color.Black;
            pbBuddyItems.HeaderColor = Color.FromArgb(231, 231, 231);
            pbBuddyItems.HeaderHeight = 40;
            pbBuddyItems.Location = new Point(14, 218);
            pbBuddyItems.Margin = new Padding(4, 3, 4, 3);
            pbBuddyItems.Name = "pbBuddyItems";
            pbBuddyItems.NoRounding = false;
            pbBuddyItems.Size = new Size(889, 290);
            pbBuddyItems.TabIndex = 19;
            pbBuddyItems.Tag = "iatag_ui_buddies";
            pbBuddyItems.Text = "Buddies";
            pbBuddyItems.TextLocation = "8; 5";
            // 
            // buddyList
            // 
            buddyList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buddyList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            buddyList.ContextMenuStrip = buddyItemListContextMenu;
            buddyList.Font = new Font("Segoe UI Semibold", 12F);
            buddyList.FullRowSelect = true;
            buddyList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            buddyList.Location = new Point(29, 60);
            buddyList.Margin = new Padding(4, 3, 4, 3);
            buddyList.MultiSelect = false;
            buddyList.Name = "buddyList";
            buddyList.Size = new Size(851, 134);
            buddyList.TabIndex = 4;
            buddyList.UseCompatibleStateImageBehavior = false;
            buddyList.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Tag = "iatag_ui_buddy_header_buddy";
            columnHeader1.Text = "Buddy";
            columnHeader1.Width = 308;
            // 
            // columnHeader2
            // 
            columnHeader2.Tag = "iatag_ui_buddy_header_items";
            columnHeader2.Text = "Items";
            columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            columnHeader3.Tag = "iatag_ui_buddy_header_isvisible";
            columnHeader3.Text = "Visible/Active";
            columnHeader3.Width = 120;
            // 
            // btnToggleBuddyVisibility
            // 
            btnToggleBuddyVisibility.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnToggleBuddyVisibility.BackColorDefault = Color.FromArgb(212, 212, 212);
            btnToggleBuddyVisibility.BackColorOverride = Color.FromArgb(245, 245, 245);
            btnToggleBuddyVisibility.BorderColor = Color.FromArgb(193, 193, 193);
            btnToggleBuddyVisibility.EnabledCalc = true;
            btnToggleBuddyVisibility.Font = new Font("Segoe UI", 10F);
            btnToggleBuddyVisibility.ForeColor = Color.FromArgb(56, 68, 80);
            btnToggleBuddyVisibility.HoverColor = Color.FromArgb(232, 232, 232);
            btnToggleBuddyVisibility.HoverForeColor = Color.FromArgb(193, 193, 193);
            btnToggleBuddyVisibility.Location = new Point(608, 202);
            btnToggleBuddyVisibility.Margin = new Padding(4, 3, 4, 3);
            btnToggleBuddyVisibility.Name = "btnToggleBuddyVisibility";
            btnToggleBuddyVisibility.Size = new Size(187, 29);
            btnToggleBuddyVisibility.TabIndex = 12;
            btnToggleBuddyVisibility.Tag = "iatag_ui_buddy_toggle";
            btnToggleBuddyVisibility.Text = "Toggle visibility";
            btnToggleBuddyVisibility.Click += btnToggleBuddyVisibility_Click;
            // 
            // btnDeleteBuddy
            // 
            btnDeleteBuddy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteBuddy.BackColorDefault = Color.FromArgb(212, 212, 212);
            btnDeleteBuddy.BackColorOverride = Color.FromArgb(245, 245, 245);
            btnDeleteBuddy.BorderColor = Color.FromArgb(193, 193, 193);
            btnDeleteBuddy.EnabledCalc = true;
            btnDeleteBuddy.Font = new Font("Segoe UI", 10F);
            btnDeleteBuddy.ForeColor = Color.FromArgb(56, 68, 80);
            btnDeleteBuddy.HoverColor = Color.FromArgb(232, 232, 232);
            btnDeleteBuddy.HoverForeColor = Color.FromArgb(193, 193, 193);
            btnDeleteBuddy.Location = new Point(414, 202);
            btnDeleteBuddy.Margin = new Padding(4, 3, 4, 3);
            btnDeleteBuddy.Name = "btnDeleteBuddy";
            btnDeleteBuddy.Size = new Size(187, 29);
            btnDeleteBuddy.TabIndex = 11;
            btnDeleteBuddy.Tag = "iatag_ui_buddy_delete";
            btnDeleteBuddy.Text = "Delete buddy";
            btnDeleteBuddy.Click += btnDeleteBuddy_Click;
            // 
            // btnModifyBuddy
            // 
            btnModifyBuddy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnModifyBuddy.BackColorDefault = Color.FromArgb(212, 212, 212);
            btnModifyBuddy.BackColorOverride = Color.FromArgb(245, 245, 245);
            btnModifyBuddy.BorderColor = Color.FromArgb(193, 193, 193);
            btnModifyBuddy.EnabledCalc = true;
            btnModifyBuddy.Font = new Font("Segoe UI", 10F);
            btnModifyBuddy.ForeColor = Color.FromArgb(56, 68, 80);
            btnModifyBuddy.HoverColor = Color.FromArgb(232, 232, 232);
            btnModifyBuddy.HoverForeColor = Color.FromArgb(193, 193, 193);
            btnModifyBuddy.Location = new Point(220, 202);
            btnModifyBuddy.Margin = new Padding(4, 3, 4, 3);
            btnModifyBuddy.Name = "btnModifyBuddy";
            btnModifyBuddy.Size = new Size(187, 29);
            btnModifyBuddy.TabIndex = 10;
            btnModifyBuddy.Tag = "iatag_ui_buddy_modify";
            btnModifyBuddy.Text = "Modify buddy";
            btnModifyBuddy.Click += btnModifyBuddy_Click;
            // 
            // helpWhatIsThis
            // 
            helpWhatIsThis.AutoSize = true;
            helpWhatIsThis.BackColor = Color.Transparent;
            helpWhatIsThis.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpWhatIsThis.Location = new Point(139, 23);
            helpWhatIsThis.Margin = new Padding(4, 0, 4, 0);
            helpWhatIsThis.Name = "helpWhatIsThis";
            helpWhatIsThis.Size = new Size(73, 13);
            helpWhatIsThis.TabIndex = 9;
            helpWhatIsThis.TabStop = true;
            helpWhatIsThis.Tag = "iatag_ui_whatisthis";
            helpWhatIsThis.Text = "What is this?";
            helpWhatIsThis.LinkClicked += helpWhatIsThis_LinkClicked;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(24, 249);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(350, 19);
            label2.TabIndex = 8;
            label2.Tag = "iatag_ui_buddy_descriptive_help_text";
            label2.Text = "Buddy items lets you see which items your friends have.";
            // 
            // btnAddBuddy
            // 
            btnAddBuddy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddBuddy.BackColorDefault = Color.FromArgb(212, 212, 212);
            btnAddBuddy.BackColorOverride = Color.FromArgb(245, 245, 245);
            btnAddBuddy.BorderColor = Color.FromArgb(193, 193, 193);
            btnAddBuddy.EnabledCalc = true;
            btnAddBuddy.Font = new Font("Segoe UI", 10F);
            btnAddBuddy.ForeColor = Color.FromArgb(56, 68, 80);
            btnAddBuddy.HoverColor = Color.FromArgb(232, 232, 232);
            btnAddBuddy.HoverForeColor = Color.FromArgb(193, 193, 193);
            btnAddBuddy.Location = new Point(29, 202);
            btnAddBuddy.Margin = new Padding(4, 3, 4, 3);
            btnAddBuddy.Name = "btnAddBuddy";
            btnAddBuddy.Size = new Size(187, 29);
            btnAddBuddy.TabIndex = 6;
            btnAddBuddy.Tag = "iatag_ui_buddy_add";
            btnAddBuddy.Text = "Add buddy";
            btnAddBuddy.Click += btnAddBuddy_Click;
            // 
            // onlineBackup
            // 
            onlineBackup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            onlineBackup.BackColor = Color.FromArgb(240, 240, 240);
            onlineBackup.Controls.Add(lbWhatIsCloudBackup);
            onlineBackup.Controls.Add(groupBoxBackupDetails);
            onlineBackup.Controls.Add(cbDontWantBackups);
            onlineBackup.Controls.Add(buttonLogin);
            onlineBackup.Font = new Font("Segoe UI Semibold", 20F);
            onlineBackup.ForeColor = Color.Black;
            onlineBackup.HeaderColor = Color.FromArgb(231, 231, 231);
            onlineBackup.HeaderHeight = 40;
            onlineBackup.Location = new Point(14, 14);
            onlineBackup.Margin = new Padding(4, 3, 4, 3);
            onlineBackup.Name = "onlineBackup";
            onlineBackup.NoRounding = false;
            onlineBackup.Size = new Size(889, 201);
            onlineBackup.TabIndex = 12;
            onlineBackup.Tag = "iatag_ui_online_backup";
            onlineBackup.Text = "Cloud Backup";
            onlineBackup.TextLocation = "8; 5";
            // 
            // lbWhatIsCloudBackup
            // 
            lbWhatIsCloudBackup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbWhatIsCloudBackup.AutoSize = true;
            lbWhatIsCloudBackup.BackColor = Color.Transparent;
            lbWhatIsCloudBackup.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbWhatIsCloudBackup.Location = new Point(800, 0);
            lbWhatIsCloudBackup.Margin = new Padding(4, 0, 4, 0);
            lbWhatIsCloudBackup.Name = "lbWhatIsCloudBackup";
            lbWhatIsCloudBackup.Size = new Size(73, 13);
            lbWhatIsCloudBackup.TabIndex = 10;
            lbWhatIsCloudBackup.TabStop = true;
            lbWhatIsCloudBackup.Tag = "iatag_ui_whatisthis";
            lbWhatIsCloudBackup.Text = "What is this?";
            lbWhatIsCloudBackup.LinkClicked += linkLabel1_LinkClicked_1;
            // 
            // groupBoxBackupDetails
            // 
            groupBoxBackupDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxBackupDetails.Controls.Add(linkViewCharacters);
            groupBoxBackupDetails.Controls.Add(labelBuddyId);
            groupBoxBackupDetails.Controls.Add(lbBuddyId);
            groupBoxBackupDetails.Controls.Add(btnRefreshBackupDetails);
            groupBoxBackupDetails.Controls.Add(linkLogout);
            groupBoxBackupDetails.Controls.Add(linkDeleteBackup);
            groupBoxBackupDetails.Controls.Add(labelStatus);
            groupBoxBackupDetails.Controls.Add(label1);
            groupBoxBackupDetails.Location = new Point(407, 53);
            groupBoxBackupDetails.Margin = new Padding(4, 3, 4, 3);
            groupBoxBackupDetails.Name = "groupBoxBackupDetails";
            groupBoxBackupDetails.Padding = new Padding(4, 3, 4, 3);
            groupBoxBackupDetails.Size = new Size(478, 144);
            groupBoxBackupDetails.TabIndex = 17;
            groupBoxBackupDetails.TabStop = false;
            groupBoxBackupDetails.Tag = "iatag_ui_backup_details";
            groupBoxBackupDetails.Text = "Details";
            // 
            // linkViewCharacters
            // 
            linkViewCharacters.AutoSize = true;
            linkViewCharacters.BackColor = Color.Transparent;
            linkViewCharacters.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkViewCharacters.Location = new Point(7, 123);
            linkViewCharacters.Margin = new Padding(4, 0, 4, 0);
            linkViewCharacters.Name = "linkViewCharacters";
            linkViewCharacters.Size = new Size(89, 13);
            linkViewCharacters.TabIndex = 19;
            linkViewCharacters.TabStop = true;
            linkViewCharacters.Tag = "iatag_ui_backup_viewcharacters";
            linkViewCharacters.Text = "View Characters";
            linkViewCharacters.LinkClicked += linkViewCharacters_LinkClicked;
            // 
            // labelBuddyId
            // 
            labelBuddyId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelBuddyId.Cursor = Cursors.Hand;
            labelBuddyId.Font = new Font("Segoe UI", 10F);
            labelBuddyId.Location = new Point(83, 72);
            labelBuddyId.Margin = new Padding(4, 0, 4, 0);
            labelBuddyId.Name = "labelBuddyId";
            labelBuddyId.Size = new Size(230, 22);
            labelBuddyId.TabIndex = 6;
            labelBuddyId.Text = "-----";
            labelBuddyId.Click += labelBuddyId_Click;
            // 
            // lbBuddyId
            // 
            lbBuddyId.AutoSize = true;
            lbBuddyId.Font = new Font("Segoe UI", 10F);
            lbBuddyId.Location = new Point(7, 72);
            lbBuddyId.Margin = new Padding(4, 0, 4, 0);
            lbBuddyId.Name = "lbBuddyId";
            lbBuddyId.Size = new Size(63, 19);
            lbBuddyId.TabIndex = 5;
            lbBuddyId.Tag = "iatag_ui_backup_buddy_id";
            lbBuddyId.Text = "BuddyId:";
            // 
            // btnRefreshBackupDetails
            // 
            btnRefreshBackupDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshBackupDetails.BackgroundImage = Properties.Resources.refresh;
            btnRefreshBackupDetails.BackgroundImageLayout = ImageLayout.Stretch;
            btnRefreshBackupDetails.Location = new Point(436, 28);
            btnRefreshBackupDetails.Margin = new Padding(4, 3, 4, 3);
            btnRefreshBackupDetails.Name = "btnRefreshBackupDetails";
            btnRefreshBackupDetails.Size = new Size(37, 37);
            btnRefreshBackupDetails.TabIndex = 4;
            btnRefreshBackupDetails.UseVisualStyleBackColor = true;
            btnRefreshBackupDetails.Click += btnRefreshBackupDetails_Click;
            // 
            // linkLogout
            // 
            linkLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLogout.AutoSize = true;
            linkLogout.Font = new Font("Segoe UI", 10F);
            linkLogout.Location = new Point(410, 104);
            linkLogout.Margin = new Padding(4, 0, 4, 0);
            linkLogout.Name = "linkLogout";
            linkLogout.Size = new Size(53, 19);
            linkLogout.TabIndex = 3;
            linkLogout.TabStop = true;
            linkLogout.Tag = "iatag_ui_backup_logout";
            linkLogout.Text = "Logout";
            linkLogout.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkDeleteBackup
            // 
            linkDeleteBackup.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkDeleteBackup.AutoSize = true;
            linkDeleteBackup.Font = new Font("Segoe UI", 6F);
            linkDeleteBackup.Location = new Point(405, 126);
            linkDeleteBackup.Margin = new Padding(4, 0, 4, 0);
            linkDeleteBackup.Name = "linkDeleteBackup";
            linkDeleteBackup.Size = new Size(57, 11);
            linkDeleteBackup.TabIndex = 2;
            linkDeleteBackup.TabStop = true;
            linkDeleteBackup.Tag = "iatag_ui_backup_delete";
            linkDeleteBackup.Text = "Delete backup";
            linkDeleteBackup.LinkClicked += linkDeleteBackup_LinkClicked;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelStatus.Font = new Font("Segoe UI", 10F);
            labelStatus.Location = new Point(72, 48);
            labelStatus.Margin = new Padding(4, 0, 4, 0);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(399, 22);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "-----";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(7, 48);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(50, 19);
            label1.TabIndex = 0;
            label1.Tag = "iatag_ui_backup_status_label";
            label1.Text = "Status:";
            // 
            // cbDontWantBackups
            // 
            cbDontWantBackups.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbDontWantBackups.Bold = false;
            cbDontWantBackups.EnabledCalc = true;
            cbDontWantBackups.Font = new Font("Segoe UI", 10F);
            cbDontWantBackups.ForeColor = Color.FromArgb(66, 78, 90);
            cbDontWantBackups.IsDarkMode = false;
            cbDontWantBackups.Location = new Point(10, 97);
            cbDontWantBackups.Margin = new Padding(4, 3, 4, 3);
            cbDontWantBackups.Name = "cbDontWantBackups";
            cbDontWantBackups.Size = new Size(410, 31);
            cbDontWantBackups.TabIndex = 16;
            cbDontWantBackups.Tag = "iatag_ui_dontwantbackups";
            cbDontWantBackups.Text = "I don't want online features, stop asking me!";
            cbDontWantBackups.UseVisualStyleBackColor = true;
            cbDontWantBackups.CheckedChanged += cbDontWantBackups_CheckedChanged;
            // 
            // buttonLogin
            // 
            buttonLogin.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonLogin.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonLogin.BorderColor = Color.FromArgb(193, 193, 193);
            buttonLogin.EnabledCalc = true;
            buttonLogin.Font = new Font("Segoe UI", 10F);
            buttonLogin.ForeColor = Color.FromArgb(56, 68, 80);
            buttonLogin.HoverColor = Color.FromArgb(232, 232, 232);
            buttonLogin.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonLogin.Location = new Point(10, 62);
            buttonLogin.Margin = new Padding(4, 3, 4, 3);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(286, 28);
            buttonLogin.TabIndex = 15;
            buttonLogin.Tag = "iatag_ui_login";
            buttonLogin.Text = "Login";
            buttonLogin.Click += firefoxButton1_Click;
            // 
            // OnlineSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(917, 528);
            Controls.Add(pbBuddyItems);
            Controls.Add(onlineBackup);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OnlineSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Backup Settings";
            Load += BackupSettings_Load;
            buddyItemListContextMenu.ResumeLayout(false);
            pbBuddyItems.ResumeLayout(false);
            pbBuddyItems.PerformLayout();
            onlineBackup.ResumeLayout(false);
            onlineBackup.PerformLayout();
            groupBoxBackupDetails.ResumeLayout(false);
            groupBoxBackupDetails.PerformLayout();
            ResumeLayout(false);

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