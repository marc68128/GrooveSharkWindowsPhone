using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkClient.Contracts
{
    public interface IUserService
    {
        User ConnectedUser { get; }
        IObservable<User> ConnectedUserObs { get; }
        ReactiveCommand<User> RefreshConnectedUser { get; }
        IObservable<bool> IsLoadingObs { get; } 
    }
}
