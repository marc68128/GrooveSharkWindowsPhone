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
        IObservable<bool> IsLoadingObs { get; }
        IObservable<bool> IsSessionAvailableObs { get; }

        IObservable<string> SessionIdObs { get; }

        string SessionId { get; }
        ReactiveCommand<string> LoadSessionId { get; }
    }
}
