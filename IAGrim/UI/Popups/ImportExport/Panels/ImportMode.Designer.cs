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
            this.panelBox3 = new PanelBox();
            this.cbItemSelection = new System.Windows.Forms.ComboBox();
            this.buttonImport = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.buttonBrowse = new FirefoxButton();
            this.panelBox2 = new PanelBox();
            this.radioGameStash = new FirefoxRadioButton();
            this.radioGDStash = new FirefoxRadioButton();
            this.radioIAStash = new FirefoxRadioButton();
            this.panelBox3.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.panelBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBox3
            // 
            this.panelBox3.Controls.Add(this.cbItemSelection);
            this.panelBox3.Controls.Add(this.buttonImport);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            this.buttonImport.EnabledCalc = true;
            this.buttonImport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonImport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.panelBox1.Controls.Add(this.buttonBrowse);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            this.buttonBrowse.EnabledCalc = true;
            this.buttonBrowse.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
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
            this.panelBox2.Controls.Add(this.radioGameStash);
            this.panelBox2.Controls.Add(this.radioGDStash);
            this.panelBox2.Controls.Add(this.radioIAStash);
            this.panelBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
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
            this.ClientSize = new System.Drawing.Size(657, 161);
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
    }
}