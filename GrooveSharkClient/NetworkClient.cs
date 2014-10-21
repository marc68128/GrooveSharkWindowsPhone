using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http.Headers;
using System.Text;

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

		HttpRequestMessage CreateRequest(HttpMethod method, string requestUri)
		{
			HttpRequestMessage req = new HttpRequestMessage(method, requestUri);
			return req;
		}

		async Task<HttpResponseMessage> SendRequest(HttpRequestMessage req, CancellationToken ct, TimeSpan timeOut)
		{
			HttpClient client = new HttpClient();
			if (timeOut != default(TimeSpan))
				client.Timeout = timeOut;

			//Debug.WriteLine(req.RequestUri);
			//DebugHeadersOfRequest(req);

			return await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
		}

		public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken ct, Action<HttpRequestMessage> customizeRequest = null, TimeSpan timeOut = default(TimeSpan)) 
		{
			HttpRequestMessage req = CreateRequest(HttpMethod.Get, requestUri);

			if (customizeRequest != null)
				customizeRequest(req);

			return await SendRequest(req, ct, timeOut);
		}

		public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken ct, Action<HttpRequestMessage> customizeRequest = null, TimeSpan timeOut = default(TimeSpan)) 
		{
			HttpRequestMessage req = CreateRequest(HttpMethod.Post, requestUri);
			req.Content = content;

			if (customizeRequest != null)
				customizeRequest(req);

			return await SendRequest(req, ct, timeOut);
		}
	}
}