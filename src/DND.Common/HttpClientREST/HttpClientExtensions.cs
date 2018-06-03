using System.Net.Http;
using System.Threading.Tasks;

namespace DND.Common.HttpClientREST
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> Get(this System.Net.Http.HttpClient client, string requestUri)
           => await Get(client, requestUri, "");

        public static async Task<HttpResponseMessage> Get(this System.Net.Http.HttpClient client, string requestUri, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Get)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);


            return await builder.SendAsync(client);
        }

        public static async Task<HttpResponseMessage> Post(this System.Net.Http.HttpClient client, string requestUri, object value)
            => await Post(client, requestUri, value, "");

        public static async Task<HttpResponseMessage> Post(this System.Net.Http.HttpClient client,
            string requestUri, object value, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(client);
        }

        public static async Task<HttpResponseMessage> Put(this System.Net.Http.HttpClient client, string requestUri, object value)
            => await Put(client, requestUri, value, "");

        public static async Task<HttpResponseMessage> Put(this System.Net.Http.HttpClient client,
            string requestUri, object value, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Put)
                                .AddRequestUri(requestUri)
                                .AddContent(new JsonContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(client);
        }

        public static async Task<HttpResponseMessage> Patch(this System.Net.Http.HttpClient client, string requestUri, object value)
            => await Patch(client, requestUri, value, "");

        public static async Task<HttpResponseMessage> Patch(this System.Net.Http.HttpClient client, string requestUri, object value, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(new HttpMethod("PATCH"))
                                .AddRequestUri(requestUri)
                                .AddContent(new PatchContent(value))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(client);
        }

        public static async Task<HttpResponseMessage> Delete(this System.Net.Http.HttpClient client, string requestUri)
            => await Delete(client, requestUri, "");

        public static async Task<HttpResponseMessage> Delete(this System.Net.Http.HttpClient client,
            string requestUri, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Delete)
                                .AddRequestUri(requestUri)
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(client);
        }

        public static async Task<HttpResponseMessage> PostFile(this System.Net.Http.HttpClient client, string requestUri,
            string filePath, string apiParamName)
            => await PostFile(client, requestUri, filePath, apiParamName, "");

        public static async Task<HttpResponseMessage> PostFile(this System.Net.Http.HttpClient client, string requestUri,
            string filePath, string apiParamName, string bearerToken)
        {
            var builder = new HttpRequestBuilder()
                                .AddMethod(HttpMethod.Post)
                                .AddRequestUri(requestUri)
                                .AddContent(new FileContent(filePath, apiParamName))
                                .AddBearerToken(bearerToken);

            return await builder.SendAsync(client);
        }
    }
}
