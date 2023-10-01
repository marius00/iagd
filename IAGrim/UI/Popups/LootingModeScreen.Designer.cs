namespace IAGrim.UI.Popups {
    partial class LootingModeScreen {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LootingModeScreen));
            this.gpInstant = new System.Windows.Forms.GroupBox();
            this.gpDelayed = new System.Windows.Forms.GroupBox();
            this.rbInstant = new System.Windows.Forms.RadioButton();
            this.rbClassic = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.gpInstant.SuspendLayout();
            this.gpDelayed.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpInstant
            // 
            this.gpInstant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gpInstant.Controls.Add(this.label1);
            this.gpInstant.Controls.Add(this.rbInstant);
            this.gpInstant.Location = new System.Drawing.Point(12, 12);
            this.gpInstant.Name = "gpInstant";
            this.gpInstant.Size = new System.Drawing.Size(380, 134);
            this.gpInstant.TabIndex = 0;
            this.gpInstant.TabStop = false;
            this.gpInstant.Tag = "iatag_ui_instantlooting";
            this.gpInstant.Text = "Loot items immediately (recommended)";
            // 
            // gpDelayed
            // 
            this.gpDelayed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpDelayed.Controls.Add(this.label2);
            this.gpDelayed.Controls.Add(this.rbClassic);
            this.gpDelayed.Location = new System.Drawing.Point(398, 12);
            this.gpDelayed.Name = "gpDelayed";
            this.gpDelayed.Size = new System.Drawing.Size(393, 134);
            this.gpDelayed.TabIndex = 1;
            this.gpDelayed.TabStop = false;
            this.gpDelayed.Tag = "iatag_ui_delayedlooting";
            this.gpDelayed.Text = "Loot items when I\'ve left the smuggler (classic)";
            // 
            // rbInstant
            // 
            this.rbInstant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbInstant.AutoSize = true;
            this.rbInstant.Location = new System.Drawing.Point(6, 111);
            this.rbInstant.Name = "rbInstant";
            this.rbInstant.Size = new System.Drawing.Size(213, 17);
            this.rbInstant.TabIndex = 0;
            this.rbInstant.TabStop = true;
            this.rbInstant.Tag = "itag_ui_lootinstant";
            this.rbInstant.Text = "I want my items to be looted immediately";
            this.rbInstant.UseVisualStyleBackColor = true;
            // 
            // rbClassic
            // 
            this.rbClassic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbClassic.AutoSize = true;
            this.rbClassic.Location = new System.Drawing.Point(6, 111);
            this.rbClassic.Name = "rbClassic";
            this.rbClassic.Size = new System.Drawing.Size(282, 17);
            this.rbClassic.TabIndex = 1;
            this.rbClassic.TabStop = true;
            this.rbClassic.Tag = "itag_ui_lootclassic";
            this.rbClassic.Text = "I want my items to be looted when i leave the smuggler";
            this.rbClassic.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.MaximumSize = new System.Drawing.Size(350, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(342, 39);
            this.label1.TabIndex = 1;
            this.label1.Tag = "iatag_ui_explain_instantlooting";
            this.label1.Text = "This is the default behavior, and supports Steam Cloud Sync. Items will be looted" +
    " as soon as they are placed in your shared transfer tab in the game.";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.MaximumSize = new System.Drawing.Size(350, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 39);
            this.label2.TabIndex = 2;
            this.label2.Tag = "iatag_ui_explain_classiclooting";
            this.label2.Text = "This is the old behavior of Item Assistant, and does NOT supports Steam Cloud Syn" +
    "c. Items will not be looted immediately, instead they will be removed when you w" +
    "alk away from the smuggler.";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(12, 146);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(779, 43);
            this.button1.TabIndex = 2;
            this.button1.Tag = "iatag_ui_btn_save";
            this.button1.Text = "Save && Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LootingModeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 191);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gpDelayed);
            this.Controls.Add(this.gpInstant);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LootingModeScreen";
            this.Tag = "itag_ui_title_lootingmode";
            this.Text = "Select your preferred looting mode";
            this.Load += new System.EventHandler(this.LootingModeScreen_Load);
            this.gpInstant.ResumeLayout(false);
            this.gpInstant.PerformLayout();
            this.gpDelayed.ResumeLayout(false);
            this.gpDelayed.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpInstant;
        private System.Windows.Forms.GroupBox gpDelayed;
        private System.Windows.Forms.RadioButton rbInstant;
        private System.Windows.Forms.RadioButton rbClassic;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}