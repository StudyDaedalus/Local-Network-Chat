using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace LNC.Client
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            Client.Registered += Client_Registered;
        }

        private void Client_Registered(object sender, EventArgs e)
        {
            MessageBox.Show("注册成功！");
            this.Close();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxUserName.Text = textBoxUserName.Text.Trim();
                if (textBoxUserName.Text == "") throw new Exception("请输入用户名");
                if (textBoxPassword.Text == "") throw new Exception("请输入密码");
                if (textBoxVerbPasswrod.Text == "") throw new Exception("请输入确认密码");
                if (textBoxPassword.Text != textBoxVerbPasswrod.Text) throw new Exception("两次输入密码不一样");
                JObject json = JObject.Parse("{\"action\":\"register\",\"username\":\"\",\"password\":\"\"}");
                json["username"] = textBoxUserName.Text;
                json["password"] = textBoxPassword.Text;
                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(Client.Stream, json);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }

        private void Register_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.Registered -= Client_Registered;
        }
    }
}
