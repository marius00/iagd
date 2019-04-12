namespace IAGrim.UI.Popups {
    partial class StashTabPicker {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StashTabPicker));
            this.gbMoveTo = new System.Windows.Forms.GroupBox();
            this.gbLootFrom = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new FirefoxButton();
            this.radioInputLast = new FirefoxRadioButton();
            this.radioOutputSecondToLast = new FirefoxRadioButton();
            this.gbMoveTo.SuspendLayout();
            this.gbLootFrom.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMoveTo
            // 
            this.gbMoveTo.Controls.Add(this.radioOutputSecondToLast);
            this.gbMoveTo.Location = new System.Drawing.Point(12, 12);
            this.gbMoveTo.Name = "gbMoveTo";
            this.gbMoveTo.Size = new System.Drawing.Size(209, 248);
            this.gbMoveTo.TabIndex = 0;
            this.gbMoveTo.TabStop = false;
            this.gbMoveTo.Tag = "iatag_ui_group_move_items_to";
            this.gbMoveTo.Text = "Move items to";
            // 
            // gbLootFrom
            // 
            this.gbLootFrom.Controls.Add(this.radioInputLast);
            this.gbLootFrom.Location = new System.Drawing.Point(238, 12);
            this.gbLootFrom.Name = "gbLootFrom";
            this.gbLootFrom.Size = new System.Drawing.Size(209, 248);
            this.gbLootFrom.TabIndex = 8;
            this.gbLootFrom.TabStop = false;
            this.gbLootFrom.Tag = "iatag_ui_group_loot_items_from";
            this.gbLootFrom.Text = "Loot items from";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(9, 302);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 13);
            this.label1.TabIndex = 10;
            this.label1.Tag = "iatag_ui_swapstashwarning";
            this.label1.Text = "Changes to \"Loot items from\" may require restarting Grim Dawn";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.EnabledCalc = true;
            this.buttonClose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonClose.Location = new System.Drawing.Point(11, 264);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(435, 35);
            this.buttonClose.TabIndex = 9;
            this.buttonClose.Tag = "iatag_ui_button_close";
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // radioInputLast
            // 
            this.radioInputLast.Bold = false;
            this.radioInputLast.Checked = false;
            this.radioInputLast.EnabledCalc = true;
            this.radioInputLast.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioInputLast.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioInputLast.Location = new System.Drawing.Point(6, 32);
            this.radioInputLast.Name = "radioInputLast";
            this.radioInputLast.Size = new System.Drawing.Size(188, 27);
            this.radioInputLast.TabIndex = 2;
            this.radioInputLast.Tag = "iatag_ui_tab_lasttab";
            this.radioInputLast.Text = "Last tab";
            this.radioInputLast.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioInputLast_CheckedChanged);
            // 
            // radioOutputSecondToLast
            // 
            this.radioOutputSecondToLast.Bold = false;
            this.radioOutputSecondToLast.Checked = false;
            this.radioOutputSecondToLast.EnabledCalc = true;
            this.radioOutputSecondToLast.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radioOutputSecondToLast.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.radioOutputSecondToLast.Location = new System.Drawing.Point(6, 32);
            this.radioOutputSecondToLast.Name = "radioOutputSecondToLast";
            this.radioOutputSecondToLast.Size = new System.Drawing.Size(188, 27);
            this.radioOutputSecondToLast.TabIndex = 2;
            this.radioOutputSecondToLast.Tag = "iatag_ui_tab_secondtolast";
            this.radioOutputSecondToLast.Text = "Second to last tab";
            this.radioOutputSecondToLast.CheckedChanged += new FirefoxRadioButton.CheckedChangedEventHandler(this.radioOutputSecondToLast_CheckedChanged);
            // 
            // StashTabPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 318);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.gbLootFrom);
            this.Controls.Add(this.gbMoveTo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StashTabPicker";
            this.ShowInTaskbar = false;
            this.Text = "Advanced Settings - Stash Configuration";
            this.Load += new System.EventHandler(this.StashTabPicker_Load);
            this.gbMoveTo.ResumeLayout(false);
            this.gbLootFrom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMoveTo;
        private FirefoxRadioButton radioOutputSecondToLast;
        private System.Windows.Forms.GroupBox gbLootFrom;
        private FirefoxRadioButton radioInputLast;
        private FirefoxButton buttonClose;
        private System.Windows.Forms.Label label1;
    }
}