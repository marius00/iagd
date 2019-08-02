namespace IAGrim.UI.Filters {
    partial class Classes {
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
            this.classesPanelBox = new IAGrim.Theme.CollapseablePanelBox();
            this.SuspendLayout();
            // 
            // classesPanelBox
            // 
            this.classesPanelBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.classesPanelBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.classesPanelBox.HeaderHeight = 29;
            this.classesPanelBox.Location = new System.Drawing.Point(3, 3);
            this.classesPanelBox.Name = "classesPanelBox";
            this.classesPanelBox.NoRounding = false;
            this.classesPanelBox.Size = new System.Drawing.Size(275, 222);
            this.classesPanelBox.TabIndex = 42;
            this.classesPanelBox.Tag = "iatag_ui_classes";
            this.classesPanelBox.Text = "Classes";
            this.classesPanelBox.TextLocation = "8; 5";
            // 
            // Classes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classesPanelBox);
            this.Name = "Classes";
            this.Size = new System.Drawing.Size(281, 228);
            this.ResumeLayout(false);

        }

        #endregion

        private Theme.CollapseablePanelBox classesPanelBox;
    }
}
