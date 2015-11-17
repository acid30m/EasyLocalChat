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
                                                date_send date not null
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

        #endregion Registration/Login
    
    
    }
}
