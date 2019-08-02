namespace IAGrim.UI.Filters {
    partial class DamageOverTimeFilter {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.dotPanel = new IAGrim.Theme.CollapseablePanelBox();
            this.cbLifeLeech = new FirefoxCheckBox();
            this.dmgVitalityDecay = new FirefoxCheckBox();
            this.dmgFrost = new FirefoxCheckBox();
            this.dmgTrauma = new FirefoxCheckBox();
            this.dmgBurn = new FirefoxCheckBox();
            this.dmgElectrocute = new FirefoxCheckBox();
            this.dmgBleeding = new FirefoxCheckBox();
            this.dotPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dotPanel
            // 
            this.dotPanel.Controls.Add(this.cbLifeLeech);
            this.dotPanel.Controls.Add(this.dmgVitalityDecay);
            this.dotPanel.Controls.Add(this.dmgFrost);
            this.dotPanel.Controls.Add(this.dmgTrauma);
            this.dotPanel.Controls.Add(this.dmgBurn);
            this.dotPanel.Controls.Add(this.dmgElectrocute);
            this.dotPanel.Controls.Add(this.dmgBleeding);
            this.dotPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.dotPanel.HeaderHeight = 29;
            this.dotPanel.Location = new System.Drawing.Point(0, 0);
            this.dotPanel.Name = "dotPanel";
            this.dotPanel.NoRounding = false;
            this.dotPanel.Size = new System.Drawing.Size(285, 277);
            this.dotPanel.TabIndex = 42;
            this.dotPanel.Tag = "iatag_ui_dot";
            this.dotPanel.Text = "Damage over Time";
            this.dotPanel.TextLocation = "8; 5";
            // 
            // cbLifeLeech
            // 
            this.cbLifeLeech.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLifeLeech.Bold = false;
            this.cbLifeLeech.EnabledCalc = true;
            this.cbLifeLeech.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbLifeLeech.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.cbLifeLeech.Location = new System.Drawing.Point(3, 231);
            this.cbLifeLeech.Name = "cbLifeLeech";
            this.cbLifeLeech.Size = new System.Drawing.Size(267, 27);
            this.cbLifeLeech.TabIndex = 6;
            this.cbLifeLeech.Tag = "iatag_ui_lifeleech";
            this.cbLifeLeech.Text = "Life Leech";
            // 
            // dmgVitalityDecay
            // 
            this.dmgVitalityDecay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgVitalityDecay.Bold = false;
            this.dmgVitalityDecay.EnabledCalc = true;
            this.dmgVitalityDecay.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgVitalityDecay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgVitalityDecay.Location = new System.Drawing.Point(3, 198);
            this.dmgVitalityDecay.Name = "dmgVitalityDecay";
            this.dmgVitalityDecay.Size = new System.Drawing.Size(267, 27);
            this.dmgVitalityDecay.TabIndex = 5;
            this.dmgVitalityDecay.Tag = "iatag_ui_decay";
            this.dmgVitalityDecay.Text = "Decay";
            // 
            // dmgFrost
            // 
            this.dmgFrost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgFrost.Bold = false;
            this.dmgFrost.EnabledCalc = true;
            this.dmgFrost.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgFrost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgFrost.Location = new System.Drawing.Point(3, 132);
            this.dmgFrost.Name = "dmgFrost";
            this.dmgFrost.Size = new System.Drawing.Size(267, 27);
            this.dmgFrost.TabIndex = 3;
            this.dmgFrost.Tag = "iatag_ui_frost";
            this.dmgFrost.Text = "Frost/freeze";
            // 
            // dmgTrauma
            // 
            this.dmgTrauma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgTrauma.Bold = false;
            this.dmgTrauma.EnabledCalc = true;
            this.dmgTrauma.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgTrauma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgTrauma.Location = new System.Drawing.Point(3, 66);
            this.dmgTrauma.Name = "dmgTrauma";
            this.dmgTrauma.Size = new System.Drawing.Size(267, 27);
            this.dmgTrauma.TabIndex = 1;
            this.dmgTrauma.Tag = "iatag_ui_trauma";
            this.dmgTrauma.Text = "Trauma";
            // 
            // dmgBurn
            // 
            this.dmgBurn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgBurn.Bold = false;
            this.dmgBurn.EnabledCalc = true;
            this.dmgBurn.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgBurn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgBurn.Location = new System.Drawing.Point(3, 99);
            this.dmgBurn.Name = "dmgBurn";
            this.dmgBurn.Size = new System.Drawing.Size(267, 27);
            this.dmgBurn.TabIndex = 2;
            this.dmgBurn.Tag = "iatag_ui_burn";
            this.dmgBurn.Text = "Burn";
            // 
            // dmgElectrocute
            // 
            this.dmgElectrocute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgElectrocute.Bold = false;
            this.dmgElectrocute.EnabledCalc = true;
            this.dmgElectrocute.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgElectrocute.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgElectrocute.Location = new System.Drawing.Point(3, 165);
            this.dmgElectrocute.Name = "dmgElectrocute";
            this.dmgElectrocute.Size = new System.Drawing.Size(267, 27);
            this.dmgElectrocute.TabIndex = 4;
            this.dmgElectrocute.Tag = "iatag_ui_electrocute";
            this.dmgElectrocute.Text = "Electrocute";
            // 
            // dmgBleeding
            // 
            this.dmgBleeding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgBleeding.Bold = false;
            this.dmgBleeding.EnabledCalc = true;
            this.dmgBleeding.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgBleeding.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgBleeding.Location = new System.Drawing.Point(3, 33);
            this.dmgBleeding.Name = "dmgBleeding";
            this.dmgBleeding.Size = new System.Drawing.Size(267, 27);
            this.dmgBleeding.TabIndex = 0;
            this.dmgBleeding.Tag = "iatag_ui_bleeding";
            this.dmgBleeding.Text = "Bleeding";
            // 
            // DamageOverTimeFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dotPanel);
            this.Name = "DamageOverTimeFilter";
            this.Size = new System.Drawing.Size(285, 277);
            this.dotPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Theme.CollapseablePanelBox dotPanel;
        private FirefoxCheckBox cbLifeLeech;
        private FirefoxCheckBox dmgVitalityDecay;
        private FirefoxCheckBox dmgFrost;
        private FirefoxCheckBox dmgTrauma;
        private FirefoxCheckBox dmgBurn;
        private FirefoxCheckBox dmgElectrocute;
        private FirefoxCheckBox dmgBleeding;
    }
}
