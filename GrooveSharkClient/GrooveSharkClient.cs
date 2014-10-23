﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Helpers;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using Newtonsoft.Json;
using ReactiveUI;

namespace GrooveSharkClient
{
    public class GrooveSharkClient : IGrooveSharkClient
    {
        private const string ServerURI = "https://api.grooveshark.com/ws3.php?sig={0}";
        private const string ServerKey = "winphone_marc2";
        private const string ServerSecret = "86b91a1ef536883aa04243b863db7281";
        private const string RequestPatern = "{\"method\":\"{0}\",\"parameters\":{{1}},\"header\":{{2}}}";

        private NetworkClient _networkClient;

        public GrooveSharkClient()
        {
            _networkClient = new NetworkClient();
        }

        private bool ShouldHaveQuote(object value)
        {
            return (value is string && !(value as string).StartsWith("{") && !(value as string).EndsWith("}"));
        }

        private async Task<HttpResponseMessage> SendHttpRequest(string method, Dictionary<string, object> parameters = null, string sessionId = null)
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

            Debug.WriteLine(uri);
            Debug.WriteLine(content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType.MediaType = "application/json";

            return await _networkClient.PostAsync(uri, httpContent, default(CancellationToken));
        }

        public IObservable<string> CreateSession()
        {
            Func<string> work = () =>
            {
                var response = SendHttpRequest("startSession").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (sessionResult.Errors != null && sessionResult.Errors.Any())
                        throw sessionResult.Errors.First();
                    if (sessionResult.Result.Success)
                    {
                        Debug.WriteLine("Session ID : " + sessionResult.Result.SessionID);
                        return sessionResult.Result.SessionID;
                    }
                }
                return null;
            };

            return Observable.Start(work);
        }

        public IObservable<CountryInfo> GetCountry()
        {
            Func<CountryInfo> work = () =>
            {
                var response = SendHttpRequest("getCountry").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var grooveSharkResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    if (grooveSharkResult.Errors != null && grooveSharkResult.Errors.Any())
                        throw grooveSharkResult.Errors.First();

                    var countryInfo = new CountryInfo(grooveSharkResult);
                    Debug.WriteLine("Country : " + countryInfo.GetCountryInfoAsJsonString());
                    return new CountryInfo(grooveSharkResult);

                }
                return null;
            };

            return Observable.Start(work);
        }

        public IObservable<User> Login(string userName, string md5Password, string session)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "login", userName }, { "password", md5Password } };

                var response = SendHttpRequest("authenticate", param, session).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var sessionResult = JsonConvert.DeserializeObject<GrooveSharkResult>(content);
                    return new User(sessionResult);
                }
                return null;
            });
        }

        public IObservable<bool> Logout(string session)
        {
            return Observable.Start(() =>
            {

                var response = SendHttpRequest("logout", null, session).Result;
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
                return false;
            });
        }

        public IObservable<User> GetUserInfo(string session)
        {
            return Observable.Start(() =>
            {
                var response = SendHttpRequest("getUserInfo", null, session).Result;
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
                var response = SendHttpRequest("getPopularSongsToday", sessionId: session).Result;

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

        public IObservable<Song[]> SearchSong(string query, string country, string session, int limit = 0, int offset = 0)
        {
            return Observable.Start(() =>
            {
                var param = new Dictionary<string, object> { { "query", query }, { "country", country } };
                if (limit != 0)
                    param.Add("limit", limit);
                if (offset != 0)
                    param.Add("offset", offset);

                var response = SendHttpRequest("getSongSearchResults", param,  session).Result;

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
    }
}
