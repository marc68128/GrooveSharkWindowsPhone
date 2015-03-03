using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrooveSharkClient.Helpers
{
    public static class Logger
    {
        public static async void Log(Log l)
        {
            var jsonLog = JsonConvert.SerializeObject(l);
            HttpContent c = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("log", jsonLog) });
            PostAsync(@"http://theskores.fr/PhpLogger/", c, default(CancellationToken));
        }
        private static async void PostAsync(string requestUri, HttpContent content, CancellationToken ct, Action<HttpRequestMessage> customizeRequest = null, TimeSpan timeOut = default(TimeSpan))
        {
            var req = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };

            if (customizeRequest != null)
                customizeRequest(req);


            var client = new HttpClient();
            if (timeOut != default(TimeSpan))
                client.Timeout = timeOut;

            await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        }
    }

    public class Log
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "stack_trace")]
        public string StackTrace { get; set; }
    }
}
