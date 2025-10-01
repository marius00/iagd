namespace IAGrim.UI.Popups {
    partial class BackupLoginNagScreen {
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupLoginNagScreen));
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            buttonClose = new Button();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(13, 688);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(79, 15);
            linkLabel1.TabIndex = 4;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Skip for now..";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(717, 688);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(97, 15);
            linkLabel2.TabIndex = 5;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Don't remind me";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(2, 3);
            webView21.Name = "webView21";
            webView21.Size = new Size(824, 682);
            webView21.TabIndex = 6;
            webView21.ZoomFactor = 1D;
            // 
            // buttonClose
            // 
            buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonClose.Font = new Font("Segoe UI", 10F);
            buttonClose.ForeColor = Color.FromArgb(56, 68, 80);
            buttonClose.Location = new Point(13, 663);
            buttonClose.Margin = new Padding(4, 3, 4, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(801, 40);
            buttonClose.TabIndex = 7;
            buttonClose.Tag = "";
            buttonClose.Text = "Close";
            buttonClose.Visible = false;
            buttonClose.Click += buttonClose_Click;
            // 
            // BackupLoginNagScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(826, 712);
            Controls.Add(buttonClose);
            Controls.Add(webView21);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "BackupLoginNagScreen";
            Tag = "iatag_ui_onlinebackups";
            Text = "Online backups";
            Load += BackupLoginNagScreen_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Button buttonClose;
    }
}