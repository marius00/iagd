namespace IAGrim.UI.Popups.ImportExport.Panels {
    partial class ImportMode {
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
            progressBar1 = new ProgressBar();
            panelBox3 = new PanelBox();
            cbItemSelection = new ComboBox();
            buttonImport = new FirefoxButton();
            panelBox1 = new PanelBox();
            buttonBrowse = new FirefoxButton();
            panelBox2 = new PanelBox();
            helpRestoreBackup = new LinkLabel();
            radioGDStash = new FirefoxRadioButton();
            radioIAStash = new FirefoxRadioButton();
            panelBox3.SuspendLayout();
            panelBox1.SuspendLayout();
            panelBox2.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(18, 142);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(226, 27);
            progressBar1.TabIndex = 8;
            progressBar1.Click += progressBar1_Click;
            // 
            // panelBox3
            // 
            panelBox3.BackColor = Color.FromArgb(240, 240, 240);
            panelBox3.Controls.Add(progressBar1);
            panelBox3.Controls.Add(cbItemSelection);
            panelBox3.Controls.Add(buttonImport);
            panelBox3.Font = new Font("Segoe UI Semibold", 20F);
            panelBox3.ForeColor = Color.Black;
            panelBox3.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox3.HeaderHeight = 40;
            panelBox3.Location = new Point(474, 14);
            panelBox3.Margin = new Padding(4, 3, 4, 3);
            panelBox3.Name = "panelBox3";
            panelBox3.NoRounding = false;
            panelBox3.Size = new Size(266, 189);
            panelBox3.TabIndex = 7;
            panelBox3.Tag = "iatag_ui_import_import";
            panelBox3.Text = "Import";
            panelBox3.TextLocation = "8; 5";
            // 
            // cbItemSelection
            // 
            cbItemSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            cbItemSelection.Font = new Font("Segoe UI", 10F);
            cbItemSelection.FormattingEnabled = true;
            cbItemSelection.Location = new Point(18, 60);
            cbItemSelection.Margin = new Padding(4, 3, 4, 3);
            cbItemSelection.Name = "cbItemSelection";
            cbItemSelection.Size = new Size(223, 25);
            cbItemSelection.TabIndex = 8;
            // 
            // buttonImport
            // 
            buttonImport.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonImport.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonImport.BorderColor = Color.FromArgb(193, 193, 193);
            buttonImport.EnabledCalc = true;
            buttonImport.Font = new Font("Segoe UI", 10F);
            buttonImport.ForeColor = Color.FromArgb(56, 68, 80);
            buttonImport.HoverColor = Color.FromArgb(232, 232, 232);
            buttonImport.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonImport.Location = new Point(18, 98);
            buttonImport.Margin = new Padding(4, 3, 4, 3);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(224, 37);
            buttonImport.TabIndex = 6;
            buttonImport.Tag = "iatag_ui_import";
            buttonImport.Text = "Import";
            buttonImport.Click += buttonImport_Click;
            // 
            // panelBox1
            // 
            panelBox1.BackColor = Color.FromArgb(240, 240, 240);
            panelBox1.Controls.Add(buttonBrowse);
            panelBox1.Font = new Font("Segoe UI Semibold", 20F);
            panelBox1.ForeColor = Color.Black;
            panelBox1.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox1.HeaderHeight = 40;
            panelBox1.Location = new Point(201, 14);
            panelBox1.Margin = new Padding(4, 3, 4, 3);
            panelBox1.Name = "panelBox1";
            panelBox1.NoRounding = false;
            panelBox1.Size = new Size(266, 189);
            panelBox1.TabIndex = 4;
            panelBox1.Tag = "iatag_ui_import_stashfile";
            panelBox1.Text = "Stash File";
            panelBox1.TextLocation = "8; 5";
            // 
            // buttonBrowse
            // 
            buttonBrowse.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonBrowse.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonBrowse.BorderColor = Color.FromArgb(193, 193, 193);
            buttonBrowse.EnabledCalc = true;
            buttonBrowse.Font = new Font("Segoe UI", 10F);
            buttonBrowse.ForeColor = Color.FromArgb(56, 68, 80);
            buttonBrowse.HoverColor = Color.FromArgb(232, 232, 232);
            buttonBrowse.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonBrowse.Location = new Point(20, 60);
            buttonBrowse.Margin = new Padding(4, 3, 4, 3);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(224, 37);
            buttonBrowse.TabIndex = 6;
            buttonBrowse.Tag = "iatag_ui_browse";
            buttonBrowse.Text = "Browse..";
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // panelBox2
            // 
            panelBox2.BackColor = Color.FromArgb(240, 240, 240);
            panelBox2.Controls.Add(helpRestoreBackup);
            panelBox2.Controls.Add(radioGDStash);
            panelBox2.Controls.Add(radioIAStash);
            panelBox2.Font = new Font("Segoe UI Semibold", 20F);
            panelBox2.ForeColor = Color.Black;
            panelBox2.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox2.HeaderHeight = 40;
            panelBox2.Location = new Point(14, 14);
            panelBox2.Margin = new Padding(4, 3, 4, 3);
            panelBox2.Name = "panelBox2";
            panelBox2.NoRounding = false;
            panelBox2.Size = new Size(180, 189);
            panelBox2.TabIndex = 3;
            panelBox2.Tag = "iatag_ui_import_filetype";
            panelBox2.Text = "File Type";
            panelBox2.TextLocation = "8; 5";
            // 
            // helpRestoreBackup
            // 
            helpRestoreBackup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            helpRestoreBackup.AutoSize = true;
            helpRestoreBackup.BackColor = Color.Transparent;
            helpRestoreBackup.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpRestoreBackup.Location = new Point(142, 69);
            helpRestoreBackup.Margin = new Padding(4, 0, 4, 0);
            helpRestoreBackup.Name = "helpRestoreBackup";
            helpRestoreBackup.Size = new Size(18, 13);
            helpRestoreBackup.TabIndex = 22;
            helpRestoreBackup.TabStop = true;
            helpRestoreBackup.Tag = "iatag_ui_questionmark";
            helpRestoreBackup.Text = " ? ";
            helpRestoreBackup.LinkClicked += helpRestoreBackup_LinkClicked;
            // 
            // radioGDStash
            // 
            radioGDStash.Bold = false;
            radioGDStash.Checked = false;
            radioGDStash.EnabledCalc = true;
            radioGDStash.Font = new Font("Segoe UI", 10F);
            radioGDStash.ForeColor = Color.FromArgb(66, 78, 90);
            radioGDStash.Location = new Point(4, 95);
            radioGDStash.Margin = new Padding(4, 3, 4, 3);
            radioGDStash.Name = "radioGDStash";
            radioGDStash.Size = new Size(160, 31);
            radioGDStash.TabIndex = 0;
            radioGDStash.Tag = "iatag_ui_gdstash";
            radioGDStash.Text = "GD Stash";
            radioGDStash.CheckedChanged += radioGDStash_CheckedChanged;
            // 
            // radioIAStash
            // 
            radioIAStash.Bold = false;
            radioIAStash.Checked = true;
            radioIAStash.EnabledCalc = true;
            radioIAStash.Font = new Font("Segoe UI", 10F);
            radioIAStash.ForeColor = Color.FromArgb(66, 78, 90);
            radioIAStash.Location = new Point(4, 60);
            radioIAStash.Margin = new Padding(4, 3, 4, 3);
            radioIAStash.Name = "radioIAStash";
            radioIAStash.Size = new Size(160, 31);
            radioIAStash.TabIndex = 1;
            radioIAStash.Tag = "iatag_ui_iastash";
            radioIAStash.Text = "IA Stash";
            radioIAStash.CheckedChanged += radioIAStash_CheckedChanged;
            // 
            // ImportMode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(750, 217);
            Controls.Add(panelBox3);
            Controls.Add(panelBox1);
            Controls.Add(panelBox2);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ImportMode";
            Text = "ImportMode";
            Load += ImportMode_Load;
            panelBox3.ResumeLayout(false);
            panelBox1.ResumeLayout(false);
            panelBox2.ResumeLayout(false);
            panelBox2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private FirefoxRadioButton radioGDStash;
        private FirefoxRadioButton radioIAStash;
        private PanelBox panelBox2;
        private FirefoxButton buttonBrowse;
        private PanelBox panelBox1;
        private PanelBox panelBox3;
        private FirefoxButton buttonImport;
        private System.Windows.Forms.ComboBox cbItemSelection;
        private System.Windows.Forms.LinkLabel helpRestoreBackup;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}