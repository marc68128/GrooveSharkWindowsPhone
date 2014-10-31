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
            if (username != null && password != null)
            {
                session.IsSessionAvailableObs.Where(x => x).Subscribe(_ =>
                {
                    IsLoading = true;
                    var userObs = client.Login(username, password, session.SessionId);
                    userObs.Do(x => IsLoading = false).Where(u => u != null).BindTo(this, self => self.ConnectedUser);
                });
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
