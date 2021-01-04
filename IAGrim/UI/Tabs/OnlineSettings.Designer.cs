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
            this.onlineBackup = new PanelBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRefreshBackupDetails = new System.Windows.Forms.Button();
            this.linkLogout = new System.Windows.Forms.LinkLabel();
            this.linkDeleteBackup = new System.Windows.Forms.LinkLabel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDontWantBackups = new FirefoxCheckBox();
            this.buttonLogin = new FirefoxButton();
            this.onlineBackup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // onlineBackup
            // 
            this.onlineBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onlineBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.onlineBackup.Controls.Add(this.groupBox1);
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
            this.onlineBackup.Text = "Online Backup";
            this.onlineBackup.TextLocation = "8; 5";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRefreshBackupDetails);
            this.groupBox1.Controls.Add(this.linkLogout);
            this.groupBox1.Controls.Add(this.linkDeleteBackup);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(398, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 125);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "iatag_ui_backup_details";
            this.groupBox1.Text = "Details";
            // 
            // btnRefreshBackupDetails
            // 
            this.btnRefreshBackupDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshBackupDetails.BackgroundImage = global::IAGrim.Properties.Resources.refresh;
            this.btnRefreshBackupDetails.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRefreshBackupDetails.Location = new System.Drawing.Point(339, 23);
            this.btnRefreshBackupDetails.Name = "btnRefreshBackupDetails";
            this.btnRefreshBackupDetails.Size = new System.Drawing.Size(16, 16);
            this.btnRefreshBackupDetails.TabIndex = 4;
            this.btnRefreshBackupDetails.UseVisualStyleBackColor = true;
            this.btnRefreshBackupDetails.Click += new System.EventHandler(this.btnRefreshBackupDetails_Click);
            // 
            // linkLogout
            // 
            this.linkLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLogout.AutoSize = true;
            this.linkLogout.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLogout.Location = new System.Drawing.Point(300, 100);
            this.linkLogout.Name = "linkLogout";
            this.linkLogout.Size = new System.Drawing.Size(32, 11);
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
            this.linkDeleteBackup.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkDeleteBackup.Location = new System.Drawing.Point(300, 111);
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
            this.labelStatus.Size = new System.Drawing.Size(293, 19);
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
            this.cbDontWantBackups.Location = new System.Drawing.Point(41, 100);
            this.cbDontWantBackups.Name = "cbDontWantBackups";
            this.cbDontWantBackups.Size = new System.Drawing.Size(499, 27);
            this.cbDontWantBackups.TabIndex = 16;
            this.cbDontWantBackups.Tag = "iatag_ui_dontwantbackups";
            this.cbDontWantBackups.Text = "I don\'t want backups, stop asking me!";
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
            this.buttonLogin.Location = new System.Drawing.Point(41, 70);
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
            this.ClientSize = new System.Drawing.Size(786, 564);
            this.Controls.Add(this.onlineBackup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OnlineSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Backup Settings";
            this.Load += new System.EventHandler(this.BackupSettings_Load);
            this.onlineBackup.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private PanelBox onlineBackup;
        private FirefoxButton buttonLogin;
        private FirefoxCheckBox cbDontWantBackups;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkDeleteBackup;
        private System.Windows.Forms.LinkLabel linkLogout;
        private System.Windows.Forms.Button btnRefreshBackupDetails;
    }
}