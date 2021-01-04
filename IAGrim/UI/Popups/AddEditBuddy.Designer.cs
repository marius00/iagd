
namespace IAGrim.UI.Popups {
    partial class AddEditBuddy {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditBuddy));
            this.buttonAdd = new FirefoxButton();
            this.tbBuddyId = new System.Windows.Forms.TextBox();
            this.tbBuddyNickname = new System.Windows.Forms.TextBox();
            this.lbBuddyNickname = new System.Windows.Forms.Label();
            this.lbBuddyId = new System.Windows.Forms.Label();
            this.lbHelpWhatisBuddyId = new System.Windows.Forms.LinkLabel();
            this.lbHelpWhatisBuddyNickname = new System.Windows.Forms.LinkLabel();
            this.lbIntro = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.BackColorDefault = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.buttonAdd.BackColorOverride = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.buttonAdd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonAdd.EnabledCalc = true;
            this.buttonAdd.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.buttonAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(68)))), ((int)(((byte)(80)))));
            this.buttonAdd.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonAdd.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.buttonAdd.Location = new System.Drawing.Point(12, 127);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(382, 35);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.Tag = "iatag_ui_buddy_save";
            this.buttonAdd.Text = "Save";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // tbBuddyId
            // 
            this.tbBuddyId.Location = new System.Drawing.Point(108, 58);
            this.tbBuddyId.Name = "tbBuddyId";
            this.tbBuddyId.Size = new System.Drawing.Size(201, 20);
            this.tbBuddyId.TabIndex = 4;
            // 
            // tbBuddyNickname
            // 
            this.tbBuddyNickname.Location = new System.Drawing.Point(108, 84);
            this.tbBuddyNickname.Name = "tbBuddyNickname";
            this.tbBuddyNickname.Size = new System.Drawing.Size(201, 20);
            this.tbBuddyNickname.TabIndex = 5;
            // 
            // lbBuddyNickname
            // 
            this.lbBuddyNickname.AutoSize = true;
            this.lbBuddyNickname.Location = new System.Drawing.Point(14, 87);
            this.lbBuddyNickname.Name = "lbBuddyNickname";
            this.lbBuddyNickname.Size = new System.Drawing.Size(91, 13);
            this.lbBuddyNickname.TabIndex = 6;
            this.lbBuddyNickname.Tag = "iatag_ui_buddy_nickname";
            this.lbBuddyNickname.Text = "Buddy Nickname:";
            // 
            // lbBuddyId
            // 
            this.lbBuddyId.AutoSize = true;
            this.lbBuddyId.Location = new System.Drawing.Point(53, 61);
            this.lbBuddyId.Name = "lbBuddyId";
            this.lbBuddyId.Size = new System.Drawing.Size(52, 13);
            this.lbBuddyId.TabIndex = 7;
            this.lbBuddyId.Tag = "iatag_ui_buddy_id";
            this.lbBuddyId.Text = "Buddy Id:";
            // 
            // lbHelpWhatisBuddyId
            // 
            this.lbHelpWhatisBuddyId.AutoSize = true;
            this.lbHelpWhatisBuddyId.BackColor = System.Drawing.Color.Transparent;
            this.lbHelpWhatisBuddyId.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHelpWhatisBuddyId.Location = new System.Drawing.Point(315, 61);
            this.lbHelpWhatisBuddyId.Name = "lbHelpWhatisBuddyId";
            this.lbHelpWhatisBuddyId.Size = new System.Drawing.Size(73, 13);
            this.lbHelpWhatisBuddyId.TabIndex = 11;
            this.lbHelpWhatisBuddyId.TabStop = true;
            this.lbHelpWhatisBuddyId.Tag = "iatag_ui_whatisthis";
            this.lbHelpWhatisBuddyId.Text = "What is this?";
            this.lbHelpWhatisBuddyId.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbHelpWhatisBuddyId_LinkClicked);
            // 
            // lbHelpWhatisBuddyNickname
            // 
            this.lbHelpWhatisBuddyNickname.AutoSize = true;
            this.lbHelpWhatisBuddyNickname.BackColor = System.Drawing.Color.Transparent;
            this.lbHelpWhatisBuddyNickname.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHelpWhatisBuddyNickname.Location = new System.Drawing.Point(315, 87);
            this.lbHelpWhatisBuddyNickname.Name = "lbHelpWhatisBuddyNickname";
            this.lbHelpWhatisBuddyNickname.Size = new System.Drawing.Size(73, 13);
            this.lbHelpWhatisBuddyNickname.TabIndex = 12;
            this.lbHelpWhatisBuddyNickname.TabStop = true;
            this.lbHelpWhatisBuddyNickname.Tag = "iatag_ui_whatisthis";
            this.lbHelpWhatisBuddyNickname.Text = "What is this?";
            this.lbHelpWhatisBuddyNickname.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbHelpWhatisBuddyNickname_LinkClicked);
            // 
            // lbIntro
            // 
            this.lbIntro.AutoSize = true;
            this.lbIntro.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIntro.Location = new System.Drawing.Point(12, 9);
            this.lbIntro.Name = "lbIntro";
            this.lbIntro.Size = new System.Drawing.Size(160, 25);
            this.lbIntro.TabIndex = 13;
            this.lbIntro.Tag = "iatag_ui_buddy_intro";
            this.lbIntro.Text = "Add new buddy";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // AddEditBuddy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 174);
            this.Controls.Add(this.lbIntro);
            this.Controls.Add(this.lbHelpWhatisBuddyNickname);
            this.Controls.Add(this.lbHelpWhatisBuddyId);
            this.Controls.Add(this.lbBuddyId);
            this.Controls.Add(this.lbBuddyNickname);
            this.Controls.Add(this.tbBuddyNickname);
            this.Controls.Add(this.tbBuddyId);
            this.Controls.Add(this.buttonAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditBuddy";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Tag = "iatag_ui_buddy_intro";
            this.Text = "Add new buddy";
            this.Load += new System.EventHandler(this.AddEditBuddy_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FirefoxButton buttonAdd;
        private System.Windows.Forms.TextBox tbBuddyId;
        private System.Windows.Forms.TextBox tbBuddyNickname;
        private System.Windows.Forms.Label lbBuddyNickname;
        private System.Windows.Forms.Label lbBuddyId;
        private System.Windows.Forms.LinkLabel lbHelpWhatisBuddyId;
        private System.Windows.Forms.LinkLabel lbHelpWhatisBuddyNickname;
        private System.Windows.Forms.Label lbIntro;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}