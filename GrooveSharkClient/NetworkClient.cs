using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace GrooveSharkClient
{
	public class NetworkClient
	{
		public NetworkClient()
		{
		}

		void DebugHeadersOfRequest(HttpRequestMessage req)
		{
			Debug.WriteLine("HEADERS:");
			foreach (var header in req.Headers)
			{
				Debug.WriteLine("\t{0} : {1}", header.Key, string.Join(",", header.Value));
			}

			if (req.Content != null && req.Content.Headers != null)
			{
				foreach (var header in req.Content.Headers)
				{
					Debug.WriteLine("\t{0} : {1}", header.Key, string.Join(",", header.Value));
				}
			}
		}


		public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken ct, Action<HttpRequestMessage> customizeRequest = null, TimeSpan timeOut = default(TimeSpan)) 
		{
			var req = new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content};

		    if (customizeRequest != null)
				customizeRequest(req);


            var client = new HttpClient();
            if (timeOut != default(TimeSpan))
                client.Timeout = timeOut;

            //Debug.WriteLine(req.RequestUri);
            //DebugHeadersOfRequest(req);

            return await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
		}
	}
}