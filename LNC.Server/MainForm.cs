using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace LNC.Server
{
    public partial class MainForm : Form
    {
        private User user;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Server.ReceiveChat += Server_ReceiveChat;
            Server.UserLogin += Server_UserLogin;
            Server.UserLogout += Server_UserLogout;
            Server.UserInformationChanged += Server_UserInformationChanged;
            Server.ReceiveOnlineUser += Server_ReceiveOnlineUser;
            try
            {
                //登陆
                user = new Login().ShowDialog();
                if (user == null) throw new Exception("noMessage");
                if (user.Level < 3) throw new Exception("对不起，只有系统管理员能登陆服务器");
            }
            catch (Exception ex)
            {
                if (!(ex.Message == "noMessage"))
                    MessageBox.Show(ex.Message);
                this.Close();
                return;
            }
            Random rand = new Random(DateTime.Now.GetHashCode());
            int port = rand.Next(16000, 17000);
            //寻找空闲端口
            while (true)
                try
                {
                    using (TcpClient tc = new TcpClient())
                    {
                        tc.Connect(Dns.GetHostName(), port);
                        tc.Close();
                        port = rand.Next(16000, 17000);
                    }
                }
                catch { break; }
            try { Server.Init(new ServerSetting().ShowDialog(port), user); } catch { this.Close(); }
            IPAddress ipAddr = Server.OpenedEndPoint.Address;
            if (ipAddr.ToString() == IPAddress.Any.ToString())
                this.Text = $"企业办公系统（Server端） 在所有IPv4网络接口的端口{Server.OpenedEndPoint.Port}上开放";
            else if (ipAddr.ToString() == IPAddress.IPv6Any.ToString())
                this.Text = $"企业办公系统（Server端） 在所有IPv6网络接口的端口{Server.OpenedEndPoint.Port}上开放";
            else
                this.Text = $"企业办公系统（Server端） 在{ipAddr}:{Server.OpenedEndPoint.Port}上开放";
        }

        private void Server_UserInformationChanged(object sender, User e)
        {
            Server_UserLogout(sender, e);
        }

        private void Server_ReceiveOnlineUser(object sender, User[] e)
        {
            onlineUserTreeView.Nodes["normalUserNode"].Nodes.Clear();
            onlineUserTreeView.Nodes["administratorNode"].Nodes.Clear();
            int tmp = 0;
            foreach (User u in e)
            {
                if (u.Level == 1)
                    onlineUserTreeView.Nodes["normalUserNode"].Nodes.Add(new TreeNode($"{u.Name}  ({u.UserName})") { Tag = u });
                else if (u.Level >= 2)
                    onlineUserTreeView.Nodes["administratorNode"].Nodes.Add(new TreeNode($"{u.Name}  ({u.UserName})") { Tag = u });
                else tmp++;
            }
            if (tmp == 0)
                onlineCountLabel.Text = $"{e.Length}人在线";
            else onlineCountLabel.Text = $"{e.Length}人在线 {tmp}人待审核";
            onlineUserTreeView.Nodes["administratorNode"].Text = $"管理员 ({onlineUserTreeView.Nodes["administratorNode"].Nodes.Count}人)";
            onlineUserTreeView.Nodes["normalUserNode"].Text = $"用户 ({onlineUserTreeView.Nodes["normalUserNode"].Nodes.Count}人)";
        }

        private void Server_UserLogout(object sender, User e)
        {
            List<User> tmp = new List<User>();
            tmp.Add(Server.User);
            foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp in Server.LoginedUser)
                tmp.Add(kvp.Value.Key);
            Server_ReceiveOnlineUser(sender, tmp.ToArray());
        }

        private void Server_UserLogin(object sender, User e)
        {
            Server_UserLogout(sender, e);
        }

        private void Server_ReceiveChat(object sender, Chat e)
        {
            if (e.ToUserId == -1)
                chatRichTextBox.Invoke(new Action(() =>
                {
                    chatRichTextBox.SelectionLength = 0;
                    chatRichTextBox.ReadOnly = false;
                    chatRichTextBox.SelectionStart = chatRichTextBox.Text.Length;
                    chatRichTextBox.SelectionBackColor = Color.Empty;
                    chatRichTextBox.SelectionIndent = 0;
                    chatRichTextBox.SelectionHangingIndent = 0;
                    chatRichTextBox.SelectionFont = new Font("微软雅黑", 8);
                    chatRichTextBox.AppendText("\n");
                    if (e.UserId == Server.User.Id)
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
                    chatRichTextBox.ReadOnly = true;
                    chatRichTextBox.ScrollToCaret();
                }));
            else if (e.ToUserId == Server.User.Id)
            {
                foreach (Form frm in Application.OpenForms)
                    if (frm != this)
                        if (frm is PrivateChatSession)
                            if (((PrivateChatSession)frm).ToUser.Id == e.User.Id)
                            { frm.Focus(); return; }
                PrivateChatSession tmp;
                try
                {
                    tmp = new PrivateChatSession(this, e.User, Server.User, (from, to, msg) =>
                    {
                        Chat.CreateChatAndSendToDatabase(from, to, msg).SendChatToStream(Server.LoginedUser[to].Value.GetStream());
                    }, null);
                }
                catch { return; }
                Server.ReceiveChat += tmp.ReceiveChat;
                Server.UserLogout += tmp.UserLogout;
                tmp.FormClosed += (a, b) =>
                {
                    Server.ReceiveChat -= tmp.ReceiveChat;
                    Server.UserLogout -= tmp.UserLogout;
                };
                tmp.Show();
            }
        }

        private void personalInfomationButton_Click(object sender, EventArgs e)
        {
            new System.Threading.Thread(() => { try { new PersonalInfomation(user, user).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }).Start();
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            string msg = sendChatRichTextBox.Rtf.Trim();
            /*string tmp = sendChatRichTextBox.Text.Trim();
            if (tmp != "")
            {*/
            Server.SendGobalChat(msg);
            sendChatRichTextBox.Rtf = "";
            //}
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void 查看资料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (onlineUserTreeView.SelectedNode != null)
                if (onlineUserTreeView.SelectedNode.Tag is User)
                {
                    User tmp = (User)onlineUserTreeView.SelectedNode.Tag;
                    new System.Threading.Thread(() => { try { new PersonalInfomation(tmp, Server.User).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }).Start();
                }
        }

        private void 发起会话ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (onlineUserTreeView.SelectedNode != null)
                if (onlineUserTreeView.SelectedNode.Tag is User)
                {
                    if (((User)onlineUserTreeView.SelectedNode.Tag).Id == Server.User.Id) return;
                    PrivateChatSession tmp;
                    try
                    {
                        tmp = new PrivateChatSession(this, (User)onlineUserTreeView.SelectedNode.Tag, Server.User, (from, to, msg) =>
                        {
                            Chat.CreateChatAndSendToDatabase(from, to, msg).SendChatToStream(Server.LoginedUser[to].Value.GetStream());
                        }, null);
                    }
                    catch { return; }
                    Server.ReceiveChat += tmp.ReceiveChat;
                    Server.UserLogout += tmp.UserLogout;
                    tmp.FormClosed += (a, b) =>
                    {
                        Server.ReceiveChat -= tmp.ReceiveChat;
                        Server.UserLogout -= tmp.UserLogout;
                    };
                    tmp.Show();
                }
        }

        private void verifyUserButton_Click(object sender, EventArgs e)
        {
            VerifyUserForm tmp = new VerifyUserForm(User.GetNotVerifiedUsers);
            tmp.AcceptUser += (a, b) =>
            {
                b.ChangeInformationAndPushToDatabase(b.Name, b.Age, b.Birth, b.Telephone, b.Pay, b.Unit, b.Number, 1, b.Banned);
            };
            tmp.DenyUser += (a, b) =>
            {
                b.ChangeInformationAndPushToDatabase(b.Name, b.Age, b.Birth, b.Telephone, b.Pay, b.Unit, b.Number, -1, b.Banned);
            };
            tmp.InformationOpened += (a, b) =>
            {
                new System.Threading.Thread(() => { try { new PersonalInfomation(b, user).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }).Start();
            };
            tmp.Show();
        }

        private void userInformationButton_Click(object sender, EventArgs e)
        {
            new System.Threading.Thread(() =>
            {
                ChangeAllUsersInformationForm cauif = null;
                (cauif = new ChangeAllUsersInformationForm(Server.User, User.GetUserFromDatabase(), (a) => { try { new PersonalInfomation(a, Server.User).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } cauif.Refresh(); })).ShowDialog();
            })
            { IsBackground = true }.Start();
        }
    }
}
