using System;
using ReactiveUI;

namespace GrooveSharkClient.Contracts
{
    public interface ISessionService : IService
    {
        IObservable<string> SessionIdObs { get; }
        string SessionId { get; }

        ReactiveCommand<string> LoadSessionId { get; }
    }
}
