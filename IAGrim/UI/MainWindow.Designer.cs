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
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            tsVersionNumber = new ToolStripStatusLabel();
            tsStashStatus = new ToolStripStatusLabel();
            tabControl1 = new TabControl();
            tabPageItems = new TabPage();
            searchPanel = new Panel();
            tabPageOnline = new TabPage();
            onlinePanel = new Panel();
            tabPageSettings = new TabPage();
            settingsPanel = new Panel();
            tabPageMods = new TabPage();
            modsPanel = new Panel();
            tabPageLog = new TabPage();
            panelLogging = new Panel();
            notifyIcon1 = new NotifyIcon(components);
            trayContextMenuStrip = new ContextMenuStrip(components);
            openToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            statusStrip.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPageItems.SuspendLayout();
            tabPageOnline.SuspendLayout();
            tabPageSettings.SuspendLayout();
            tabPageMods.SuspendLayout();
            tabPageLog.SuspendLayout();
            trayContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, tsVersionNumber, tsStashStatus });
            statusStrip.Location = new Point(0, 625);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(1210, 22);
            statusStrip.TabIndex = 25;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1032, 17);
            statusLabel.Spring = true;
            statusLabel.Text = "GD Item Assistant";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tsVersionNumber
            // 
            tsVersionNumber.ImageAlign = ContentAlignment.MiddleRight;
            tsVersionNumber.Name = "tsVersionNumber";
            tsVersionNumber.Size = new Size(69, 17);
            tsVersionNumber.Tag = "";
            tsVersionNumber.Text = "placeholder";
            tsVersionNumber.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tsStashStatus
            // 
            tsStashStatus.ImageAlign = ContentAlignment.MiddleRight;
            tsStashStatus.Name = "tsStashStatus";
            tsStashStatus.Size = new Size(92, 17);
            tsStashStatus.Tag = "iatag_stash_unknown";
            tsStashStatus.Text = "Stash: Unknown";
            tsStashStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPageItems);
            tabControl1.Controls.Add(tabPageOnline);
            tabControl1.Controls.Add(tabPageSettings);
            tabControl1.Controls.Add(tabPageMods);
            tabControl1.Controls.Add(tabPageLog);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1210, 622);
            tabControl1.TabIndex = 34;
            // 
            // tabPageItems
            // 
            tabPageItems.BackColor = SystemColors.Control;
            tabPageItems.Controls.Add(searchPanel);
            tabPageItems.Location = new Point(4, 24);
            tabPageItems.Margin = new Padding(0);
            tabPageItems.Name = "tabPageItems";
            tabPageItems.Size = new Size(1202, 594);
            tabPageItems.TabIndex = 0;
            tabPageItems.Tag = "iatag_ui_tab_items";
            tabPageItems.Text = "Items";
            // 
            // searchPanel
            // 
            searchPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            searchPanel.BackColor = Color.Transparent;
            searchPanel.Location = new Point(-8, -3);
            searchPanel.Margin = new Padding(0);
            searchPanel.Name = "searchPanel";
            searchPanel.Size = new Size(1217, 599);
            searchPanel.TabIndex = 1;
            // 
            // tabPageOnline
            // 
            tabPageOnline.Controls.Add(onlinePanel);
            tabPageOnline.Location = new Point(4, 24);
            tabPageOnline.Margin = new Padding(4, 3, 4, 3);
            tabPageOnline.Name = "tabPageOnline";
            tabPageOnline.Size = new Size(1202, 594);
            tabPageOnline.TabIndex = 7;
            tabPageOnline.Tag = "iatag_ui_tab_online";
            tabPageOnline.Text = "Online";
            tabPageOnline.UseVisualStyleBackColor = true;
            // 
            // onlinePanel
            // 
            onlinePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            onlinePanel.Location = new Point(-2, 0);
            onlinePanel.Margin = new Padding(4, 3, 4, 3);
            onlinePanel.Name = "onlinePanel";
            onlinePanel.Size = new Size(1286, 646);
            onlinePanel.TabIndex = 1;
            // 
            // tabPageSettings
            // 
            tabPageSettings.BackColor = SystemColors.Control;
            tabPageSettings.Controls.Add(settingsPanel);
            tabPageSettings.Location = new Point(4, 24);
            tabPageSettings.Margin = new Padding(0);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Size = new Size(1202, 594);
            tabPageSettings.TabIndex = 1;
            tabPageSettings.Tag = "iatag_ui_tab_settings";
            tabPageSettings.Text = "Settings";
            // 
            // settingsPanel
            // 
            settingsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            settingsPanel.BackColor = Color.Transparent;
            settingsPanel.Location = new Point(-8, -3);
            settingsPanel.Margin = new Padding(4, 3, 4, 3);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(1293, 653);
            settingsPanel.TabIndex = 0;
            // 
            // tabPageMods
            // 
            tabPageMods.Controls.Add(modsPanel);
            tabPageMods.Location = new Point(4, 24);
            tabPageMods.Margin = new Padding(4, 3, 4, 3);
            tabPageMods.Name = "tabPageMods";
            tabPageMods.Padding = new Padding(4, 3, 4, 3);
            tabPageMods.Size = new Size(1202, 594);
            tabPageMods.TabIndex = 4;
            tabPageMods.Tag = "iatag_ui_tab_mods";
            tabPageMods.Text = "Grim Dawn";
            tabPageMods.UseVisualStyleBackColor = true;
            // 
            // modsPanel
            // 
            modsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modsPanel.Location = new Point(-5, 0);
            modsPanel.Margin = new Padding(4, 3, 4, 3);
            modsPanel.Name = "modsPanel";
            modsPanel.Size = new Size(1286, 643);
            modsPanel.TabIndex = 1;
            // 
            // tabPageLog
            // 
            tabPageLog.Controls.Add(panelLogging);
            tabPageLog.Location = new Point(4, 24);
            tabPageLog.Margin = new Padding(4, 3, 4, 3);
            tabPageLog.Name = "tabPageLog";
            tabPageLog.Padding = new Padding(4, 3, 4, 3);
            tabPageLog.Size = new Size(1202, 594);
            tabPageLog.TabIndex = 6;
            tabPageLog.Tag = "iatag_ui_tab_log";
            tabPageLog.Text = "Log";
            tabPageLog.UseVisualStyleBackColor = true;
            // 
            // panelLogging
            // 
            panelLogging.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelLogging.ForeColor = SystemColors.Control;
            panelLogging.Location = new Point(-5, 0);
            panelLogging.Margin = new Padding(4, 3, 4, 3);
            panelLogging.Name = "panelLogging";
            panelLogging.Size = new Size(1286, 651);
            panelLogging.TabIndex = 1;
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipTitle = "Item Assistant";
            notifyIcon1.ContextMenuStrip = trayContextMenuStrip;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "GD Item Assistant";
            notifyIcon1.Visible = true;
            // 
            // trayContextMenuStrip
            // 
            trayContextMenuStrip.Items.AddRange(new ToolStripItem[] { openToolStripMenuItem, exitToolStripMenuItem });
            trayContextMenuStrip.Name = "trayContextMenuStrip";
            trayContextMenuStrip.Size = new Size(104, 48);
            trayContextMenuStrip.Opening += trayContextMenuStrip_Opening;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(103, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1210, 647);
            Controls.Add(tabControl1);
            Controls.Add(statusStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(931, 686);
            Name = "MainWindow";
            Text = "Grim Dawn Item Assistant";
            Load += MainWindow_Load;
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPageItems.ResumeLayout(false);
            tabPageOnline.ResumeLayout(false);
            tabPageSettings.ResumeLayout(false);
            tabPageMods.ResumeLayout(false);
            tabPageLog.ResumeLayout(false);
            trayContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageItems;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.Panel settingsPanel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip trayContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageMods;
        private System.Windows.Forms.Panel modsPanel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Panel panelLogging;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageOnline;
        private System.Windows.Forms.Panel onlinePanel;
        private System.Windows.Forms.ToolStripStatusLabel tsStashStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsVersionNumber;
    }
}