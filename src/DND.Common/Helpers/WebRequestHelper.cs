using DND.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Helpers
{
    public class WebRequestHelper
    {
        public class ResponseAcceptType
        {
            public const string XML = "application/xml";
            public const string JSON = "application/json";
        }

        private static readonly Lazy<HttpClient> HttpClient = new Lazy<HttpClient>(() => GetHttpClient());
        //TODO private static readonly int ConnectionLeaseTimeout = ServicePointMonitor.DefaultConnectionLeaseTimeout;
        //TODO private static readonly TimeSpan RequestTimeout = ServicePointMonitor.DefaultRequestTimeout;

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            //client.Timeout = RequestTimeout;
            return client;
        }

        public async static Task<Tuple<byte[], HttpResponseHeaders>> GetAsync(string url, string accept, CancellationToken cancellationToken)
        {
            //X-Forwarded-For
           // SetConnectionLeaseTimeout(url, ConnectionLeaseTimeout);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            //request.Version = HttpVersion.Version10; //connection close - keep alive false
            //request.Version = HttpVersion.Version11; //keep alive
            //request.Headers.ConnectionClose = true;

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

            using (HttpResponseMessage response = await HttpClient.Value.SendAsync(request, cancellationToken))
            {
                await response.EnsureSuccessStatusCodeAsync();
                byte[] result = await response.Content.ReadAsByteArrayAsync();
                return new Tuple<byte[], HttpResponseHeaders>(result, response.Headers);
            }
        }

        public async static Task<Tuple<byte[], HttpResponseHeaders>> PostFormAsync(string url, Dictionary<string, string> postVariables, string accept, CancellationToken cancellationToken)
        {
           // SetConnectionLeaseTimeout(url, ConnectionLeaseTimeout);

            var dataString = postVariables.ToQueryString();

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            //request.Version = HttpVersion.Version10; //connection close
            //request.Headers.ConnectionClose = true;

            request.Content = new StringContent(dataString, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

            using (HttpResponseMessage response = await HttpClient.Value.SendAsync(request, cancellationToken))
            {
                await response.EnsureSuccessStatusCodeAsync();
                byte[] result = await response.Content.ReadAsByteArrayAsync();
                return new Tuple<byte[], HttpResponseHeaders>(result, response.Headers);
            }
        }

        public async static Task<Tuple<byte[], HttpResponseHeaders>> PostJSONAsync(string url, string JSONString, CancellationToken cancellationToken)
        {
           // SetConnectionLeaseTimeout(url, ConnectionLeaseTimeout);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            //request.Version = HttpVersion.Version10; //connection close
            //request.Headers.ConnectionClose = true;

            request.Content = new StringContent(JSONString, Encoding.UTF8, ResponseAcceptType.JSON);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ResponseAcceptType.JSON));

            using (HttpResponseMessage response = await HttpClient.Value.SendAsync(request, cancellationToken))
            {
                await response.EnsureSuccessStatusCodeAsync();
                byte[] result = await response.Content.ReadAsByteArrayAsync();
                return new Tuple<byte[], HttpResponseHeaders>(result, response.Headers);
            }
        }

        private static void SetConnectionLeaseTimeout(string url, int connectionLeaseTimeout)
        {
            var sp = ServicePointManager.FindServicePoint(new Uri(url));
            sp.ConnectionLeaseTimeout = connectionLeaseTimeout; // -1 by default means connection will stay open indefinitely
        }
    }

    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content != null)
                response.Content.Dispose();
            throw new SimpleHttpResponseException(response.StatusCode, content);
        }
    }

    public class SimpleHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}
