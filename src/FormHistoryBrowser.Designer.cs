namespace hirc
{
    partial class FormHistoryBrowser
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
            this.richTextBoxHistory = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripCopy = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripCopy.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxHistory
            // 
            this.richTextBoxHistory.ContextMenuStrip = this.contextMenuStripCopy;
            this.richTextBoxHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxHistory.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxHistory.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxHistory.Name = "richTextBoxHistory";
            this.richTextBoxHistory.ReadOnly = true;
            this.richTextBoxHistory.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBoxHistory.Size = new System.Drawing.Size(624, 442);
            this.richTextBoxHistory.TabIndex = 0;
            this.richTextBoxHistory.TabStop = false;
            this.richTextBoxHistory.Text = "";
            this.richTextBoxHistory.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxHistory_LinkClicked);
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
            // FormHistoryBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.richTextBoxHistory);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormHistoryBrowser";
            this.Text = "Chat History Browser";
            this.contextMenuStripCopy.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxHistory;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCopy;
        private System.Windows.Forms.ToolStripMenuItem copyCToolStripMenuItem;
    }
}