using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Contracts
{
    public interface ISessionService
    {
        IObservable<bool> IsSessionIdAvailable { get; }
        IObservable<string> SessionIdObs { get; }
        string SessionId { get; }
    }
}
