namespace IAGrim.UI.Filters {
    partial class Resistances {
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
            this.resistancePanel = new IAGrim.Theme.CollapseablePanelBox();
            this.resistBleeding = new FirefoxCheckBox();
            this.resistElemental = new FirefoxCheckBox();
            this.resistFire = new FirefoxCheckBox();
            this.resistPiercing = new FirefoxCheckBox();
            this.resistPhysical = new FirefoxCheckBox();
            this.resistPoison = new FirefoxCheckBox();
            this.resistCold = new FirefoxCheckBox();
            this.resistChaos = new FirefoxCheckBox();
            this.resistLightning = new FirefoxCheckBox();
            this.resistVitality = new FirefoxCheckBox();
            this.resistAether = new FirefoxCheckBox();
            this.resistStun = new FirefoxCheckBox();
            this.resistancePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // resistancePanel
            // 
            this.resistancePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistancePanel.Controls.Add(this.resistStun);
            this.resistancePanel.Controls.Add(this.resistBleeding);
            this.resistancePanel.Controls.Add(this.resistElemental);
            this.resistancePanel.Controls.Add(this.resistFire);
            this.resistancePanel.Controls.Add(this.resistPiercing);
            this.resistancePanel.Controls.Add(this.resistPhysical);
            this.resistancePanel.Controls.Add(this.resistPoison);
            this.resistancePanel.Controls.Add(this.resistCold);
            this.resistancePanel.Controls.Add(this.resistChaos);
            this.resistancePanel.Controls.Add(this.resistLightning);
            this.resistancePanel.Controls.Add(this.resistVitality);
            this.resistancePanel.Controls.Add(this.resistAether);
            this.resistancePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resistancePanel.HeaderHeight = 29;
            this.resistancePanel.Location = new System.Drawing.Point(3, 3);
            this.resistancePanel.Name = "resistancePanel";
            this.resistancePanel.NoRounding = false;
            this.resistancePanel.Size = new System.Drawing.Size(310, 437);
            this.resistancePanel.TabIndex = 41;
            this.resistancePanel.Tag = "iatag_ui_resistances";
            this.resistancePanel.Text = "Resistances";
            this.resistancePanel.TextLocation = "8; 5";
            // 
            // resistBleeding
            // 
            this.resistBleeding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistBleeding.Bold = false;
            this.resistBleeding.EnabledCalc = true;
            this.resistBleeding.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistBleeding.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistBleeding.Location = new System.Drawing.Point(3, 367);
            this.resistBleeding.Name = "resistBleeding";
            this.resistBleeding.Size = new System.Drawing.Size(291, 27);
            this.resistBleeding.TabIndex = 10;
            this.resistBleeding.Tag = "iatag_ui_resistance_bleeding";
            this.resistBleeding.Text = "Bleeding";
            // 
            // resistElemental
            // 
            this.resistElemental.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistElemental.Bold = false;
            this.resistElemental.EnabledCalc = true;
            this.resistElemental.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistElemental.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistElemental.Location = new System.Drawing.Point(3, 334);
            this.resistElemental.Name = "resistElemental";
            this.resistElemental.Size = new System.Drawing.Size(291, 27);
            this.resistElemental.TabIndex = 9;
            this.resistElemental.Tag = "iatag_ui_resistance_elemental";
            this.resistElemental.Text = "Elemental";
            // 
            // resistFire
            // 
            this.resistFire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistFire.Bold = false;
            this.resistFire.EnabledCalc = true;
            this.resistFire.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistFire.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistFire.Location = new System.Drawing.Point(3, 103);
            this.resistFire.Name = "resistFire";
            this.resistFire.Size = new System.Drawing.Size(291, 27);
            this.resistFire.TabIndex = 2;
            this.resistFire.Tag = "iatag_ui_resistance_fire";
            this.resistFire.Text = "Fire";
            // 
            // resistPiercing
            // 
            this.resistPiercing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistPiercing.Bold = false;
            this.resistPiercing.EnabledCalc = true;
            this.resistPiercing.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistPiercing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistPiercing.Location = new System.Drawing.Point(3, 70);
            this.resistPiercing.Name = "resistPiercing";
            this.resistPiercing.Size = new System.Drawing.Size(291, 27);
            this.resistPiercing.TabIndex = 1;
            this.resistPiercing.Tag = "iatag_ui_resistance_piercing";
            this.resistPiercing.Text = "Piercing";
            // 
            // resistPhysical
            // 
            this.resistPhysical.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistPhysical.Bold = false;
            this.resistPhysical.EnabledCalc = true;
            this.resistPhysical.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistPhysical.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistPhysical.Location = new System.Drawing.Point(3, 37);
            this.resistPhysical.Name = "resistPhysical";
            this.resistPhysical.Size = new System.Drawing.Size(291, 27);
            this.resistPhysical.TabIndex = 0;
            this.resistPhysical.Tag = "iatag_ui_resistance_physical";
            this.resistPhysical.Text = "Physical";
            // 
            // resistPoison
            // 
            this.resistPoison.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistPoison.Bold = false;
            this.resistPoison.EnabledCalc = true;
            this.resistPoison.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistPoison.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistPoison.Location = new System.Drawing.Point(3, 301);
            this.resistPoison.Name = "resistPoison";
            this.resistPoison.Size = new System.Drawing.Size(291, 27);
            this.resistPoison.TabIndex = 8;
            this.resistPoison.Tag = "iatag_ui_resistance_poison";
            this.resistPoison.Text = "Poison";
            // 
            // resistCold
            // 
            this.resistCold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistCold.Bold = false;
            this.resistCold.EnabledCalc = true;
            this.resistCold.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistCold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistCold.Location = new System.Drawing.Point(3, 136);
            this.resistCold.Name = "resistCold";
            this.resistCold.Size = new System.Drawing.Size(291, 27);
            this.resistCold.TabIndex = 3;
            this.resistCold.Tag = "iatag_ui_resistance_cold";
            this.resistCold.Text = "Cold";
            // 
            // resistChaos
            // 
            this.resistChaos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistChaos.Bold = false;
            this.resistChaos.EnabledCalc = true;
            this.resistChaos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistChaos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistChaos.Location = new System.Drawing.Point(3, 268);
            this.resistChaos.Name = "resistChaos";
            this.resistChaos.Size = new System.Drawing.Size(291, 27);
            this.resistChaos.TabIndex = 7;
            this.resistChaos.Tag = "iatag_ui_resistance_chaos";
            this.resistChaos.Text = "Chaos";
            // 
            // resistLightning
            // 
            this.resistLightning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistLightning.Bold = false;
            this.resistLightning.EnabledCalc = true;
            this.resistLightning.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistLightning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistLightning.Location = new System.Drawing.Point(3, 169);
            this.resistLightning.Name = "resistLightning";
            this.resistLightning.Size = new System.Drawing.Size(291, 27);
            this.resistLightning.TabIndex = 4;
            this.resistLightning.Tag = "iatag_ui_resistance_lightning";
            this.resistLightning.Text = "Lightning";
            // 
            // resistVitality
            // 
            this.resistVitality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistVitality.Bold = false;
            this.resistVitality.EnabledCalc = true;
            this.resistVitality.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistVitality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistVitality.Location = new System.Drawing.Point(3, 235);
            this.resistVitality.Name = "resistVitality";
            this.resistVitality.Size = new System.Drawing.Size(291, 27);
            this.resistVitality.TabIndex = 6;
            this.resistVitality.Tag = "iatag_ui_resistance_vitality";
            this.resistVitality.Text = "Vitality";
            // 
            // resistAether
            // 
            this.resistAether.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistAether.Bold = false;
            this.resistAether.EnabledCalc = true;
            this.resistAether.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistAether.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistAether.Location = new System.Drawing.Point(3, 202);
            this.resistAether.Name = "resistAether";
            this.resistAether.Size = new System.Drawing.Size(291, 27);
            this.resistAether.TabIndex = 5;
            this.resistAether.Tag = "iatag_ui_resistance_aether";
            this.resistAether.Text = "Aether";
            // 
            // resistStun
            // 
            this.resistStun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resistStun.Bold = false;
            this.resistStun.EnabledCalc = true;
            this.resistStun.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.resistStun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.resistStun.Location = new System.Drawing.Point(3, 400);
            this.resistStun.Name = "resistStun";
            this.resistStun.Size = new System.Drawing.Size(291, 27);
            this.resistStun.TabIndex = 11;
            this.resistStun.Tag = "iatag_ui_resistance_stun";
            this.resistStun.Text = "Stun";
            // 
            // Resistances
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resistancePanel);
            this.Name = "Resistances";
            this.Size = new System.Drawing.Size(316, 446);
            this.resistancePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Theme.CollapseablePanelBox resistancePanel;
        private FirefoxCheckBox resistBleeding;
        private FirefoxCheckBox resistElemental;
        private FirefoxCheckBox resistFire;
        private FirefoxCheckBox resistPiercing;
        private FirefoxCheckBox resistPhysical;
        private FirefoxCheckBox resistPoison;
        private FirefoxCheckBox resistCold;
        private FirefoxCheckBox resistChaos;
        private FirefoxCheckBox resistLightning;
        private FirefoxCheckBox resistVitality;
        private FirefoxCheckBox resistAether;
        private FirefoxCheckBox resistStun;
    }
}
