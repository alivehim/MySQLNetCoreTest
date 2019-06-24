using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core.Model
{
    public static class LoginType
    {
        public readonly static string ByPassword = "1";
        public readonly static string ByPhone = "2";
        public readonly static string ByTourist = "3";
    }

    public class LoginHandler
    {


        public LoginHandler(string loginType, Type handlerType)
        {
            LoginType = loginType;
            Handler = handlerType;
        }

        public string LoginType { get; set; }
        public Type Handler { get; set; }
    }
}
