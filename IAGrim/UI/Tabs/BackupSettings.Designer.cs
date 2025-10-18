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
            panelBox5 = new PanelBox();
            linkLabel1 = new LinkLabel();
            lbOpenCustomBackupFolder = new LinkLabel();
            buttonBackupNow = new FirefoxButton();
            buttonCustom = new Button();
            cbCustom = new FirefoxCheckBox();
            panelBox5.SuspendLayout();
            SuspendLayout();
            // 
            // panelBox5
            // 
            panelBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelBox5.BackColor = Color.FromArgb(240, 240, 240);
            panelBox5.Controls.Add(linkLabel1);
            panelBox5.Controls.Add(lbOpenCustomBackupFolder);
            panelBox5.Controls.Add(buttonBackupNow);
            panelBox5.Controls.Add(buttonCustom);
            panelBox5.Controls.Add(cbCustom);
            panelBox5.Font = new Font("Segoe UI Semibold", 20F);
            panelBox5.ForeColor = Color.Black;
            panelBox5.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox5.HeaderHeight = 40;
            panelBox5.Location = new Point(4, 14);
            panelBox5.Margin = new Padding(4, 3, 4, 3);
            panelBox5.Name = "panelBox5";
            panelBox5.NoRounding = false;
            panelBox5.Size = new Size(889, 419);
            panelBox5.TabIndex = 10;
            panelBox5.Tag = "iatag_ui_backup_location";
            panelBox5.Text = "Local Backup";
            panelBox5.TextLocation = "8; 5";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Transparent;
            linkLabel1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.Location = new Point(255, 122);
            linkLabel1.Margin = new Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(144, 13);
            linkLabel1.TabIndex = 18;
            linkLabel1.TabStop = true;
            linkLabel1.Tag = "iatag_ui_howtorestore";
            linkLabel1.Text = "How do I restore backups?";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // lbOpenCustomBackupFolder
            // 
            lbOpenCustomBackupFolder.AutoSize = true;
            lbOpenCustomBackupFolder.BackColor = Color.Transparent;
            lbOpenCustomBackupFolder.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbOpenCustomBackupFolder.Location = new Point(158, 70);
            lbOpenCustomBackupFolder.Margin = new Padding(4, 0, 4, 0);
            lbOpenCustomBackupFolder.Name = "lbOpenCustomBackupFolder";
            lbOpenCustomBackupFolder.Size = new Size(70, 13);
            lbOpenCustomBackupFolder.TabIndex = 17;
            lbOpenCustomBackupFolder.TabStop = true;
            lbOpenCustomBackupFolder.Tag = "iatag_ui_opencustombackup";
            lbOpenCustomBackupFolder.Text = "Open folder";
            lbOpenCustomBackupFolder.LinkClicked += lbOpenCustomBackupFolder_LinkClicked;
            // 
            // buttonBackupNow
            // 
            buttonBackupNow.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonBackupNow.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonBackupNow.BorderColor = Color.FromArgb(193, 193, 193);
            buttonBackupNow.EnabledCalc = true;
            buttonBackupNow.Font = new Font("Segoe UI", 10F);
            buttonBackupNow.ForeColor = Color.FromArgb(56, 68, 80);
            buttonBackupNow.HoverColor = Color.FromArgb(232, 232, 232);
            buttonBackupNow.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonBackupNow.Location = new Point(23, 113);
            buttonBackupNow.Margin = new Padding(4, 3, 4, 3);
            buttonBackupNow.Name = "buttonBackupNow";
            buttonBackupNow.Size = new Size(224, 37);
            buttonBackupNow.TabIndex = 11;
            buttonBackupNow.Tag = "iatag_ui_backupnow";
            buttonBackupNow.Text = "Backup Now";
            buttonBackupNow.Click += buttonBackupNow_Click;
            // 
            // buttonCustom
            // 
            buttonCustom.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonCustom.Location = new Point(64, 64);
            buttonCustom.Margin = new Padding(4, 3, 4, 3);
            buttonCustom.Name = "buttonCustom";
            buttonCustom.Size = new Size(88, 27);
            buttonCustom.TabIndex = 8;
            buttonCustom.Tag = "iatag_ui_custom";
            buttonCustom.Text = "Custom..";
            buttonCustom.UseVisualStyleBackColor = true;
            buttonCustom.Click += buttonCustom_Click;
            // 
            // cbCustom
            // 
            cbCustom.Bold = false;
            cbCustom.EnabledCalc = true;
            cbCustom.Font = new Font("Segoe UI", 10F);
            cbCustom.ForeColor = Color.FromArgb(66, 78, 90);
            cbCustom.IsDarkMode = false;
            cbCustom.Location = new Point(23, 62);
            cbCustom.Margin = new Padding(4, 3, 4, 3);
            cbCustom.Name = "cbCustom";
            cbCustom.Size = new Size(34, 31);
            cbCustom.TabIndex = 7;
            // 
            // BackupSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(917, 444);
            Controls.Add(panelBox5);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BackupSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Backup Settings";
            Load += BackupSettings_Load;
            panelBox5.ResumeLayout(false);
            panelBox5.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox5;
        private System.Windows.Forms.Button buttonCustom;
        private FirefoxCheckBox cbCustom;
        private FirefoxButton buttonBackupNow;
        private System.Windows.Forms.LinkLabel lbOpenCustomBackupFolder;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}