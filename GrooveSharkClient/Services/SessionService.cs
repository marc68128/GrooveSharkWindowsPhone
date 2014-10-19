using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using Microsoft.Practices.Unity;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class SessionService : ISessionService
    {
        public SessionService(IGrooveSharkClient client)
        {
            SessionIdObs = client.CreateSession();             
            SessionIdObs.BindTo(this, self => self.SessionId);
            IsSessionIdAvailable =
                SessionIdObs.Select(s => !string.IsNullOrEmpty(s)).StartWith(!string.IsNullOrEmpty(SessionId));
        }

        public IObservable<bool> IsSessionIdAvailable { get; private set; }
        public IObservable<string> SessionIdObs { get; private set; }
        public string SessionId { get; private set; }
    }
}
