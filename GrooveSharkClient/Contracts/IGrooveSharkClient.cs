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
        IObservable<User> Register(string emailAddress, string password, string fullName, string session, string userName = null);
        IObservable<bool> Logout(string session);
        IObservable<Song[]> GetPopularSongToday(string session);
        IObservable<User> GetUserInfo(string session);
        IObservable<Playlist[]> GetUserPlaylists(string session, int limit = 0);
        IObservable<Playlist> GetPlaylist(string session, string playlistId, int limit = 0);
        IObservable<Playlist> GetPlaylistInfos(string session, string playlistId);
        IObservable<Song[]> GetUserFavoriteSongs(string session, int limit = 0);
        IObservable<Song[]> GetUserLibrarySongs(string session, int limit = 0);
        IObservable<bool> RemoveUserFavoriteSongs(string songId, string session);
        IObservable<bool> AddPlaylist(int[] songIds, string playlistName, string session);
        IObservable<bool> SetPlaylistSongs(int[] songIds, int playlistId, string session);
        IObservable<bool> AddSongToUserFavourites(string session, string songId);
        IObservable<Song[]> SearchSong(string query, string country, string session, int limit = 0, int offset = 0);
        IObservable<Playlist[]> SearchPlaylist(string query, string session, int limit = 0);
        IObservable<Artist[]> SearchArtist(string query, string session, int limit = 0);
        IObservable<Album[]> SearchAlbum(string query, string session, int limit = 0);

        IObservable<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchAll(string query, string country, string session, int limit = 0, int offset = 0);

        IObservable<StreamInfo> GetStreamInfo(string session, string country, string songId, bool lowBitrate = false);
    }
}
