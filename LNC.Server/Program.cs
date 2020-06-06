using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace LNC.Server
{
    static class Program
    {
        public static MainForm mainForm;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //打开数据库连接
            try { Database.Open(); } catch (Exception e) { MessageBox.Show(e.Message, "无法连接到数据库", MessageBoxButtons.OK); return; }
            mainForm = new MainForm();
            Application.Run(mainForm);
            //关闭数据库连接
            Database.Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
