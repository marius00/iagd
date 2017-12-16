namespace IAGrim.UI.Popups {
    partial class OnlineBackupLogin {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineBackupLogin));
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelVerify = new FirefoxH1();
            this.buttonLogin = new FirefoxButton();
            this.labelCheckSpam = new FirefoxH2();
            this.SuspendLayout();
            // 
            // tbEmail
            // 
            this.tbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEmail.Location = new System.Drawing.Point(43, 62);
            this.tbEmail.MaxLength = 254;
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(581, 22);
            this.tbEmail.TabIndex = 0;
            this.tbEmail.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(2, 151);
            this.progressBar1.MarqueeAnimationSpeed = 15;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(672, 32);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 17;
            this.label1.Tag = "iatag_ui_onlinebackuplogin";
            this.label1.Text = "Online backup login";
            // 
            // labelVerify
            // 
            this.labelVerify.AutoSize = true;
            this.labelVerify.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.labelVerify.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(100)))));
            this.labelVerify.Location = new System.Drawing.Point(25, 32);
            this.labelVerify.Name = "labelVerify";
            this.labelVerify.Size = new System.Drawing.Size(617, 37);
            this.labelVerify.TabIndex = 1;
            this.labelVerify.Tag = "iatag_ui_click_verification_link";
            this.labelVerify.Text = "Please click on the verification link sent via email.";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLogin.EnabledCalc = true;
            this.buttonLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonLogin.Location = new System.Drawing.Point(43, 90);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(581, 32);
            this.buttonLogin.TabIndex = 1;
            this.buttonLogin.Tag = "iatag_ui_login";
            this.buttonLogin.Text = "Login";
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // labelCheckSpam
            // 
            this.labelCheckSpam.AutoSize = true;
            this.labelCheckSpam.BackColor = System.Drawing.SystemColors.Control;
            this.labelCheckSpam.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelCheckSpam.ForeColor = System.Drawing.Color.Red;
            this.labelCheckSpam.Location = new System.Drawing.Point(239, 69);
            this.labelCheckSpam.Name = "labelCheckSpam";
            this.labelCheckSpam.Size = new System.Drawing.Size(174, 19);
            this.labelCheckSpam.TabIndex = 18;
            this.labelCheckSpam.Tag = "iatag_ui_checkyourspam";
            this.labelCheckSpam.Text = "Check your spam folder!";
            // 
            // OnlineBackupLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 186);
            this.Controls.Add(this.labelCheckSpam);
            this.Controls.Add(this.labelVerify);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.buttonLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OnlineBackupLogin";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "iatag_ui_login";
            this.Text = "Online Backup Login";
            this.Load += new System.EventHandler(this.OnlineBackupLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbEmail;
        private FirefoxButton buttonLogin;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private FirefoxH1 labelVerify;
        private FirefoxH2 labelCheckSpam;
    }
}