namespace IAGrim.UI
{
    partial class MainWindow {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsStashStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageItems = new System.Windows.Forms.TabPage();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.tabPageMods = new System.Windows.Forms.TabPage();
            this.modsPanel = new System.Windows.Forms.Panel();
            this.tabPageOnline = new System.Windows.Forms.TabPage();
            this.onlinePanel = new System.Windows.Forms.Panel();
            this.tabPageBackups = new System.Windows.Forms.TabPage();
            this.backupPanel = new System.Windows.Forms.Panel();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.settingsPanel = new System.Windows.Forms.Panel();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.panelLogging = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageItems.SuspendLayout();
            this.tabPageMods.SuspendLayout();
            this.tabPageOnline.SuspendLayout();
            this.tabPageBackups.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.trayContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.tsStashStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1037, 22);
            this.statusStrip.TabIndex = 25;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(930, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "GD Item Assistant";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsStashStatus
            // 
            this.tsStashStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tsStashStatus.Name = "tsStashStatus";
            this.tsStashStatus.Size = new System.Drawing.Size(92, 17);
            this.tsStashStatus.Tag = "iatag_stash_unknown";
            this.tsStashStatus.Text = "Stash: Unknown";
            this.tsStashStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageItems);
            this.tabControl1.Controls.Add(this.tabPageMods);
            this.tabControl1.Controls.Add(this.tabPageOnline);
            this.tabControl1.Controls.Add(this.tabPageBackups);
            this.tabControl1.Controls.Add(this.tabPageSettings);
            this.tabControl1.Controls.Add(this.tabPageLog);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1037, 539);
            this.tabControl1.TabIndex = 34;
            // 
            // tabPageItems
            // 
            this.tabPageItems.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageItems.Controls.Add(this.searchPanel);
            this.tabPageItems.Location = new System.Drawing.Point(4, 22);
            this.tabPageItems.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageItems.Name = "tabPageItems";
            this.tabPageItems.Size = new System.Drawing.Size(1029, 513);
            this.tabPageItems.TabIndex = 0;
            this.tabPageItems.Tag = "iatag_ui_tab_items";
            this.tabPageItems.Text = "Items";
            // 
            // searchPanel
            // 
            this.searchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchPanel.BackColor = System.Drawing.Color.Transparent;
            this.searchPanel.Location = new System.Drawing.Point(-7, -3);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(1043, 519);
            this.searchPanel.TabIndex = 1;
            // 
            // tabPageMods
            // 
            this.tabPageMods.Controls.Add(this.modsPanel);
            this.tabPageMods.Location = new System.Drawing.Point(4, 22);
            this.tabPageMods.Name = "tabPageMods";
            this.tabPageMods.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMods.Size = new System.Drawing.Size(1029, 513);
            this.tabPageMods.TabIndex = 4;
            this.tabPageMods.Tag = "iatag_ui_tab_mods";
            this.tabPageMods.Text = "Grim Dawn";
            this.tabPageMods.UseVisualStyleBackColor = true;
            // 
            // modsPanel
            // 
            this.modsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modsPanel.Location = new System.Drawing.Point(-4, 0);
            this.modsPanel.Name = "modsPanel";
            this.modsPanel.Size = new System.Drawing.Size(1102, 557);
            this.modsPanel.TabIndex = 1;
            // 
            // tabPageOnline
            // 
            this.tabPageOnline.Controls.Add(this.onlinePanel);
            this.tabPageOnline.Location = new System.Drawing.Point(4, 22);
            this.tabPageOnline.Name = "tabPageOnline";
            this.tabPageOnline.Size = new System.Drawing.Size(1029, 513);
            this.tabPageOnline.TabIndex = 7;
            this.tabPageOnline.Tag = "iatag_ui_tab_online";
            this.tabPageOnline.Text = "Online";
            this.tabPageOnline.UseVisualStyleBackColor = true;
            // 
            // onlinePanel
            // 
            this.onlinePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onlinePanel.Location = new System.Drawing.Point(-2, 0);
            this.onlinePanel.Name = "onlinePanel";
            this.onlinePanel.Size = new System.Drawing.Size(1102, 560);
            this.onlinePanel.TabIndex = 1;
            // 
            // tabPageBackups
            // 
            this.tabPageBackups.Controls.Add(this.backupPanel);
            this.tabPageBackups.Location = new System.Drawing.Point(4, 22);
            this.tabPageBackups.Name = "tabPageBackups";
            this.tabPageBackups.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBackups.Size = new System.Drawing.Size(1029, 513);
            this.tabPageBackups.TabIndex = 3;
            this.tabPageBackups.Tag = "iatag_ui_tab_backups";
            this.tabPageBackups.Text = "Backups";
            this.tabPageBackups.UseVisualStyleBackColor = true;
            // 
            // backupPanel
            // 
            this.backupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backupPanel.Location = new System.Drawing.Point(-4, 0);
            this.backupPanel.Name = "backupPanel";
            this.backupPanel.Size = new System.Drawing.Size(1102, 560);
            this.backupPanel.TabIndex = 0;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSettings.Controls.Add(this.settingsPanel);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Size = new System.Drawing.Size(1029, 513);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Tag = "iatag_ui_tab_settings";
            this.tabPageSettings.Text = "Settings";
            // 
            // settingsPanel
            // 
            this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanel.BackColor = System.Drawing.Color.Transparent;
            this.settingsPanel.Location = new System.Drawing.Point(-7, -3);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(1108, 566);
            this.settingsPanel.TabIndex = 0;
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.panelLogging);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(1029, 513);
            this.tabPageLog.TabIndex = 6;
            this.tabPageLog.Tag = "iatag_ui_tab_log";
            this.tabPageLog.Text = "Log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // panelLogging
            // 
            this.panelLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLogging.ForeColor = System.Drawing.SystemColors.Control;
            this.panelLogging.Location = new System.Drawing.Point(-4, 0);
            this.panelLogging.Name = "panelLogging";
            this.panelLogging.Size = new System.Drawing.Size(1102, 564);
            this.panelLogging.TabIndex = 1;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipTitle = "Item Assistant";
            this.notifyIcon1.ContextMenuStrip = this.trayContextMenuStrip;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "GD Item Assistant";
            this.notifyIcon1.Visible = true;
            // 
            // trayContextMenuStrip
            // 
            this.trayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.trayContextMenuStrip.Name = "trayContextMenuStrip";
            this.trayContextMenuStrip.Size = new System.Drawing.Size(104, 48);
            this.trayContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.trayContextMenuStrip_Opening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1037, 561);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainWindow";
            this.Tag = "iatag_ui_itemassistant";
            this.Text = "Grim Dawn Item Assistant";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageItems.ResumeLayout(false);
            this.tabPageMods.ResumeLayout(false);
            this.tabPageOnline.ResumeLayout(false);
            this.tabPageBackups.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.trayContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel tsStashStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageItems;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip trayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageBackups;
        private System.Windows.Forms.Panel backupPanel;
        private System.Windows.Forms.TabPage tabPageMods;
        private System.Windows.Forms.Panel modsPanel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Panel panelLogging;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageOnline;
        private System.Windows.Forms.Panel onlinePanel;
    }
}