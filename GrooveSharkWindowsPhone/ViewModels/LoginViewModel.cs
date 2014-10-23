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
            UserName = "marc68128";


            LoginCommand =
                ReactiveCommand.CreateAsyncObservable(
                    _ => _session.SessionIdObs.SelectMany(s => _client.Login(UserName, MD5.GetHashString(Password), s)));


            LoginCommand.Where(u => u != null).Subscribe(u =>
            {
                if (u.UserID == 0)
                {
                    var messageDialog = new MessageDialog(string.Format("Invalid login/password combinaison !"));
                    messageDialog.ShowAsync();
                }
                else
                {
                    AppSettings.AddValue("UserName", UserName);
                    AppSettings.AddValue("Md5Password", MD5.GetHashString(Password));
                    NavigationHelper.GoBack();
                }
                
            });

            LoginCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Subscribe(e => new MessageDialog(e.Description).ShowAsync());
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
