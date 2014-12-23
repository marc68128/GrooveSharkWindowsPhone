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

        public IObservable<string> CreateSession()
        {
            Func<string> work = () =>
            {
                var response = SendHttpRequest("startSession", timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    if (sessionResult.Result.Success)
                    {
                        return sessionResult.Result.SessionID;
                    }
                }
                else
                {
                    throw new WebException("Unable to access the server !\nVerify your network connection.");
                }
                return null;

            };

            return Observable.Start(work);

        }

        public IObservable<CountryInfo> GetCountry()
        {
            Func<CountryInfo> work = () =>
            {
                var response = SendHttpRequest("getCountry", timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var grooveSharkResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (grooveSharkResult.Errors != null && grooveSharkResult.Errors.Any())
                        throw grooveSharkResult.Errors.First();

                    var countryInfo = new CountryInfo(grooveSharkResult);
                    Debug.WriteLine("Country : " + countryInfo.Serialize());
                    return new CountryInfo(grooveSharkResult);

                }
                throw new WebException("Unable to access the server !\nVerify your network connection.");
            };

            return Observable.Start(work);
        }

        public IObservable<User> Login(string userName, string md5Password, string session)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "login", userName }, { "password", md5Password } };

                var response = SendHttpRequest("authenticate", param, session, timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var grooveSharkResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);

                    if (grooveSharkResult.Errors != null && grooveSharkResult.Errors.Any())
                        throw grooveSharkResult.Errors.First();

                    return new User(grooveSharkResult);
                }
                throw new WebException("Unable To Connect");
            });
        }

        public IObservable<User> Register(string emailAddress, string password, string fullName, string session, string userName = null)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "emailAddress", emailAddress }, { "password", password }, { "fullName", fullName } };

                if (userName != null)
                    param.Add("username", userName);

                var response = SendHttpRequest("registerUser", param, session, timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var grooveSharkResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);

                    if (grooveSharkResult.Errors != null && grooveSharkResult.Errors.Any())
                        throw new GrooveSharkException { Description = grooveSharkResult.Result.Description, Code = grooveSharkResult.Errors.First().Code };

                    return new User(grooveSharkResult);
                }
                throw new WebException("Unable To Connect");
            });
        }

        public IObservable<bool> Logout(string session)
        {
            return Observable.Start(() =>
            {

                var response = SendHttpRequest("logout", null, session, timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                    {
                        throw sessionResult.Errors.First();
                    }
                    return sessionResult.Result.Success;
                }
                throw new WebException("Unable To Logout");
            });
        }

        public IObservable<User> GetUserInfo(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("getUserInfo", null, session, timeOut: _defautTimeOut).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    return new User(sessionResult);
                }
                return null;
            });
        }

        public IObservable<Song[]> GetPopularSongToday(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("getPopularSongsToday", sessionId: session, timeOut: _defautTimeOut).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Songs;
                }
                throw new WebException("Unable To GetPopular song");
            });
        }

        public IObservable<Song[]> SearchSong(string query, string country, string session, int limit = 0, int offset = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query }, { "country", country } };
                if (limit != 0)
                    param.Add("limit", limit);
                if (offset != 0)
                    param.Add("offset", offset);

                var response = SendHttpRequest("getSongSearchResults", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Songs;
                }
                return null;
            });
        }

        public IObservable<Playlist[]> SearchPlaylist(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getPlaylistSearchResults", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Playlists;
                }
                return null;
            });
        }

        public IObservable<Artist[]> SearchArtist(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getArtistSearchResults", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();

                    return sessionResult.Result.Artists;
                }
                return null;
            });
        }

        public IObservable<Album[]> SearchAlbum(string query, string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query } };
                if (limit != 0)
                    param.Add("limit", limit);


                var response = SendHttpRequest("getAlbumSearchResults", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();

                    return sessionResult.Result.Albums;
                }
                return null;
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

        public IObservable<Playlist[]> GetUserPlaylists(string session, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object>();
                if (limit != 0)
                    param.Add("limit", limit);
                else
                    param = null;

                var response = SendHttpRequest("getUserPlaylists", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Playlists;
                }
                throw new WebException("Unable to get User Playlists");

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

                var response = SendHttpRequest("getUserFavoriteSongs", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Songs;
                }
                throw new WebException("Unable to get User Favourites");

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

                var response = SendHttpRequest("getUserLibrarySongs", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Songs;
                }
                throw new WebException("Unable to get User Library songs");

            });
        }

        public IObservable<bool> RemoveUserFavoriteSongs(int songId, string session)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "songIDs", songId } };

                var response = SendHttpRequest("removeUserFavoriteSongs", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Success;
                }
                return false;
            });
        }

        public IObservable<bool> AddPlaylist(int[] songIds, string playlistName, string session)
        {
            return Observable.Start(() =>
            {
                var songs = "[" + songIds.Select(i => i.ToString()).Aggregate((a, b) => a + "," + b) + "]";
                var param = new Dictionary<string, object> { { "songIDs", songs }, { "name", playlistName } };


                var response = SendHttpRequest("createPlaylist", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Success;
                }
                throw new WebException("Unable to create playlist");
            });
        }

        public IObservable<bool> SetPlaylistSongs(int[] songIds, int playlistId, string session)
        {
            return Observable.Start(() =>
            {
                var songs = "[" + songIds.Select(i => i.ToString()).Aggregate((a, b) => a + "," + b) + "]";
                var param = new Dictionary<string, object> { { "songIDs", songs }, { "playlistID", playlistId } };


                var response = SendHttpRequest("setPlaylistSongs", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Success;
                }
                throw new WebException("Unable to set playlist songs");
            });
        }

        public IObservable<bool> AddSongToUserFavourites(string session, int songId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "songID", songId } };


                var response = SendHttpRequest("addUserFavoriteSong", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Success;
                }
                throw new WebException("Unable to Add song to user favourites");
            });
        }

        public IObservable<Playlist> GetPlaylist(string session, int playlistId, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "playlistID", playlistId } };
                if (limit != 0)
                    param.Add("limit", limit);

                var response = SendHttpRequest("getPlaylist", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return new Playlist(sessionResult);
                }
                throw new WebException("Unable to get Playlist");

            });
        }

        public IObservable<Playlist> GetPlaylistInfos(string session, int playlistId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "playlistID", playlistId } };

                var response = SendHttpRequest("getPlaylistInfo", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return new Playlist(sessionResult);
                }
                throw new WebException("Unable to get Playlist");

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

                var response = SendHttpRequest("getSubscriberStreamKey", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var gsResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (gsResult.Errors != null && gsResult.Errors.Any())
                        throw gsResult.Errors.First();
                    return new StreamInfo(gsResult);
                }
                throw new WebException("Unable to get Playlist");

            });
        }

        public IObservable<Song[]> GetAlbumSongs(string session, int albumId, int limit = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "albumID", albumId } };
                if (limit != 0)
                    param.Add("limit", limit);

                var response = SendHttpRequest("getAlbumSongs", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Songs;
                }
                throw new WebException("Unable to get Album");

            });
        }

        public IObservable<Album[]> GetArtistAlbums(string session, int artistId)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "artistID", artistId } };


                var response = SendHttpRequest("getArtistVerifiedAlbums", param, session).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    return sessionResult.Result.Albums;
                }
                throw new WebException("Unable to get Albums");

            });
        }
    }
}
