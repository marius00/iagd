namespace IAGrim.UI {
    partial class BackupSettings {
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
            this.panelBox1 = new PanelBox();
            this.backupEmailLabel = new System.Windows.Forms.Label();
            this.labelItemSyncFeedback = new System.Windows.Forms.Label();
            this.buttonLogin = new FirefoxButton();
            this.logoutThisComputer = new System.Windows.Forms.LinkLabel();
            this.logoutAllComputers = new System.Windows.Forms.LinkLabel();
            this.panelBox5 = new PanelBox();
            this.buttonBackupNow = new FirefoxButton();
            this.buttonCustom = new System.Windows.Forms.Button();
            this.cbCustom = new FirefoxCheckBox();
            this.cbSkydrive = new FirefoxCheckBox();
            this.cbDropbox = new FirefoxCheckBox();
            this.pbSkydrive = new System.Windows.Forms.PictureBox();
            this.pbDropbox = new System.Windows.Forms.PictureBox();
            this.pbGoogle = new System.Windows.Forms.PictureBox();
            this.cbGoogle = new FirefoxCheckBox();
            this.panelBox1.SuspendLayout();
            this.panelBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkydrive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGoogle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBox1
            // 
            this.panelBox1.Controls.Add(this.backupEmailLabel);
            this.panelBox1.Controls.Add(this.labelItemSyncFeedback);
            this.panelBox1.Controls.Add(this.buttonLogin);
            this.panelBox1.Controls.Add(this.logoutThisComputer);
            this.panelBox1.Controls.Add(this.logoutAllComputers);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.HeaderHeight = 40;
            this.panelBox1.Location = new System.Drawing.Point(12, 12);
            this.panelBox1.Name = "panelBox1";
            this.panelBox1.NoRounding = false;
            this.panelBox1.Size = new System.Drawing.Size(359, 174);
            this.panelBox1.TabIndex = 12;
            this.panelBox1.Tag = "iatag_ui_backup_online";
            this.panelBox1.Text = "Online Backup";
            this.panelBox1.TextLocation = "8; 5";
            // 
            // backupEmailLabel
            // 
            this.backupEmailLabel.AutoSize = true;
            this.backupEmailLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.backupEmailLabel.Location = new System.Drawing.Point(3, 43);
            this.backupEmailLabel.Name = "backupEmailLabel";
            this.backupEmailLabel.Size = new System.Drawing.Size(15, 19);
            this.backupEmailLabel.TabIndex = 17;
            this.backupEmailLabel.Text = "-";
            // 
            // labelItemSyncFeedback
            // 
            this.labelItemSyncFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelItemSyncFeedback.AutoSize = true;
            this.labelItemSyncFeedback.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelItemSyncFeedback.Location = new System.Drawing.Point(3, 153);
            this.labelItemSyncFeedback.Name = "labelItemSyncFeedback";
            this.labelItemSyncFeedback.Size = new System.Drawing.Size(0, 13);
            this.labelItemSyncFeedback.TabIndex = 16;
            // 
            // buttonLogin
            // 
            this.buttonLogin.EnabledCalc = true;
            this.buttonLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLogin.Location = new System.Drawing.Point(41, 70);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(245, 24);
            this.buttonLogin.TabIndex = 15;
            this.buttonLogin.Tag = "iatag_ui_login";
            this.buttonLogin.Text = "Login";
            this.buttonLogin.Click += new System.EventHandler(this.firefoxButton1_Click);
            // 
            // logoutThisComputer
            // 
            this.logoutThisComputer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.logoutThisComputer.AutoSize = true;
            this.logoutThisComputer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.logoutThisComputer.Location = new System.Drawing.Point(250, 129);
            this.logoutThisComputer.Name = "logoutThisComputer";
            this.logoutThisComputer.Size = new System.Drawing.Size(106, 13);
            this.logoutThisComputer.TabIndex = 13;
            this.logoutThisComputer.TabStop = true;
            this.logoutThisComputer.Tag = "iatag_ui_logout_all_computers";
            this.logoutThisComputer.Text = "Logout this computer";
            this.logoutThisComputer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // logoutAllComputers
            // 
            this.logoutAllComputers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.logoutAllComputers.AutoSize = true;
            this.logoutAllComputers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.logoutAllComputers.Location = new System.Drawing.Point(250, 148);
            this.logoutAllComputers.Name = "logoutAllComputers";
            this.logoutAllComputers.Size = new System.Drawing.Size(105, 13);
            this.logoutAllComputers.TabIndex = 14;
            this.logoutAllComputers.TabStop = true;
            this.logoutAllComputers.Tag = "iatag_ui_logout_all_computers";
            this.logoutAllComputers.Text = "Logout all computers";
            this.logoutAllComputers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // panelBox5
            // 
            this.panelBox5.Controls.Add(this.buttonBackupNow);
            this.panelBox5.Controls.Add(this.buttonCustom);
            this.panelBox5.Controls.Add(this.cbCustom);
            this.panelBox5.Controls.Add(this.cbSkydrive);
            this.panelBox5.Controls.Add(this.cbDropbox);
            this.panelBox5.Controls.Add(this.pbSkydrive);
            this.panelBox5.Controls.Add(this.pbDropbox);
            this.panelBox5.Controls.Add(this.pbGoogle);
            this.panelBox5.Controls.Add(this.cbGoogle);
            this.panelBox5.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox5.HeaderHeight = 40;
            this.panelBox5.Location = new System.Drawing.Point(12, 192);
            this.panelBox5.Name = "panelBox5";
            this.panelBox5.NoRounding = false;
            this.panelBox5.Size = new System.Drawing.Size(359, 363);
            this.panelBox5.TabIndex = 10;
            this.panelBox5.Tag = "iatag_ui_backup_location";
            this.panelBox5.Text = "Local Backup";
            this.panelBox5.TextLocation = "8; 5";
            // 
            // buttonBackupNow
            // 
            this.buttonBackupNow.EnabledCalc = true;
            this.buttonBackupNow.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonBackupNow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonBackupNow.Location = new System.Drawing.Point(17, 270);
            this.buttonBackupNow.Name = "buttonBackupNow";
            this.buttonBackupNow.Size = new System.Drawing.Size(192, 32);
            this.buttonBackupNow.TabIndex = 11;
            this.buttonBackupNow.Tag = "iatag_ui_backupnow";
            this.buttonBackupNow.Text = "Backup Now";
            this.buttonBackupNow.Click += new System.EventHandler(this.buttonBackupNow_Click);
            // 
            // buttonCustom
            // 
            this.buttonCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buttonCustom.Location = new System.Drawing.Point(52, 228);
            this.buttonCustom.Name = "buttonCustom";
            this.buttonCustom.Size = new System.Drawing.Size(75, 23);
            this.buttonCustom.TabIndex = 8;
            this.buttonCustom.Tag = "iatag_ui_custom";
            this.buttonCustom.Text = "Custom..";
            this.buttonCustom.UseVisualStyleBackColor = true;
            this.buttonCustom.Click += new System.EventHandler(this.buttonCustom_Click);
            // 
            // cbCustom
            // 
            this.cbCustom.Bold = false;
            this.cbCustom.EnabledCalc = true;
            this.cbCustom.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbCustom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbCustom.Location = new System.Drawing.Point(17, 226);
            this.cbCustom.Name = "cbCustom";
            this.cbCustom.Size = new System.Drawing.Size(29, 27);
            this.cbCustom.TabIndex = 7;
            // 
            // cbSkydrive
            // 
            this.cbSkydrive.Bold = false;
            this.cbSkydrive.EnabledCalc = true;
            this.cbSkydrive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbSkydrive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbSkydrive.Location = new System.Drawing.Point(17, 180);
            this.cbSkydrive.Name = "cbSkydrive";
            this.cbSkydrive.Size = new System.Drawing.Size(29, 27);
            this.cbSkydrive.TabIndex = 6;
            // 
            // cbDropbox
            // 
            this.cbDropbox.Bold = false;
            this.cbDropbox.EnabledCalc = true;
            this.cbDropbox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDropbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDropbox.Location = new System.Drawing.Point(17, 123);
            this.cbDropbox.Name = "cbDropbox";
            this.cbDropbox.Size = new System.Drawing.Size(29, 27);
            this.cbDropbox.TabIndex = 5;
            // 
            // pbSkydrive
            // 
            this.pbSkydrive.BackgroundImage = global::IAGrim.Properties.Resources.onedrive;
            this.pbSkydrive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbSkydrive.Location = new System.Drawing.Point(52, 168);
            this.pbSkydrive.Name = "pbSkydrive";
            this.pbSkydrive.Size = new System.Drawing.Size(49, 50);
            this.pbSkydrive.TabIndex = 4;
            this.pbSkydrive.TabStop = false;
            this.pbSkydrive.Click += new System.EventHandler(this.pbSkydrive_Click);
            // 
            // pbDropbox
            // 
            this.pbDropbox.BackgroundImage = global::IAGrim.Properties.Resources.dropbox;
            this.pbDropbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbDropbox.Location = new System.Drawing.Point(52, 112);
            this.pbDropbox.Name = "pbDropbox";
            this.pbDropbox.Size = new System.Drawing.Size(49, 50);
            this.pbDropbox.TabIndex = 3;
            this.pbDropbox.TabStop = false;
            this.pbDropbox.Click += new System.EventHandler(this.pbDropbox_Click);
            // 
            // pbGoogle
            // 
            this.pbGoogle.BackgroundImage = global::IAGrim.Properties.Resources.gdrive;
            this.pbGoogle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbGoogle.Location = new System.Drawing.Point(52, 56);
            this.pbGoogle.Name = "pbGoogle";
            this.pbGoogle.Size = new System.Drawing.Size(49, 50);
            this.pbGoogle.TabIndex = 2;
            this.pbGoogle.TabStop = false;
            this.pbGoogle.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // cbGoogle
            // 
            this.cbGoogle.Bold = false;
            this.cbGoogle.EnabledCalc = true;
            this.cbGoogle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbGoogle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbGoogle.Location = new System.Drawing.Point(17, 70);
            this.cbGoogle.Name = "cbGoogle";
            this.cbGoogle.Size = new System.Drawing.Size(29, 27);
            this.cbGoogle.TabIndex = 0;
            // 
            // BackupSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 564);
            this.Controls.Add(this.panelBox1);
            this.Controls.Add(this.panelBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackupSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Backup Settings";
            this.Load += new System.EventHandler(this.BackupSettings_Load);
            this.panelBox1.ResumeLayout(false);
            this.panelBox1.PerformLayout();
            this.panelBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSkydrive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGoogle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox5;
        private FirefoxCheckBox cbGoogle;
        private System.Windows.Forms.PictureBox pbDropbox;
        private System.Windows.Forms.PictureBox pbGoogle;
        private System.Windows.Forms.PictureBox pbSkydrive;
        private FirefoxCheckBox cbSkydrive;
        private FirefoxCheckBox cbDropbox;
        private System.Windows.Forms.Button buttonCustom;
        private FirefoxCheckBox cbCustom;
        private FirefoxButton buttonBackupNow;
        private PanelBox panelBox1;
        private System.Windows.Forms.LinkLabel logoutThisComputer;
        private System.Windows.Forms.LinkLabel logoutAllComputers;
        private FirefoxButton buttonLogin;
        private System.Windows.Forms.Label labelItemSyncFeedback;
        private System.Windows.Forms.Label backupEmailLabel;
    }
}