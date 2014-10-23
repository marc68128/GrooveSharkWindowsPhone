using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;

namespace GrooveSharkClient.Contracts
{
    public interface IGrooveSharkClient
    {
        IObservable<string> CreateSession();
        IObservable<CountryInfo> GetCountry();
        IObservable<User> Login(string userName, string md5Password, string session);
        IObservable<bool> Logout(string session);
        IObservable<Song[]> GetPopularSongToday(string session);
        IObservable<User> GetUserInfo(string session);

        IObservable<Song[]> SearchSong(string query, string country, string session, int limit = 0, int offset = 0);
        IObservable<Playlist[]> SearchPlaylist(string query, string session, int limit = 0);
        IObservable<Artist[]> SearchArtist(string query, string session, int limit = 0);
        IObservable<Album[]> SearchAlbum(string query, string session, int limit = 0);

        IObservable<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchAll(string query, string country, string session, int limit = 0, int offset = 0);

    }
}
