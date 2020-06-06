using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace LNC.Server
{
    public partial class ServerSetting : Form
    {
        public ServerSetting()
        {
            InitializeComponent();
        }

        public IPEndPoint ShowDialog(int defaultPort)
        {
            this.serverSettingPortTextBox.Text = defaultPort.ToString();
            if (base.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    IPAddress ipAddr = IPAddress.Parse(serverSettingIpAddressTextBox.Text);
                    return new IPEndPoint(ipAddr, int.Parse(serverSettingPortTextBox.Text));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            return null;
        }

        private void ServerSetting_Load(object sender, EventArgs e)
        {

        }

        private void serverSettingStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress ipAddr = IPAddress.Parse(serverSettingIpAddressTextBox.Text);
                if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && !IPAddress.IsLoopback(ipAddr))
                    if (MessageBox.Show("IPv6不支持广播，您可能需要将IP地址告诉他人才能连接\n是否继续", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        this.DialogResult = DialogResult.OK;
                    else return;
                else this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
