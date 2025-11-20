namespace IAGrim.UI.Popups {
    partial class UpdateModal {
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateModal));
            btnUpdateNow = new Button();
            lnkRemindMeLater = new LinkLabel();
            label1 = new Label();
            lnkWhatHasChanged = new LinkLabel();
            SuspendLayout();
            // 
            // btnUpdateNow
            // 
            btnUpdateNow.Location = new Point(122, 183);
            btnUpdateNow.Name = "btnUpdateNow";
            btnUpdateNow.Size = new Size(210, 48);
            btnUpdateNow.TabIndex = 1;
            btnUpdateNow.Text = "Update now";
            btnUpdateNow.UseVisualStyleBackColor = true;
            btnUpdateNow.Click += btnUpdateNow_Click;
            // 
            // lnkRemindMeLater
            // 
            lnkRemindMeLater.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lnkRemindMeLater.AutoSize = true;
            lnkRemindMeLater.Location = new Point(347, 236);
            lnkRemindMeLater.Name = "lnkRemindMeLater";
            lnkRemindMeLater.Size = new Size(100, 15);
            lnkRemindMeLater.TabIndex = 3;
            lnkRemindMeLater.TabStop = true;
            lnkRemindMeLater.Text = "Remind me later..";
            lnkRemindMeLater.LinkClicked += lnkRemindMeLater_LinkClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(24, 9);
            label1.Name = "label1";
            label1.Size = new Size(417, 32);
            label1.TabIndex = 4;
            label1.Text = "A new version is available is available!";
            // 
            // lnkWhatHasChanged
            // 
            lnkWhatHasChanged.AutoSize = true;
            lnkWhatHasChanged.Location = new Point(168, 234);
            lnkWhatHasChanged.Name = "lnkWhatHasChanged";
            lnkWhatHasChanged.Size = new Size(124, 15);
            lnkWhatHasChanged.TabIndex = 5;
            lnkWhatHasChanged.TabStop = true;
            lnkWhatHasChanged.Text = "See what has changed";
            lnkWhatHasChanged.LinkClicked += lnkWhatHasChanged_LinkClicked;
            // 
            // UpdateModal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(456, 260);
            Controls.Add(lnkWhatHasChanged);
            Controls.Add(label1);
            Controls.Add(lnkRemindMeLater);
            Controls.Add(btnUpdateNow);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(472, 299);
            MinimumSize = new Size(472, 299);
            Name = "UpdateModal";
            Text = "Update available";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnUpdateNow;
        private LinkLabel lnkRemindMeLater;
        private Label label1;
        private LinkLabel lnkWhatHasChanged;
    }
}