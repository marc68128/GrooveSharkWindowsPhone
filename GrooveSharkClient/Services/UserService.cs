using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class UserService : ReactiveObject, IUserService
    {

        public UserService(IGrooveSharkClient client, ISessionService session, string username = null, string password = null)
        {
            Username = username;
            Password = password;

            #region LoginCommand

            var canLogin =
                session.IsDataAvailableObs.CombineLatest(
                    this.WhenAnyValue(self => self.Username)
                        .Select(u => !string.IsNullOrEmpty(u))
                        .CombineLatest(this.WhenAnyValue(self => self.Password)
                            .Select(p => !string.IsNullOrEmpty(p)), (a, b) => a & b), (b, b1) => b1 & b);

            LoginCommand = ReactiveCommand.CreateAsyncObservable(canLogin, _ =>
                {
                    IsLoading = true;
                    return client.Login(username, password, session.SessionId);
                });

            LoginCommand.Where(u => u != null).Subscribe(u =>
            {
                Debug.WriteLine("[UserService] User : " + u.FName);
                IsLoading = false;
                IsUserAvailable = true;
                ConnectedUser = u; 
            });

            LoginCommand.ThrownExceptions.Subscribe(ex =>
            {
                Debug.WriteLine("[UserService]" + ex);
                IsLoading = false;
                IsUserAvailable = false; 
            });
            #endregion

            #region RefreshUser

            RefreshConnectedUserCommand = ReactiveCommand.CreateAsyncObservable(session.IsDataAvailableObs, _ =>
            {
                IsLoading = true;
                IsUserAvailable = false; 
                return client.GetUserInfo(session.SessionId);
            });

            RefreshConnectedUserCommand.Where(u => u != null).Subscribe(u =>
            {
                Debug.WriteLine("[UserService] User : " + u.FName);
                IsLoading = false;
                IsUserAvailable = true;
                ConnectedUser = u; 
            });


            #endregion

            if (username != null && password != null)
                LoginCommand.CanExecuteObservable.Where(b => b).Take(1).Subscribe(_ => LoginCommand.Execute(null));
            else
                RefreshConnectedUserCommand.CanExecuteObservable.Where(b => b).Take(1).Subscribe(_ => RefreshConnectedUserCommand.Execute(null));

        }

        #region UserInfos

        private string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        #endregion

        #region ReactiveCommands

        public ReactiveCommand<User> LoginCommand { get; protected set; }
        public ReactiveCommand<User> RefreshConnectedUserCommand { get; private set; }

        #endregion


        private User _connectedUser;
        public User ConnectedUser
        {
            get { return _connectedUser; }
            set { this.RaiseAndSetIfChanged(ref _connectedUser, value); }
        }
        public IObservable<User> ConnectedUserObs { get { return this.WhenAnyValue(self => self.ConnectedUser); } }



        #region Loading

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }
        public IObservable<bool> IsLoadingObs
        {
            get { return this.WhenAnyValue(self => self.IsLoading); }
        }

        private bool _isUserAvailable;
        public bool IsUserAvailable
        {
            get { return _isUserAvailable; }
            set { this.RaiseAndSetIfChanged(ref _isUserAvailable, value); }
        }
        public IObservable<bool> IsUserAvailableObs
        {
            get { return this.WhenAnyValue(self => self.IsUserAvailable); }
        }

        #endregion

    }
}
