using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace LNC
{
    public static class NetworkStreamProcessing
    {
        /// <summary>
        /// 从网络流中读取数据
        /// </summary>
        /// <param name="ns">网络流</param>
        /// <returns></returns>
        public static byte[] ReadDataFromNetworkStream(NetworkStream ns)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                int s;
                do
                {
                    s = ns.Read(buffer, 0, 8192);
                    stream.Write(buffer, 0, s);
                } while (ns.DataAvailable);
                try { stream.Position = 0; buffer = new byte[stream.Length]; stream.Read(buffer, 0, (int)stream.Length); return buffer; } catch { return null; }
            }
        }

        /// <summary>
        /// 将base64字节转换成JObject
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <returns></returns>
        public static JObject GetJObjectFromBase64Bytes(byte[] bytes) { return JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(bytes)))); }
        /// <summary>
        /// 将JObject转换成base64字节
        /// </summary>
        /// <param name="jObject">JObject</param>
        /// <returns></returns>
        public static byte[] GetBase64BytesFromJObject(JObject jObject) { return Encoding.ASCII.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(jObject.ToString()))); }
        /// <summary>
        /// 将JObject编码成base64字节写入到网络流
        /// </summary>
        /// <param name="ns">网络流</param>
        /// <param name="jObject">JObject</param>
        public static void WriteBase64BytesEncodedJObjectToNetworkStream(NetworkStream ns, JObject jObject)
        {
            if ((string)jObject["application"] == null) jObject.Add("application", "LNC");
            try { bool error = (bool)jObject["error"]; } catch { jObject.Add("error", false); }
            byte[] bytes = GetBase64BytesFromJObject(jObject);
            ns.Write(bytes, 0, bytes.Length);
        }
    }
}