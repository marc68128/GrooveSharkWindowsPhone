using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Contracts
{
    public interface ILoadingService
    {
        string CurrentStatus { get; }
        bool IsLoading { get; }
        void RemoveLoadingStatus(string status);
        void AddLoadingStatus(string status);
        IObservable<bool> IsLoadingObs { get; }
        IObservable<string> CurrentStatusObs { get; }
    }
}
