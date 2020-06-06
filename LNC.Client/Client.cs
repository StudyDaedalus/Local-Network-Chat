using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LNC.Client
{
    static class Client
    {
        private static TcpClient client = new TcpClient();
        private static IPAddress serverIP;
        private static string lastError = null;

        /// <summary>
        /// 网络流
        /// </summary>
        public static NetworkStream Stream { get { return client.GetStream(); } }
        /// <summary>
        /// 用户
        /// </summary>
        public static User User { get; private set; }
        /// <summary>
        /// 是否登陆
        /// </summary>
        public static bool IsLogined { get { return User != null; } }
        /// <summary>
        /// 是否初始化
        /// </summary>
        public static bool IsInited { get { return client.Connected; } }
        private static List<Chat> _chatMessages = new List<Chat>();
        /// <summary>
        /// 聊天记录
        /// </summary>
        public static Chat[] ChatMessages { get { return _chatMessages.ToArray(); } }
        /// <summary>
        /// 已经登陆的用户
        /// </summary>
        public static Dictionary<int, User> LoginedUser { get; } = new Dictionary<int, User>();
        /// <summary>
        /// 文件发送池（哈希值，文件名）
        /// </summary>
        public static Dictionary<int, string> SendFilePool { get; } = new Dictionary<int, string>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ipep"></param>
        public static void Init(IPEndPoint ipep)
        {
            if (ipep.Address.ToString() == IPAddress.Any.ToString())
                foreach (IPAddress ipaddr in Dns.GetHostAddresses(Dns.GetHostName()))
                    try
                    {
                        client.Connect(new IPEndPoint(ipaddr, ipep.Port));
                        serverIP = ipaddr;
                        break;
                    }
                    catch { }
            if (!client.Connected) client.Connect(ipep);
            serverIP = ipep.Address;
            new Thread(ProcessTcpClient) { IsBackground = true }.Start();
        }

        /// <summary>
        /// 处理客户端的方法
        /// </summary>
        public static void ProcessTcpClient()
        {
            using (NetworkStream ns = client.GetStream())
            {
                string receiveError = "";
                while (client.Connected)
                {
                    JObject json;
                    try { json = NetworkStreamProcessing.GetJObjectFromBase64Bytes(NetworkStreamProcessing.ReadDataFromNetworkStream(ns)); } catch (Exception e) { receiveError = e.Message; break; }
                    if ((string)json["application"] != "LNC") client.Close();
                    try
                    {
                        if ((bool)json["error"] == true)
                        {
                            #region error
                            string action = (string)json["action"];
                            string errorMessage = (string)json["errorMessage"];
                            string errorType = (string)json["errorType"];
                            new Thread(() =>
                            {
                                System.Windows.Forms.MessageBox.Show($"详细信息如下：\n\n执行的动作: {action}\n\n错误类型:{errorType}\n\n错误信息:\n{errorMessage}", "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                lastError = (string)json["action"];
                            })
                            { IsBackground = true }.Start();
                            #endregion
                        }
                        else
                            switch ((string)json["action"])
                            {
                                #region login
                                case "login":
                                    User = User.GetUserFromJObject((JObject)json["user"]);
                                    break;
                                #endregion
                                #region changeUserInfo
                                case "changeUserInfo":
                                    User tmp04 = User.GetUserFromJObject((JObject)json["user"]);
                                    if (tmp04.Id == User.Id) User.ChangeInformation(tmp04);
                                    else LoginedUser[tmp04.Id].ChangeInformation(tmp04);
                                    Program.mainForm.Invoke(new Action(() => { UserInformationChanged?.Invoke(null, tmp04); }));
                                    break;
                                #endregion
                                #region chat
                                case "chat":
                                    Chat tmp = Chat.GetChatFromJObject(JObject.Parse(json["chat"].ToString()));
                                    try { tmp.User = LoginedUser[tmp.UserId]; } catch { break; }
                                    if (tmp.ToUserId == -1) _chatMessages.Add(tmp);
                                    else LoginedUser[tmp.UserId].Messages.Add(tmp);
                                    Program.mainForm.Invoke(new Action(() => { ReceiveChat?.Invoke(null, tmp); }));
                                    break;
                                #endregion
                                #region getAllUser
                                case "getAllUser":
                                    List<User> tmp1 = new List<User>();
                                    foreach (JToken t in JArray.Parse(json["users"].ToString()))
                                    {
                                        User u = User.GetUserFromJObject(JObject.Parse(t.ToString()));
                                        LoginedUser.Add(u.Id, u);
                                        tmp1.Add(u);
                                    }
                                    Program.mainForm.Invoke(new Action(() => { ReceiveOnlineUser?.Invoke(null, tmp1.ToArray()); }));
                                    break;
                                #endregion
                                #region login2
                                case "login2":
                                    User user = User.GetUserFromJObject(JObject.Parse(json["user"].ToString()));
                                    LoginedUser.Add(user.Id, user);
                                    Program.mainForm.Invoke(new Action(() => { UserLogin?.Invoke(null, user); }));
                                    break;
                                #endregion
                                #region logout
                                case "logout":
                                    User user2 = User.GetUserFromJObject(JObject.Parse(json["user"].ToString()));
                                    var tmp3 = LoginedUser[user2.Id];
                                    LoginedUser.Remove(user2.Id);
                                    Program.mainForm.Invoke(new Action(() => { UserLogout?.Invoke(null, tmp3); }));
                                    break;
                                #endregion
                                #region file
                                case "file":
                                    string filename = (string)json["filename"];
                                    int hash = (int)json["hash"];
                                    int toUser = (int)json["toUser"];
                                    int fromUser = (int)json["fromUser"];
                                    int port = (int)json["port"];
                                    new Thread(() =>
                                    {
                                        TcpClient tmp0 = new TcpClient();
                                        tmp0.Connect(new IPEndPoint(serverIP, port));
                                        //Thread.Sleep(1000);
                                        JObject json1 = JObject.Parse("{\"type\":\"\"}");
                                        if (fromUser == User.Id)
                                            json["type"] = "send";
                                        else json["type"] = "receive";
                                        NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(tmp0.GetStream(), json);
                                        JObject json2 = NetworkStreamProcessing.GetJObjectFromBase64Bytes(NetworkStreamProcessing.ReadDataFromNetworkStream(tmp0.GetStream()));
                                        if ((int)json2["hash"] != hash) return;
                                        NetworkStream ns0 = tmp0.GetStream();
                                        if (fromUser == User.Id)
                                        {
                                            FileStream fs;
                                            try { fs = new FileStream(SendFilePool[hash], FileMode.Open); } catch { if (tmp0.Connected) tmp0.Close(); return; }
                                            while (fs.Position < fs.Length)
                                            {
                                                byte[] buffer = new byte[8192];
                                                try
                                                {
                                                    int s = fs.Read(buffer, 0, 8192);
                                                    ns0.Write(buffer, 0, s);
                                                }
                                                catch { if (tmp0.Connected) tmp0.Close(); fs.Dispose(); return; }
                                            }
                                            try { fs.Close(); } catch { }
                                        }
                                        else
                                        {
                                            if (!Directory.Exists("ReceiveFiles"))
                                                Directory.CreateDirectory("ReceiveFiles");
                                            FileStream fs;
                                            try { fs = new FileStream(@"ReceiveFiles\" + filename, FileMode.CreateNew); } catch { if (tmp0.Connected) tmp0.Close(); return; }
                                            while (tmp0.Connected)
                                            {
                                                try
                                                {
                                                    byte[] buffer = NetworkStreamProcessing.ReadDataFromNetworkStream(ns0);
                                                    if (buffer.Length == 0) tmp0.Close();
                                                    fs.Write(buffer, 0, buffer.Length);
                                                }
                                                catch { if (tmp0.Connected) tmp0.Close(); fs.Dispose(); }
                                            }
                                            try { fs.Close(); } catch { }
                                            if (tmp0.Connected) tmp0.Close();
                                            if (File.ReadAllBytes(@"ReceiveFiles\" + filename).GetHashCode() != hash)
                                                System.Windows.Forms.MessageBox.Show($"\"{filename}\" 文件已损坏！");
                                        }
                                    })
                                    { IsBackground = true }.Start();
                                    break;
                                #endregion
                                #region register
                                case "register":
                                    if ((string)json["status"] == "success")
                                        Registered?.Invoke(null, EventArgs.Empty);
                                    break;
                                #endregion
                                #region getNotVerifiedUsers
                                case "getNotVerifiedUsers":
                                    List<User> tmp01 = new List<User>();
                                    foreach (JObject t in JArray.Parse(json["users"].ToString()))
                                        tmp01.Add(User.GetUserFromJObject(t));
                                    User[] tmp03 = tmp01.ToArray();
                                    new Thread(() =>
                                    {
                                        VerifyUserForm tmp02 = new VerifyUserForm(null, tmp03);
                                        tmp02.AcceptUser += (a, b) =>
                                        {
                                            b.ChangeInformationAndPushToServer(b.Name, b.Age, b.Birth, b.Telephone, b.Pay, b.Unit, b.Number, 1, b.Banned, Client.Stream);
                                        };
                                        tmp02.DenyUser += (a, b) =>
                                        {
                                            b.ChangeInformationAndPushToServer(b.Name, b.Age, b.Birth, b.Telephone, b.Pay, b.Unit, b.Number, -1, b.Banned, Client.Stream);
                                        };
                                        tmp02.InformationOpened += (a, b) =>
                                        {
                                            new Thread(() => { try { new PersonalInfomation(b, User).ShowDialog(); } catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); } }).Start();
                                        };
                                        tmp02.ShowDialog();
                                    })
                                    { IsBackground = true }.Start();
                                    break;
                                #endregion
                                #region getAllUserFromDatabase
                                case "getAllUserFromDatabase":
                                    List<User> tmp001 = new List<User>();
                                    foreach (JObject t in JArray.Parse(json["users"].ToString()))
                                        tmp001.Add(User.GetUserFromJObject(t));
                                    User[] tmp003 = tmp001.ToArray();
                                    new Thread(() =>
                                    {
                                        ChangeAllUsersInformationForm cauif = null;
                                        (cauif = new ChangeAllUsersInformationForm(User, tmp003, (a) => { try { new PersonalInfomation(a, User).ShowDialog(); } catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); } cauif.Refresh(); })).ShowDialog();
                                    })
                                    { IsBackground = true }.Start();
                                    break;
                                    #endregion
                            }
                    }
                    catch { }
                }
                if (receiveError != "")
                    System.Windows.Forms.MessageBox.Show($"与服务器断开连接！\n\n详细信息：{receiveError}");
                else System.Windows.Forms.MessageBox.Show($"与服务器断开连接！");
                Disconnected?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static User Login(string username, string password)
        {
            if (IsLogined) throw new Exception("已经登陆了");
            JObject tmp = JObject.Parse("{\"action\":\"login\",\"username\":\"\",\"password\":\"\"}");
            tmp["username"] = username;
            tmp["password"] = password;
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(client.GetStream(), tmp);
            while (User == null) if (lastError == "login") { Exception e = new Exception(lastError); lastError = null; throw e; }
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(client.GetStream(), JObject.Parse("{\"action\":\"getAllUser\"}"));
            return User;
        }
        /// <summary>
        /// 接收到消息的事件
        /// </summary>
        public static event EventHandler<Chat> ReceiveChat;
        /// <summary>
        /// 用户登入事件
        /// </summary>
        public static event EventHandler<User> UserLogin;
        /// <summary>
        /// 用户登出事件
        /// </summary>
        public static event EventHandler<User> UserLogout;
        /// <summary>
        /// 用户信息改变事件
        /// </summary>
        public static event EventHandler<User> UserInformationChanged;
        /// <summary>
        /// 接收到在线用户事件
        /// </summary>
        public static event EventHandler<User[]> ReceiveOnlineUser;
        /// <summary>
        /// 注册成功事件
        /// </summary>
        public static event EventHandler Registered;
        /// <summary>
        /// 与服务器断开连接事件
        /// </summary>
        public static event EventHandler Disconnected;
    }
}
