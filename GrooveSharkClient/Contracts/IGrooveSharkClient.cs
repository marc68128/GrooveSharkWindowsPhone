using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;

namespace GrooveSharkClient.Contracts
{
    public interface IGrooveSharkClient
    {
        IObservable<string> CreateSession();
        IObservable<User> Login(string userName, string md5Password, string session);
        IObservable<bool> Logout(string session);
        IObservable<Song[]> GetPopularSongToday(string session);
        IObservable<User> GetUserInfo(string session);
    }
}
