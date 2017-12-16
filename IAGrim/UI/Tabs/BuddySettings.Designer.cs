namespace IAGrim.UI {
    partial class BuddySettings {
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripDescription = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panelBox3 = new PanelBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.firefoxButton2 = new FirefoxButton();
            this.buddyId = new System.Windows.Forms.TextBox();
            this.useridLabel = new System.Windows.Forms.Label();
            this.buddyList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.panelBox1 = new PanelBox();
            this.buttonSyncNow = new FirefoxButton();
            this.buddySyncEnabled = new FirefoxCheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStripDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.panelBox3.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStripDescription
            // 
            this.contextMenuStripDescription.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.contextMenuStripDescription.Name = "contextMenuStripDescription";
            this.contextMenuStripDescription.Size = new System.Drawing.Size(95, 26);
            this.contextMenuStripDescription.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDescription_Opening);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // panelBox3
            // 
            this.panelBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox3.Controls.Add(this.button1);
            this.panelBox3.Controls.Add(this.label1);
            this.panelBox3.Controls.Add(this.descriptionLabel);
            this.panelBox3.Controls.Add(this.firefoxButton2);
            this.panelBox3.Controls.Add(this.buddyId);
            this.panelBox3.Controls.Add(this.useridLabel);
            this.panelBox3.Controls.Add(this.buddyList);
            this.panelBox3.Controls.Add(this.tbDescription);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox3.HeaderHeight = 40;
            this.panelBox3.Location = new System.Drawing.Point(12, 12);
            this.panelBox3.Name = "panelBox3";
            this.panelBox3.NoRounding = false;
            this.panelBox3.Size = new System.Drawing.Size(531, 406);
            this.panelBox3.TabIndex = 3;
            this.panelBox3.Tag = "iatag_ui_buddies";
            this.panelBox3.Text = "Buddies";
            this.panelBox3.TextLocation = "8; 5";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackgroundImage = global::IAGrim.Properties.Resources.edit;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(2, 352);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(20, 20);
            this.button1.TabIndex = 7;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.Location = new System.Drawing.Point(21, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 19);
            this.label1.TabIndex = 8;
            this.label1.Tag = "iatag_ui_buddy_descriptive_help_text";
            this.label1.Text = "Enabling buddy items lets your friends see which items you have.";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.ContextMenuStrip = this.contextMenuStripDescription;
            this.descriptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 8F);
            this.descriptionLabel.Location = new System.Drawing.Point(22, 355);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(36, 13);
            this.descriptionLabel.TabIndex = 7;
            this.descriptionLabel.Text = "label1";
            // 
            // firefoxButton2
            // 
            this.firefoxButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.firefoxButton2.EnabledCalc = true;
            this.firefoxButton2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.firefoxButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.firefoxButton2.Location = new System.Drawing.Point(353, 276);
            this.firefoxButton2.Name = "firefoxButton2";
            this.firefoxButton2.Size = new System.Drawing.Size(160, 25);
            this.firefoxButton2.TabIndex = 6;
            this.firefoxButton2.Tag = "iatag_ui_add";
            this.firefoxButton2.Text = "Add";
            this.firefoxButton2.Click += new System.EventHandler(this.firefoxButton2_Click);
            // 
            // buddyId
            // 
            this.buddyId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buddyId.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.buddyId.Location = new System.Drawing.Point(25, 276);
            this.buddyId.Name = "buddyId";
            this.buddyId.Size = new System.Drawing.Size(322, 25);
            this.buddyId.TabIndex = 5;
            // 
            // useridLabel
            // 
            this.useridLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.useridLabel.AutoSize = true;
            this.useridLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.useridLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.useridLabel.Location = new System.Drawing.Point(3, 375);
            this.useridLabel.Name = "useridLabel";
            this.useridLabel.Size = new System.Drawing.Size(132, 25);
            this.useridLabel.TabIndex = 4;
            this.useridLabel.Text = "User ID: None";
            // 
            // buddyList
            // 
            this.buddyList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buddyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.buddyList.ContextMenuStrip = this.contextMenuStrip1;
            this.buddyList.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.buddyList.FullRowSelect = true;
            this.buddyList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.buddyList.HideSelection = false;
            this.buddyList.Location = new System.Drawing.Point(25, 52);
            this.buddyList.MultiSelect = false;
            this.buddyList.Name = "buddyList";
            this.buddyList.Size = new System.Drawing.Size(488, 218);
            this.buddyList.TabIndex = 4;
            this.buddyList.UseCompatibleStateImageBehavior = false;
            this.buddyList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "iatag_ui_buddy_header_buddy";
            this.columnHeader1.Text = "Buddy";
            this.columnHeader1.Width = 361;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "iatag_ui_buddy_header_items";
            this.columnHeader2.Text = "Items";
            this.columnHeader2.Width = 89;
            // 
            // tbDescription
            // 
            this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbDescription.Font = new System.Drawing.Font("Arial", 8F);
            this.tbDescription.Location = new System.Drawing.Point(22, 352);
            this.tbDescription.MaxLength = 32;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(192, 20);
            this.tbDescription.TabIndex = 7;
            this.tbDescription.Visible = false;
            this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
            // 
            // panelBox1
            // 
            this.panelBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox1.Controls.Add(this.buttonSyncNow);
            this.panelBox1.Controls.Add(this.buddySyncEnabled);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.HeaderHeight = 40;
            this.panelBox1.Location = new System.Drawing.Point(549, 12);
            this.panelBox1.Name = "panelBox1";
            this.panelBox1.NoRounding = false;
            this.panelBox1.Size = new System.Drawing.Size(230, 406);
            this.panelBox1.TabIndex = 2;
            this.panelBox1.Tag = "iatag_ui_configuration";
            this.panelBox1.Text = "Configuration";
            this.panelBox1.TextLocation = "8; 5";
            // 
            // buttonSyncNow
            // 
            this.buttonSyncNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSyncNow.EnabledCalc = true;
            this.buttonSyncNow.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonSyncNow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonSyncNow.Location = new System.Drawing.Point(20, 360);
            this.buttonSyncNow.Name = "buttonSyncNow";
            this.buttonSyncNow.Size = new System.Drawing.Size(192, 32);
            this.buttonSyncNow.TabIndex = 6;
            this.buttonSyncNow.Tag = "iatag_ui_syncnow";
            this.buttonSyncNow.Text = "Sync Now";
            this.buttonSyncNow.Click += new System.EventHandler(this.buttonSyncNow_Click);
            // 
            // buddySyncEnabled
            // 
            this.buddySyncEnabled.Bold = false;
            this.buddySyncEnabled.EnabledCalc = true;
            this.buddySyncEnabled.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buddySyncEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.buddySyncEnabled.Location = new System.Drawing.Point(3, 49);
            this.buddySyncEnabled.Name = "buddySyncEnabled";
            this.buddySyncEnabled.Size = new System.Drawing.Size(160, 27);
            this.buddySyncEnabled.TabIndex = 4;
            this.buddySyncEnabled.Tag = "iatag_ui_enabled";
            this.buddySyncEnabled.Text = "Enabled";
            // 
            // BuddySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 422);
            this.Controls.Add(this.panelBox3);
            this.Controls.Add(this.panelBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuddySettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BuddySettings";
            this.Load += new System.EventHandler(this.BuddySettings_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStripDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.panelBox3.ResumeLayout(false);
            this.panelBox3.PerformLayout();
            this.panelBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox1;
        private PanelBox panelBox3;
        private FirefoxCheckBox buddySyncEnabled;
        private System.Windows.Forms.Label useridLabel;
        private System.Windows.Forms.ListView buddyList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private FirefoxButton firefoxButton2;
        private System.Windows.Forms.TextBox buddyId;
        private FirefoxButton buttonSyncNow;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDescription;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}