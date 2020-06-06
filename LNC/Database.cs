using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace LNC
{
    /// <summary>
    /// 数据库类
    /// </summary>
    public static class Database
    {
        //数据库文件位置
        private static string databasePath = System.IO.Directory.GetCurrentDirectory() + @"\Database.mdf";
        //数据库连接字符串
        private static string connectString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{databasePath}"";Integrated Security=True";
        //数据库连接
        private static SqlConnection connection = new SqlConnection(connectString);


        //数据库状态
        public static ConnectionState State { get { return connection.State; } }

        // 创建数据库
        public static void CreateDatabase()
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB"))
            {
                conn.Open();
                CreateDatabase: SqlCommand cmd = new SqlCommand($@"
CREATE DATABASE LNCDatabase ON PRIMARY
(NAME = LNCDatabase_Data,
FILENAME = '{System.IO.Directory.GetCurrentDirectory() + @"\Database.mdf"}',
SIZE = 2MB, MAXSIZE = 1000MB, FILEGROWTH = 10MB)
LOG ON (NAME = LNCDatabase_Log,
FILENAME = '{System.IO.Directory.GetCurrentDirectory() + @"\Database_log.ldf"}',
SIZE = 1MB,
MAXSIZE = 100MB,
FILEGROWTH = 10MB)
", conn);
                try { cmd.ExecuteNonQuery(); }
                catch
                {
                    cmd = new SqlCommand("sp_detach_db LNCDatabase", conn);
                    cmd.ExecuteNonQuery();
                    goto CreateDatabase;
                }
            }
            Open();
            string[] sql = @"
CREATE FUNCTION dbo.GetMessageId ()
RETURNS INT
AS
BEGIN
RETURN 0
END
GO
CREATE FUNCTION dbo.GetUserId ()
RETURNS INT
AS
BEGIN
RETURN 0
END
GO
CREATE TABLE [dbo].[t_account] (
	[Id]       INT           NOT NULL,
    [UserName] NVARCHAR (10) NOT NULL,
    [Password] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[t_chat] (
    [Id]       INT             NOT NULL,
    [Time]     DATETIME        DEFAULT (getdate()) NOT NULL,
    [UserId]   INT             NOT NULL,
    [ToUserId] INT             DEFAULT ((-1)) NOT NULL,
    [Message]  NVARCHAR (4000) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
ALTER FUNCTION [dbo].GetMessageId()
RETURNS INT
AS
BEGIN
	RETURN (select count(Id) from dbo.t_chat)
END
GO
ALTER FUNCTION [dbo].GetUserId()
RETURNS int
AS
BEGIN
	RETURN (select count(Id) From t_account)
END
GO
ALTER TABLE dbo.t_chat
ADD	DEFAULT ([dbo].[GetMessageId]()) FOR Id
GO
CREATE TABLE [dbo].[t_user] (
    [Id]        INT           NOT NULL,
    [Name]      NVARCHAR (10) NULL,
    [Age]       INT           NULL,
    [Birth]     DATE          NULL,
    [Telephone] NCHAR (11)    NULL,
    [Pay]       DECIMAL (18)  NULL,
    [Unit]      NVARCHAR (10) NULL,
    [Number]    INT           NULL,
    [Level]     INT           DEFAULT ((0)) NOT NULL,
    [Banned]    BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE VIEW [dbo].v_chat
	AS SELECT dbo.t_chat.Id,
			  dbo.t_chat.Message,
			  dbo.t_chat.Time,
			  dbo.t_chat.ToUserId,
			  dbo.t_chat.UserId,
	          dbo.t_user.Name
		 FROM dbo.t_chat,
			  dbo.t_user
		where dbo.t_chat.UserId=dbo.t_user.Id
GO
CREATE VIEW [dbo].[v_user]
	AS SELECT dbo.t_user.*,
			  dbo.t_account.UserName
		
		FROM dbo.t_user,
			 dbo.t_account
		
		WHERE dbo.t_user.Id = dbo.t_account.Id
GO
CREATE PROCEDURE [dbo].ChangeUserInformation
	@id int,
	@name nvarchar(10)=NULL,
	@age int=NULL,
	@birth date=NULL,
	@telephone nvarchar(11)=NULL,
	@pay decimal(18,0)=NULL,
	@unit nvarchar(10)=NULL,
	@number int=NULL,
	@level int,
	@banned bit
AS
	if @name = null
	update dbo.t_user set Name = (select UserName from t_account where Id = @id) where Id = @id
	else update dbo.t_user set Name = @name where Id = @id
	update dbo.t_user set Age = @age where Id = @id
	update dbo.t_user set Birth = @birth where Id = @id
	update dbo.t_user set Telephone = @telephone where Id = @id
	update dbo.t_user set Pay = @pay where Id = @id
	update dbo.t_user set Unit = @unit where Id = @id
	update dbo.t_user set Number = @number where Id = @id
	update dbo.t_user set Level = @level where Id = @id
	update dbo.t_user set Banned = @banned where Id = @id
RETURN 0
GO
CREATE PROCEDURE [dbo].CreateChatMessage
	@userId int,
	@message nvarchar(500),
	@toUserId int = -1
AS
	insert into dbo.t_chat
			(UserId,Message,ToUserId)
	  values(@userId,@message,@toUserId)
RETURN 0
GO
CREATE PROCEDURE [dbo].CreateUser
	@username nvarchar(10),
	@password nvarchar(20)
AS
	insert into dbo.t_user(Id, Name, Level) values(dbo.GetUserId(), @username, 0)
	insert into dbo.t_account(Id, UserName, Password) values(dbo.GetUserId(), @username, @password)
RETURN 0
GO
CREATE FUNCTION [dbo].[CanLogin]
(
	@username nvarchar(10),
	@password nvarchar(20)
)
RETURNS bit
AS
BEGIN
	if(@password = (select Password from dbo.t_account where dbo.t_account.UserName = @username))
	return 1
	return 0
END
GO
CREATE FUNCTION [dbo].GetIfUserIdExisted
(
	@id int
)
RETURNS bit
AS
BEGIN
	if((select count(Id) from dbo.t_account where Id=@id) = 0)
	RETURN 0
	return 1
END
GO
CREATE FUNCTION [dbo].[GetLevel]
(
	@id int
)
RETURNS INT
AS
BEGIN
	RETURN (select Level from dbo.t_user where dbo.t_user.Id = @id)
END
GO
CREATE FUNCTION [dbo].GetUserIdByUserName
(
	@username nvarchar(10)
)
RETURNS INT
AS
BEGIN
	if((select UserName from dbo.t_account where UserName=@username)=@username)
	return (select Id from dbo.t_account where UserName=@username)
	return -1
END
GO
dbo.CreateUser 'admin','admin'
GO
update dbo.v_user set Level=3 where UserName='admin'
".Split(new string[] { "GO" },StringSplitOptions.RemoveEmptyEntries);
            foreach (string tmp in sql)
                ExecuteNonQuery(tmp);
        }

        
        // 打开数据库连接
   
        public static void Open()
        {
            if (!System.IO.File.Exists(databasePath)) CreateDatabase();
            else connection.Open();
        }
        
        // 关闭数据库连接

        public static void Close() { connection.Close(); }

        public static int ExecuteNonQuery(SqlCommand cmd)
        {
            cmd.Connection = connection;
            return cmd.ExecuteNonQuery();
        }
        
        public static int ExecuteNonQuery(string cmd) { return ExecuteNonQuery(new SqlCommand(cmd)); }

        public static SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            cmd.Connection = connection;
            return cmd.ExecuteReader();
        }
        
        public static SqlDataReader ExecuteReader(string cmd) { return ExecuteReader(new SqlCommand(cmd)); }

  
        // 执行查询，并返回由查询返回的结果集中的第一行的第一列。其他列或行将被忽略。
       
        public static object ExecuteScalar(SqlCommand cmd)
        {
            cmd.Connection = connection;
            return cmd.ExecuteScalar();
        }
 
        // 执行查询，并返回由查询返回的结果集中的第一行的第一列。其他列或行将被忽略。
        
        public static T ExecuteScalar<T>(SqlCommand cmd) { return (T)ExecuteScalar(cmd); }
        
        // 执行查询，并返回由查询返回的结果集中的第一行的第一列。其他列或行将被忽略。
       
        public static object ExecuteScalar(string cmd) { return ExecuteScalar(new SqlCommand(cmd)); }
        
        // 执行查询，并返回由查询返回的结果集中的第一行的第一列。其他列或行将被忽略。
        
        public static T ExecuteScalar<T>(string cmd) { return (T)ExecuteScalar(cmd); }
    }
}
