using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Models;
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


            //_session.UserInfoObs.BindTo(this, self => self.CurrentUser);

            //_session.UserInfoObs.ObserveOn(RxApp.MainThreadScheduler).Do(_ => IsLoading = false).BindTo(this, self => self.CurrentUser); 

            this.WhenAnyValue(self => self.CurrentUser)
                .Where(u => u != null)
                .Subscribe(u =>
                {
                    if (u.UserID != 0)
                    {
                        CurrentUserMessage = "Hello " + u.FName;
                        ToggleConnectionLabel = "Logout";

                    }
                    else
                    {
                        CurrentUserMessage = "You are not connected !";
                        ToggleConnectionLabel = "Login";
                    }
                });

        }

        private void InitCommands()
        {
            RefreshCurrentUserCommand = ReactiveCommand.Create();
            RefreshCurrentUserCommand.Subscribe(_ =>
            {
                ShowLoader = true;
               // _session.RefreshUserCommand.Execute(null);
            });


            LogoutCommand = ReactiveCommand.CreateAsyncObservable(_ => _session.SessionIdObs.SelectMany(s => _client.Logout(s)));
            LogoutCommand
                .Do(s => RefreshCurrentUserCommand.Execute(null))
                .Subscribe(b =>
                {
                    if (b)
                    {
                        AppSettings.RemoveCredential();
                    }
                    var message = b ? "Logout successfull" : "Error : failed to logout";
                    new MessageDialog(message).ShowAsync();
                });

            ToggleConnectionCommand = ReactiveCommand.Create(this.WhenAnyValue(self => self.CurrentUser).Select(u => u != null));
            ToggleConnectionCommand.Subscribe(_ =>
            {
                if (CurrentUser.UserID == 0)
                {
                    NavigationHelper.Navigate(typeof (LoginView), typeof (SettingsView));
                }
                else
                {
                    LogoutCommand.Execute(null);
                }
            });


        }

        public ReactiveCommand<object> RefreshCurrentUserCommand { get; set; }

        public ReactiveCommand<object> ToggleConnectionCommand { get; set; }

        public ReactiveCommand<bool> LogoutCommand { get; set; }

        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { this.RaiseAndSetIfChanged(ref _currentUser, value); }
        }

        private string _currentUserMessage;
        public string CurrentUserMessage
        {
            get { return _currentUserMessage; }
            set { this.RaiseAndSetIfChanged(ref _currentUserMessage, value); }
        }

        private string _toggleConnectionLabel;
        public string ToggleConnectionLabel
        {
            get { return _toggleConnectionLabel; }
            set { this.RaiseAndSetIfChanged(ref _toggleConnectionLabel, value); }
        }

        private string _registerLabel;
        public string RegisterLabel
        {
            get { return _registerLabel; }
            set { this.RaiseAndSetIfChanged(ref _registerLabel, value); }
        }
        
        
    }
}
