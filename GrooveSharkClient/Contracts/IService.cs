using System;

namespace GrooveSharkClient.Contracts
{
    public interface IService
    {
        IObservable<bool> IsDataAvailableObs { get; }
        IObservable<Exception> ThrownExceptionObs { get; } 
    }
}
