using System;
using System.Reactive.Linq;
using Windows.UI.Popups;
using GrooveSharkClient.Models.Entity;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = "Settings";
            RegisterLabel = "Register";
            InitCommands();


            _user.ConnectedUserObs.BindTo(this, self => self.CurrentUser);


            IsUserConnectedObs = 
                this.WhenAnyValue(self => self.CurrentUser)
                .Select(u => u != null && u.UserID != 0);


        }

        private void InitCommands()
        {
            RefreshCurrentUserCommand = _user.RefreshConnectedUserCommand;

            NavigateToRegisterCommand = ReactiveCommand.Create();
            NavigateToRegisterCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(RegisterView)));

            NavigateToLoginCommand = ReactiveCommand.Create();
            NavigateToLoginCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(LoginView)));

            LogoutCommand = ReactiveCommand.CreateAsyncObservable(_session.IsDataAvailableObs, _ => _client.Logout(_session.SessionId));
            LogoutCommand.Subscribe(b => {
                RefreshCurrentUserCommand.Execute(null);
                if (b)
                {
                    AppSettings.RemoveCredential();
                }
                var message = b ? "Logout successfull" : "Error : failed to logout";
                new MessageDialog(message).ShowAsync();
            });

        }

        public ReactiveCommand<object> NavigateToRegisterCommand { get; private set; }
        public ReactiveCommand<object> NavigateToLoginCommand { get; private set; }
        public ReactiveCommand<bool> LogoutCommand { get; private set; }
        public ReactiveCommand<User> RefreshCurrentUserCommand { get; private set; }


        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { this.RaiseAndSetIfChanged(ref _currentUser, value); }
        }

        public IObservable<bool> IsUserConnectedObs { get; set; } 

        private string _registerLabel;
        public string RegisterLabel
        {
            get { return _registerLabel; }
            set { this.RaiseAndSetIfChanged(ref _registerLabel, value); }
        }


    }
}
