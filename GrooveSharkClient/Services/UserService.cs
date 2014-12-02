using System;
using System.Diagnostics;
using System.Reactive.Linq;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class UserService : ReactiveObject, IUserService
    {

        public UserService(IGrooveSharkClient client, ISessionService session, LoadingService loadingService, string username = null, string password = null)
        {

            Username = username;
            Password = password;

            #region LoginCommand

            LoginCommand = ReactiveCommand.CreateAsyncObservable(session.IsDataAvailableObs, _ =>
            {
                loadingService.AddLoadingStatus("Login...");
                return client.Login(Username, Password, session.SessionId);
            });

            LoginCommand.Where(u => u != null).Subscribe(u =>
            {
                Debug.WriteLine("[UserService] User : " + u.FName);
                loadingService.RemoveLoadingStatus("Login...");
                IsDataAvailable = true;
                ConnectedUser = u;
            });

            LoginCommand.ThrownExceptions.Subscribe(ex =>
            {
                Debug.WriteLine("[UserService]" + ex);
                ThrownException = ex;
                loadingService.RemoveLoadingStatus("Login...");
                IsDataAvailable = false;
            });
            #endregion

            #region RefreshUser

            RefreshConnectedUserCommand = ReactiveCommand.CreateAsyncObservable(session.IsDataAvailableObs, _ =>
            {
                loadingService.AddLoadingStatus("Refresh User...");
                IsDataAvailable = false;
                return client.GetUserInfo(session.SessionId);
            });

            RefreshConnectedUserCommand.Where(u => u != null).Subscribe(u =>
            {
                Debug.WriteLine("[UserService] User : " + u.FName);
                loadingService.RemoveLoadingStatus("Refresh User...");
                IsDataAvailable = true;
                ConnectedUser = u;
            });

            RefreshConnectedUserCommand.ThrownExceptions.Subscribe(ex =>
            {
                Debug.WriteLine("[UserService]" + ex);
                ThrownException = ex;
                loadingService.RemoveLoadingStatus("Refresh User...");
                IsDataAvailable = false;
            });


            #endregion

            if (username != null && password != null)
                LoginCommand.CanExecuteObservable.Where(b => b).Take(1).Subscribe(_ => LoginCommand.Execute(null));

        }

        #region ReactiveCommands

        public ReactiveCommand<User> LoginCommand { get; protected set; }
        public ReactiveCommand<User> RefreshConnectedUserCommand { get; private set; }

        #endregion

        #region User

        private User _connectedUser;
        public User ConnectedUser
        {
            get { return _connectedUser; }
            private set { this.RaiseAndSetIfChanged(ref _connectedUser, value); }
        }
        public IObservable<User> ConnectedUserObs
        {
            get { return this.WhenAnyValue(self => self.ConnectedUser); }
        }

        public string Username { get; set; }
        public string Password { get; set; }

        #endregion

        #region Loading

        private bool _isDataAvailable;
        public bool IsDataAvailable
        {
            get { return _isDataAvailable; }
            private set { this.RaiseAndSetIfChanged(ref _isDataAvailable, value); }
        }
        public IObservable<bool> IsDataAvailableObs
        {
            get { return this.WhenAnyValue(self => self.IsDataAvailable); }
        }

        #endregion

        #region Exceptions

        private Exception _thrownException;
        public Exception ThrownException
        {
            get { return _thrownException; }
            private set { this.RaiseAndSetIfChanged(ref _thrownException, value); }
        }
        public IObservable<Exception> ThrownExceptionObs
        {
            get { return this.WhenAnyValue(self => self.ThrownException); }
        }

        #endregion
    }
}
