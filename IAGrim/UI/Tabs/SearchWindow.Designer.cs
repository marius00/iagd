namespace IAGrim.UI {
    partial class SearchWindow {
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
            this.components = new System.ComponentModel.Container();
            this.cbModFilter = new System.Windows.Forms.ComboBox();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.comboBoxItemQuality = new System.Windows.Forms.ComboBox();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.slotFilterDropdown = new System.Windows.Forms.ComboBox();
            this.searchField = new System.Windows.Forms.TextBox();
            this.checkBoxOrderByLevel = new System.Windows.Forms.CheckBox();
            this.minLevel = new System.Windows.Forms.TextBox();
            this.maxLevel = new System.Windows.Forms.TextBox();
            this.tooltipThingie = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbModFilter
            // 
            this.cbModFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbModFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModFilter.FormattingEnabled = true;
            this.cbModFilter.Location = new System.Drawing.Point(775, 14);
            this.cbModFilter.Name = "cbModFilter";
            this.cbModFilter.Size = new System.Drawing.Size(102, 21);
            this.cbModFilter.TabIndex = 37;
            this.tooltipThingie.SetToolTip(this.cbModFilter, "Game Mod");
            // 
            // toolStripContainer
            // 
            this.toolStripContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.label1);
            this.toolStripContainer.ContentPanel.Controls.Add(this.outputLabel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(737, 580);
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(213, 41);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(737, 605);
            this.toolStripContainer.TabIndex = 44;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 554);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.outputLabel.Location = new System.Drawing.Point(0, 567);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(0, 13);
            this.outputLabel.TabIndex = 0;
            // 
            // comboBoxItemQuality
            // 
            this.comboBoxItemQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxItemQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxItemQuality.FormattingEnabled = true;
            this.comboBoxItemQuality.Location = new System.Drawing.Point(586, 14);
            this.comboBoxItemQuality.Name = "comboBoxItemQuality";
            this.comboBoxItemQuality.Size = new System.Drawing.Size(59, 21);
            this.comboBoxItemQuality.TabIndex = 43;
            this.tooltipThingie.SetToolTip(this.comboBoxItemQuality, "Item Quality");
            this.comboBoxItemQuality.SelectedIndexChanged += new System.EventHandler(this.comboBoxItemQuality_SelectedIndexChanged);
            // 
            // panelFilter
            // 
            this.panelFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelFilter.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelFilter.Location = new System.Drawing.Point(4, 5);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(209, 641);
            this.panelFilter.TabIndex = 38;
            this.panelFilter.TabStop = true;
            // 
            // slotFilterDropdown
            // 
            this.slotFilterDropdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slotFilterDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.slotFilterDropdown.FormattingEnabled = true;
            this.slotFilterDropdown.Location = new System.Drawing.Point(651, 14);
            this.slotFilterDropdown.Name = "slotFilterDropdown";
            this.slotFilterDropdown.Size = new System.Drawing.Size(118, 21);
            this.slotFilterDropdown.TabIndex = 41;
            this.tooltipThingie.SetToolTip(this.slotFilterDropdown, "Item slot");
            this.slotFilterDropdown.SelectedIndexChanged += new System.EventHandler(this.slotFilter_SelectedIndexChanged);
            // 
            // searchField
            // 
            this.searchField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchField.Location = new System.Drawing.Point(219, 15);
            this.searchField.Name = "searchField";
            this.searchField.Size = new System.Drawing.Size(261, 20);
            this.searchField.TabIndex = 40;
            this.tooltipThingie.SetToolTip(this.searchField, "Item Name");
            this.searchField.TextChanged += new System.EventHandler(this.searchField_TextChanged);
            // 
            // checkBoxOrderByLevel
            // 
            this.checkBoxOrderByLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxOrderByLevel.AutoSize = true;
            this.checkBoxOrderByLevel.Location = new System.Drawing.Point(486, 17);
            this.checkBoxOrderByLevel.Name = "checkBoxOrderByLevel";
            this.checkBoxOrderByLevel.Size = new System.Drawing.Size(96, 17);
            this.checkBoxOrderByLevel.TabIndex = 45;
            this.checkBoxOrderByLevel.Tag = "iatag_ui_orderbylevel";
            this.checkBoxOrderByLevel.Text = "Order By Level";
            this.checkBoxOrderByLevel.UseVisualStyleBackColor = true;
            this.checkBoxOrderByLevel.CheckedChanged += new System.EventHandler(this.checkBoxOrderByLevel_CheckedChanged);
            // 
            // minLevel
            // 
            this.minLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minLevel.Location = new System.Drawing.Point(883, 14);
            this.minLevel.MaxLength = 3;
            this.minLevel.Name = "minLevel";
            this.minLevel.Size = new System.Drawing.Size(30, 20);
            this.minLevel.TabIndex = 46;
            this.minLevel.Text = "0";
            this.minLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tooltipThingie.SetToolTip(this.minLevel, "Minimum level");
            this.minLevel.WordWrap = false;
            // 
            // maxLevel
            // 
            this.maxLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxLevel.Location = new System.Drawing.Point(919, 14);
            this.maxLevel.MaxLength = 3;
            this.maxLevel.Name = "maxLevel";
            this.maxLevel.Size = new System.Drawing.Size(30, 20);
            this.maxLevel.TabIndex = 47;
            this.maxLevel.Text = "110";
            this.maxLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tooltipThingie.SetToolTip(this.maxLevel, "Maximum Level");
            this.maxLevel.WordWrap = false;
            // 
            // SearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 650);
            this.Controls.Add(this.maxLevel);
            this.Controls.Add(this.minLevel);
            this.Controls.Add(this.checkBoxOrderByLevel);
            this.Controls.Add(this.cbModFilter);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.comboBoxItemQuality);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.slotFilterDropdown);
            this.Controls.Add(this.searchField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SearchWindow";
            this.Load += new System.EventHandler(this.SearchWindow_Load);
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.ContentPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbModFilter;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.ComboBox comboBoxItemQuality;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.ComboBox slotFilterDropdown;
        private System.Windows.Forms.TextBox searchField;
        private System.Windows.Forms.CheckBox checkBoxOrderByLevel;
        private System.Windows.Forms.TextBox minLevel;
        private System.Windows.Forms.TextBox maxLevel;
        private System.Windows.Forms.ToolTip tooltipThingie;
    }
}