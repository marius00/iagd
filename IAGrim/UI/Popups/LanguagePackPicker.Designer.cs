namespace IAGrim.UI {
    partial class LanguagePackPicker {
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(LanguagePackPicker));
            groupBox1 = new GroupBox();
            pictureBox1 = new PictureBox();
            lblWarning = new Label();
            buttonSelect = new FirefoxButton();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(pictureBox1);
            groupBox1.Location = new Point(9, 10);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(936, 307);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Tag = "iatag_ui_language_selection";
            groupBox1.Text = "Language Selection";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.BackgroundImage = Properties.Resources.languages;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(816, 7);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 119);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblWarning
            // 
            lblWarning.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblWarning.AutoSize = true;
            lblWarning.Location = new Point(9, 520);
            lblWarning.Margin = new Padding(4, 0, 4, 0);
            lblWarning.Name = "lblWarning";
            lblWarning.Size = new Size(194, 15);
            lblWarning.TabIndex = 4;
            lblWarning.Tag = "iatag_ui_language_change_warning";
            lblWarning.Text = "iatag_ui_language_change_warning";
            // 
            // buttonSelect
            // 
            buttonSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonSelect.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonSelect.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonSelect.BorderColor = Color.FromArgb(193, 193, 193);
            buttonSelect.EnabledCalc = true;
            buttonSelect.Font = new Font("Segoe UI", 10F);
            buttonSelect.ForeColor = Color.FromArgb(56, 68, 80);
            buttonSelect.HoverColor = Color.FromArgb(232, 232, 232);
            buttonSelect.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonSelect.Location = new Point(6, 357);
            buttonSelect.Margin = new Padding(4, 3, 4, 3);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new Size(939, 40);
            buttonSelect.TabIndex = 2;
            buttonSelect.Tag = "iatag_ui_change_language";
            buttonSelect.Text = "Change Language";
            buttonSelect.Click += buttonSelect_Click;
            // 
            // LanguagePackPicker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(959, 411);
            Controls.Add(lblWarning);
            Controls.Add(groupBox1);
            Controls.Add(buttonSelect);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(975, 450);
            MinimizeBox = false;
            MinimumSize = new Size(975, 450);
            Name = "LanguagePackPicker";
            Opacity = 0.95D;
            StartPosition = FormStartPosition.CenterParent;
            SizeGripStyle = SizeGripStyle.Hide;
            Tag = "iatag_ui_change_language";
            Text = "Change Language";
            FormClosing += LanguagePackPicker_FormClosing;
            Load += LanguagePackPicker_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private FirefoxButton buttonSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblWarning;
    }
}