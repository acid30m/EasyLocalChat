using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.BLL
{
    interface IDAL
    {
        
        bool CheckConnectionToServer();

        bool CreateUser(string name, byte[] password);

        bool CheckDBTables();

        bool CreateDBTables();

        
        bool CheckUserNick(string nickName);

        bool CheckUserPass(string password);

        int GetUserIdByNick(string nickName);

        int GetTalkMsgCountByName(string talkName);

        List<string> GetAllTalkMsgsByName(string talkName);

        void SendMessage(string talkName,int userId,string message);

        List<string> GetUsersOnlineExceptCurrent(int userId);

        string GetUserNickById(int userId);

        int CheckIfPersonalChatExists(string userNick1, string userNick2);

        void CreatePersonalChat(string userNick1, string userNick2);

        string GetTalkNameById(int talkId);

        int GetTalkIdByName(string talkName);

        List<string> GetAllGroupTalksName();

        int CheckIfGroupChatExists(string name);

        void CreateGroupChat(string name, int userId);

        void ChangeUserStatus(int userId, int status);

        void CreateConTalk(int userId, int talkId);


    }
}
