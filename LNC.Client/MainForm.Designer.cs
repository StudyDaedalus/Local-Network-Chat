namespace LNC.Client
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("管理员 ({0}人)");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("用户 ({0}人)");
            this.chatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.sendChatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.onlineCountLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.发起会话ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看资料ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onlineUserTreeView = new System.Windows.Forms.TreeView();
            this.verifyUserButton = new System.Windows.Forms.Button();
            this.personalInfomationButton = new System.Windows.Forms.Button();
            this.userInformationButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chatRichTextBox
            // 
            this.chatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatRichTextBox.BackColor = System.Drawing.Color.White;
            this.chatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.chatRichTextBox.Name = "chatRichTextBox";
            this.chatRichTextBox.ReadOnly = true;
            this.chatRichTextBox.Size = new System.Drawing.Size(676, 455);
            this.chatRichTextBox.TabIndex = 0;
            this.chatRichTextBox.Text = "";
            // 
            // sendChatRichTextBox
            // 
            this.sendChatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendChatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sendChatRichTextBox.Location = new System.Drawing.Point(0, 481);
            this.sendChatRichTextBox.Name = "sendChatRichTextBox";
            this.sendChatRichTextBox.Size = new System.Drawing.Size(676, 122);
            this.sendChatRichTextBox.TabIndex = 1;
            this.sendChatRichTextBox.Text = "";
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendMessageButton.Location = new System.Drawing.Point(633, 454);
            this.sendMessageButton.Margin = new System.Windows.Forms.Padding(0);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(43, 27);
            this.sendMessageButton.TabIndex = 0;
            this.sendMessageButton.Text = "发送";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.sendMessageButton_Click);
            // 
            // onlineCountLabel
            // 
            this.onlineCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.onlineCountLabel.AutoSize = true;
            this.onlineCountLabel.Location = new System.Drawing.Point(682, 4);
            this.onlineCountLabel.Name = "onlineCountLabel";
            this.onlineCountLabel.Size = new System.Drawing.Size(64, 19);
            this.onlineCountLabel.TabIndex = 3;
            this.onlineCountLabel.Text = "{0}人在线";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.发起会话ToolStripMenuItem,
            this.查看资料ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // 发起会话ToolStripMenuItem
            // 
            this.发起会话ToolStripMenuItem.Name = "发起会话ToolStripMenuItem";
            this.发起会话ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.发起会话ToolStripMenuItem.Text = "发起会话";
            this.发起会话ToolStripMenuItem.Click += new System.EventHandler(this.发起会话ToolStripMenuItem_Click);
            // 
            // 查看资料ToolStripMenuItem
            // 
            this.查看资料ToolStripMenuItem.Name = "查看资料ToolStripMenuItem";
            this.查看资料ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.查看资料ToolStripMenuItem.Text = "查看资料";
            this.查看资料ToolStripMenuItem.Click += new System.EventHandler(this.查看资料ToolStripMenuItem_Click);
            // 
            // onlineUserTreeView
            // 
            this.onlineUserTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.onlineUserTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.onlineUserTreeView.Location = new System.Drawing.Point(686, 26);
            this.onlineUserTreeView.Name = "onlineUserTreeView";
            treeNode1.Name = "administratorNode";
            treeNode1.Text = "管理员 ({0}人)";
            treeNode2.Name = "normalUserNode";
            treeNode2.Text = "用户 ({0}人)";
            this.onlineUserTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.onlineUserTreeView.Size = new System.Drawing.Size(210, 536);
            this.onlineUserTreeView.TabIndex = 5;
            // 
            // verifyUserButton
            // 
            this.verifyUserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.verifyUserButton.Location = new System.Drawing.Point(828, 568);
            this.verifyUserButton.Name = "verifyUserButton";
            this.verifyUserButton.Size = new System.Drawing.Size(69, 28);
            this.verifyUserButton.TabIndex = 6;
            this.verifyUserButton.Text = "人员审核";
            this.verifyUserButton.UseVisualStyleBackColor = true;
            this.verifyUserButton.Click += new System.EventHandler(this.verifyUserButton_Click);
            // 
            // personalInfomationButton
            // 
            this.personalInfomationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.personalInfomationButton.Location = new System.Drawing.Point(686, 568);
            this.personalInfomationButton.Name = "personalInfomationButton";
            this.personalInfomationButton.Size = new System.Drawing.Size(69, 28);
            this.personalInfomationButton.TabIndex = 6;
            this.personalInfomationButton.Text = "个人资料";
            this.personalInfomationButton.UseVisualStyleBackColor = true;
            this.personalInfomationButton.Click += new System.EventHandler(this.personalInfomationButton_Click);
            // 
            // userInformationButton
            // 
            this.userInformationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.userInformationButton.Location = new System.Drawing.Point(757, 568);
            this.userInformationButton.Name = "userInformationButton";
            this.userInformationButton.Size = new System.Drawing.Size(69, 28);
            this.userInformationButton.TabIndex = 7;
            this.userInformationButton.Text = "人员管理";
            this.userInformationButton.UseVisualStyleBackColor = true;
            this.userInformationButton.Click += new System.EventHandler(this.userInformationButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 604);
            this.Controls.Add(this.userInformationButton);
            this.Controls.Add(this.verifyUserButton);
            this.Controls.Add(this.personalInfomationButton);
            this.Controls.Add(this.onlineUserTreeView);
            this.Controls.Add(this.onlineCountLabel);
            this.Controls.Add(this.sendMessageButton);
            this.Controls.Add(this.sendChatRichTextBox);
            this.Controls.Add(this.chatRichTextBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(680, 450);
            this.Name = "MainForm";
            this.Text = "企业办公系统（Client端）";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox chatRichTextBox;
        private System.Windows.Forms.RichTextBox sendChatRichTextBox;
        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.Label onlineCountLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 发起会话ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看资料ToolStripMenuItem;
        private System.Windows.Forms.TreeView onlineUserTreeView;
        private System.Windows.Forms.Button verifyUserButton;
        private System.Windows.Forms.Button personalInfomationButton;
        private System.Windows.Forms.Button userInformationButton;
    }
}

