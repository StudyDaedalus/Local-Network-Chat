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
    public partial class VerifyUserForm : Form
    {
        public event EventHandler<User> InformationOpened;
        public event EventHandler<User> DenyUser;
        public event EventHandler<User> AcceptUser;

        private Func<User[]> GetNotVerifiedUsers;

        private User[] users;

        public VerifyUserForm(Func<User[]> GetNotVerifiedUsers)
        {
            this.GetNotVerifiedUsers = GetNotVerifiedUsers;
            InitializeComponent();
        }

        public VerifyUserForm(Func<User[]> GetNotVerifiedUsers, User[] users)
        {
            this.GetNotVerifiedUsers = GetNotVerifiedUsers;
            this.users = users;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
                if (treeView1.SelectedNode.Tag is User)
                    InformationOpened?.Invoke(null, (User)treeView1.SelectedNode.Tag);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            treeView1.Nodes.Clear();
            User[] tmp;
            if (users == null)
                tmp = User.GetNotVerifiedUsers();
            else tmp = users;
            foreach (User u in tmp)
                treeView1.Nodes.Add(new TreeNode(u.UserName) { Tag = u });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
                if (treeView1.SelectedNode.Tag is User)
                {
                    AcceptUser?.Invoke(null, (User)treeView1.SelectedNode.Tag);
                    linkLabel1_LinkClicked(null, null);
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
                if (treeView1.SelectedNode.Tag is User)
                {
                    DenyUser?.Invoke(null, (User)treeView1.SelectedNode.Tag);
                    linkLabel1_LinkClicked(null, null);
                }
        }

        private void VerifyUserForm_Load(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
                if (frm is VerifyUserForm)
                    if (frm != this)
                    {
                        frm.Focus();
                        this.Close();
                        return;
                    }
            linkLabel1_LinkClicked(null, null);
        }
    }
}
