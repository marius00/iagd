namespace IAGrim.UI {
    partial class ModsDatabaseConfig {
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
            this.panelBox5 = new PanelBox();
            this.buttonUpdateItemStats = new FirefoxButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonForceUpdate = new FirefoxButton();
            this.panelBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBox5
            // 
            this.panelBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBox5.Controls.Add(this.buttonUpdateItemStats);
            this.panelBox5.Controls.Add(this.listView1);
            this.panelBox5.Controls.Add(this.buttonForceUpdate);
            this.panelBox5.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.panelBox5.HeaderHeight = 40;
            this.panelBox5.Location = new System.Drawing.Point(12, 12);
            this.panelBox5.Name = "panelBox5";
            this.panelBox5.NoRounding = false;
            this.panelBox5.Size = new System.Drawing.Size(460, 383);
            this.panelBox5.TabIndex = 10;
            this.panelBox5.Tag = "iatag_ui_mods_header";
            this.panelBox5.Text = "Grim Dawn Database";
            this.panelBox5.TextLocation = "8; 5";
            // 
            // buttonUpdateItemStats
            // 
            this.buttonUpdateItemStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateItemStats.EnabledCalc = true;
            this.buttonUpdateItemStats.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonUpdateItemStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonUpdateItemStats.Location = new System.Drawing.Point(252, 337);
            this.buttonUpdateItemStats.Name = "buttonUpdateItemStats";
            this.buttonUpdateItemStats.Size = new System.Drawing.Size(192, 32);
            this.buttonUpdateItemStats.TabIndex = 5;
            this.buttonUpdateItemStats.Tag = "iatag_ui_updateitemstats";
            this.buttonUpdateItemStats.Text = "Update Item Stats";
            this.buttonUpdateItemStats.Click += new System.EventHandler(this.buttonUpdateItemStats_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(9, 55);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(435, 270);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Tag = "iatag_ui_mod_database_mods_header";
            this.columnHeader1.Text = "Database/Mod";
            this.columnHeader1.Width = 203;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "iatag_ui_mod_loaded_header";
            this.columnHeader2.Text = "Loaded";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Tag = "iatag_ui_mod_path";
            this.columnHeader3.Text = "Path";
            this.columnHeader3.Width = 200;
            // 
            // buttonForceUpdate
            // 
            this.buttonForceUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForceUpdate.EnabledCalc = true;
            this.buttonForceUpdate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonForceUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonForceUpdate.Location = new System.Drawing.Point(9, 337);
            this.buttonForceUpdate.Name = "buttonForceUpdate";
            this.buttonForceUpdate.Size = new System.Drawing.Size(192, 32);
            this.buttonForceUpdate.TabIndex = 4;
            this.buttonForceUpdate.Tag = "iatag_ui_load_database";
            this.buttonForceUpdate.Text = "Load Database";
            this.buttonForceUpdate.Click += new System.EventHandler(this.buttonForceUpdate_Click);
            // 
            // ModsDatabaseConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 407);
            this.Controls.Add(this.panelBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ModsDatabaseConfig";
            this.Text = "ModsDatabaseConfig";
            this.Load += new System.EventHandler(this.ModsDatabaseConfig_Load);
            this.panelBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private FirefoxButton buttonForceUpdate;
        private PanelBox panelBox5;
        private FirefoxButton buttonUpdateItemStats;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}