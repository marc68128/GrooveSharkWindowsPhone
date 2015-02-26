using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Wallet;
using Windows.Foundation;
using Newtonsoft.Json;

namespace AudioPlayer
{
    public sealed class AudioPlayerClient
    {
        private const string ServerURI = "https://api.grooveshark.com/ws3.php?sig={0}";
        private const string ServerKey = "winphone_marc2";
        private const string ServerSecret = "86b91a1ef536883aa04243b863db7281";
        private const string RequestPatern = "{\"method\":\"{0}\",\"parameters\":{{1}},\"header\":{{2}}}";

        public string SessionId { get; set; }
        public string CountryInfos { get; set; }


        private async Task<StreamInfo> GetStreamInfosInternal(SongViewModel vm)
        {

            var header = "\"wsKey\":\"" + ServerKey + "\"";
            header += ",\"sessionID\":\"" + SessionId + "\"";

            var parameter = "\"country\":" + CountryInfos + ",\"songID\":" + vm.SongId;



            var content = RequestPatern
                .Replace("{0}", "getSubscriberStreamKey")
                .Replace("{1}", parameter)
                .Replace("{2}", header);


            var sig = HmacMd5.Hash(ServerSecret, content);
            var uri = string.Format(ServerURI, sig);

            Debug.WriteLine("[Networking]" + uri + "\n[Networking]" + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType.MediaType = "application/json";

            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = httpContent };


            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 10);

            var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

            if (res.IsSuccessStatusCode)
            {
                var responceTxt = res.Content.ReadAsStringAsync().Result;
                var gsResult = JsonConvert.DeserializeObject<GrooveSharkResult>(responceTxt);
                if (gsResult.Errors != null && gsResult.Errors.Any())
                    throw gsResult.Errors.First();
                return new StreamInfo(gsResult);
            }
            return null;
        }

        public async void MarkStreamKeyOver30S(SongViewModel vm)
        {

            var header = "\"wsKey\":\"" + ServerKey + "\"";
            header += ",\"sessionID\":\"" + SessionId + "\"";

            var parameter = "\"streamKey\":\"" + vm.StreamKey + "\",\"streamServerID\":" + vm.StreamServerId;



            var content = RequestPatern
                .Replace("{0}", "markStreamKeyOver30Secs")
                .Replace("{1}", parameter)
                .Replace("{2}", header);


            var sig = HmacMd5.Hash(ServerSecret, content);
            var uri = string.Format(ServerURI, sig);

            Debug.WriteLine("[Networking]" + uri + "\n[Networking]" + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType.MediaType = "application/json";

            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = httpContent };


            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 10);

            var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

            if (res.IsSuccessStatusCode)
            {
                var responceTxt = res.Content.ReadAsStringAsync().Result;
                var gsResult = JsonConvert.DeserializeObject<GrooveSharkResult>(responceTxt);
                if (gsResult.Errors != null && gsResult.Errors.Any())
                    throw gsResult.Errors.First();

#if DEBUG
                if (gsResult.Result.Success)
                    Debug.WriteLine("[MarkStreamKeyOver30Secs] " + vm.SongName);
#endif
            }
        }

        public async void MarkSongComplete(SongViewModel vm)
        {

            var header = "\"wsKey\":\"" + ServerKey + "\"";
            header += ",\"sessionID\":\"" + SessionId + "\"";

            var parameter = "\"songID\":" + vm.SongId + ",\"streamKey\":\"" + vm.StreamKey + "\",\"streamServerID\":" + vm.StreamServerId;



            var content = RequestPatern
                .Replace("{0}", "markSongComplete")
                .Replace("{1}", parameter)
                .Replace("{2}", header);


            var sig = HmacMd5.Hash(ServerSecret, content);
            var uri = string.Format(ServerURI, sig);

            Debug.WriteLine("[Networking]" + uri + "\n[Networking]" + content);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType.MediaType = "application/json";

            var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = httpContent };


            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 0, 10);

            var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

            if (res.IsSuccessStatusCode)
            {
                var responceTxt = res.Content.ReadAsStringAsync().Result;
                var gsResult = JsonConvert.DeserializeObject<GrooveSharkResult>(responceTxt);
                if (gsResult.Errors != null && gsResult.Errors.Any())
                    throw gsResult.Errors.First();

#if DEBUG
                if (gsResult.Result.Success)
                    Debug.WriteLine("[MarkSongComplete] " + vm.SongName);
#endif
            }




        }

        public IAsyncOperation<StreamInfo> GetStreamInfos(SongViewModel vm)
        {
            return GetStreamInfosInternal(vm).AsAsyncOperation();
        }



    }
}
