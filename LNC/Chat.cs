using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace LNC
{
    /// <summary>
    /// 聊天消息类
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 发送消息的用户Id
        /// </summary>
        public int UserId { get; }
        /// <summary>
        /// 接收消息的用户Id（-1为全局消息）
        /// </summary>
        public int ToUserId { get; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// 发送消息的用户
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        public Chat(int id, int userId, int toUserId, string message) { Id = id; UserId = userId; ToUserId = toUserId; Message = message; }

        /// <summary>
        /// 转换成JObject
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            JObject tmp = JObject.Parse("{\"id\":-1,\"userId\":-1,\"toUserId\":-1,\"message\":\"\"}");
            tmp["id"] = this.Id;
            tmp["userId"] = this.UserId;
            tmp["toUserId"] = this.ToUserId;
            tmp["message"] = Convert.ToBase64String(Encoding.Default.GetBytes(this.Message));
            return tmp;
        }

        /// <summary>
        /// 将JObjcet转换成Chat
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static Chat GetChatFromJObject(JObject jObject)
        {
            return new Chat((int)jObject["id"], (int)jObject["userId"], (int)jObject["toUserId"], Encoding.Default.GetString(Convert.FromBase64String((string)jObject["message"])));
        }
        /// <summary>
        /// 创建消息并发送到数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Chat CreateChatAndSendToDatabase(int userId, int toUserId, string message)
        {
            int id = Database.ExecuteScalar<int>("select dbo.GetMessageId()");
            Chat chat = new Chat(id, userId, toUserId, message);
            chat.SendToDatabase();
            return chat;
        }
        /// <summary>
        /// 创建消息并发送到网络流
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static void SendChatToStream(int userId, int toUserId, string message, System.Net.Sockets.NetworkStream ns) { new Chat(-1, userId, toUserId, message).SendChatToStream(ns); }

        /// <summary>
        /// 将消息发送到网络流
        /// </summary>
        /// <param name="ns"></param>
        public void SendChatToStream(System.Net.Sockets.NetworkStream ns)
        {
            JObject json = JObject.Parse("{\"action\":\"chat\",\"chat\":null}");
            json["chat"] = this.ToJObject();
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, json);
        }
        /// <summary>
        /// 将消息发送到数据库
        /// </summary>
        public void SendToDatabase()
        {
            using (SqlCommand cmd = new SqlCommand($"select count(Id) from dbo.t_chat where Id={this.Id}"))
            {
                if (Database.ExecuteScalar<int>(cmd) != 0) throw new Exception("存在相同Id的消息");
            }
            using (SqlCommand cmd = new SqlCommand("dbo.CreateChatMessage @a,@b,@c"))
            {
                cmd.Parameters.Add("@a", SqlDbType.Int).Value = this.UserId;
                cmd.Parameters.Add("@b", SqlDbType.NVarChar).Value = this.Message;
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = this.ToUserId;
                if (Database.ExecuteNonQuery(cmd) == 0) throw new Exception("无法将消息储存到数据库");
            }
        }
    }
}
