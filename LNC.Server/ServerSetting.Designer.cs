namespace LNC.Server
{
    partial class ServerSetting
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
            this.label2 = new System.Windows.Forms.Label();
            this.serverSettingPortTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serverSettingIpAddressTextBox = new System.Windows.Forms.TextBox();
            this.serverSettingStartButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "端口";
            // 
            // serverSettingPortTextBox
            // 
            this.serverSettingPortTextBox.Location = new System.Drawing.Point(78, 50);
            this.serverSettingPortTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.serverSettingPortTextBox.Name = "serverSettingPortTextBox";
            this.serverSettingPortTextBox.Size = new System.Drawing.Size(125, 25);
            this.serverSettingPortTextBox.TabIndex = 4;
            this.serverSettingPortTextBox.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "IP地址";
            // 
            // serverSettingIpAddressTextBox
            // 
            this.serverSettingIpAddressTextBox.Location = new System.Drawing.Point(78, 19);
            this.serverSettingIpAddressTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.serverSettingIpAddressTextBox.Name = "serverSettingIpAddressTextBox";
            this.serverSettingIpAddressTextBox.Size = new System.Drawing.Size(125, 25);
            this.serverSettingIpAddressTextBox.TabIndex = 5;
            this.serverSettingIpAddressTextBox.Text = "0.0.0.0";
            // 
            // serverSettingStartButton
            // 
            this.serverSettingStartButton.Location = new System.Drawing.Point(94, 87);
            this.serverSettingStartButton.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.serverSettingStartButton.Name = "serverSettingStartButton";
            this.serverSettingStartButton.Size = new System.Drawing.Size(48, 28);
            this.serverSettingStartButton.TabIndex = 3;
            this.serverSettingStartButton.Text = "启动";
            this.serverSettingStartButton.UseVisualStyleBackColor = true;
            this.serverSettingStartButton.Click += new System.EventHandler(this.serverSettingStartButton_Click);
            // 
            // ServerSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 127);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverSettingPortTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverSettingIpAddressTextBox);
            this.Controls.Add(this.serverSettingStartButton);
            this.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerSetting";
            this.Text = "启动服务器";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ServerSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverSettingPortTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverSettingIpAddressTextBox;
        private System.Windows.Forms.Button serverSettingStartButton;
    }
}