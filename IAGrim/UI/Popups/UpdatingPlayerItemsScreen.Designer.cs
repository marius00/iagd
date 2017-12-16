namespace IAGrim.UI {
    partial class UpdatingPlayerItemsScreen {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatingPlayerItemsScreen));
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.firefoxH11 = new FirefoxH1();
            this.SuspendLayout();
            // 
            // progressBar2
            // 
            this.progressBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar2.Location = new System.Drawing.Point(12, 77);
            this.progressBar2.MarqueeAnimationSpeed = 50;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(462, 23);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 4;
            // 
            // firefoxH11
            // 
            this.firefoxH11.AutoSize = true;
            this.firefoxH11.Font = new System.Drawing.Font("Segoe UI Semibold", 20F);
            this.firefoxH11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(88)))), ((int)(((byte)(100)))));
            this.firefoxH11.Location = new System.Drawing.Point(12, 21);
            this.firefoxH11.Name = "firefoxH11";
            this.firefoxH11.Size = new System.Drawing.Size(396, 37);
            this.firefoxH11.TabIndex = 3;
            this.firefoxH11.Text = "Updating stats for your items...";
            // 
            // UpdatingPlayerItemsScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 120);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.firefoxH11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatingPlayerItemsScreen";
            this.Text = "Updating stats for owned items..";
            this.Load += new System.EventHandler(this.UpdatingPlayerItemsScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar2;
        private FirefoxH1 firefoxH11;

    }
}