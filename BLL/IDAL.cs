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

    }
}
