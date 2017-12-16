namespace IAGrim.UI.Popups.ImportExport.Panels {
    partial class ImportExportModePicker {
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
            this.buttonExport = new FirefoxButton();
            this.buttonImport = new FirefoxButton();
            this.SuspendLayout();
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.EnabledCalc = true;
            this.buttonExport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonExport.Location = new System.Drawing.Point(332, 37);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(329, 120);
            this.buttonExport.TabIndex = 9;
            this.buttonExport.Tag = "iatag_ui_export";
            this.buttonExport.Text = "Export";
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.EnabledCalc = true;
            this.buttonImport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonImport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonImport.Location = new System.Drawing.Point(12, 37);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(314, 120);
            this.buttonImport.TabIndex = 8;
            this.buttonImport.Tag = "iatag_ui_import";
            this.buttonImport.Text = "Import";
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // ImportExportModePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 200);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ImportExportModePicker";
            this.ShowInTaskbar = false;
            this.Text = "ImportExportModePicker";
            this.Load += new System.EventHandler(this.ImportExportModePicker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private FirefoxButton buttonImport;
        private FirefoxButton buttonExport;
    }
}