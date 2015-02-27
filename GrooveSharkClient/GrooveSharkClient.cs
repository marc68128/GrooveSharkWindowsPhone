using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Helpers;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using Newtonsoft.Json;

namespace GrooveSharkClient
{
    public class GrooveSharkClient : IGrooveSharkClient
    {
        private const string ServerURI = "https://api.grooveshark.com/ws3.php?sig={0}";
        private const string ServerKey = "winphone_marc2";
        private const string ServerSecret = "86b91a1ef536883aa04243b863db7281";
        private const string RequestPatern = "{\"method\":\"{0}\",\"parameters\":{{1}},\"header\":{{2}}}";
        private readonly TimeSpan _defautTimeOut = new TimeSpan(0, 0, 15);

        private readonly NetworkClient _networkClient;

        public GrooveSharkClient()
        {
            _networkClient = new NetworkClient();
        }

        private bool ShouldHaveQuote(object value)
        {
            return (value is string && !(value as string).StartsWith("{") && !(value as string).EndsWith("}") && !(value as string).StartsWith("[") && !(value as string).EndsWith("]"));
        }


        private async Task<HttpResponseMessage> SendHttpRequest(string method, Dictionary<string, object> parameters = null, string sessionId = null, CancellationToken ct = default(CancellationToken), TimeSpan timeOut = default(TimeSpan))
        {
            var header = "\"wsKey\":\"" + ServerKey + "\"";
            if (sessionId != null)
                header += ",\"sessionID\":\"" + sessionId + "\"";

            var parameter = "";
            if (parameters != null)
                parameter = parameters
                    .Select(kvp => "\"" + kvp.Key + "\":" + (ShouldHaveQuote(kvp.Value) ? "\"" : "") + kvp.Value + (ShouldHaveQuote(kvp.Value) ? "\"" : ""))
                    .Aggregate((a, b) => a + "," + b);


            var content = RequestPatern
                .Replace("{0}", method)
                .Replace("{1}", parameter)
                .Replace("{2}", header);


            var sig = HmacMd5.Hash(ServerSecret, content);
            var uri = string.Format(ServerURI, sig);

            Debug.WriteLine("[Networking]" + uri + "\n[Networking]" + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType.MediaType = "application/json";

            return await _networkClient.PostAsync(uri, httpContent, ct, null, timeOut);
        }

        private async Task<T> Parse<T>(Task<HttpResponseMessage> res, Func<GrooveSharkResult, T> creator)
        {
            var response = await res;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var grooveSharkResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                if (grooveSharkResult.Errors != null && grooveSharkResult.Errors.Any())
                    throw grooveSharkResult.Errors.First();

                return creator(grooveSharkResult); 
            }
            throw new WebException("Unable to access the server !\nVerify your network connection.");
        }



         

        public IObservable<string> CreateSession()
        {
            Func<string> work = () =>
            {
                var response = SendHttpRequest("startSession", timeOut: _defautTimeOut);
                return Parse(response, result => result.Result.SessionID).Result;
            };

            return Observable.Start(work);

        }

        public IObservable<CountryInfo> GetCountry()
        {
            Func<CountryInfo> work = () =>
            {
                var response = SendHttpRequest("getCountry", timeOut: _defautTimeOut);
                return Parse(response, result => new CountryInfo(result)).Result;
            };
            return Observable.Start(work);
        }

