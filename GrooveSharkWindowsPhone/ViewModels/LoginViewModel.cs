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
using ReactiveCommand = ReactiveUI.Legacy.ReactiveCommand;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private IGrooveSharkClient _client;
        private ISessionService _sessionService;

        public LoginViewModel()
        {
            Title = "Login";

            _client = Locator.Current.GetService<IGrooveSharkClient>();
            _sessionService = Locator.Current.GetService<ISessionService>();
            

            LoginCommand = new ReactiveCommand(_sessionService.IsSessionIdAvailable);
            var userObs = LoginCommand.RegisterAsync(_ =>
            {
                IsLoading = true; 
                return _client.Login(UserName, MD5.GetHashString(Password), _sessionService.SessionId);
            });

            userObs.Subscribe(u =>
            {
                MessageDialog messageDialog = new MessageDialog(string.Format("Vous êtes bien connecté :\nEmail : {0}\nName : {1}", u.Email, u.FName));
                messageDialog.ShowAsync();
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

        private bool _isLoading;    
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public IObservable<bool> IsLoadingObs { get { return this.WhenAnyValue(self => self.IsLoading); } }
        

        public ReactiveCommand LoginCommand { get; set; }

    }
}
