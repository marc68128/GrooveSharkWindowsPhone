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
        ReactiveCommand<User> RefreshConnectedUserCommand { get; }
        IObservable<bool> IsLoadingObs { get; }
        IObservable<bool> IsUserAvailableObs { get; }
        ReactiveCommand<User> LoginCommand { get; }

        string Username { get; set; }
        string Password { get; set; }


    }
}
