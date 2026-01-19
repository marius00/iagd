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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DonateNagScreen));
            panel1 = new Panel();
            button2 = new Button();
            panel2 = new Panel();
            button1 = new Button();
            buttonPatreon = new Button();
            panel3 = new Panel();
            button4 = new Button();
            panel4 = new Panel();
            firefoxH11 = new FirefoxH1();
            buttonNoThanks = new FirefoxButton();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button2);
            panel1.Controls.Add(panel2);
            panel1.Location = new Point(372, 67);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(246, 126);
            panel1.TabIndex = 3;
            // 
            // button2
            // 
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            button2.Location = new Point(9, 9);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(224, 106);
            button2.TabIndex = 4;
            button2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(246, 126);
            panel2.TabIndex = 5;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackgroundImage = Properties.Resources.donate;
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.Image = Properties.Resources.donate1;
            button1.Location = new Point(382, 76);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(224, 106);
            button1.TabIndex = 1;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // buttonPatreon
            // 
            buttonPatreon.BackgroundImage = Properties.Resources.Patreon_Navy;
            buttonPatreon.BackgroundImageLayout = ImageLayout.Stretch;
            buttonPatreon.Image = (Image)resources.GetObject("buttonPatreon.Image");
            buttonPatreon.Location = new Point(42, 76);
            buttonPatreon.Margin = new Padding(4, 3, 4, 3);
            buttonPatreon.Name = "buttonPatreon";
            buttonPatreon.Size = new Size(224, 106);
            buttonPatreon.TabIndex = 4;
            buttonPatreon.UseVisualStyleBackColor = true;
            buttonPatreon.Click += buttonPatreon_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(button4);
            panel3.Controls.Add(panel4);
            panel3.Location = new Point(33, 67);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(246, 126);
            panel3.TabIndex = 5;
            // 
            // button4
            // 
            button4.BackgroundImageLayout = ImageLayout.Stretch;
            button4.Location = new Point(9, 9);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(224, 106);
            button4.TabIndex = 4;
            button4.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Location = new Point(0, 0);
            panel4.Margin = new Padding(4, 3, 4, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(246, 126);
            panel4.TabIndex = 5;
            // 
            // firefoxH11
            // 
            firefoxH11.AutoSize = true;
            firefoxH11.Font = new Font("Segoe UI Semibold", 14F);
            firefoxH11.ForeColor = Color.FromArgb(76, 88, 100);
            firefoxH11.Location = new Point(12, 9);
            firefoxH11.Name = "firefoxH11";
            firefoxH11.Size = new Size(531, 25);
            firefoxH11.TabIndex = 2;
            firefoxH11.Tag = "iatag_ui_nagscreen_title";
            firefoxH11.Text = "Would you like to donate to the developer of Item Assistant?";
            // 
            // buttonNoThanks
            // 
            buttonNoThanks.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonNoThanks.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonNoThanks.BorderColor = Color.FromArgb(193, 193, 193);
            buttonNoThanks.Enabled = false;
            buttonNoThanks.EnabledCalc = false;
            buttonNoThanks.Font = new Font("Segoe UI", 10F);
            buttonNoThanks.ForeColor = Color.FromArgb(56, 68, 80);
            buttonNoThanks.HoverColor = Color.FromArgb(232, 232, 232);
            buttonNoThanks.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonNoThanks.Location = new Point(218, 257);
            buttonNoThanks.Name = "buttonNoThanks";
            buttonNoThanks.Size = new Size(192, 32);
            buttonNoThanks.TabIndex = 0;
            buttonNoThanks.Tag = "iatag_ui_nagscreen_button";
            buttonNoThanks.Text = "Maybe later...";
            buttonNoThanks.Click += buttonNoThanks_Click;
            // 
            // DonateNagScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(646, 292);
            Controls.Add(buttonPatreon);
            Controls.Add(panel3);
            Controls.Add(button1);
            Controls.Add(buttonNoThanks);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(662, 331);
            MinimizeBox = false;
            MinimumSize = new Size(662, 331);
            Name = "DonateNagScreen";
            SizeGripStyle = SizeGripStyle.Hide;
            Tag = "iatag_ui_nagscreen_header";
            Text = "IA : Would you like to donate?";
            TopMost = true;
            Load += DonateNagScreen_Load;
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);

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