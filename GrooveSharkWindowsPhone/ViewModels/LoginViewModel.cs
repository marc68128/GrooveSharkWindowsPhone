using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using Microsoft.Practices.Unity;
using ReactiveUI;
using Splat;
using xBrainLab.Security.Cryptography;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {


        public LoginViewModel()
        {
            Title = "Login";
            UserNameLabel = "UserName";
            PasswordLabel = "Password";
#if DEBUG
            UserName = "marc68128";
#endif

            this.WhenAnyValue(self => self.UserName).BindTo(_user, service => service.Username);
            this.WhenAnyValue(self => self.Password).Select(MD5.GetHashString).BindTo(_user, service => service.Password);


            LoginCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _user.Username = UserName;
                _user.Password = MD5.GetHashString(Password);
                return _user.LoginCommand.ExecuteAsync(null);
            }); 

            LoginCommand.Where(u => u != null).Subscribe(u =>
            {
                if (u.UserID == 0)
                {
                    var messageDialog = new MessageDialog(string.Format("Invalid login/password combinaison !"));
                    messageDialog.ShowAsync();
                }
                else
                {
                    AppSettings.SaveCredential(UserName, MD5.GetHashString(Password));
                    if (NavigationHelper.CanGoBack())
                        NavigationHelper.GoBack();
                    else
                        NavigationHelper.Navigate(typeof (HomeView));
                }
                
            });
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { this.RaiseAndSetIfChanged(ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _userNameLabel;
        public string UserNameLabel
        {
            get { return _userNameLabel; }
            set { this.RaiseAndSetIfChanged(ref _userNameLabel, value); }
        }

        private string _passwordLabel;
        public string PasswordLabel
        {
            get { return _passwordLabel; }
            set { this.RaiseAndSetIfChanged(ref _passwordLabel, value); }
        }


        public ReactiveCommand<User> LoginCommand { get; set; }

    }
}