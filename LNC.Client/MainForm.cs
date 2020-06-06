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
using System.IO;

namespace LNC.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            Client.ReceiveChat += Client_ReceiveChat;
            Client.UserLogin += Client_UserLogin;
            Client.UserLogout += Client_UserLogout;
            Client.UserInformationChanged += Client_UserInformationChanged;
            Client.ReceiveOnlineUser += Client_ReceiveOnlineUser;
            Client.Disconnected += (a, b) => { this.Invoke(new Action(() => { this.Close(); })); };
            try
            {
                //登陆
                User user = new Login().ShowDialog();
                if (user == null) throw new Exception("noMessage");
                if (user.Level < 2) { userInformationButton.Visible = false; verifyUserButton.Visible = false; }
            }
            catch (Exception ex)
            {
                if (!(ex.Message == "noMessage"))
                    MessageBox.Show(ex.Message);
                this.Close();
                return;
            }
        }

        private void Client_UserInformationChanged(object sender, User e)
        {
            Client.LoginedUser[e.Id].ChangeInformation(e);
            Client_UserLogout(sender, e);
        }

        private void Client_UserLogout(object sender, User e)
        {
            List<User> tmp = new List<User>();
            foreach (KeyValuePair<int, User> kvp in Client.LoginedUser)
                tmp.Add(kvp.Value);
            Client_ReceiveOnlineUser(sender, tmp.ToArray());
        }

        private void Client_UserLogin(object sender, User e)
        {
            Client_UserLogout(sender, e);
        }

        private void Client_ReceiveOnlineUser(object sender, User[] e)
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

        private void Client_ReceiveChat(object sender, Chat e)
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
                    if (e.UserId == Client.User.Id)
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
            else if (e.ToUserId == Client.User.Id)
            {
                foreach (Form frm in Application.OpenForms)
                    if (frm != this)
                        if (frm is PrivateChatSession)
                            if (((PrivateChatSession)frm).ToUser.Id == e.User.Id)
                            { frm.Focus(); return; }
                PrivateChatSession tmp;
                try
                {
                    tmp = new PrivateChatSession(this, e.User, Client.User, (from, to, msg) =>
                    {
                        Chat.SendChatToStream(from, to, msg, Client.Stream);
                    }, (filename, to) =>
                    {
                        int hash;
                        try { hash = File.ReadAllBytes(filename).GetHashCode(); } catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                        try { var tmp1 = Client.SendFilePool[hash]; MessageBox.Show("正在发送此文件"); return; }
                        catch
                        {
                            Client.SendFilePool.Add(hash, filename);
                            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse($"{{\"action\":\"file\",\"hash\":{hash},\"filename\":\"\",\"toUser\":{to}}}");
                            json["filename"] = filename.Substring(filename.LastIndexOf('\\') + 1);
                            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(Client.Stream, json);
                        }
                    });
                }
                catch { return; }
                Client.ReceiveChat += tmp.ReceiveChat;
                Client.UserLogout += tmp.UserLogout;
                tmp.FormClosed += (a, b) =>
                {
                    Client.ReceiveChat -= tmp.ReceiveChat;
                    Client.UserLogout -= tmp.UserLogout;
                };
                tmp.Show();
            }
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            string msg = sendChatRichTextBox.Rtf.Trim();
            /*if (msg != "")
            {*/
            Chat.SendChatToStream(Client.User.Id, -1, msg, Client.Stream);
            sendChatRichTextBox.Rtf = "";
            //}
        }

        private void personalInfomationButton_Click(object sender, EventArgs e)
        {
            new System.Threading.Thread(() => { try { new PersonalInfomation(Client.User, Client.User).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }).Start();
        }

        private void 查看资料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (onlineUserTreeView.SelectedNode != null)
                if (onlineUserTreeView.SelectedNode.Tag is User)
                {
                    User tmp = (User)onlineUserTreeView.SelectedNode.Tag;
                    new System.Threading.Thread(() => { try { new PersonalInfomation(tmp, Client.User).ShowDialog(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }).Start();
                }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void 发起会话ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (onlineUserTreeView.SelectedNode != null)
                if (onlineUserTreeView.SelectedNode.Tag is User)
                {
                    if (((User)onlineUserTreeView.SelectedNode.Tag).Id == Client.User.Id) return;
                    PrivateChatSession tmp;
                    try
                    {
                        tmp = new PrivateChatSession(this, (User)onlineUserTreeView.SelectedNode.Tag, Client.User, (from, to, msg) =>
                        {
                            Chat.SendChatToStream(from, to, msg, Client.Stream);
                        }, (filename, to) =>
                        {
                            int hash;
                            try { hash = File.ReadAllBytes(filename).GetHashCode(); } catch (Exception ex) { MessageBox.Show(ex.Message); return; }
                            try { var tmp1 = Client.SendFilePool[hash]; MessageBox.Show("正在发送此文件"); return; }
                            catch
                            {
                                Client.SendFilePool.Add(hash, filename);
                                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse($"{{\"action\":\"file\",\"hash\":{hash},\"filename\":\"\",\"toUser\":{to}}}");
                                json["filename"] = filename.Substring(filename.LastIndexOf('\\') + 1);
                                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(Client.Stream, json);
                            }
                        });
                    }
                    catch { return; }
                    Client.ReceiveChat += tmp.ReceiveChat;
                    Client.UserLogout += tmp.UserLogout;
                    tmp.FormClosed += (a, b) =>
                    {
                        Client.ReceiveChat -= tmp.ReceiveChat;
                        Client.UserLogout -= tmp.UserLogout;
                    };
                    tmp.Show();
                }
        }

        private void verifyUserButton_Click(object sender, EventArgs e)
        {
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(Client.Stream, Newtonsoft.Json.Linq.JObject.Parse("{\"action\":\"getNotVerifiedUsers\"}"));
        }

        private void userInformationButton_Click(object sender, EventArgs e)
        {
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(Client.Stream, Newtonsoft.Json.Linq.JObject.Parse("{\"action\":\"getAllUserFromDatabase\"}"));
        }
    }
}
