using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
/*
 * bug fixed by hoxily(hoxily@qq.com):
 * 1.line (\r\n) of every message received from server is now separated
 *   clearly.
 * 2.Use delegate(EventHandler) to manipulate multithread communication.
 * 3.We can set "Form.CheckForIllegalCrossThreadCalls = true;" now!
 */
/*
 * bugs: found at 2012-05-25 by hoxily(hoxily@qq.com).
 * 1.Quit message shouldn't sent to all tabs.
 * 2.Maybe user use NICK to change their nickname, but I don't implement this.
 * 3.It is horrible that tab is full of PART/QUIT/JOIN messages. How about turn on/off these messages?
 */
/*
 * bugs: found at 2012-06-12 by hoxily(hoxily@qq.com).
 * 1. PART message crash me. Due to call Substring with -1 length.
 */

namespace hirc
{
    public partial class FormMain : Form
    {
        private const string CRLF = "\r\n";
        public   Config config;
        byte[] buffer;
        int length;
        byte[] inbuffer;
        int inlength = 0;
        byte[] outbuffer;
        int outlength;
        Socket socket;
        Thread inputthread;
        bool SessionActive;
        string[] TabPageTexts;
        TabPage[] ChannelTabs;
        RichTextBox[] DisplayTextBoxes;
        ListBox[] NicknameListBoxes;
        private DateTime LastWelcomeTime;
        private Delegate MessageReceived;
        string[] Completion;
        int CompletionIndex;

