using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LNC
{
    public partial class PrivateChatSession : Form
    {
        /// <summary>
        /// 对话的用户
        /// </summary>
        public User ToUser, ThisUser;
        private Form mainForm;
        private Action<int, int, string> onSendMessage;
        private Action<string, int> onSendFile;

        public PrivateChatSession(Form mainForm, User user, User thisUser, Action<int, int, string> onSendMessage, Action<string, int> onSendFile)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.onSendMessage = onSendMessage;
            if (onSendFile == null)
                sendFileButton.Enabled = false;
            else
                this.onSendFile = onSendFile;
            ToUser = user; ThisUser = thisUser;
            this.Text = $"{user.Name} ({user.UserName})";
            foreach (Chat chat in user.Messages)
                ReceiveChat(null, chat);
            mainForm.Invoke(new Action(() =>
            {
                foreach (Form frm in Application.OpenForms)
                    if (frm != this)
                        if (frm is PrivateChatSession)
                            if (((PrivateChatSession)frm).ToUser.Id == this.ToUser.Id)
                            { frm.Focus(); throw new Exception("已经打开了一个会话窗口"); }
            }));
            /*
             * Rtf可以传输文件，不需要加入文件传输功能
             */
            sendFileButton.Visible = false;
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            string msg = sendChatRichTextBox.Rtf.Trim();
            if (msg != "")
            {
                onSendMessage(ThisUser.Id, ToUser.Id, msg);
                Chat tmp = new Chat(0, ThisUser.Id, ToUser.Id, msg);
                tmp.User = ThisUser;
                ReceiveChat(null, tmp);
                sendChatRichTextBox.Rtf = "";
            }
        }

        public void ReceiveChat(object sender, Chat e)
        {
            if (e.UserId == ThisUser.Id && e.ToUserId == ToUser.Id || e.UserId == ToUser.Id && e.ToUserId == ThisUser.Id)
                mainForm.Invoke(new Action(() =>
                {
                    chatRichTextBox.SelectionLength = 0;
                    chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
                    chatRichTextBox.SelectionBackColor = Color.Empty;
                    chatRichTextBox.SelectionIndent = 0;
                    chatRichTextBox.SelectionHangingIndent = 0;
                    chatRichTextBox.SelectionFont = new Font("微软雅黑", 8);
                    chatRichTextBox.AppendText("\n");
                    if (e.UserId == ThisUser.Id)
                        chatRichTextBox.SelectionColor = Color.DarkGreen;
                    else chatRichTextBox.SelectionColor = Color.Blue;
                    chatRichTextBox.AppendText($"{e.User.Name} ({e.User.UserName})  {DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")}\n");
                    chatRichTextBox.SelectionColor = Color.Black;
                    chatRichTextBox.SelectionHangingIndent = 20;
                    chatRichTextBox.SelectionFont = new Font("微软雅黑", 10);
                    chatRichTextBox.AppendText(" ");
                    chatRichTextBox.SelectionStart--;
                    chatRichTextBox.SelectionLength = 1;
                    chatRichTextBox.SelectedRtf = e.Message;
                    chatRichTextBox.ScrollToCaret();
                }));
        }

        private void sendFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog tmp = new OpenFileDialog())
                if (tmp.ShowDialog() == DialogResult.OK)
                    onSendFile(tmp.FileName, ToUser.Id);
        }

        private void PrivateChatSession_Load(object sender, EventArgs e)
        {

        }

        public void UserLogout(object sender, User e)
        {
            if (e.Id == ToUser.Id) this.Close();
        }
    }
}
