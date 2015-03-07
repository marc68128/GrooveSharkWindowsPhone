using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Profile;
using Newtonsoft.Json;

namespace AudioPlayer
{
    public static class Logger
    {
        [Conditional("BETA")]
        public static async void Log(Log l)
        {
            l.AppName = "GrooveShark";
            l.PhoneId = HardwareIdentification.GetPackageSpecificToken(null).Id.ToArray().Select(b => b.ToString()).Aggregate((b, next) => b + "," + next);

            var jsonLog = JsonConvert.SerializeObject(l);
            HttpContent c = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("jsonLog", jsonLog) });
            PostAsync(@"http://loggerapi.azurewebsites.net/Home/Log", c, default(CancellationToken));
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

    public sealed class Log
    {
        [JsonProperty(PropertyName = "app_name")]
        public string AppName { get; set; }

        [JsonProperty(PropertyName = "phone_id")]
        public string PhoneId { get; set; }

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