        private void DispatchMessages(string sender, string act, string leftover)
        {//if sender is null, it means you send this message by yourself
            //else you receive this message from server
            act = act.ToUpper();
            string receiver = null;
            switch (act)
            {
                case "NOTICE":
                    break;

                case "PRIVMSG":
                    if (sender == null)
                    {
                        sender = config.nickname;
                    }
                    receiver = leftover.Substring(0, leftover.IndexOf(' '));
                    leftover = leftover.Substring(leftover.IndexOf(':') + 1);
                    DisplayMessage(sender, receiver, leftover);
                    break;

                case "PART":
                    // receiver = leftover.Substring(0, leftover.IndexOf(' '));
                    // 如果没有part的理由，leftover.IndexOf(' ')就会返回-1了。
                    // 因此要先判断存不存在':'
                    string partReason;
                    if(leftover.IndexOf(':') >= 0)
                    {
                        receiver = leftover.Substring(0, leftover.IndexOf(' '));
                        partReason = leftover.Substring(leftover.IndexOf(':') + 1);
                    }
                    else
                    {
                        receiver = leftover;
                        partReason = "";
                    }
                    DisplayMessage(receiver, sender + " left " + receiver + " (" + partReason + ")");
                    DeleteNicknameFromNicknameList(receiver, GetNickname(sender));
                    break;

                case "JOIN":
                    receiver = leftover;
                    if (sender == null)
                    {
                        return;
                    }
                    DisplayMessage(receiver, sender + " has joined " + receiver);
                    AddNicknameToNicknameList(receiver, GetNickname(sender));
                    break;

                case "QUIT":
                    string quitReason;
                    if (sender == null)
                    {
                        return;
                    }
                    if (leftover.IndexOf(':') >= 0)
                    {
                        quitReason = leftover.Substring(leftover.IndexOf(':') + 1);
                    }
                    else
                    {
                        quitReason = "";
                    }
                    DisplayMessage("*", sender + " has quit " + "(" + quitReason + ")");
                    DeleteNicknameFromNicknameList("*", GetNickname(sender));
                    break;

                case "353"://nicknames in that channel
                    string[] nicknames;
                    receiver = leftover.Split(' ')[2];
                    nicknames = leftover.Substring(leftover.IndexOf(':') + 1).Split(
                        new char[]{' '},
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach(string nickname in nicknames)
                    {
                        AddNicknameToNicknameList(receiver, nickname.TrimStart('@'));
                    }
                    break;
            }
        }

        private void DeleteNicknameFromNicknameList(string receiver, string nickname)
        {
            int i;
            if (nickname.StartsWith("@"))//operator signal
            {
                nickname = nickname.TrimStart('@');
            }
            if (receiver == "*")
            {
                for (i = 0; i < ChannelTabs.Length; i++)
                {
                    DeleteNicknameFromListBox(NicknameListBoxes[i], nickname);
                    if (tabControlTabs.SelectedIndex == i)
                    {
                        tabControlTabs_SelectedIndexChanged(null, null);
                    }
                }
            }
            else
            {
                for (i = 0; i < ChannelTabs.Length; i++)
                {
                    if (ChannelTabs[i].Text.ToUpper() == receiver.ToUpper())
                    {
                        DeleteNicknameFromListBox(NicknameListBoxes[i], nickname);
                        if (tabControlTabs.SelectedIndex == i)
                        {
                            tabControlTabs_SelectedIndexChanged(null, null);
                        }
                        break;
                    }
                }
            }
        }

        private void DeleteNicknameFromListBox(ListBox listBox, string nickname)
        {
            int i;
            for (i = 0; i < listBox.Items.Count; i++)
            {
                string s = listBox.Items[i] as string;
                if (s.ToUpper() == nickname.ToUpper())
                {
                    listBox.Items.RemoveAt(i);
                    break;
                }
            }
        }

        private void AddNicknameToNicknameList(string receiver, string nickname)
        {
            int i;
            if (nickname.StartsWith("@"))//operator signal
            {
                nickname = nickname.TrimStart('@');
            }
            for (i = 0; i < ChannelTabs.Length; i++)
            {
                if (ChannelTabs[i].Text.ToUpper() == receiver.ToUpper())
                {
                    AddNickNameToListBox(NicknameListBoxes[i], nickname);
                    if (tabControlTabs.SelectedIndex == i)
                    {
                        tabControlTabs_SelectedIndexChanged(null, null);
                    }
                    break;
                }
                
            }
        }

        private void AddNickNameToListBox(ListBox listBox, string nickname)
        {
            int i;
            for (i = 0; i < listBox.Items.Count; i++)
            {
                string s = listBox.Items[i] as string;
                if (s.ToUpper() == nickname.ToUpper())
                {
                    return;//not need to add, already exists
                }
            }
            listBox.Items.Add(nickname);
        }

        private void ProcessReceivedMessage(object sender, MessageReceiveEvent e)
        {
            string msg = e.Message;

            InHistoryAdd(msg);//log this message in server tab

            if (sender == null)//message received from internal
            {
                if (e.Message == "Connect Register")
                {
                    ConnectRegister();
                }
            }
            else if (sender == inputthread)//message received from server
            {
                //reply PING with PONG to keep alive
                if (msg.ToUpper().StartsWith("PING"))
                {
                    string sndmsg = "PONG " + msg.Split(' ')[1];
                    write(sndmsg);
                }
                else if (msg.StartsWith(":"))
                {//normal form of IRC messages received from server
                    string msgSender;
                    string msgAction;
                    string msgLeftover;
                    string tmp = msg.Substring(1);//remove ":"
                    msgSender = tmp.Substring(0, tmp.IndexOf(' '));
                    tmp = tmp.Substring(msgSender.Length + 1);
                    tmp = tmp.TrimStart(' ');//maybe there are more than 1 space to separate
                    msgAction = tmp.Substring(0, tmp.IndexOf(' '));
                    tmp = tmp.Substring(msgAction.Length + 1);
                    tmp = tmp.TrimStart(' ');
                    msgLeftover = tmp.Substring(0, tmp.Length - 2);//remove CRLF
                    DispatchMessages(msgSender, msgAction, msgLeftover);
                }
            }
        }

        void init()
        {
            inbuffer = new byte[1024];
            outbuffer = new byte[1024];
            buffer = new byte[1024];
            inlength = 0;
            length = 0;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveBufferSize = 1024;
            socket.SendBufferSize = 1024;
            try
            {
                socket.Connect(config.hostname,config.port);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "We got a big error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                SessionActive = false;
                return;
            }
            inputthread = new Thread(new ThreadStart(run));
            inputthread.Start();
            LastWelcomeTime = DateTime.Now;
        }
        string GetNickname(string sender)
        {
            if (sender.Contains('!') && sender.Contains('@'))
            {
                return sender.Substring(0, sender.IndexOf('!'));
            }
            else
            {
                return sender;
            }
        }

        void dispose()
        {
            socket.Close();
        }
        void read()
        {
            length = socket.Receive(buffer);
            int i;
            for (i = 0; i < length; i++)//concatonate to inbuffer until find \r\n
            {
                inbuffer[inlength] = buffer[i];
                inlength++;
                if (inlength >= 2)
                {
                    if (inbuffer[inlength - 1] == '\n' &&
                        inbuffer[inlength - 2] == '\r')
                    {
                        //we get a complete message in inbuffer
                        string rawMessage = config.encoding.GetString(inbuffer, 0, inlength);
                        inlength = 0;
                        this.Invoke(this.MessageReceived, inputthread, new MessageReceiveEvent(rawMessage));
                    }
                }
            }
        }
        void write(string s)
        {
            if (!SessionActive)
                return;
            if (s.Length <= 2)//at least \r\n
                return;
            outlength = config.encoding.GetBytes(s, 0, s.Length, outbuffer, 0);
            socket.Send(outbuffer, outlength, SocketFlags.None);
            OutHistoryAdd(s);
            int i = s.IndexOf(' ');
            string sender = null;
            string act;
            string leftover;
            if (i >= 0)
            {
                act = s.Substring(0, i);
                leftover = s.Substring(i + 1);
                leftover = leftover.Substring(0, leftover.Length - 2);//remove CRLF
            }
            else
            {
                act = s;
                leftover = null;
            }
            DispatchMessages(sender, act, leftover);
        }
        
        void OutHistoryAdd(string str)
        {
            RichTextBox servertabTextBox = DisplayTextBoxes[0];
            servertabTextBox.AppendText("<OUT> " + str);
            servertabTextBox.ScrollToCaret();
        }

        void InHistoryAdd(string str)
        {
            RichTextBox servertabTextBox = DisplayTextBoxes[0];
            servertabTextBox.AppendText("<IN> " + str);
            servertabTextBox.ScrollToCaret();
        }

        private string RemoveMIRCColorCode(string msg)
        {
            string puremsg = "";
            Regex r = new Regex(@"\u0003[0-9][0-9]");
            puremsg = r.Replace(msg, "");
            return puremsg;
        }

        private void DisplayMessage(string sender, string receiver, string msgContent)
        {
            string senderNickname;
            if (sender == null)
            {
                senderNickname = config.nickname;//yourself
            }
            else
            {
                senderNickname = GetNickname(sender);
            }
            for (int i = 1; i < DisplayTextBoxes.Length; i++)
            {
                msgContent = RemoveMIRCColorCode(msgContent);
                TabPage tp = tabControlTabs.TabPages[i];
                if (tp.Text.ToUpper() == receiver.ToUpper())
                {
                    DisplayTextBoxes[i].AppendText(string.Format("[{2}] {0}\r\n        {1}\r\n",
                        senderNickname,
                        msgContent,
                        DateTime.Now.ToShortTimeString()));
                    DisplayTextBoxes[i].ScrollToCaret();
                    break;
                }
            }
        }

        private void DisplayMessage(string receiver, string msgContent)
        {
            if (receiver == null)//yourself
            {

            }
            if (receiver == "*")
            {
                for (int i = 1; i < DisplayTextBoxes.Length; i++)
                {
                    msgContent = RemoveMIRCColorCode(msgContent);
                    TabPage tp = tabControlTabs.TabPages[i];
                    DisplayTextBoxes[i].AppendText(string.Format("[{1}] **** {0}\r\n",
                        msgContent,
                        DateTime.Now.ToShortTimeString()));
                    DisplayTextBoxes[i].ScrollToCaret();
                }
            }
            else
            {
                for (int i = 1; i < DisplayTextBoxes.Length; i++)
                {
                    msgContent = RemoveMIRCColorCode(msgContent);
                    TabPage tp = tabControlTabs.TabPages[i];
                    if (tp.Text.ToUpper() == receiver.ToUpper())
                    {
                        DisplayTextBoxes[i].AppendText(string.Format("[{1}] **** {0}\r\n",
                            msgContent,
                            DateTime.Now.ToShortTimeString()));
                        DisplayTextBoxes[i].ScrollToCaret();
                        break;
                    }
                }
            }
        }

        void run()
        {
            SessionActive = true;
            this.Invoke(this.MessageReceived, null, new MessageReceiveEvent("Connect Register"));
            while (true)
            {
                try
                {
                    read();
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "We got a big error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                }
            }
            this.Text += "(Inactive)";
            SessionActive = false;
            dispose();
        }

        public FormMain()
        {
            Form.CheckForIllegalCrossThreadCalls = true;
            InitializeComponent();
            config = new Config();
            this.MessageReceived = new EventHandler<MessageReceiveEvent>(ProcessReceivedMessage);
            Completion = null;
            CompletionIndex = -1;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            init();
            string[] arr = ("server," + config.nickname + "," + config.favoritechannel).Split(',');
            TabPageTexts = arr;
            ChannelTabs = new TabPage[TabPageTexts.Length];
            DisplayTextBoxes = new RichTextBox[TabPageTexts.Length];
            NicknameListBoxes = new ListBox[TabPageTexts.Length];
            for (int i = 0; i < ChannelTabs.Length; i++)
            {
                ChannelTabs[i] = new TabPage(TabPageTexts[i]);
                DisplayTextBoxes[i] = new RichTextBox();
                DisplayTextBoxes[i].Dock = DockStyle.Fill;
                DisplayTextBoxes[i].ScrollBars = RichTextBoxScrollBars.ForcedVertical;
                DisplayTextBoxes[i].TabStop = false;
                DisplayTextBoxes[i].ReadOnly = true;
                DisplayTextBoxes[i].LinkClicked += new LinkClickedEventHandler(richTextBox_LinkClicked);
                DisplayTextBoxes[i].Font = new System.Drawing.Font("宋体", 12.0f);
                DisplayTextBoxes[i].ContextMenuStrip = contextMenuStripCopy;
                ChannelTabs[i].Controls.Add(DisplayTextBoxes[i]);

                NicknameListBoxes[i] = new ListBox();
                NicknameListBoxes[i].TabStop = false;
                NicknameListBoxes[i].Font = listBoxNicknameList.Font;
                NicknameListBoxes[i].ScrollAlwaysVisible = true;
                NicknameListBoxes[i].Sorted = true;
            }
            tabControlTabs.Controls.AddRange(ChannelTabs);
            tabControlTabs.SelectedIndex = 0;
            NewSize();
            this.Text = config.nickname + "@" + config.hostname + ":" + config.port.ToString();
        }
        void ConnectRegister()
        {
            string msg;
            msg = "NICK "+config.nickname + CRLF;
            write(msg);
            msg = "USER " + config.nickname + " 0 * " + config.realname + CRLF;
            write(msg);
        }
        void JoinFavoriteChannel()
        {
            string msg;
            string[] channels = config.favoritechannel.Split(',');
            foreach (string chan in channels)
            {
                msg = "JOIN " + chan + CRLF;
                write(msg);
            }
        }

        private void textBoxOut_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxOut.TextLength <= 0)
                return;
            if (e.KeyChar == '\r')
            {
                string str="";
                string chan = str;
                if(tabControlTabs.SelectedIndex == 0)
                {//raw message mode, send text direct to server
                    str = textBoxOut.Text + CRLF;
                }
                else
                {
                    chan = tabControlTabs.SelectedTab.Text;
                    str = "PRIVMSG " + chan + " :" + textBoxOut.Text + CRLF;
                }
                write(str);
                textBoxOut.Clear();
            }
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||
                (e.KeyChar >= 'a' && e.KeyChar <= 'z') ||
                e.KeyChar == '`' ||
                e.KeyChar == '_' ||
                e.KeyChar == '^')
            {
                Completion = NicknameComplete(
                        NicknameListBoxes[tabControlTabs.SelectedIndex],
                        textBoxOut.Text + e.KeyChar.ToString());
                CompletionIndex = 0;
            }
        }

