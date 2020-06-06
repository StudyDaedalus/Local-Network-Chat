using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LNC.Server
{
    /// <summary>
    /// 服务器类
    /// </summary>
    static class Server
    {
        private static TcpListener listener;
        private static IPAddress openedAddress;
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public static bool IsInited { get { return listener != null; } }
        /// <summary>
        /// 已经登陆的用户
        /// </summary>
        public static Dictionary<int, KeyValuePair<User, TcpClient>> LoginedUser { get; } = new Dictionary<int, KeyValuePair<User, TcpClient>>();
        /// <summary>
        /// 服务器监听接口
        /// </summary>
        public static IPEndPoint OpenedEndPoint { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ipep">IP终结点</param>
        public static void Init(IPEndPoint ipep, User user)
        {
            if (IsInited) throw new Exception("请不要重复初始化");
            if (ipep == null) throw new Exception("IPEndPoint不能为null");
            listener = new TcpListener(ipep);
            //开始监听
            listener.Start(10);
            new Thread(() =>
            {
                while (true)
                {
                    new Thread(ProcessTcpClient)
                    { IsBackground = true }.Start(listener.AcceptTcpClient());
                }
            })
            { IsBackground = true }.Start();
            User = user;
            ReceiveOnlineUser?.Invoke(null, new User[] { user });
            new Thread(broadcast) { IsBackground = true }.Start(ipep.Port);
            openedAddress = ipep.Address;
            OpenedEndPoint = ipep;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="port">端口</param>
        public static void Init(int port, User user) { Init(new IPEndPoint(IPAddress.Any, port), user); }

        private static List<Chat> _messages = new List<Chat>();
        /// <summary>
        /// 全局聊天消息
        /// </summary>
        public static Chat[] Messages { get { return _messages.ToArray(); } }

        /// <summary>
        /// 服务器上登陆的用户
        /// </summary>
        public static User User { get; private set; }

        /// <summary>
        /// 处理客户端
        /// </summary>
        /// <param name="obj">客户端 (TcpClient)</param>
        public static void ProcessTcpClient(object obj)
        {
            TcpClient client = obj as TcpClient;
            if (client == null) return;

            using (NetworkStream ns = client.GetStream())
            {
                User user = null;
                while (client.Connected)
                {
                    try
                    {
                        JObject json = NetworkStreamProcessing.GetJObjectFromBase64Bytes(NetworkStreamProcessing.ReadDataFromNetworkStream(ns));
                        if ((string)json["application"] != "LNC") client.Close();
                        try
                        {
                            if (user == null)
                                switch ((string)json["action"])
                                {
                                    #region login
                                    case "login":
                                        if (User.Name == (string)json["username"]) throw new Exception("该用户已经登陆了");
                                        foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> tmp1 in LoginedUser)
                                            if (tmp1.Value.Key.UserName == (string)json["username"]) throw new Exception("该用户已经登陆了");
                                        if (!User.CanLogin((string)json["username"], (string)json["password"])) throw new Exception("账号或密码错误");
                                        else
                                        {
                                            user = User.GetUserByUserNameFromDatabase((string)json["username"]);
                                            if (user.Level == 0) throw new Exception("你还没有审核");
                                            if (user.Level == -1) throw new Exception("用户审核未通过");
                                            JObject tmp = JObject.Parse("{\"action\":\"login\",\"result\":\"success\",\"user\":null}");
                                            tmp["user"] = user.ToJObject();
                                            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, tmp);
                                            JObject json3 = JObject.Parse("{\"action\":\"login2\",\"user\":null}");
                                            json3["user"] = user.ToJObject();
                                            foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp in LoginedUser)
                                                new Thread(() => { try { NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(kvp.Value.Value.GetStream(), json3); } catch { } })
                                                { IsBackground = true }.Start();
                                            LoginedUser.Add(user.Id, new KeyValuePair<User, TcpClient>(user, client));
                                            Program.mainForm.Invoke(new Action(() => { UserLogin?.Invoke(null, user); }));
                                        }
                                        break;
                                    #endregion
                                    #region register
                                    case "register":
                                        string tmp01 = (string)json["username"];
                                        string tmp02 = (string)json["password"];
                                        User.RegisterAndPushToDatabase(tmp01, tmp02);
                                        json.Add("result", "success");
                                        json.Remove("username");
                                        json.Remove("password");
                                        NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, json);
                                        break;
                                    #endregion
                                    default:
                                        throw new Exception("你还没有登陆");
                                }
                            else
                                switch ((string)json["action"])
                                {
                                    #region chat
                                    case "chat":
                                        if (user.Level == 0) throw new Exception("您没有权限发送消息");
                                        Chat tmp0 = Chat.GetChatFromJObject((JObject)json["chat"]);
                                        if (tmp0.ToUserId == -1 && user.Banned) throw new Exception("您已被禁言，无法发言");
                                        string message = tmp0.Message;
                                        if (message.Trim() == "") throw new Exception("不能发送空消息");
                                        int toUserId = tmp0.ToUserId;
                                        if (toUserId != -1)
                                            if (!User.IsExisted(toUserId)) throw new Exception("用户不存在");
                                        Chat tmp = Chat.CreateChatAndSendToDatabase(user.Id, toUserId, message);
                                        tmp.User = user;
                                        if (tmp.ToUserId == -1)
                                        {
                                            _messages.Add(tmp);
                                            foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp in LoginedUser)
                                                new Thread(() => { try { tmp.SendChatToStream(kvp.Value.Value.GetStream()); } catch { } })
                                                { IsBackground = true }.Start();
                                            Program.mainForm.Invoke(new Action(() => { ReceiveChat?.Invoke(null, tmp); }));
                                        }
                                        else if (tmp.ToUserId == User.Id)
                                        {
                                            KeyValuePair<User, TcpClient> tmp3;
                                            try { tmp3 = LoginedUser[tmp.UserId]; } catch { break; }
                                            tmp3.Key.Messages.Add(tmp);
                                            Program.mainForm.Invoke(new Action(() => { ReceiveChat?.Invoke(null, tmp); }));
                                        }
                                        else
                                        {
                                            KeyValuePair<User, TcpClient> tmp3;
                                            try { tmp3 = LoginedUser[tmp.ToUserId]; } catch { break; }
                                            tmp.SendChatToStream(tmp3.Value.GetStream());
                                        }
                                        break;
                                    #endregion
                                    #region changeUserInfo
                                    case "changeUserInfo":
                                        User tmp1 = User.GetUserFromJObject(JObject.Parse(json["user"].ToString()));
                                        User tmp2 = User.GetUserByIdFromDatabase(tmp1.Id);
                                        if (tmp2 == null) throw new Exception("无法找到用户");
                                        else if (tmp1 == tmp2) throw new Exception("信息没有改变");
                                        switch (user.Level)
                                        {
                                            case 0:
                                                throw new Exception("没有权限");
                                            case 1:
                                                tmp2.ChangeInformationAndPushToDatabase(tmp1.Name, tmp1.Age, tmp1.Birth, tmp1.Telephone, tmp2.Pay, tmp2.Unit, tmp2.Number, tmp2.Level, tmp2.Banned);
                                                break;
                                            case 2:
                                                if (tmp2.Level >= 2 && tmp1.Id != tmp2.Id) throw new Exception("不能更改管理员的信息");
                                                if (tmp1.Id == tmp2.Id)
                                                    tmp2.ChangeInformationAndPushToDatabase(tmp1.Name, tmp1.Age, tmp1.Birth, tmp1.Telephone, tmp2.Pay, tmp2.Unit, tmp2.Number, tmp2.Level, tmp2.Banned);
                                                else tmp2.ChangeInformationAndPushToDatabase(tmp1.Name, tmp1.Age, tmp1.Birth, tmp1.Telephone, tmp2.Pay, tmp1.Unit, tmp2.Number, tmp2.Level, tmp1.Banned);
                                                break;
                                            case 3:
                                                tmp2.ChangeInformationAndPushToDatabase(tmp1.Name, tmp1.Age, tmp1.Birth, tmp1.Telephone, tmp1.Pay, tmp1.Unit, tmp1.Number, tmp1.Level, tmp1.Banned);
                                                break;
                                            default: throw new Exception("没有权限");
                                        }
                                        try { var tmp00001 = LoginedUser[tmp1.Id]; } catch { break; }
                                        json["user"] = tmp2.ToJObject();
                                        foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp1 in LoginedUser)
                                            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(kvp1.Value.Value.GetStream(), json);
                                        tmp2.Messages.AddRange(LoginedUser[tmp2.Id].Key.Messages);
                                        LoginedUser[tmp2.Id] = new KeyValuePair<User, TcpClient>(tmp2, LoginedUser[tmp2.Id].Value);
                                        if (tmp2.Id == user.Id) user = tmp2;
                                        Program.mainForm.Invoke(new Action(() =>
                                        {
                                            UserInformationChanged?.Invoke(null, tmp2);
                                        }));
                                        break;
                                    #endregion
                                    #region getAllUser
                                    case "getAllUser":
                                        JObject tmp4 = JObject.Parse("{\"action\":\"getAllUser\",\"users\":null}");
                                        JArray tmp5 = new JArray();
                                        tmp5.Add(User.ToJObject());
                                        foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> tmp6 in LoginedUser)
                                            tmp5.Add(tmp6.Value.Key.ToJObject());
                                        tmp4["users"] = tmp5;
                                        NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, tmp4);
                                        break;
                                    #endregion
                                    #region file
                                    case "file":
                                        string filename = (string)json["filename"];
                                        int hash = (int)json["hash"];
                                        int tmp7 = (int)json["toUser"];
                                        json.Add("fromUser", user.Id);
                                        //Chat.SendChatToStream(user.Id, tmp7, $"向您发送了文件“{filename}”", LoginedUser[tmp7].Value.GetStream());
                                        if (tmp7 != User.Id)
                                            new Thread(() =>
                                            {
                                                Random rand = new Random(DateTime.Now.GetHashCode());
                                                int port = rand.Next(16000, 17000);
                                                //寻找空闲端口
                                                while (true)
                                                    try
                                                    {
                                                        using (TcpClient tc = new TcpClient())
                                                        {
                                                            tc.Connect(Dns.GetHostName(), port);
                                                            tc.Close();
                                                            port = rand.Next(17000, 18000);
                                                        }
                                                    }
                                                    catch { break; }
                                                TcpListener tmp3 = new TcpListener(new IPEndPoint(openedAddress, port));
                                                tmp3.Start(2);

                                                json.Add("port", port);
                                                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(LoginedUser[tmp7].Value.GetStream(), json);
                                                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, json);

                                                TcpClient tmp6 = null, tmp9 = null;
                                                while (tmp6 == null || tmp9 == null)
                                                    try
                                                    {
                                                        TcpClient tmp10 = tmp3.AcceptTcpClient();
                                                        switch ((string)NetworkStreamProcessing.GetJObjectFromBase64Bytes(NetworkStreamProcessing.ReadDataFromNetworkStream(tmp10.GetStream()))["type"])
                                                        {
                                                            case "send":
                                                                tmp6 = tmp10;
                                                                break;
                                                            case "receive":
                                                                tmp9 = tmp10;
                                                                break;
                                                        }
                                                    }
                                                    catch { return; }
                                                JObject tmp8 = JObject.Parse($"{{\"action\":\"file\",\"hash\":{hash}}}");
                                                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(tmp6.GetStream(), json);
                                                NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(tmp9.GetStream(), json);
                                                NetworkStream tmp11 = tmp6.GetStream(), tmp12 = tmp9.GetStream();
                                                while (tmp6.Connected)
                                                    try
                                                    {
                                                        byte[] buffer = NetworkStreamProcessing.ReadDataFromNetworkStream(tmp11);
                                                        tmp12.Write(buffer, 0, buffer.Length);
                                                    }
                                                    catch (Exception e) { try { tmp6.Close(); } catch { } break; }
                                                tmp9.Close();
                                            })
                                            { IsBackground = true }.Start();
                                        else throw new Exception("禁止向服务器发送文件");
                                        break;
                                    #endregion
                                    #region getNotVerifiedUsers
                                    case "getNotVerifiedUsers":
                                        if (user.Level < 2) throw new Exception("没有权限");
                                        JArray tmp03 = new JArray();
                                        User[] tmp04 = User.GetNotVerifiedUsers();
                                        foreach (User tmp05 in tmp04)
                                            tmp03.Add(tmp05.ToJObject());
                                        json.Add("users", tmp03);
                                        NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, json);
                                        break;
                                    #endregion
                                    #region getAllUserFromDatabase
                                    case "getAllUserFromDatabase":
                                        if (user.Level < 2) throw new Exception("没有权限");
                                        JObject tmp06 = JObject.Parse("{\"action\":\"getAllUser\",\"users\":null}");
                                        JArray tmp08 = new JArray();
                                        User[] tmp07 = User.GetUserFromDatabase();
                                        foreach (User tmp6 in tmp07)
                                            tmp08.Add(tmp6.ToJObject());
                                        tmp06["users"] = tmp08;
                                        NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, tmp06);
                                        break;
                                    #endregion

                                    default: throw new Exception("未知的行为");
                                }
                        }
                        catch (Exception e)
                        {
                            #region error
                            new Thread(() =>
                            {
                                JObject tmp = JObject.Parse("{\"action\":\"\",\"error\":true,\"errorMessage\":\"\",\"errorType\":\"\"}");
                                tmp["action"] = json["action"];
                                tmp["errorMessage"] = e.Message;
                                tmp["errorType"] = e.GetType().FullName;
                                try { NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, tmp); }
                                catch { }
                            })
                            { IsBackground = true }.Start();
                            #endregion
                        }
                    }
                    catch { }
                }
                if (user != null)
                {
                    #region logout
                    LoginedUser.Remove(user.Id);
                    JObject json2 = JObject.Parse("{\"action\":\"logout\",\"user\":null}");
                    json2["user"] = user.ToJObject();
                    foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp in LoginedUser)
                        new Thread(() => { try { NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(kvp.Value.Value.GetStream(), json2); } catch { } })
                        { IsBackground = true }.Start();
                    Program.mainForm.Invoke(new Action(() => { UserLogout?.Invoke(null, user); }));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 发送全局聊天
        /// </summary>
        /// <param name="message"></param>
        public static void SendGobalChat(string message)
        {
            Chat tmp = Chat.CreateChatAndSendToDatabase(User.Id, -1, message);
            tmp.User = User;
            foreach (KeyValuePair<int, KeyValuePair<User, TcpClient>> kvp in LoginedUser)
                new Thread(() => { try { tmp.SendChatToStream(kvp.Value.Value.GetStream()); } catch { } })
                { IsBackground = true }.Start();
            _messages.Add(tmp);
            Program.mainForm.Invoke(new Action(() => { ReceiveChat?.Invoke(null, tmp); }));
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
        /// 持续向广播地址发送服务器的IP终结点
        /// </summary>
        /// <param name="obj">端口</param>
        private static void broadcast(object obj)
        {
            if (!(obj is int)) return;
            int port = (int)obj;
            JObject json = JObject.Parse("{\"application\":\"LNC\",\"action\":\"serverIPEndPort\",\"hostname\":\"\",port:0}");
            json["hostname"] = Dns.GetHostName();
            json["port"] = port;

            byte[] dgram = NetworkStreamProcessing.GetBase64BytesFromJObject(json);
            int bytes = dgram.Length;
            IPEndPoint boradcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 16999);
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            client.EnableBroadcast = true;
            while (true)
            {
                client.Send(dgram, bytes, boradcastEndPoint);
                Thread.Sleep(200);
            }
        }

    }
}
