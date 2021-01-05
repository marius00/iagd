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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panelBox3 = new PanelBox();
            this.cbItemSelection = new System.Windows.Forms.ComboBox();
            this.buttonImport = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.buttonBrowse = new FirefoxButton();
            this.panelBox2 = new PanelBox();
            this.helpRestoreBackup = new System.Windows.Forms.LinkLabel();
            this.radioGameStash = new FirefoxRadioButton();
            this.radioGDStash = new FirefoxRadioButton();
            this.radioIAStash = new FirefoxRadioButton();
            this.panelBox3.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.panelBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 157);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(622, 23);
            this.progressBar1.TabIndex = 8;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // panelBox3
            // 
            this.panelBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox3.Controls.Add(this.cbItemSelection);
            this.panelBox3.Controls.Add(this.buttonImport);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox3.ForeColor = System.Drawing.Color.Black;
            this.panelBox3.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox3.HeaderHeight = 40;
            this.panelBox3.Location = new System.Drawing.Point(406, 12);
            this.panelBox3.Name = "panelBox3";
            this.panelBox3.NoRounding = false;
            this.panelBox3.Size = new System.Drawing.Size(228, 142);
            this.panelBox3.TabIndex = 7;
            this.panelBox3.Tag = "iatag_ui_import_import";
            this.panelBox3.Text = "Import";
            this.panelBox3.TextLocation = "8; 5";
            // 
            // cbItemSelection
            // 
            this.cbItemSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbItemSelection.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbItemSelection.FormattingEnabled = true;
            this.cbItemSelection.Location = new System.Drawing.Point(15, 52);
            this.cbItemSelection.Name = "cbItemSelection";
            this.cbItemSelection.Size = new System.Drawing.Size(192, 25);
            this.cbItemSelection.TabIndex = 8;
            // 
            // buttonImport
            // 
            this.buttonImport.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonImport.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonImport.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonImport.EnabledCalc = true;
            this.buttonImport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonImport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonImport.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonImport.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonImport.Location = new System.Drawing.Point(15, 85);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(192, 32);
            this.buttonImport.TabIndex = 6;
            this.buttonImport.Tag = "iatag_ui_import";
            this.buttonImport.Text = "Import";
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // panelBox1
            // 
            this.panelBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox1.Controls.Add(this.buttonBrowse);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.ForeColor = System.Drawing.Color.Black;
            this.panelBox1.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox1.HeaderHeight = 40;
            this.panelBox1.Location = new System.Drawing.Point(172, 12);
            this.panelBox1.Name = "panelBox1";
            this.panelBox1.NoRounding = false;
            this.panelBox1.Size = new System.Drawing.Size(228, 142);
            this.panelBox1.TabIndex = 4;
            this.panelBox1.Tag = "iatag_ui_import_stashfile";
            this.panelBox1.Text = "Stash File";
            this.panelBox1.TextLocation = "8; 5";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonBrowse.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonBrowse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonBrowse.EnabledCalc = true;
            this.buttonBrowse.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonBrowse.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonBrowse.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonBrowse.Location = new System.Drawing.Point(14, 85);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(192, 32);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Tag = "iatag_ui_browse";
            this.buttonBrowse.Text = "Browse..";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // panelBox2
            // 
            this.panelBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelBox2.Controls.Add(this.helpRestoreBackup);
            this.panelBox2.Controls.Add(this.radioGameStash);
            this.panelBox2.Controls.Add(this.radioGDStash);
            this.panelBox2.Controls.Add(this.radioIAStash);
            this.panelBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox2.ForeColor = System.Drawing.Color.Black;
            this.panelBox2.HeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(231)))), ((int)(((byte)(231)))));
            this.panelBox2.HeaderHeight = 40;
            this.panelBox2.Location = new System.Drawing.Point(12, 12);
            this.panelBox2.Name = "panelBox2";
            this.panelBox2.NoRounding = false;
            this.panelBox2.Size = new System.Drawing.Size(154, 142);
            this.panelBox2.TabIndex = 3;
            this.panelBox2.Tag = "iatag_ui_import_filetype";
            this.panelBox2.Text = "File Type";
            this.panelBox2.TextLocation = "8; 5";
            // 
            // helpRestoreBackup
            // 
            this.helpRestoreBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpRestoreBackup.AutoSize = true;
            this.helpRestoreBackup.BackColor = System.Drawing.Color.Transparent;
            this.helpRestoreBackup.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpRestoreBackup.Location = new System.Drawing.Point(122, 60);
            this.helpRestoreBackup.Name = "helpRestoreBackup";
            this.helpRestoreBackup.Size = new System.Drawing.Size(18, 13);
            this.helpRestoreBackup.TabIndex = 22;
            this.helpRestoreBackup.TabStop = true;
            this.helpRestoreBackup.Tag = "iatag_ui_questionmark";
            this.helpRestoreBackup.Text = " ? ";
            this.helpRestoreBackup.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpRestoreBackup_LinkClicked);
            // 
            // radioGameStash
            // 
            this.radioGameStash.Bold = false;
            this.radioGameStash.Checked = false;
            this.radioGameStash.EnabledCalc = true;
            this.radioGameStash.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioGameStash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioGameStash.Location = new System.Drawing.Point(3, 112);
            this.radioGameStash.Name = "radioGameStash";
            this.radioGameStash.Size = new System.Drawing.Size(137, 27);
            this.radioGameStash.TabIndex = 2;
            this.radioGameStash.Tag = "iatag_ui_gamestash";
            this.radioGameStash.Text = "Game Stash";
            this.radioGameStash.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioGameStash_CheckedChanged);
            // 
            // radioGDStash
            // 
            this.radioGDStash.Bold = false;
            this.radioGDStash.Checked = false;
            this.radioGDStash.EnabledCalc = true;
            this.radioGDStash.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioGDStash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioGDStash.Location = new System.Drawing.Point(3, 82);
            this.radioGDStash.Name = "radioGDStash";
            this.radioGDStash.Size = new System.Drawing.Size(137, 27);
            this.radioGDStash.TabIndex = 0;
            this.radioGDStash.Tag = "iatag_ui_gdstash";
            this.radioGDStash.Text = "GD Stash";
            this.radioGDStash.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioGDStash_CheckedChanged);
            // 
            // radioIAStash
            // 
            this.radioIAStash.Bold = false;
            this.radioIAStash.Checked = true;
            this.radioIAStash.EnabledCalc = true;
            this.radioIAStash.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioIAStash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioIAStash.Location = new System.Drawing.Point(3, 52);
            this.radioIAStash.Name = "radioIAStash";
            this.radioIAStash.Size = new System.Drawing.Size(137, 27);
            this.radioIAStash.TabIndex = 1;
            this.radioIAStash.Tag = "iatag_ui_iastash";
            this.radioIAStash.Text = "IA Stash";
            this.radioIAStash.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioIAStash_CheckedChanged);
            // 
            // ImportMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 188);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panelBox3);
            this.Controls.Add(this.panelBox1);
            this.Controls.Add(this.panelBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ImportMode";
            this.Text = "ImportMode";
            this.Load += new System.EventHandler(this.ImportMode_Load);
            this.panelBox3.ResumeLayout(false);
            this.panelBox1.ResumeLayout(false);
            this.panelBox2.ResumeLayout(false);
            this.panelBox2.PerformLayout();
            this.ResumeLayout(false);

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
        private FirefoxRadioButton radioGameStash;
        private System.Windows.Forms.LinkLabel helpRestoreBackup;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}