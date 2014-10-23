using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkClient.Contracts
{
    public interface ISessionService
    {
        IObservable<bool> IsSessionIdAvailable { get; }
        IObservable<string> SessionIdObs { get; }
        IObservable<User> UserInfoObs { get; }
        User User { get; }
        string SessionId { get; }

        ReactiveCommand<User> RefreshUserCommand { get; set; }
    }
}
