using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hirc
{
    public partial class FormHistoryBrowser : Form
    {
        public FormHistoryBrowser(string historyText)
        {
            InitializeComponent();
            richTextBoxHistory.AppendText(historyText);
            richTextBoxHistory.ScrollToCaret();
        }

        private void richTextBoxHistory_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
            }
        }

        private void copyCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxHistory.Copy();
            //string text = richTextBoxHistory.SelectedText;
            //if(string.IsNullOrEmpty(text))
            //    return;
            //System.Windows.Forms.Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
    }
}