        private string[] NicknameComplete(ListBox listBox, string nicknamePrefix)
        {
            string[] ret;
            string nickname;
            int i;
            int j;
            for (i = 0; i < listBox.Items.Count; i++)
            {
                nickname = listBox.Items[i] as string;
                if (nickname.StartsWith(nicknamePrefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
            }
            for (j = listBox.Items.Count - 1; j >= 0; j--)
            {
                nickname = listBox.Items[j] as string;
                if (nickname.StartsWith(nicknamePrefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
            }

            if (i > j)
            {
                ret = new string[0];
            }
            else
            {
                ret = new string[j - i + 1];
                int k = 0;
                while (k < ret.Length)
                {
                    ret[k] = listBox.Items[i] as string;
                    i++;
                    k++;
                }
            }
            return ret;
        }

        private void textBoxOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControlTabs.SelectedIndex == 0)//server tab
            {
                if (e.KeyData == Keys.F1)//identify nickname
                {
                    //ConnectRegister();
                    IdentifyNickname();
                    return;
                }
                if (e.KeyData == Keys.F2)//join favorite channel
                {
                    JoinFavoriteChannel();
                    return;
                }
            }

            if (e.KeyData == Keys.F3)//show history
            {
                var historyForm = new FormHistoryBrowser(DisplayTextBoxes[tabControlTabs.SelectedIndex].Text);
                historyForm.MdiParent = this.MdiParent;
                historyForm.Show();
            }
            else if (e.KeyData == Keys.Escape)
            {
                if (Completion == null)
                {
                    return;
                }
                if (CompletionIndex >= 0 && CompletionIndex < Completion.Length)
                {
                    textBoxOut.Clear();
                    textBoxOut.AppendText(Completion[CompletionIndex] + ": ");
                    textBoxOut.ScrollToCaret();
                    CompletionIndex++;
                    if (CompletionIndex >= Completion.Length)
                        CompletionIndex = 0;
                }
            }
        }

        private void IdentifyNickname()
        {
            if (config.password == null || config.password == "")
                return;
            write("PRIVMSG NickServ :identify " + config.password + CRLF);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (inputthread == null)
                return;
            if (inputthread.IsAlive)
            {
                inputthread.Abort();
                socket.Close();
            }
        }
        private void NewSize()
        {
            int width = this.Width-15;
            int height = this.Height-35;
            tabControlTabs.Width = width - 150;
            tabControlTabs.Height = height - textBoxOut.Height;
            textBoxOut.Top = tabControlTabs.Height;
            textBoxOut.Left = 0;
            textBoxOut.Width=width;
            listBoxNicknameList.Top = 0;
            listBoxNicknameList.Left = tabControlTabs.Width;
            listBoxNicknameList.Height = tabControlTabs.Height;
        }
        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            NewSize();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            NewSize();
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
        }

        private void tabControlTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox.ObjectCollection items = NicknameListBoxes[tabControlTabs.SelectedIndex].Items;
            listBoxNicknameList.Items.Clear();
            listBoxNicknameList.Items.AddRange(items);
        }

        private void copyCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayTextBoxes[tabControlTabs.SelectedIndex].Copy();
        }

        private void listBoxNicknameList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string selectedNick = listBoxNicknameList.SelectedItem as string;
            write("whois " + selectedNick + CRLF);
        }
    }
}
