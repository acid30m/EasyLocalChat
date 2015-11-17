using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.UI
{
    interface IBLL
    {

        bool CheckConnection(string conString);
        bool CreateUser(string nickName, string password);
        byte[] CreatePasswordHash(string password);
        bool CheckUserNick(string nickName);
        bool CheckUserPass(string nickName, string password);
        void CheckDBTables();
        int GetUserIdByNick(string nickName);
        int GetTalkMsgCountByName(string talkName);
        List<string> GetAllTalkMsgsByName(string talkName);
        void SendMessage(string talkName,int userId,string message);
        
    }
}
