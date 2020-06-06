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
    public partial class ChangeAllUsersInformationForm : Form
    {
        User[] users;
        User thisUser;
        Action<User> onChangeUser;
        public ChangeAllUsersInformationForm(User thisUser, User[] users, Action<User> onChangeUser)
        {
            InitializeComponent();
            this.users = users;
            this.thisUser = thisUser;
            this.onChangeUser = onChangeUser;
        }
        public void RefreshList()
        {
            userListView.Items.Clear();
            foreach (User u in users)
                userListView.Items.Add(new ListViewItem(new string[] { u.Id.ToString(), u.UserName, u.Name, u.Age == null ? "空" : u.Age.ToString(), u.Unit == null ? "空" : u.Unit, u.Pay == null ? "空" : u.Pay.ToString(), u.Level.ToString() }) { Tag = u });
        }
        private void userListView_DoubleClick(object sender, EventArgs e)
        {
            if (userListView.SelectedItems.Count == 1)
                if (userListView.SelectedItems[0].Tag is User)
                    onChangeUser((User)userListView.SelectedItems[0].Tag);
        }

        private void ChangeAllUsersInformationForm_Load(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
                if (frm is ChangeAllUsersInformationForm)
                    if (frm != this)
                    {
                        frm.Focus();
                        this.Close();
                        return;
                    }
            RefreshList();
        }
    }
}
