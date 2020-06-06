using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LNC.Client
{
    public partial class PersonalInfomation : Form
    {
        private bool loading = true;
        public int viewId;
        private int editLevel = 0, level = 0;
        private User user;
        public PersonalInfomation(User user, User editUser)
        {
            foreach (Form frm in Application.OpenForms)
                if (frm is PersonalInfomation)
                    if (((PersonalInfomation)frm).viewId == user.Id)
                        throw new Exception("已经打开了同样的窗口！");
            this.user = user;
            viewId = user.Id;
            InitializeComponent();
            nameTextBox.Text = user.Name;
            ageTextBox.Text = user.Age.ToString();
            if (user.Pay != null)
                payTextBox.Text = ((int)user.Pay).ToString();
            unitTextBox.Text = user.Unit;
            if (user.Number != null)
                numberTextBox.Text = ((int)user.Number).ToString();
            usernameTextBox.Text = user.UserName;
            string tmp = "未知";
            level = user.Level;
            switch (level)
            {
                case 0:
                    tmp = "待审核";
                    break;
                case 1:
                    tmp = "普通用户";
                    break;
                case 2:
                    tmp = "管理员";
                    break;
                case 3:
                    tmp = "系统管理员";
                    break;
            }
            levelTextBox.Text = tmp;
            if (user.Birth != null)
                birthTextBox.Text = ((DateTime)user.Birth).ToString("yyyy-MM-dd");
            telephoneTextBox.Text = user.Telephone;
            if (user.Id != editUser.Id)
            {
                if (editUser.Level < 2) editLevel = 0;
                else if (user.Level < 2 && editUser.Level >= 2) editLevel = editUser.Level;
                else if (editUser.Level == 3) editLevel = 3;
            }
            else
            {
                if (editUser.Level < 2) editLevel = editUser.Level;
                else if (editUser.Level == 2) editLevel = 1;
                else if (editUser.Level == 3) editLevel = 3;
            }
            switch (editLevel)
            {
                case 0:
                    nameTextBox.ReadOnly = ageTextBox.ReadOnly = birthTextBox.ReadOnly = telephoneTextBox.ReadOnly = true;
                    break;
                case 1:
                    break;
                case 2:
                    unitTextBox.ReadOnly = false;
                    break;
                case 3:
                    payTextBox.ReadOnly = unitTextBox.ReadOnly = numberTextBox.ReadOnly = false;
                    break;
            }
        }
        private bool changed = false;
        private void informationChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                changed = true;
                this.Text = "查看资料 - 已更改";
            }
        }
        public bool canclose = false;
        private void PersonalInfomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!changed || canclose) return;
            if (MessageBox.Show("资料已更改，是否保存？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            e.Cancel = true;
            this.Enabled = false;
            try
            {
                if (string.IsNullOrEmpty(nameTextBox.Text)) nameTextBox.Text = user.Name;
                int? age = null;
                if (!string.IsNullOrEmpty(ageTextBox.Text)) age = int.Parse(ageTextBox.Text);
                DateTime? birth = null;
                if (!string.IsNullOrEmpty(birthTextBox.Text)) birth = DateTime.Parse(birthTextBox.Text);
                decimal? pay = null;
                if (!string.IsNullOrEmpty(payTextBox.Text)) pay = decimal.Parse(payTextBox.Text);
                int? number = null;
                if (!string.IsNullOrEmpty(numberTextBox.Text)) number = int.Parse(numberTextBox.Text);
                user.ChangeInformationAndPushToServer(nameTextBox.Text, age, birth, telephoneTextBox.Text, pay, unitTextBox.Text, number, level, user.Banned, Client.Stream);
                this.Enabled = true;
                this.Text = "查看资料";
                canclose = true;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); this.Enabled = true; }
        }

        private void PersonalInfomation_Load(object sender, EventArgs e)
        {
            loading = false;
        }

        private void levelTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (editLevel == 3)
                switch (e.KeyCode)
                {
                    case Keys.NumPad0:
                        level = 0;
                        break;
                    case Keys.NumPad1:
                        level = 1;
                        break;
                    case Keys.NumPad2:
                        level = 2;
                        break;
                    case Keys.NumPad3:
                        level = 3;
                        break;
                    default:
                        level = -1;
                        break;
                }
            string tmp = "未知";
            switch (level)
            {
                case 0:
                    tmp = "待审核";
                    break;
                case 1:
                    tmp = "普通用户";
                    break;
                case 2:
                    tmp = "管理员";
                    break;
                case 3:
                    tmp = "系统管理员";
                    break;
            }
            levelTextBox.Text = tmp;
        }
    }
}
