using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;



namespace WpfApplication1.BLL
{
    class BLL : UI.IBLL
    {
        private DAL.DAL dal = new DAL.DAL();

        public BLL(){}

        #region Connection

        public bool CheckConnection(string conString)
        {
            DAL.DAL.connectionStr = conString;
            if (dal.CheckConnectionToServer())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckDBTables()
        {
            if (!dal.CheckDBTables())
            {
                dal.CreateDBTables();
            }
        }


        #endregion Connection


        #region Registration/Login

        public bool CreateUser(string nickName, string password)
        {
            byte[] hashedPass = CreatePasswordHash(password);
            if (dal.CheckUserNick(nickName))
            {
                return false;
            }
            if (dal.CreateUser(nickName, CreatePasswordHash(password)))
            {
                return true;
            }
            
            return false;
            
        }

        public bool CheckUserNick(string nickName)
        {
            return dal.CheckUserNick(nickName);
        }

        public bool CheckUserPass(string nickName, string password)
        {
            return dal.CheckUserPass( nickName, BitConverter.ToString(CreatePasswordHash(password)).Replace("-", ""));
        }

        public byte[] CreatePasswordHash(string inputString)
        {
	        HashAlgorithm algorithm = SHA1.Create();  
	        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public int GetUserIdByNick(string nickName)
        {
            return dal.GetUserIdByNick(nickName);
        }

        public void ChangeUserStatus(int userId, int status)
        {
            dal.ChangeUserStatus(userId, status);
        }

        #endregion Registration/Login

        #region Chat


        public int GetTalkMsgCountByName(string talkName)
        {
            return dal.GetTalkMsgCountByName(talkName);
        }

        

        public List<string> GetAllTalkMsgsByName(string talkName)
        {
            return dal.GetAllTalkMsgsByName(talkName);
        }

        public void SendMessage(string talkName, int userId, string message)
        {
            dal.SendMessage(talkName, userId, message);
            
            
        }

        public List<string> GetUsersOnlineExceptCurrent(int userId)
        {
            return dal.GetUsersOnlineExceptCurrent(userId);
        }

        public string GetUserNickById(int userId)
        {
            return dal.GetUserNickById(userId);
        }

        public int CheckIfPersonalChatExists(string userNick1, string userNick2)
        {
            return dal.CheckIfPersonalChatExists(userNick1, userNick2);
        }

        public  void CreatePersonalChat(string userNick1, string userNick2)
        {
            dal.CreatePersonalChat(userNick1, userNick2);
        }

        public string GetTalkNameById(int talkId)
        {
            return dal.GetTalkNameById(talkId);
        }

        public List<string> GetAllGroupTalksName()
        {
            return dal.GetAllGroupTalksName();
        }

        public int CheckIfGroupChatExists(string name)
        {
            return dal.CheckIfGroupChatExists(name);
        }

        public void CreateGroupChat(string name, int userId)
        {
            dal.CreateGroupChat(name, userId);
        }

        public void CreateConTalk(int userId, int talkId)
        {
            dal.CreateConTalk(userId, talkId);
        }

        public int GetTalkIdByName(string talkName)
        {
            return dal.GetTalkIdByName(talkName);
        }

        public void GrantAccessToTalk(int userId, int talkId)
        {
           dal.GrantAccessToTalk(userId,talkId);
        }

        public bool CheckTalkForNewMsgs(string talkName)
        {
            return dal.CheckTalkForNewMsgs(talkName);
        }



        #endregion

    }
}
