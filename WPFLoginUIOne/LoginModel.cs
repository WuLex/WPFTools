using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WPF_LoginUI
{
    public class LoginModel
    {
        public string? Account { get; set; }
        public string? Password { get; set; }

        public bool IsAuth(string? Account, string? Password)
        {
            string? account = ConfigurationManager.AppSettings["username"];
            string? password = ConfigurationManager.AppSettings["password"];
            return !string.IsNullOrEmpty(Account) && !string.IsNullOrEmpty(Password) && Account == account && Password == password;
        }
    }
}
