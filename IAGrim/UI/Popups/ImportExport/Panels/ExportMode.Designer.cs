namespace IAGrim.UI.Popups.ImportExport.Panels {
    partial class ExportMode {
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
            this.buttonExport = new FirefoxButton();
            this.panelBox1 = new PanelBox();
            this.buttonBrowse = new FirefoxButton();
            this.panelBox3.SuspendLayout();
            this.panelBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBox3
            // 
            this.panelBox3.Controls.Add(this.cbItemSelection);
            this.panelBox3.Controls.Add(this.buttonExport);
            this.panelBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox3.HeaderHeight = 40;
            this.panelBox3.Location = new System.Drawing.Point(246, 7);
            this.panelBox3.Name = "panelBox3";
            this.panelBox3.NoRounding = false;
            this.panelBox3.Size = new System.Drawing.Size(228, 142);
            this.panelBox3.TabIndex = 8;
            this.panelBox3.Tag = "iatag_ui_export_export";
            this.panelBox3.Text = "Export";
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
            // buttonExport
            // 
            this.buttonExport.EnabledCalc = false;
            this.buttonExport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonExport.Location = new System.Drawing.Point(15, 85);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(192, 32);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Tag = "iatag_ui_export";
            this.buttonExport.Text = "Export";
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // panelBox1
            // 
            this.panelBox1.Controls.Add(this.buttonBrowse);
            this.panelBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox1.HeaderHeight = 40;
            this.panelBox1.Location = new System.Drawing.Point(12, 7);
            this.panelBox1.Name = "panelBox1";
            this.panelBox1.NoRounding = false;
            this.panelBox1.Size = new System.Drawing.Size(228, 142);
            this.panelBox1.TabIndex = 5;
            this.panelBox1.Tag = "iatag_ui_export_stashfile";
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
            // ExportMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 161);
            this.Controls.Add(this.panelBox3);
            this.Controls.Add(this.panelBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ExportMode";
            this.Text = "ExportMode";
            this.Load += new System.EventHandler(this.ExportMode_Load);
            this.panelBox3.ResumeLayout(false);
            this.panelBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelBox panelBox1;
        private FirefoxButton buttonBrowse;
        private PanelBox panelBox3;
        private System.Windows.Forms.ComboBox cbItemSelection;
        private FirefoxButton buttonExport;
    }
}