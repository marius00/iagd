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
            panelBox5 = new PanelBox();
            buttonConfigure = new FirefoxButton();
            buttonClean = new FirefoxButton();
            helpFindGrimdawnInstall = new LinkLabel();
            listViewMods = new ListView();
            columnHeader2 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            buttonUpdateItemStats = new FirefoxButton();
            listViewInstalls = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            buttonForceUpdate = new FirefoxButton();
            panelBox5.SuspendLayout();
            SuspendLayout();
            // 
            // panelBox5
            // 
            panelBox5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBox5.BackColor = Color.FromArgb(240, 240, 240);
            panelBox5.Controls.Add(buttonConfigure);
            panelBox5.Controls.Add(buttonClean);
            panelBox5.Controls.Add(helpFindGrimdawnInstall);
            panelBox5.Controls.Add(listViewMods);
            panelBox5.Controls.Add(buttonUpdateItemStats);
            panelBox5.Controls.Add(listViewInstalls);
            panelBox5.Controls.Add(buttonForceUpdate);
            panelBox5.Font = new Font("Segoe UI Semibold", 20F);
            panelBox5.ForeColor = Color.Black;
            panelBox5.HeaderColor = Color.FromArgb(231, 231, 231);
            panelBox5.HeaderHeight = 40;
            panelBox5.Location = new Point(14, 14);
            panelBox5.Margin = new Padding(4, 3, 4, 3);
            panelBox5.Name = "panelBox5";
            panelBox5.NoRounding = false;
            panelBox5.Size = new Size(896, 465);
            panelBox5.TabIndex = 10;
            panelBox5.Tag = "iatag_ui_mods_header";
            panelBox5.Text = "Grim Dawn Database";
            panelBox5.TextLocation = "8; 5";
            // 
            // buttonConfigure
            // 
            buttonConfigure.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonConfigure.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonConfigure.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonConfigure.BorderColor = Color.FromArgb(193, 193, 193);
            buttonConfigure.EnabledCalc = true;
            buttonConfigure.Font = new Font("Segoe UI", 10F);
            buttonConfigure.ForeColor = Color.FromArgb(56, 68, 80);
            buttonConfigure.HoverColor = Color.FromArgb(232, 232, 232);
            buttonConfigure.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonConfigure.Location = new Point(241, 412);
            buttonConfigure.Margin = new Padding(4, 3, 4, 3);
            buttonConfigure.Name = "buttonConfigure";
            buttonConfigure.Size = new Size(174, 37);
            buttonConfigure.TabIndex = 9;
            buttonConfigure.Tag = "iatag_ui_manual_db";
            buttonConfigure.Text = "Configure";
            buttonConfigure.Click += buttonConfigure_Click;
            // 
            // buttonClean
            // 
            buttonClean.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonClean.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonClean.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonClean.BorderColor = Color.FromArgb(193, 193, 193);
            buttonClean.EnabledCalc = true;
            buttonClean.Font = new Font("Segoe UI", 10F);
            buttonClean.ForeColor = Color.FromArgb(56, 68, 80);
            buttonClean.HoverColor = Color.FromArgb(232, 232, 232);
            buttonClean.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonClean.Location = new Point(422, 412);
            buttonClean.Margin = new Padding(4, 3, 4, 3);
            buttonClean.Name = "buttonClean";
            buttonClean.Size = new Size(224, 37);
            buttonClean.TabIndex = 8;
            buttonClean.Tag = "iatag_ui_clean";
            buttonClean.Text = "Clean Database";
            buttonClean.Click += buttonClean_Click;
            // 
            // helpFindGrimdawnInstall
            // 
            helpFindGrimdawnInstall.AutoSize = true;
            helpFindGrimdawnInstall.BackColor = Color.Transparent;
            helpFindGrimdawnInstall.Font = new Font("Segoe UI", 8.25F);
            helpFindGrimdawnInstall.Location = new Point(331, 23);
            helpFindGrimdawnInstall.Margin = new Padding(4, 0, 4, 0);
            helpFindGrimdawnInstall.Name = "helpFindGrimdawnInstall";
            helpFindGrimdawnInstall.Size = new Size(201, 13);
            helpFindGrimdawnInstall.TabIndex = 7;
            helpFindGrimdawnInstall.TabStop = true;
            helpFindGrimdawnInstall.Tag = "iatag_ui_howdoifindgrimdawn";
            helpFindGrimdawnInstall.Text = "How can I find the Grim Dawn install?";
            helpFindGrimdawnInstall.LinkClicked += helpFindGrimdawnInstall_LinkClicked;
            // 
            // listViewMods
            // 
            listViewMods.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewMods.Columns.AddRange(new ColumnHeader[] { columnHeader2, columnHeader4 });
            listViewMods.Font = new Font("Microsoft Sans Serif", 8.25F);
            listViewMods.FullRowSelect = true;
            listViewMods.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listViewMods.Location = new Point(503, 63);
            listViewMods.Margin = new Padding(4, 3, 4, 3);
            listViewMods.MultiSelect = false;
            listViewMods.Name = "listViewMods";
            listViewMods.Size = new Size(374, 334);
            listViewMods.TabIndex = 6;
            listViewMods.UseCompatibleStateImageBehavior = false;
            listViewMods.View = View.Details;
            // 
            // columnHeader2
            // 
            columnHeader2.Tag = "iatag_ui_mod_database_mods_header";
            columnHeader2.Text = "Mod";
            columnHeader2.Width = 203;
            // 
            // columnHeader4
            // 
            columnHeader4.Tag = "iatag_ui_mod_path";
            columnHeader4.Text = "Path";
            columnHeader4.Width = 200;
            // 
            // buttonUpdateItemStats
            // 
            buttonUpdateItemStats.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonUpdateItemStats.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonUpdateItemStats.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonUpdateItemStats.BorderColor = Color.FromArgb(193, 193, 193);
            buttonUpdateItemStats.EnabledCalc = true;
            buttonUpdateItemStats.Font = new Font("Segoe UI", 10F);
            buttonUpdateItemStats.ForeColor = Color.FromArgb(56, 68, 80);
            buttonUpdateItemStats.HoverColor = Color.FromArgb(232, 232, 232);
            buttonUpdateItemStats.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonUpdateItemStats.Location = new Point(653, 412);
            buttonUpdateItemStats.Margin = new Padding(4, 3, 4, 3);
            buttonUpdateItemStats.Name = "buttonUpdateItemStats";
            buttonUpdateItemStats.Size = new Size(224, 37);
            buttonUpdateItemStats.TabIndex = 5;
            buttonUpdateItemStats.Tag = "iatag_ui_update_item_stats";
            buttonUpdateItemStats.Text = "Clear cache";
            buttonUpdateItemStats.Click += buttonUpdateItemStats_Click;
            // 
            // listViewInstalls
            // 
            listViewInstalls.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listViewInstalls.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader3 });
            listViewInstalls.Font = new Font("Microsoft Sans Serif", 8.25F);
            listViewInstalls.FullRowSelect = true;
            listViewInstalls.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listViewInstalls.Location = new Point(10, 63);
            listViewInstalls.Margin = new Padding(4, 3, 4, 3);
            listViewInstalls.MultiSelect = false;
            listViewInstalls.Name = "listViewInstalls";
            listViewInstalls.Size = new Size(485, 334);
            listViewInstalls.TabIndex = 0;
            listViewInstalls.UseCompatibleStateImageBehavior = false;
            listViewInstalls.View = View.Details;
            listViewInstalls.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Tag = "iatag_ui_mod_database_mods_header";
            columnHeader1.Text = "Install";
            columnHeader1.Width = 203;
            // 
            // columnHeader3
            // 
            columnHeader3.Tag = "iatag_ui_mod_path";
            columnHeader3.Text = "Path";
            columnHeader3.Width = 200;
            // 
            // buttonForceUpdate
            // 
            buttonForceUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonForceUpdate.BackColorDefault = Color.FromArgb(212, 212, 212);
            buttonForceUpdate.BackColorOverride = Color.FromArgb(245, 245, 245);
            buttonForceUpdate.BorderColor = Color.FromArgb(193, 193, 193);
            buttonForceUpdate.EnabledCalc = true;
            buttonForceUpdate.Font = new Font("Segoe UI", 10F);
            buttonForceUpdate.ForeColor = Color.FromArgb(56, 68, 80);
            buttonForceUpdate.HoverColor = Color.FromArgb(232, 232, 232);
            buttonForceUpdate.HoverForeColor = Color.FromArgb(193, 193, 193);
            buttonForceUpdate.Location = new Point(10, 412);
            buttonForceUpdate.Margin = new Padding(4, 3, 4, 3);
            buttonForceUpdate.Name = "buttonForceUpdate";
            buttonForceUpdate.Size = new Size(224, 37);
            buttonForceUpdate.TabIndex = 4;
            buttonForceUpdate.Tag = "iatag_ui_load_database";
            buttonForceUpdate.Text = "Load Database";
            buttonForceUpdate.Click += buttonForceUpdate_Click;
            // 
            // ModsDatabaseConfig
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 493);
            Controls.Add(panelBox5);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ModsDatabaseConfig";
            Text = "ModsDatabaseConfig";
            Load += ModsDatabaseConfig_Load;
            panelBox5.ResumeLayout(false);
            panelBox5.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewInstalls;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private FirefoxButton buttonForceUpdate;
        private PanelBox panelBox5;
        private FirefoxButton buttonUpdateItemStats;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView listViewMods;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.LinkLabel helpFindGrimdawnInstall;
        private FirefoxButton buttonClean;
        private FirefoxButton buttonConfigure;
    }
}