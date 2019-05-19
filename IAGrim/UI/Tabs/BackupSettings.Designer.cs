namespace IAGrim.UI.Tabs {
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
            this.onlineBackup = new PanelBox();
            this.cbDontWantBackups = new FirefoxCheckBox();
            this.buttonLogin = new FirefoxButton();
            this.panelBox5 = new PanelBox();
            this.helpWhyOnedriveDisabled = new System.Windows.Forms.LinkLabel();
            this.helpWhyDropboxDisabled = new System.Windows.Forms.LinkLabel();
            this.helpWhyGdriveDisabled = new System.Windows.Forms.LinkLabel();
            this.buttonBackupNow = new FirefoxButton();
            this.buttonCustom = new System.Windows.Forms.Button();
            this.cbCustom = new FirefoxCheckBox();
            this.cbOneDrive = new FirefoxCheckBox();
            this.cbDropbox = new FirefoxCheckBox();
            this.pbSkydrive = new System.Windows.Forms.PictureBox();
            this.pbDropbox = new System.Windows.Forms.PictureBox();
            this.pbGoogle = new System.Windows.Forms.PictureBox();
            this.cbGoogle = new FirefoxCheckBox();
            this.onlineBackup.SuspendLayout();
            this.panelBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkydrive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDropbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGoogle)).BeginInit();
            this.SuspendLayout();
            // 
            // onlineBackup
            // 
            this.onlineBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onlineBackup.Controls.Add(this.cbDontWantBackups);
            this.onlineBackup.Controls.Add(this.buttonLogin);
            this.onlineBackup.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.onlineBackup.HeaderHeight = 40;
            this.onlineBackup.Location = new System.Drawing.Point(12, 12);
            this.onlineBackup.Name = "onlineBackup";
            this.onlineBackup.NoRounding = false;
            this.onlineBackup.Size = new System.Drawing.Size(329, 174);
            this.onlineBackup.TabIndex = 12;
            this.onlineBackup.Tag = "iatag_ui_online_backup";
            this.onlineBackup.Text = "Online Backup";
            this.onlineBackup.TextLocation = "8; 5";
            // 
            // cbDontWantBackups
            // 
            this.cbDontWantBackups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDontWantBackups.Bold = false;
            this.cbDontWantBackups.EnabledCalc = true;
            this.cbDontWantBackups.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDontWantBackups.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbDontWantBackups.Location = new System.Drawing.Point(41, 100);
            this.cbDontWantBackups.Name = "cbDontWantBackups";
            this.cbDontWantBackups.Size = new System.Drawing.Size(285, 27);
            this.cbDontWantBackups.TabIndex = 16;
            this.cbDontWantBackups.Tag = "iatag_ui_dontwantbackups";
            this.cbDontWantBackups.Text = "I don\'t want backups, stop asking me!";
            this.cbDontWantBackups.UseVisualStyleBackColor = true;
            this.cbDontWantBackups.CheckedChanged += new System.EventHandler(this.cbDontWantBackups_CheckedChanged);
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
            // panelBox5
            // 
            this.panelBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox5.Controls.Add(this.helpWhyOnedriveDisabled);
            this.panelBox5.Controls.Add(this.helpWhyDropboxDisabled);
            this.panelBox5.Controls.Add(this.helpWhyGdriveDisabled);
            this.panelBox5.Controls.Add(this.buttonBackupNow);
            this.panelBox5.Controls.Add(this.buttonCustom);
            this.panelBox5.Controls.Add(this.cbCustom);
            this.panelBox5.Controls.Add(this.cbOneDrive);
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
            this.panelBox5.Size = new System.Drawing.Size(329, 363);
            this.panelBox5.TabIndex = 10;
            this.panelBox5.Tag = "iatag_ui_backup_location";
            this.panelBox5.Text = "Local Backup";
            this.panelBox5.TextLocation = "8; 5";
            // 
            // helpWhyOnedriveDisabled
            // 
            this.helpWhyOnedriveDisabled.AutoSize = true;
            this.helpWhyOnedriveDisabled.BackColor = System.Drawing.Color.Transparent;
            this.helpWhyOnedriveDisabled.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhyOnedriveDisabled.Location = new System.Drawing.Point(107, 187);
            this.helpWhyOnedriveDisabled.Name = "helpWhyOnedriveDisabled";
            this.helpWhyOnedriveDisabled.Size = new System.Drawing.Size(115, 13);
            this.helpWhyOnedriveDisabled.TabIndex = 15;
            this.helpWhyOnedriveDisabled.TabStop = true;
            this.helpWhyOnedriveDisabled.Tag = "iatag_ui_backupsdisabled";
            this.helpWhyOnedriveDisabled.Text = "Why is this disabled?";
            this.helpWhyOnedriveDisabled.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhyOnedriveDisabled_LinkClicked);
            // 
            // helpWhyDropboxDisabled
            // 
            this.helpWhyDropboxDisabled.AutoSize = true;
            this.helpWhyDropboxDisabled.BackColor = System.Drawing.Color.Transparent;
            this.helpWhyDropboxDisabled.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhyDropboxDisabled.Location = new System.Drawing.Point(107, 130);
            this.helpWhyDropboxDisabled.Name = "helpWhyDropboxDisabled";
            this.helpWhyDropboxDisabled.Size = new System.Drawing.Size(115, 13);
            this.helpWhyDropboxDisabled.TabIndex = 14;
            this.helpWhyDropboxDisabled.TabStop = true;
            this.helpWhyDropboxDisabled.Tag = "iatag_ui_backupsdisabled";
            this.helpWhyDropboxDisabled.Text = "Why is this disabled?";
            this.helpWhyDropboxDisabled.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhyDropboxDisabled_LinkClicked);
            // 
            // helpWhyGdriveDisabled
            // 
            this.helpWhyGdriveDisabled.AutoSize = true;
            this.helpWhyGdriveDisabled.BackColor = System.Drawing.Color.Transparent;
            this.helpWhyGdriveDisabled.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpWhyGdriveDisabled.Location = new System.Drawing.Point(107, 77);
            this.helpWhyGdriveDisabled.Name = "helpWhyGdriveDisabled";
            this.helpWhyGdriveDisabled.Size = new System.Drawing.Size(115, 13);
            this.helpWhyGdriveDisabled.TabIndex = 13;
            this.helpWhyGdriveDisabled.TabStop = true;
            this.helpWhyGdriveDisabled.Tag = "iatag_ui_backupsdisabled";
            this.helpWhyGdriveDisabled.Text = "Why is this disabled?";
            this.helpWhyGdriveDisabled.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpWhyGdriveDisabled_LinkClicked);
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
            // cbOneDrive
            // 
            this.cbOneDrive.Bold = false;
            this.cbOneDrive.EnabledCalc = true;
            this.cbOneDrive.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbOneDrive.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbOneDrive.Location = new System.Drawing.Point(17, 180);
            this.cbOneDrive.Name = "cbOneDrive";
            this.cbOneDrive.Size = new System.Drawing.Size(29, 27);
            this.cbOneDrive.TabIndex = 6;
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
            this.ClientSize = new System.Drawing.Size(353, 564);
            this.Controls.Add(this.onlineBackup);
            this.Controls.Add(this.panelBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackupSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Backup Settings";
            this.Load += new System.EventHandler(this.BackupSettings_Load);
            this.onlineBackup.ResumeLayout(false);
            this.panelBox5.ResumeLayout(false);
            this.panelBox5.PerformLayout();
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
        private FirefoxCheckBox cbOneDrive;
        private FirefoxCheckBox cbDropbox;
        private System.Windows.Forms.Button buttonCustom;
        private FirefoxCheckBox cbCustom;
        private FirefoxButton buttonBackupNow;
        private PanelBox onlineBackup;
        private FirefoxButton buttonLogin;
        private FirefoxCheckBox cbDontWantBackups;
        private System.Windows.Forms.LinkLabel helpWhyGdriveDisabled;
        private System.Windows.Forms.LinkLabel helpWhyOnedriveDisabled;
        private System.Windows.Forms.LinkLabel helpWhyDropboxDisabled;
    }
}