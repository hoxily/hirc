namespace hirc
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxOut = new System.Windows.Forms.TextBox();
            this.tabControlTabs = new System.Windows.Forms.TabControl();
            this.listBoxNicknameList = new System.Windows.Forms.ListBox();
            this.contextMenuStripCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxOut
            // 
            this.textBoxOut.AcceptsTab = true;
            this.textBoxOut.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxOut.Location = new System.Drawing.Point(0, 402);
            this.textBoxOut.MaxLength = 510;
            this.textBoxOut.Name = "textBoxOut";
            this.textBoxOut.Size = new System.Drawing.Size(630, 26);
            this.textBoxOut.TabIndex = 2;
            this.textBoxOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxOut_KeyDown);
            this.textBoxOut.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxOut_KeyPress);
            // 
            // tabControlTabs
            // 
            this.tabControlTabs.Location = new System.Drawing.Point(0, 0);
            this.tabControlTabs.Name = "tabControlTabs";
            this.tabControlTabs.SelectedIndex = 0;
            this.tabControlTabs.Size = new System.Drawing.Size(472, 387);
            this.tabControlTabs.TabIndex = 3;
            this.tabControlTabs.TabStop = false;
            this.tabControlTabs.SelectedIndexChanged += new System.EventHandler(this.tabControlTabs_SelectedIndexChanged);
            // 
            // listBoxNicknameList
            // 
            this.listBoxNicknameList.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxNicknameList.FormattingEnabled = true;
            this.listBoxNicknameList.Location = new System.Drawing.Point(480, 0);
            this.listBoxNicknameList.Name = "listBoxNicknameList";
            this.listBoxNicknameList.ScrollAlwaysVisible = true;
            this.listBoxNicknameList.Size = new System.Drawing.Size(150, 381);
            this.listBoxNicknameList.Sorted = true;
            this.listBoxNicknameList.TabIndex = 4;
            this.listBoxNicknameList.TabStop = false;
            this.listBoxNicknameList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxNicknameList_MouseDoubleClick);
            // 
            // contextMenuStripCopy
            // 
            this.contextMenuStripCopy.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyCToolStripMenuItem});
            this.contextMenuStripCopy.Name = "contextMenuStrip1";
            this.contextMenuStripCopy.Size = new System.Drawing.Size(153, 48);
            // 
            // copyCToolStripMenuItem
            // 
            this.copyCToolStripMenuItem.Name = "copyCToolStripMenuItem";
            this.copyCToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyCToolStripMenuItem.Text = "Copy(&C)";
            this.copyCToolStripMenuItem.Click += new System.EventHandler(this.copyCToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.listBoxNicknameList);
            this.Controls.Add(this.tabControlTabs);
            this.Controls.Add(this.textBoxOut);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HIRC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.contextMenuStripCopy.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOut;
        private System.Windows.Forms.TabControl tabControlTabs;
        private System.Windows.Forms.ListBox listBoxNicknameList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCopy;
        private System.Windows.Forms.ToolStripMenuItem copyCToolStripMenuItem;
    }
}