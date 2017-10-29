using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace hirc
{
    public partial class FormShell : Form
    {
        FormMain freenode;
        FormMain ourirc;
        Config freenodeConfig;
        Config ourircConfig;
        private static string UsageMessage =
            "Connect --> Freenode 连接freenode服务器;\r\n" +
            "Connect --> Ourirc 连接ourirc服务器;\r\n" +
            "server Tab 显示原始IRC消息，用<IN> <OUT> 对收发进行区分;\r\n"+
            "nickname Tab 显示收到的私聊消息，比如别人用'/msg 你的昵称 msg' 你所收到的消息;\r\n"+
            "channelname Tab 显示频道里的公开聊天;\r\n"+
            "在server tab情况下，在输入文本框里按下F1将进行nickname认证;\r\n" +
            "在server tab情况下，在输入文本框里按下F2将加入FavoriteChannels;\r\n"+
            "相关的配置信息全部在程序所在目录的config.ini文件中;\r\n"+
            "HIRC Version 4.0";
        public FormShell()
        {
            InitializeComponent();
            LoadConfigFromFile();
        }
        private void LoadConfigFromFile()
        {
            if (!File.Exists(Program.StartUpDirectory + "\\config.ini"))
            {
                SetDefaultConfig();
            }
            else
            {
                StreamReader r = new StreamReader(Program.StartUpDirectory + "\\config.ini");
                r.ReadLine();//[freenode]
                string[] arr = new string[7];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = r.ReadLine();
                }
                freenodeConfig = Config.FromStrings(arr);
                r.ReadLine();//[ourirc]
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = r.ReadLine();
                }
                ourircConfig = Config.FromStrings(arr);
                r.Close();
            }
        }
        private void SetDefaultConfig()
        {
            freenodeConfig = new Config();
            freenodeConfig.hostname = "irc.freenode.net";
            freenodeConfig.port = 6667;
            freenodeConfig.nickname = "hoxily1";
            freenodeConfig.password = "password";
            freenodeConfig.favoritechannel = "#ubuntu-cn,#botwar";
            freenodeConfig.encoding = Encoding.UTF8;
            ourircConfig = new Config();
            ourircConfig.hostname = "irc.ourirc.com";
            ourircConfig.port = 6668;
            ourircConfig.nickname = "hoxily1";
            ourircConfig.password = "password";
            ourircConfig.favoritechannel = "#新兵训练营";
            ourircConfig.encoding = Encoding.UTF8;

        }
        private void freenodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (freenode != null)
            {
                if (freenode.IsDisposed)
                {
                    freenode = new FormMain();
                    freenode.MdiParent = this;
                    freenode.config = freenodeConfig;
                    freenode.Show();
                }
            }
            else
            {
                freenode = new FormMain();
                freenode.MdiParent = this;
                freenode.config = freenodeConfig;
                freenode.Show();
            }
        }

        private void ourircToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ourirc != null)
            {
                if (ourirc.IsDisposed)
                {
                    ourirc = new FormMain();
                    ourirc.MdiParent = this;
                    ourirc.config = ourircConfig;
                    ourirc.Show();
                }
            }
            else
            {
                ourirc = new FormMain();
                ourirc.MdiParent = this;
                ourirc.config = ourircConfig;
                ourirc.Show();
            }
        }

        private void FormShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter w = new StreamWriter(Program.StartUpDirectory + "\\config.ini");
            w.WriteLine("[freenode]");
            w.Write(freenodeConfig.ToString());
            w.WriteLine("[ourirc]");
            w.Write(ourircConfig.ToString());
            w.Close();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(UsageMessage, "HELP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
