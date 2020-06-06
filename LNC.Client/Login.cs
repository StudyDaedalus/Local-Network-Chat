using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace LNC.Client
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private User user;

        /// <summary>
        /// 显示登陆框并返回用户
        /// </summary>
        /// <returns></returns>
        public new User ShowDialog()
        {
            new Thread(() =>
            {
                UdpClient client;
                try { client = new UdpClient(new IPEndPoint(IPAddress.Any, 16999)); } catch { return; }
                client.EnableBroadcast = true;
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
                byte[] receive;
                try { receive = client.Receive(ref ipep); } catch { try { client.Close(); } catch { } return; }
                try { client.Close(); } catch { }
                Newtonsoft.Json.Linq.JObject json;
                try { json = NetworkStreamProcessing.GetJObjectFromBase64Bytes(receive); }
                catch { return; }
                IPAddress ipaddr = null;
                int port = (int)json["port"];
                foreach (IPAddress tmp in Dns.GetHostAddresses((string)json["hostname"]))
                    try { TcpClient tmp1 = new TcpClient(); tmp1.Connect(tmp, port); tmp1.Close(); ipaddr = tmp; break; } catch { }
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        serverSettingIpAddressTextBox.Text = ipaddr.ToString();
                        serverSettingPortTextBox.Text = port.ToString();
                    }
                    catch { }
                }));
            })
            { IsBackground = true }.Start();
            if (base.ShowDialog() == DialogResult.OK)
            {
                return user;
            }
            else return null;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            textBoxUserName.Text = textBoxUserName.Text.Trim();
            serverSettingIpAddressTextBox.Text = serverSettingIpAddressTextBox.Text.Trim();
            serverSettingPortTextBox.Text = serverSettingPortTextBox.Text.Trim();
            try
            {
                if (serverSettingIpAddressTextBox.Text == "" || serverSettingPortTextBox.Text == "") throw new Exception("请输入服务器IP地址和端口");
                if (textBoxPassword.Text == "" || textBoxUserName.Text == "") throw new Exception("请输入账号和密码");
                if (!Client.IsInited)
                    Client.Init(new IPEndPoint(IPAddress.Parse(serverSettingIpAddressTextBox.Text), int.Parse(serverSettingPortTextBox.Text)));

                string username = textBoxUserName.Text, password = textBoxPassword.Text;
                new Thread(() =>
                {
                    try { Client.Login(username, password); } catch { this.Invoke(new Action(() => { this.Enabled = true; return; })); }
                    user = Client.User;
                    this.Invoke(new Action(() => { this.Enabled = true; }));
                    DialogResult = DialogResult.OK;
                })
                { IsBackground = true }.Start();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); this.Enabled = true; }
        }

        private void registerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (serverSettingIpAddressTextBox.Text == "" || serverSettingPortTextBox.Text == "") throw new Exception("请输入服务器IP地址和端口");
                if (!Client.IsInited)
                    Client.Init(new IPEndPoint(IPAddress.Parse(serverSettingIpAddressTextBox.Text), int.Parse(serverSettingPortTextBox.Text)));
                serverSettingIpAddressTextBox.Enabled = false;
                serverSettingPortTextBox.Enabled = false;
                new Register().ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); this.Enabled = true; }
        }
    }
}
