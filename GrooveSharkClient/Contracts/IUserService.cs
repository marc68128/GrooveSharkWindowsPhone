using System;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkClient.Contracts
{
    public interface IUserService : IService
    {
        User ConnectedUser { get; }
        IObservable<User> ConnectedUserObs { get; }

        ReactiveCommand<User> RefreshConnectedUserCommand { get; }
        ReactiveCommand<User> LoginCommand { get; }

        string Username { get; set; }
        string Password { get; set; }
    }
}
