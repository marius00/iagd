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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panelBox3 = new PanelBox();
            this.helpWhatIsThis = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.firefoxButton2 = new FirefoxButton();
            this.buddyId = new System.Windows.Forms.TextBox();
            this.useridLabel = new System.Windows.Forms.Label();
            this.buddyList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelBox1 = new PanelBox();
            this.buddySyncEnabled = new FirefoxCheckBox();
            this.contextMenuStrip1.SuspendLayout();
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // panelBox3
            // 
            this.panelBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox3.Controls.Add(this.helpWhatIsThis);
            this.panelBox3.Controls.Add(this.label1);
            this.panelBox3.Controls.Add(this.firefoxButton2);
            this.panelBox3.Controls.Add(this.buddyId);
            this.panelBox3.Controls.Add(this.useridLabel);
            this.panelBox3.Controls.Add(this.buddyList);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox3.ForeColor = System.Drawing.Color.Black;
            this.panelBox3.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
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
            // firefoxButton2
            // 
            this.firefoxButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.firefoxButton2.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.firefoxButton2.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.firefoxButton2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.firefoxButton2.EnabledCalc = true;
            this.firefoxButton2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.firefoxButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.firefoxButton2.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.firefoxButton2.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
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
            this.columnHeader1.Width = 308;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "iatag_ui_buddy_header_items";
            this.columnHeader2.Text = "Items";
            this.columnHeader2.Width = 161;
            // 
            // panelBox1
            // 
            this.panelBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox1.Controls.Add(this.buddySyncEnabled);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.ForeColor = System.Drawing.Color.Black;
            this.panelBox1.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
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
            // buddySyncEnabled
            // 
            this.buddySyncEnabled.Bold = false;
            this.buddySyncEnabled.EnabledCalc = true;
            this.buddySyncEnabled.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buddySyncEnabled.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.buddySyncEnabled.IsDarkMode = false;
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
            this.Controls.Add(this.panelBox1);
            this.Controls.Add(this.panelBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuddySettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BuddySettings";
            this.Load += new System.EventHandler(this.BuddySettings_Load);
            this.contextMenuStrip1.ResumeLayout(false);
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
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel helpWhatIsThis;
    }
}