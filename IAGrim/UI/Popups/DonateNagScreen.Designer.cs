namespace IAGrim.UI {
    partial class DonateNagScreen {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DonateNagScreen));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonPatreon = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.firefoxH11 = new FirefoxH1();
            this.buttonNoThanks = new FirefoxButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(319, 58);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(211, 109);
            this.panel1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::IAGrim.Properties.Resources.donate;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Location = new System.Drawing.Point(8, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 92);
            this.button2.TabIndex = 4;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(211, 109);
            this.panel2.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackgroundImage = global::IAGrim.Properties.Resources.donate;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Location = new System.Drawing.Point(327, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 92);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonPatreon
            // 
            this.buttonPatreon.BackgroundImage = global::IAGrim.Properties.Resources.Patreon_Navy;
            this.buttonPatreon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPatreon.Location = new System.Drawing.Point(36, 66);
            this.buttonPatreon.Name = "buttonPatreon";
            this.buttonPatreon.Size = new System.Drawing.Size(192, 92);
            this.buttonPatreon.TabIndex = 4;
            this.buttonPatreon.UseVisualStyleBackColor = true;
            this.buttonPatreon.Click += new System.EventHandler(this.buttonPatreon_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Location = new System.Drawing.Point(28, 58);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(211, 109);
            this.panel3.TabIndex = 5;
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::IAGrim.Properties.Resources.donate;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.Location = new System.Drawing.Point(8, 8);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(192, 92);
            this.button4.TabIndex = 4;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(211, 109);
            this.panel4.TabIndex = 5;
            // 
            // firefoxH11
            // 
            this.firefoxH11.AutoSize = true;
            this.firefoxH11.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.firefoxH11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(100)))));
            this.firefoxH11.Location = new System.Drawing.Point(12, 9);
            this.firefoxH11.Name = "firefoxH11";
            this.firefoxH11.Size = new System.Drawing.Size(531, 25);
            this.firefoxH11.TabIndex = 2;
            this.firefoxH11.Tag = "iatag_ui_nagscreen_title";
            this.firefoxH11.Text = "Would you like to donate to the developer of Item Assistant?";
            // 
            // buttonNoThanks
            // 
            this.buttonNoThanks.Enabled = false;
            this.buttonNoThanks.EnabledCalc = false;
            this.buttonNoThanks.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonNoThanks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonNoThanks.Location = new System.Drawing.Point(195, 188);
            this.buttonNoThanks.Name = "buttonNoThanks";
            this.buttonNoThanks.Size = new System.Drawing.Size(192, 32);
            this.buttonNoThanks.TabIndex = 0;
            this.buttonNoThanks.Tag = "iatag_ui_nagscreen_button";
            this.buttonNoThanks.Text = "Maybe later...";
            this.buttonNoThanks.Click += new System.EventHandler(this.buttonNoThanks_Click);
            // 
            // DonateNagScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 253);
            this.Controls.Add(this.buttonPatreon);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.firefoxH11);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonNoThanks);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(570, 292);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(570, 292);
            this.Name = "DonateNagScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "iatag_ui_nagscreen_header";
            this.Text = "IA : Would you like to donate?";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DonateNagScreen_Load);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FirefoxButton buttonNoThanks;
        private System.Windows.Forms.Button button1;
        private FirefoxH1 firefoxH11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonPatreon;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel4;
    }
}