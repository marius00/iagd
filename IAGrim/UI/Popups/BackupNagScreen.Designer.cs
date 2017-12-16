namespace IAGrim.UI.Popups {
    partial class BackupNagScreen {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupNagScreen));
            this.firefoxH12 = new FirefoxH1();
            this.firefoxH11 = new FirefoxH1();
            this.buttonNeverRemindMe = new FirefoxButton();
            this.buttonThanks = new FirefoxButton();
            this.SuspendLayout();
            // 
            // firefoxH12
            // 
            this.firefoxH12.AutoSize = true;
            this.firefoxH12.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.firefoxH12.ForeColor = System.Drawing.Color.Red;
            this.firefoxH12.Location = new System.Drawing.Point(161, 9);
            this.firefoxH12.Name = "firefoxH12";
            this.firefoxH12.Size = new System.Drawing.Size(138, 34);
            this.firefoxH12.TabIndex = 4;
            this.firefoxH12.Tag = "iatag_ui_backupnag_warning";
            this.firefoxH12.Text = "Warning!";
            // 
            // firefoxH11
            // 
            this.firefoxH11.AutoSize = true;
            this.firefoxH11.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.firefoxH11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(100)))));
            this.firefoxH11.Location = new System.Drawing.Point(77, 55);
            this.firefoxH11.Name = "firefoxH11";
            this.firefoxH11.Size = new System.Drawing.Size(310, 37);
            this.firefoxH11.TabIndex = 3;
            this.firefoxH11.Tag = "iatag_ui_backupnag_nobackups";
            this.firefoxH11.Text = "No backups configured!";
            // 
            // buttonNeverRemindMe
            // 
            this.buttonNeverRemindMe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNeverRemindMe.EnabledCalc = true;
            this.buttonNeverRemindMe.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonNeverRemindMe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonNeverRemindMe.Location = new System.Drawing.Point(12, 127);
            this.buttonNeverRemindMe.Name = "buttonNeverRemindMe";
            this.buttonNeverRemindMe.Size = new System.Drawing.Size(222, 38);
            this.buttonNeverRemindMe.TabIndex = 1;
            this.buttonNeverRemindMe.Tag = "iatag_ui_backupnag_button_dontremindme";
            this.buttonNeverRemindMe.Text = "Don\'t remind me anymore";
            this.buttonNeverRemindMe.Click += new System.EventHandler(this.buttonNeverRemindMe_Click);
            // 
            // buttonThanks
            // 
            this.buttonThanks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonThanks.EnabledCalc = true;
            this.buttonThanks.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonThanks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonThanks.Location = new System.Drawing.Point(240, 127);
            this.buttonThanks.Name = "buttonThanks";
            this.buttonThanks.Size = new System.Drawing.Size(222, 38);
            this.buttonThanks.TabIndex = 0;
            this.buttonThanks.Tag = "iatag_ui_backupnag_button_thanks";
            this.buttonThanks.Text = "Thanks!";
            this.buttonThanks.Click += new System.EventHandler(this.buttonThanks_Click);
            // 
            // BackupNagScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 180);
            this.Controls.Add(this.firefoxH12);
            this.Controls.Add(this.firefoxH11);
            this.Controls.Add(this.buttonNeverRemindMe);
            this.Controls.Add(this.buttonThanks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BackupNagScreen";
            this.Text = "IA: No backups configured!";
            this.Load += new System.EventHandler(this.BackupNagScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FirefoxButton buttonThanks;
        private FirefoxButton buttonNeverRemindMe;
        private FirefoxH1 firefoxH11;
        private FirefoxH1 firefoxH12;
    }
}