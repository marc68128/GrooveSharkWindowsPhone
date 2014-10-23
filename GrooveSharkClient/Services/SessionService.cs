using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using Microsoft.Practices.Unity;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class SessionService : ISessionService
    {
        private readonly IGrooveSharkClient _client;
        public SessionService(IGrooveSharkClient client, string userName, string password)
        {
            _client = client;

            SessionIdObs = _client.CreateSession();
            SessionIdObs.BindTo(this, self => self.SessionId);

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName))
            {
                var userObs = SessionIdObs.SelectMany(s => _client.Login(userName, password, s));
                userObs.Subscribe(_ => RefreshUserCommand.Execute(null));
            }

            RefreshUserCommand = ReactiveCommand.CreateAsyncObservable(_ => SessionIdObs.SelectMany(_client.GetUserInfo));
            UserInfoObs = RefreshUserCommand;
            UserInfoObs.BindTo(this, self => self.User);

            IsSessionIdAvailable = SessionIdObs.Select(s => !string.IsNullOrEmpty(s)).StartWith(!string.IsNullOrEmpty(SessionId));
        }

        public IObservable<bool> IsSessionIdAvailable { get; private set; }
        public IObservable<string> SessionIdObs { get; private set; }
        public IObservable<User> UserInfoObs { get; private set; }
        public User User { get; private set; }
        public string SessionId { get; private set; }

        public ReactiveCommand<User> RefreshUserCommand { get; set; }
    }
}
