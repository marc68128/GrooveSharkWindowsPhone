using System;
using System.Collections.Generic;
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

            #region Login

            var canLogin =
                session.IsSessionAvailableObs.CombineLatest(
                    this.WhenAnyValue(self => self.Username)
                        .Select(u => !string.IsNullOrEmpty(u))
                        .CombineLatest(this.WhenAnyValue(self => self.Password)
                            .Select(p => !string.IsNullOrEmpty(p)), (a, b) => a & b), (b, b1) => b1 & b);

            Login = ReactiveCommand.CreateAsyncObservable(canLogin, _ =>
                {
                    IsLoading = true;
                    return client.Login(username, password, session.SessionId);
                });

            Login.Where(u => u != null).Do(_ => IsLoading = false).BindTo(this, self => self.ConnectedUser);

            #endregion


            if (username != null && password != null)
            {
                Login.CanExecuteObservable.Where(b => b).Take(1).Subscribe(_ => Login.Execute(null));
            }

            RefreshConnectedUser = ReactiveCommand.CreateAsyncObservable(session.IsSessionAvailableObs, _ =>
            {
                IsLoading = true;
                return client.GetUserInfo(session.SessionId);
            });

            RefreshConnectedUser
                .Do(_ => IsLoading = false)
                .Where(u => u != null)
                .BindTo(this, self => self.ConnectedUser);

        }

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

        public ReactiveCommand<User> Login { get; protected set; }

        private User _connectedUser;
        public User ConnectedUser
        {
            get { return _connectedUser; }
            set { this.RaiseAndSetIfChanged(ref _connectedUser, value); }
        }
        public IObservable<User> ConnectedUserObs { get { return this.WhenAnyValue(self => self.ConnectedUser); } }

        public ReactiveCommand<User> RefreshConnectedUser { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }
        public IObservable<bool> IsLoadingObs { get { return this.WhenAnyValue(self => self.IsLoading); } }
    }
}