        public IObservable<User> Login(string userName, string md5Password, string session)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "login", userName }, { "password", md5Password } };

                var response = SendHttpRequest("authenticate", param, session, timeOut: _defautTimeOut);
                return Parse(response, result => new User(result)).Result;
            });
        }

        public IObservable<User> Register(string emailAddress, string password, string fullName, string session, string userName = null)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "emailAddress", emailAddress }, { "password", password }, { "fullName", fullName } };
                if (userName != null)
                    param.Add("username", userName);


                var response = SendHttpRequest("registerUser", param, session, timeOut: _defautTimeOut);
                return Parse(response, result => new User(result)).Result;
            });
        }

        public IObservable<bool> Logout(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("logout", null, session, timeOut: _defautTimeOut);
                return Parse(response, result => result.Result.Success).Result;
            });
        }

        public IObservable<User> GetUserInfo(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("getUserInfo", null, session, timeOut: _defautTimeOut);
                return Parse(response, result => new User(result)).Result;
            });
        }

        public IObservable<Song[]> GetPopularSongToday(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("getPopularSongsToday", sessionId: session, timeOut: _defautTimeOut);
                return Parse(response, result => result.Result.Songs).Result; 
            });
        }

        #region Search

        public IObservable<Song[]> SearchSong(string query, string country, string session, int limit = 0, int offset = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query }, { "country", country } };
                if (limit != 0)
                    param.Add("limit", limit);
                if (offset != 0)
                    param.Add("offset", offset);

                var response = SendHttpRequest("getSongSearchResults", param, session);
                return Parse(response, result => result.Result.Songs).Result;
            });
        }

        public IObservable<Playlist[]> SearchPlaylist(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getPlaylistSearchResults", param, session);
                return Parse(response, result => result.Result.Playlists).Result;
            });
        }

        public IObservable<Artist[]> SearchArtist(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getArtistSearchResults", param, session);
                return Parse(response, result => result.Result.Artists).Result;
            });
        }

        public IObservable<Album[]> SearchAlbum(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getAlbumSearchResults", param, session);
                return Parse(response, result => result.Result.Albums).Result;
            });
        }

        public IObservable<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchAll(string query, string country, string session, int limit = 0, int offset = 0)
        {
            var songsObs = SearchSong(query, country, session, limit, offset);
            var playlistsObs = SearchPlaylist(query, session, limit);
            var artistsObs = SearchArtist(query, session, limit);
            var albumsObs = SearchAlbum(query, session, limit);

            return Observable.When(songsObs.And(playlistsObs).And(artistsObs).And(albumsObs).Then(Tuple.Create));
        }

        #endregion

        public IObservable<Playlist[]> GetUserPlaylists(string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object>();
                if (limit != 0)
                    param.Add("limit", limit);
                else
                    param = null;

                var response = SendHttpRequest("getUserPlaylists", param, session);
                return Parse(response, result => result.Result.Playlists).Result;

            });
        }

         

        public IObservable<Song[]> GetUserFavoriteSongs(string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object>();
                if (limit != 0)
                    param.Add("limit", limit);
                else
                    param = null;

                var response = SendHttpRequest("getUserFavoriteSongs", param, session);
                return Parse(response, result => result.Result.Songs).Result;
                

            });
        }

        public IObservable<Song[]> GetUserLibrarySongs(string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object>();
                if (limit != 0)
                    param.Add("limit", limit);
                else
                    param = null;

                var response = SendHttpRequest("getUserLibrarySongs", param, session);
                return Parse(response, result => result.Result.Songs).Result; 

            });
        }

        public IObservable<bool> RemoveUserFavoriteSongs(int songId, string session)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "songIDs", songId } };

                var response = SendHttpRequest("removeUserFavoriteSongs", param, session);
                return Parse(response, result => result.Result.Success).Result;

            });
        }

        public IObservable<bool> AddPlaylist(int[] songIds, string playlistName, string session)
        {
            return Observable.Start(() =>
            {
                var songs = "[" + songIds.Select(i => i.ToString()).Aggregate((a, b) => a + "," + b) + "]";
                var param = new Dictionary<string, object> { { "songIDs", songs }, { "name", playlistName } };


                var response = SendHttpRequest("createPlaylist", param, session);
                return Parse(response, result => result.Result.Success).Result;
            });
        }

        public IObservable<bool> SetPlaylistSongs(int[] songIds, int playlistId, string session)
        {
            return Observable.Start(() =>
            {
                var songs = "[" + songIds.Select(i => i.ToString()).Aggregate((a, b) => a + "," + b) + "]";
                var param = new Dictionary<string, object> { { "songIDs", songs }, { "playlistID", playlistId } };


                var response = SendHttpRequest("setPlaylistSongs", param, session);
                return Parse(response, result => result.Result.Success).Result;
            });
        }

        public IObservable<bool> AddSongToUserFavourites(string session, int songId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "songID", songId } };


                var response = SendHttpRequest("addUserFavoriteSong", param, session);
                return Parse(response, result => result.Result.Success).Result;

            });
        }

        public IObservable<bool> AddSongToUserLibrary(string session, List<Song> songs)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "songs", JsonConvert.SerializeObject(songs.Select(s => new { s.SongID, s.AlbumID, s.ArtistID, trackNum = s.Sort})) } };


                var response = SendHttpRequest("addUserLibrarySongsEx", param, session);
                return Parse(response, result => result.Result.Success).Result;

            });
        }


        public IObservable<Playlist> GetPlaylist(string session, int playlistId, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "playlistID", playlistId } };
                if (limit != 0)
                    param.Add("limit", limit);

                var response = SendHttpRequest("getPlaylist", param, session);
                return Parse(response, result => new Playlist(result)).Result;

            });
        }

        public IObservable<Playlist> GetPlaylistInfos(string session, int playlistId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "playlistID", playlistId } };

                var response = SendHttpRequest("getPlaylistInfo", param, session);
                return Parse(response, result => new Playlist(result)).Result; 

            });
        }

        public IObservable<StreamInfo> GetStreamInfo(string session, string country, int songId, bool lowBitrate = false)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object>
                {
                    { "country", country },
                    { "songID", songId }
                };

                var response = SendHttpRequest("getSubscriberStreamKey", param, session);
                return Parse(response, result => new StreamInfo(result)).Result; 

            });
        }

        public IObservable<Song[]> GetAlbumSongs(string session, int albumId, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "albumID", albumId } };
                if (limit != 0)
                    param.Add("limit", limit);

                var response = SendHttpRequest("getAlbumSongs", param, session);

                return Parse(response, result => result.Result.Songs).Result; 

            });
        }

        public IObservable<Album[]> GetArtistAlbums(string session, int artistId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "artistID", artistId } };


                var response = SendHttpRequest("getArtistVerifiedAlbums", param, session);
                return Parse(response, result => result.Result.Albums).Result;

            });
        }
    }
}
