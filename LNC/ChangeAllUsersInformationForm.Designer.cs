namespace LNC
{
    partial class ChangeAllUsersInformationForm
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
            this.userListView = new System.Windows.Forms.ListView();
            this.idColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.usernameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ageColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.unitColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.payColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.levelColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // userListView
            // 
            this.userListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idColumnHeader,
            this.usernameColumnHeader,
            this.nameColumnHeader,
            this.ageColumnHeader,
            this.unitColumnHeader,
            this.payColumnHeader,
            this.levelColumnHeader});
            this.userListView.Location = new System.Drawing.Point(12, 12);
            this.userListView.MultiSelect = false;
            this.userListView.Name = "userListView";
            this.userListView.Size = new System.Drawing.Size(524, 339);
            this.userListView.TabIndex = 0;
            this.userListView.UseCompatibleStateImageBehavior = false;
            this.userListView.View = System.Windows.Forms.View.Details;
            this.userListView.DoubleClick += new System.EventHandler(this.userListView_DoubleClick);
            // 
            // idColumnHeader
            // 
            this.idColumnHeader.Text = "Id";
            this.idColumnHeader.Width = 30;
            // 
            // usernameColumnHeader
            // 
            this.usernameColumnHeader.Text = "用户名";
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "姓名";
            // 
            // ageColumnHeader
            // 
            this.ageColumnHeader.Text = "年龄";
            this.ageColumnHeader.Width = 40;
            // 
            // unitColumnHeader
            // 
            this.unitColumnHeader.Text = "单位";
            // 
            // payColumnHeader
            // 
            this.payColumnHeader.Text = "薪资";
            // 
            // levelColumnHeader
            // 
            this.levelColumnHeader.Text = "权限";
            this.levelColumnHeader.Width = 46;
            // 
            // ChangeAllUsersInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 363);
            this.Controls.Add(this.userListView);
            this.Name = "ChangeAllUsersInformationForm";
            this.Text = "人员管理";
            this.Load += new System.EventHandler(this.ChangeAllUsersInformationForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView userListView;
        private System.Windows.Forms.ColumnHeader idColumnHeader;
        private System.Windows.Forms.ColumnHeader usernameColumnHeader;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader ageColumnHeader;
        private System.Windows.Forms.ColumnHeader unitColumnHeader;
        private System.Windows.Forms.ColumnHeader payColumnHeader;
        private System.Windows.Forms.ColumnHeader levelColumnHeader;
    }
}