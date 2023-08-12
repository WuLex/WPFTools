using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF_LoginUI
{
    public partial class LoginVM:ObservableObject
    {
        private MainWindow _main;
        public LoginVM(MainWindow main)
        {
            _main = main; 
        }

        private LoginModel _LoginM = new LoginModel();

        [ObservableProperty]
        public string? _account;

        [ObservableProperty]
        public string? _password;

        [ObservableProperty]
        public bool _checked = false;

        bool CanLoginExecute() => Checked;

        [RelayCommand]
        private void LoginFunc()
        {
            if (_LoginM.IsAuth(Account, Password))
            {
                Index index = new Index();
                index.Show();
                _main.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect Account or Password!");
                Account = "";
                Password = "";
            }
        }
    }
}
