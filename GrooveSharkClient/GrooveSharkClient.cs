using System;
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
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Helpers;
using GrooveSharkClient.Models;
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

        private async Task<HttpResponseMessage> SendHttpRequest(string method, Dictionary<string, object> parameters = null, string sessionId = null)
        {
            var header = "\"wsKey\":\"" + ServerKey + "\"";
            if (sessionId != null)
                header += ",\"sessionID\":\"" + sessionId + "\"";

            var parameter = "";
            if (parameters != null)
                parameter = parameters
                    .Select(kvp => "\"" + kvp.Key + "\":" + (kvp.Value is string ? "\"" : "") + kvp.Value + (kvp.Value is string ? "\"" : ""))
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
                    if (sessionResult.Result.Success)
                    {
                        Debug.WriteLine("Session ID : " + sessionResult.Result.SessionID);
                        return sessionResult.Result.SessionID;
                    }
                }
                return null;
            };

            return Observable
              .Defer(() => Observable.Start(work, RxApp.TaskpoolScheduler))
              .Publish()
              .RefCount();
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
                    return sessionResult.Result.Songs;
                }
                return null;
            });
        }

    }
}
