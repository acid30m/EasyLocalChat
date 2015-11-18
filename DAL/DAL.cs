using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;
using System.Data.SqlClient;

namespace WpfApplication1.DAL
{
    class DAL
    {

        public static string connectionStr { get; set; }

       

        public DAL() { }


        #region Connection

        public bool CheckConnectionToServer()
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }
        
        
        public bool CheckDBTables()
        {
            string query = string.Format(@"IF OBJECT_ID('Users', 'U') IS NOT NULL select N'exist' as result 
                                            ELSE
                                                select N'not exist' as result ")
                ;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader["result"].ToString() == "exist")
                    {
                        return true;
                    }
                }
                
            }
            catch (SqlException exception)
            {
                throw (exception);
            }
            return false;
        }


        public bool CreateDBTables()
        {
            string query = string.Format(@"create table Users
                                            (
                                                id_user int IDENTITY(1,1) primary key,
                                                nick_name nvarchar(33) not null,
                                                passwd nvarchar(100) not null,
                                                attachment varbinary(max),
                                                status int not null
                                            );



                                            create table Talks 
                                            (
                                                id_talk int IDENTITY(1,1) primary key,
                                                name nvarchar(33) not null
                                            );




                                            create table [Messages]
                                            (
                                                id_message int IDENTITY(1,1) primary key,
                                                content nvarchar(333) not null,
                                                id_user int foreign key references Users(id_user),
                                                date_send DATETIME not null
                                            );



                                            create table UserTalks 
                                            (
                                                id_usertalk int IDENTITY(1,1) primary key,
                                                id_talk int foreign key references Talks(id_talk),
                                                id_user int foreign key references Users(id_user),
                                                status int not null
                                            );



                                            create table MessTalks 
                                            (
    
                                                id_talk int foreign key references Talks(id_talk),
                                                id_message int foreign key references [Messages](id_message)	
                                            );


                                            INSERT into Talks values(N'General');")
                ;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                }
                return true;

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
            return false;
        }

        

        #endregion Connection

        #region Registration/Login

        public bool CreateUser(string name, byte[] password)
        {
            
            string query = string.Format(@"INSERT INTO Users values (N'{0}',N'{1}', null, 1)", name, BitConverter.ToString(password).Replace("-", ""));
             try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    return true;
                }
                
            }
            catch (SqlException e)
            {
                string ex = e.Message;
                return false;
            }
        }

        public bool CheckUserNick(string nickName)
        {
            string query = string.Format(@"if exists ( select id_user from Users where nick_name = N'{0}' ) 
                                            BEGIN
                                                select N'Exists' as result
                                            END
                                           ELSE
                                            BEGIN
                                                select N'Not exists' as result
                                            END"
                                        ,nickName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader["result"].ToString() == "Exists")
                    {
                        return true;
                    }
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
            return false;
        }

        public bool CheckUserPass(string nickName, string password)
        {
            string query = string.Format(@"if exists ( select id_user from Users where nick_name = N'{0}' AND passwd = N'{1}' ) 
                                            BEGIN
                                                select N'Exists' as result
                                            END
                                           ELSE
                                            BEGIN
                                                select N'Not exists' as result
                                            END"
                                        , nickName, password);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader["result"].ToString() == "Exists")
                    {
                        return true;
                    }
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
            return false;
        }


        public int GetUserIdByNick(string nickName)
        {
            string query = string.Format(@"SELECT TOP 1 id_user from Users where nick_name = N'{0}'"
                                        , nickName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return int.Parse(reader[0].ToString());
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
        }

        public string GetUserNickById(int userId)
        {
            string query = string.Format(@"select TOP 1 u.nick_name
                                            from  Users as u 
                                            where id_user = N'{0}'"
                                            , userId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return reader[0].ToString();
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
        }

        public void ChangeUserStatus(int userId, int status)
        {
            string query = string.Format(@"UPDATE Users
                                           set status = {1}
                                            where id_user = {0}"
                                            , userId, status);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;

            }
        }

        #endregion Registration/Login


        #region Chat

        public int GetTalkMsgCountByName(string talkName)
        {


            string query = string.Format(@"select COUNT(m.id_message)
                                            from [Messages] as m
                                            join MessTalks as mt on mt.id_message = m.id_message
                                            join Talks as t on t.id_talk = mt.id_talk AND t.name = N'{0}'"
                                            , talkName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return int.Parse(reader[0].ToString());
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }

        }


        public List<string> GetAllTalkMsgsByName(string talkName)
        {
            List<string> result = new List<string>();
            string query = string.Format(@"select u.nick_name + N': ' + m.content + N'  *' + CONVERT(VARCHAR(19),m.date_send) + N'*'
                                            from [Messages] as m
                                            join Users as u on u.id_user = m.id_user
                                            join MessTalks as mt on mt.id_message = m.id_message
                                            join Talks as t on t.id_talk = mt.id_talk AND t.name = N'{0}'
                                            ORDER BY m.date_send ASC"
                                            , talkName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(reader[0].ToString());
                    }
                    
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
            return result;
        }


        public int GetTalkIdByName(string talkName)
        {
            string query = string.Format(@"select TOP 1 t.id_talk
                                            from  Talks as t 
                                            where t.name = N'{0}'"
                                            , talkName);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return int.Parse(reader[0].ToString());
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
        }

        public string GetTalkNameById(int talkId)
        {
            string query = string.Format(@"select TOP 1 t.name 
                                            from  Talks as t 
                                            where t.id_talk = N'{0}'"
                                           , talkId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return reader[0].ToString();
                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }
        }

                
        public void SendMessage(string talkName, int userId, string message)
        {
            string query = string.Format(@"INSERT INTO [Messages] values (N'{0}',{1}, GETDATE())
                                            INSERT INTO MessTalks values (N'{2}', (select TOP 1 m.id_message
                                                                                    from [Messages] as m
                                                                                    where m.content = N'{0}' AND m.id_user = {1}
                                                                                    order by m.date_send DESC) ) "
                                            , message, userId, GetTalkIdByName(talkName));
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;
                
            }
        }


        public List<string> GetUsersOnlineExceptCurrent(int userId)
        {
            List<string> result = new List<string>();

            string query = string.Format(@"select u.nick_name
                                            from  Users as u 
                                            where u.id_user != {0} AND u.status = 1"
                                            , userId);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(reader[0].ToString());
                    }

                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }

            return result;
        }

        public int CheckIfPersonalChatExists(string userNick1, string userNick2)
        {
            string query = string.Format(@"if exists ( select id_talk 
                                                       from Talks 
                                                       where name = N'pm_{0}:{1}' OR name = N'pm_{1}:{0}' ) 
                                            BEGIN
                                                select id_talk as result
                                                from Talks 
                                                where name = N'pm_{0}:{1}' OR name = N'pm_{1}:{0}' 
                                            END
                                           ELSE
                                            BEGIN
                                                select 0 as result
                                            END"
                                       , userNick1, userNick2);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return int.Parse(reader["result"].ToString());
                }

            }
            catch (SqlException exception)
            {
                return 0;
                throw (exception);                
            }
            
        }

        public void CreatePersonalChat(string userNick1, string userNick2)
        {
            string query = string.Format(@"INSERT INTO Talks values (N'pm_{0}:{1}')"
                                            , userNick1, userNick2
                                            );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    AddPersonalTalkUserCon(userNick1, userNick2, GetTalkIdByName(string.Format("pm_{0}:{1}", userNick1, userNick2)));
                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;

            }
        }


        private void AddPersonalTalkUserCon(string userNick1, string userNick2, int talkId)
        {
            string query = string.Format(@" INSERT INTO UserTalks values ({0}, {1}, 1 )
                                            INSERT INTO UserTalks values ({0}, {2}, 1 )"
                                            , GetTalkIdByName(string.Format("pm_{0}:{1}", userNick1, userNick2))
                                            , GetUserIdByNick(userNick1)
                                            , GetUserIdByNick(userNick2));
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;

            }
        }

        public List<string> GetAllGroupTalksName()
        {
            List<string> result = new List<string>();

            string query = @"select t.name
                            from  Talks as t 
                            where t.name NOT Like 'pm_%'";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(reader[0].ToString());
                    }

                }

            }
            catch (SqlException exception)
            {
                throw (exception);
            }

            return result;
        }

        public int CheckIfGroupChatExists(string name)
        {
            string query = string.Format(@"if exists ( select id_talk 
                                                       from Talks 
                                                       where name = N'{0}' ) 
                                            BEGIN
                                                select id_talk as result
                                                from Talks 
                                                where name = N'{0}'  
                                            END
                                           ELSE
                                            BEGIN
                                                select 0 as result
                                            END"
                                       , name);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    return int.Parse(reader["result"].ToString());
                }

            }
            catch (SqlException exception)
            {
                return 0;
                throw (exception);
            }
        }


        public void CreateGroupChat(string name, int userId)
        {
            string query = string.Format(@"INSERT INTO Talks values (N'{0}')"
                                            , name
                                            );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    GrantAccessToTalk(userId, GetTalkIdByName(name));
                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;

            }
        }


        public void GrantAccessToTalk(int userId, int talkId)
        {
            string query = string.Format(@"if exists ( select status 
                                                       from UserTalks 
                                                       where id_talk = {0} AND id_user = {1} ) 
                                            BEGIN
                                                select 1 as result                                                 
                                            END
                                           ELSE
                                            BEGIN
                                                select 0 as result
                                            END"
                                            , userId, talkId
                                            );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (int.Parse(reader[0].ToString()) == 1)
                    {
                        UpdateConTalkStatus(userId, talkId);
                    }
                    else
                    {
                        CreateConTalk(userId, talkId);
                    }
                    
                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;
            }
        }

        public void CreateConTalk(int userId, int talkId)
        {
            string query = string.Format(@"INSERT INTO UserTalks values ({1},{0},1)"
                                            , userId, talkId
                                            );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;

            }
        }

        private void UpdateConTalkStatus(int userId, int talkId)
        {
            string query = string.Format(@"UPDATE UserTalks 
                                            set status = 1
                                            where id_talk = {1} AND id_user = {0}"
                                            , userId, talkId
                                            );
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                }

            }
            catch (SqlException e)
            {
                string ex = e.Message;
            }
        }


        #endregion Chat

    }
}
