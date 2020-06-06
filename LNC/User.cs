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
    /// 用户类
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; private set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; private set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; private set; }

        /// <summary>
        /// 薪资
        /// </summary>
        public decimal? Pay { get; private set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Unit { get; private set; }
        /// <summary>
        /// 工号
        /// </summary>
        public int? Number { get; private set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }
        /// <summary>
        /// 是否被禁言
        /// </summary>
        public bool Banned { get; private set; }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdministrator { get { return Level >= 3; } }

        /// <summary>
        /// 单独会话的聊天记录
        /// </summary>
        public List<Chat> Messages { get; } = new List<Chat>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public User(int id, string userName, string name, int? age, DateTime? birth, string telephone, decimal? pay, string unit, int? number, int level, bool banned) { Id = id; UserName = userName; Name = name; Age = age; Birth = birth; Telephone = telephone; Pay = pay; Unit = unit; Number = number; Level = level; Banned = banned; }

        /// <summary>
        /// 获取没有审核的用户
        /// </summary>
        /// <returns></returns>
        public static User[] GetNotVerifiedUsers()
        {
            using (SqlCommand cmd = new SqlCommand("select Id from dbo.t_user where Level=0"))
            {
                List<User> tmp = new List<User>();
                List<int> tmp2 = new List<int>();
                SqlDataReader sdr = Database.ExecuteReader(cmd);
                while (sdr.Read())
                {
                    tmp2.Add((int)sdr[0]);
                }
                sdr.Close();
                foreach (int i in tmp2)
                    tmp.Add(GetUserByIdFromDatabase(i));
                return tmp.ToArray();
            }
        }
        public static User[] GetUserFromDatabase()
        {
            using (SqlCommand cmd = new SqlCommand("select Id from dbo.t_user where Level!=-1"))
            {
                List<User> tmp = new List<User>();
                List<int> tmp2 = new List<int>();
                SqlDataReader sdr = Database.ExecuteReader(cmd);
                while (sdr.Read())
                {
                    tmp2.Add((int)sdr[0]);
                }
                sdr.Close();
                foreach (int i in tmp2)
                    tmp.Add(GetUserByIdFromDatabase(i));
                return tmp.ToArray();
            }
        }
        /// <summary>
        /// 从数据库获取用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns></returns>
        public static User GetUserByIdFromDatabase(int id)
        {
            using (SqlDataReader sdr = Database.ExecuteReader(string.Format("select * from v_user where Id={0}", id)))
            {
                try
                {
                    if (!sdr.Read()) throw new Exception("没有此用户");
                    int _id = (int)sdr["Id"];
                    string userName = (string)sdr["UserName"];
                    string name = sdr["Name"] as string;
                    int? age = sdr["Age"] as int?;
                    DateTime? birth = sdr["Birth"] as DateTime?;
                    string telephone = sdr["Telephone"] as string;
                    decimal? pay = sdr["Pay"] as decimal?;
                    string unit = sdr["Unit"] as string;
                    int? number = sdr["Number"] as int?;
                    int level = (int)sdr["Level"];
                    bool banned = (bool)sdr["Banned"];
                    sdr.Close();
                    return new User(_id, userName, name, age, birth, telephone, pay, unit, number, level, banned);
                }
                catch
                {
                    sdr.Close();
                    return null;
                }
            }
        }
        /// <summary>
        /// 从数据库获取用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static User GetUserByUserNameFromDatabase(string userName)
        {
            SqlCommand sql = new SqlCommand(string.Format("select dbo.GetUserIdByUserName('{0}')", userName.Replace("'", "''")));
            int id = Database.ExecuteScalar<int>(sql);
            if (id == -1) return null;
            return GetUserByIdFromDatabase(id);
        }
        /// <summary>
        /// 从JObject创建User
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static User GetUserFromJObject(JObject jObject)
        {
            int id = (int)jObject["id"];
            string username = (string)jObject["username"];
            string name = null;
            try { name = (string)jObject["name"]; } catch { }
            int? age = null;
            try { age = (int)jObject["age"]; } catch { }
            DateTime? birth = null;
            try { birth = DateTime.Parse((string)jObject["birth"]); } catch { }
            string telephone = null;
            try { telephone = (string)jObject["telephone"]; } catch { }
            decimal? pay = null;
            try { pay = (decimal)jObject["pay"]; } catch { }
            string unit = null;
            try { unit = (string)jObject["unit"]; } catch { }
            int? number = null;
            try { number = (int)jObject["number"]; } catch { }
            int level = (int)jObject["level"];
            bool banned = (bool)jObject["banned"];
            return new User(id, username, name, age, birth, telephone, pay, unit, number, level, banned);
        }
        /// <summary>
        /// 检查用户名和密码是否正确
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CanLogin(string username, string password)
        {
            using (SqlCommand cmd = new SqlCommand(string.Format("select dbo.CanLogin('{0}','{1}')", username.Replace("'", "''"), password.Replace("'", "''"))))
            {
                return Database.ExecuteScalar<bool>(cmd);
            }
        }
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsExisted(int id)
        {
            using (SqlCommand cmd = new SqlCommand($"select dbo.GetIfUserIdExisted({id})"))
            {
                return Database.ExecuteScalar<bool>(cmd);
            }
        }
        /// <summary>
        /// 注册并推送到服务器
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void RegisterAndPushToDatabase(string username, string password)
        {
            using (SqlCommand cmd = new SqlCommand($"select count(Id) from t_account where UserName='{username.Replace("'", "''")}'"))
                if (Database.ExecuteScalar<int>(cmd) != 0)
                    throw new Exception("用户名已存在");
            using (SqlCommand cmd = new SqlCommand("dbo.CreateUser @a,@b"))
            {
                cmd.Parameters.Add("@a", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@b", SqlDbType.NVarChar).Value = password;
                if (Database.ExecuteNonQuery(cmd) == 0) throw new Exception("无法创建用户");
            }
        }

        #region 覆写
        public override bool Equals(object obj)
        {
            if (!(obj is User)) return false;
            User user = (User)obj;
            if (this.Id == user.Id && this.UserName == user.UserName && this.Name == user.Name && this.Age == user.Age && this.Telephone == user.Telephone && this.Pay == user.Pay && this.Unit == user.Unit && this.Number == user.Number && this.Level == user.Level && this.Banned == user.Banned)
                return true;
            else return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        /// <summary>
        /// 转换成JObject
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject()
        {
            JObject tmp = JObject.Parse("{\"id\":-1,\"username\":\"\",\"name\":null,\"age\":null,\"birth\":null,\"telephone\":null,\"pay\":null,\"unit\":null,\"number\":null,\"level\":-1,\"banned\":false}");
            tmp["id"] = this.Id;
            tmp["username"] = this.UserName;
            tmp["name"] = this.Name;
            tmp["age"] = this.Age;
            if (this.Birth != null)
                tmp["birth"] = ((DateTime)this.Birth).ToString("yyyy-MM-dd");
            tmp["telephone"] = this.Telephone;
            tmp["pay"] = this.Pay;
            tmp["unit"] = this.Unit;
            tmp["number"] = this.Number;
            tmp["level"] = this.Level;
            tmp["banned"] = this.Banned;
            return tmp;
        }

        /// <summary>
        /// 直接修改信息（不向服务器验证）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="birth"></param>
        /// <param name="telephone"></param>
        /// <param name="pay"></param>
        /// <param name="unit"></param>
        /// <param name="number"></param>
        /// <param name="level"></param>
        /// <param name="banned"></param>
        public void ChangeInformation(string name, int? age, DateTime? birth, string telephone, decimal? pay, string unit, int? number, int level, bool banned)
        {
            this.Name = name;
            this.Age = age;
            this.Birth = birth;
            this.Telephone = telephone;
            this.Pay = pay;
            this.Unit = unit;
            this.Number = number;
            this.Level = level;
            this.Banned = banned;
        }
        /// <summary>
        /// 直接修改信息（不向服务器验证）
        /// </summary>
        /// <param name="user"></param>
        public void ChangeInformation(User user) => ChangeInformation(user.Name, user.Age, user.Birth, user.Telephone, user.Pay, user.Unit, user.Number, user.Level, user.Banned);
        /// <summary>
        /// 修改信息并推送到数据库里
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="birth"></param>
        /// <param name="telephone"></param>
        /// <param name="pay"></param>
        /// <param name="unit"></param>
        /// <param name="number"></param>
        /// <param name="level"></param>
        /// <param name="banned"></param>
        public void ChangeInformationAndPushToDatabase(string name, int? age, DateTime? birth, string telephone, decimal? pay, string unit, int? number, int level, bool banned)
        {
            using (SqlCommand cmd = new SqlCommand("dbo.ChangeUserInformation @aid,@aname,@aage,@abirth,@atelephone,@apay,@aunit,@anumber,@alevel,@abanned"))
            {
                cmd.Parameters.Add("@aid", SqlDbType.Int).Value = this.Id;
                cmd.Parameters.Add("@aname", SqlDbType.NVarChar).Value = this.Name = name;
                if (age != null)
                    cmd.Parameters.Add("@aage", SqlDbType.Int).Value = this.Age = age;
                else cmd.CommandText = cmd.CommandText.Replace("@aage", "NULL");
                if (birth != null)
                    cmd.Parameters.Add("@abirth", SqlDbType.Date).Value = this.Birth = birth;
                else cmd.CommandText = cmd.CommandText.Replace("@abirth", "NULL");
                if (telephone != null)
                    cmd.Parameters.Add("@atelephone", SqlDbType.NVarChar).Value = this.Telephone = telephone;
                else cmd.CommandText = cmd.CommandText.Replace("@atelephone", "NULL");
                if (pay != null)
                    cmd.Parameters.Add("@apay", SqlDbType.Decimal).Value = this.Pay = pay;
                else cmd.CommandText = cmd.CommandText.Replace("@apay", "NULL");
                if (unit != null)
                    cmd.Parameters.Add("@aunit", SqlDbType.NVarChar).Value = this.Unit = unit;
                else cmd.CommandText = cmd.CommandText.Replace("@aunit", "NULL");
                if (number != null)
                    cmd.Parameters.Add("@anumber", SqlDbType.Int).Value = this.Number = number;
                else cmd.CommandText = cmd.CommandText.Replace("@anumber", "NULL");
                cmd.Parameters.Add("@alevel", SqlDbType.Int).Value = this.Level = level;
                cmd.Parameters.Add("@abanned", SqlDbType.Bit).Value = this.Banned = banned;
                Database.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// 修改信息并推送到服务器里
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <param name="birth"></param>
        /// <param name="telephone"></param>
        /// <param name="pay"></param>
        /// <param name="unit"></param>
        /// <param name="number"></param>
        /// <param name="level"></param>
        /// <param name="banned"></param>
        public void ChangeInformationAndPushToServer(string name, int? age, DateTime? birth, string telephone, decimal? pay, string unit, int? number, int level, bool banned, System.Net.Sockets.NetworkStream ns)
        {
            JObject json = JObject.Parse("{\"action\":\"changeUserInfo\",\"user\":null}");
            json["user"] = new User(this.Id, this.UserName, name, age, birth, telephone, pay, unit, number, level, banned).ToJObject().ToString();
            NetworkStreamProcessing.WriteBase64BytesEncodedJObjectToNetworkStream(ns, json);
        }
    }
}
