namespace IAGrim.UI.Filters {
    partial class Damage {
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
            this.damagePanel = new IAGrim.Theme.CollapseablePanelBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.totalDamage = new FirefoxCheckBox();
            this.dmgRetaliation = new FirefoxCheckBox();
            this.dmgElemental = new FirefoxCheckBox();
            this.dmgPhysical = new FirefoxCheckBox();
            this.dmgPiercing = new FirefoxCheckBox();
            this.dmgFire = new FirefoxCheckBox();
            this.dmgAether = new FirefoxCheckBox();
            this.dmgCold = new FirefoxCheckBox();
            this.dmgPoison = new FirefoxCheckBox();
            this.dmgLightning = new FirefoxCheckBox();
            this.dmgVitality = new FirefoxCheckBox();
            this.dmgChaos = new FirefoxCheckBox();
            this.damagePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // damagePanel
            // 
            this.damagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.damagePanel.Controls.Add(this.panel1);
            this.damagePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.damagePanel.HeaderHeight = 29;
            this.damagePanel.Location = new System.Drawing.Point(3, 3);
            this.damagePanel.Name = "damagePanel";
            this.damagePanel.NoRounding = false;
            this.damagePanel.Size = new System.Drawing.Size(293, 436);
            this.damagePanel.TabIndex = 39;
            this.damagePanel.Tag = "iatag_ui_damage";
            this.damagePanel.Text = "Damage";
            this.damagePanel.TextLocation = "8; 5";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.totalDamage);
            this.panel1.Controls.Add(this.dmgRetaliation);
            this.panel1.Controls.Add(this.dmgElemental);
            this.panel1.Controls.Add(this.dmgPhysical);
            this.panel1.Controls.Add(this.dmgPiercing);
            this.panel1.Controls.Add(this.dmgFire);
            this.panel1.Controls.Add(this.dmgAether);
            this.panel1.Controls.Add(this.dmgCold);
            this.panel1.Controls.Add(this.dmgPoison);
            this.panel1.Controls.Add(this.dmgLightning);
            this.panel1.Controls.Add(this.dmgVitality);
            this.panel1.Controls.Add(this.dmgChaos);
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(290, 401);
            this.panel1.TabIndex = 0;
            // 
            // totalDamage
            // 
            this.totalDamage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.totalDamage.Bold = false;
            this.totalDamage.EnabledCalc = true;
            this.totalDamage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.totalDamage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.totalDamage.Location = new System.Drawing.Point(8, 368);
            this.totalDamage.Name = "totalDamage";
            this.totalDamage.Size = new System.Drawing.Size(275, 27);
            this.totalDamage.TabIndex = 23;
            this.totalDamage.Tag = "iatag_ui_totaldmg";
            this.totalDamage.Text = "Total Damage";
            // 
            // dmgRetaliation
            // 
            this.dmgRetaliation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgRetaliation.Bold = false;
            this.dmgRetaliation.EnabledCalc = true;
            this.dmgRetaliation.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgRetaliation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgRetaliation.Location = new System.Drawing.Point(8, 335);
            this.dmgRetaliation.Name = "dmgRetaliation";
            this.dmgRetaliation.Size = new System.Drawing.Size(275, 27);
            this.dmgRetaliation.TabIndex = 22;
            this.dmgRetaliation.Tag = "iatag_ui_retaliation";
            this.dmgRetaliation.Text = "Retaliation";
            // 
            // dmgElemental
            // 
            this.dmgElemental.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgElemental.Bold = false;
            this.dmgElemental.EnabledCalc = true;
            this.dmgElemental.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgElemental.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgElemental.Location = new System.Drawing.Point(8, 302);
            this.dmgElemental.Name = "dmgElemental";
            this.dmgElemental.Size = new System.Drawing.Size(275, 27);
            this.dmgElemental.TabIndex = 21;
            this.dmgElemental.Tag = "iatag_ui_elemental";
            this.dmgElemental.Text = "Elemental";
            // 
            // dmgPhysical
            // 
            this.dmgPhysical.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgPhysical.Bold = false;
            this.dmgPhysical.EnabledCalc = true;
            this.dmgPhysical.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgPhysical.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgPhysical.Location = new System.Drawing.Point(8, 5);
            this.dmgPhysical.Name = "dmgPhysical";
            this.dmgPhysical.Size = new System.Drawing.Size(275, 27);
            this.dmgPhysical.TabIndex = 12;
            this.dmgPhysical.Tag = "iatag_ui_physical";
            this.dmgPhysical.Text = "Physical";
            // 
            // dmgPiercing
            // 
            this.dmgPiercing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgPiercing.Bold = false;
            this.dmgPiercing.EnabledCalc = true;
            this.dmgPiercing.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgPiercing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgPiercing.Location = new System.Drawing.Point(8, 38);
            this.dmgPiercing.Name = "dmgPiercing";
            this.dmgPiercing.Size = new System.Drawing.Size(275, 27);
            this.dmgPiercing.TabIndex = 13;
            this.dmgPiercing.Tag = "iatag_ui_piercing";
            this.dmgPiercing.Text = "Piercing";
            // 
            // dmgFire
            // 
            this.dmgFire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgFire.Bold = false;
            this.dmgFire.EnabledCalc = true;
            this.dmgFire.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgFire.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgFire.Location = new System.Drawing.Point(8, 71);
            this.dmgFire.Name = "dmgFire";
            this.dmgFire.Size = new System.Drawing.Size(275, 27);
            this.dmgFire.TabIndex = 14;
            this.dmgFire.Tag = "iatag_ui_fire";
            this.dmgFire.Text = "Fire";
            // 
            // dmgAether
            // 
            this.dmgAether.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgAether.Bold = false;
            this.dmgAether.EnabledCalc = true;
            this.dmgAether.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgAether.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgAether.Location = new System.Drawing.Point(8, 170);
            this.dmgAether.Name = "dmgAether";
            this.dmgAether.Size = new System.Drawing.Size(275, 27);
            this.dmgAether.TabIndex = 17;
            this.dmgAether.Tag = "iatag_ui_aether";
            this.dmgAether.Text = "Aether";
            // 
            // dmgCold
            // 
            this.dmgCold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgCold.Bold = false;
            this.dmgCold.EnabledCalc = true;
            this.dmgCold.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgCold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgCold.Location = new System.Drawing.Point(8, 104);
            this.dmgCold.Name = "dmgCold";
            this.dmgCold.Size = new System.Drawing.Size(275, 27);
            this.dmgCold.TabIndex = 15;
            this.dmgCold.Tag = "iatag_ui_cold";
            this.dmgCold.Text = "Cold";
            // 
            // dmgPoison
            // 
            this.dmgPoison.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgPoison.Bold = false;
            this.dmgPoison.EnabledCalc = true;
            this.dmgPoison.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgPoison.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgPoison.Location = new System.Drawing.Point(8, 269);
            this.dmgPoison.Name = "dmgPoison";
            this.dmgPoison.Size = new System.Drawing.Size(275, 27);
            this.dmgPoison.TabIndex = 20;
            this.dmgPoison.Tag = "iatag_ui_poison";
            this.dmgPoison.Text = "Poison";
            // 
            // dmgLightning
            // 
            this.dmgLightning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgLightning.Bold = false;
            this.dmgLightning.EnabledCalc = true;
            this.dmgLightning.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgLightning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgLightning.Location = new System.Drawing.Point(8, 137);
            this.dmgLightning.Name = "dmgLightning";
            this.dmgLightning.Size = new System.Drawing.Size(275, 27);
            this.dmgLightning.TabIndex = 16;
            this.dmgLightning.Tag = "iatag_ui_lightning";
            this.dmgLightning.Text = "Lightning";
            // 
            // dmgVitality
            // 
            this.dmgVitality.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgVitality.Bold = false;
            this.dmgVitality.EnabledCalc = true;
            this.dmgVitality.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgVitality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgVitality.Location = new System.Drawing.Point(8, 203);
            this.dmgVitality.Name = "dmgVitality";
            this.dmgVitality.Size = new System.Drawing.Size(275, 27);
            this.dmgVitality.TabIndex = 18;
            this.dmgVitality.Tag = "iatag_ui_vitality";
            this.dmgVitality.Text = "Vitality";
            // 
            // dmgChaos
            // 
            this.dmgChaos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmgChaos.Bold = false;
            this.dmgChaos.EnabledCalc = true;
            this.dmgChaos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dmgChaos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(78)))), ((int)(((byte)(90)))));
            this.dmgChaos.Location = new System.Drawing.Point(8, 236);
            this.dmgChaos.Name = "dmgChaos";
            this.dmgChaos.Size = new System.Drawing.Size(275, 27);
            this.dmgChaos.TabIndex = 19;
            this.dmgChaos.Tag = "iatag_ui_chaos";
            this.dmgChaos.Text = "Chaos";
            // 
            // Damage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.damagePanel);
            this.Name = "Damage";
            this.Size = new System.Drawing.Size(299, 441);
            this.damagePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Theme.CollapseablePanelBox damagePanel;
        private System.Windows.Forms.Panel panel1;
        private FirefoxCheckBox totalDamage;
        private FirefoxCheckBox dmgRetaliation;
        private FirefoxCheckBox dmgElemental;
        private FirefoxCheckBox dmgPhysical;
        private FirefoxCheckBox dmgPiercing;
        private FirefoxCheckBox dmgFire;
        private FirefoxCheckBox dmgAether;
        private FirefoxCheckBox dmgCold;
        private FirefoxCheckBox dmgPoison;
        private FirefoxCheckBox dmgLightning;
        private FirefoxCheckBox dmgVitality;
        private FirefoxCheckBox dmgChaos;
    }
}
