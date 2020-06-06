namespace LNC
{
    partial class PrivateChatSession
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.sendChatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.chatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.sendFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendMessageButton.Location = new System.Drawing.Point(640, 328);
            this.sendMessageButton.Margin = new System.Windows.Forms.Padding(0);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(43, 27);
            this.sendMessageButton.TabIndex = 5;
            this.sendMessageButton.Text = "发送";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.sendMessageButton_Click);
            // 
            // sendChatRichTextBox
            // 
            this.sendChatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendChatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sendChatRichTextBox.Location = new System.Drawing.Point(1, 355);
            this.sendChatRichTextBox.Name = "sendChatRichTextBox";
            this.sendChatRichTextBox.Size = new System.Drawing.Size(682, 122);
            this.sendChatRichTextBox.TabIndex = 0;
            this.sendChatRichTextBox.Text = "";
            // 
            // chatRichTextBox
            // 
            this.chatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatRichTextBox.Location = new System.Drawing.Point(1, 1);
            this.chatRichTextBox.Name = "chatRichTextBox";
            this.chatRichTextBox.Size = new System.Drawing.Size(682, 327);
            this.chatRichTextBox.TabIndex = 6;
            this.chatRichTextBox.Text = "";
            // 
            // sendFileButton
            // 
            this.sendFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendFileButton.Location = new System.Drawing.Point(1, 328);
            this.sendFileButton.Margin = new System.Windows.Forms.Padding(0);
            this.sendFileButton.Name = "sendFileButton";
            this.sendFileButton.Size = new System.Drawing.Size(69, 27);
            this.sendFileButton.TabIndex = 5;
            this.sendFileButton.Text = "发送文件";
            this.sendFileButton.UseVisualStyleBackColor = true;
            this.sendFileButton.Click += new System.EventHandler(this.sendFileButton_Click);
            // 
            // PrivateChatSession
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 477);
            this.Controls.Add(this.sendFileButton);
            this.Controls.Add(this.sendMessageButton);
            this.Controls.Add(this.sendChatRichTextBox);
            this.Controls.Add(this.chatRichTextBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(460, 450);
            this.Name = "PrivateChatSession";
            this.Text = "{name} ({username})";
            this.Load += new System.EventHandler(this.PrivateChatSession_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.RichTextBox sendChatRichTextBox;
        private System.Windows.Forms.RichTextBox chatRichTextBox;
        private System.Windows.Forms.Button sendFileButton;
    }
}