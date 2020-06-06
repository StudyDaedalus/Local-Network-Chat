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

namespace LNC.Server
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示登陆框并返回用户
        /// </summary>
        /// <returns></returns>
        public new User ShowDialog()
        {
            if (base.ShowDialog() == DialogResult.OK)
            {
                textBoxUserName.Text = textBoxUserName.Text.Trim();
                if (textBoxPassword.Text == "" || textBoxUserName.Text == "") throw new Exception("请输入账号和密码");
                if (!User.CanLogin(textBoxUserName.Text.Replace("'", "''"), textBoxPassword.Text.Replace("'", "''"))) throw new Exception("账号或密码错误");
                return User.GetUserByUserNameFromDatabase(textBoxUserName.Text);
            }
            else return null;
        }
    }
}
