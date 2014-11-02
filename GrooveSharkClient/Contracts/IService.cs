using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Contracts
{
    public interface IService
    {
        IObservable<bool> IsLoadingObs { get; }
        IObservable<bool> IsDataAvailableObs { get; }

        IObservable<Exception> ThrownExceptionObs { get; } 
    }
}
