using HiperServiceResultHandler.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace HiperServiceResultHandler
{
    public static class HttpClientExtensions
    {
        static HttpClientExtensions()
        {
            try
            {
                HiperUserAgentHeader = new ProductInfoHeaderValue("HiperServiceClient", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
            catch (Exception)
            {
                HiperUserAgentHeader = new ProductInfoHeaderValue("HiperServiceClient", "1.0");
            }
        }

        private static readonly ProductInfoHeaderValue HiperUserAgentHeader;

        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        };

        public static JsonSerializerSettings SerializerSettings { get; set; } = DefaultSerializerSettings;

        private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");

        public static void Replace<T>(this HttpHeaderValueCollection<T> collection, T newValue) where T : class
        {
            if(collection != null)
            {
                collection.Clear();
                collection.Add(newValue);
            }
        }

        /// <summary>
        /// Send GET request and process service response with query string parameters.
        /// </summary>
        public static Task<R> GetAsync<R>(this HttpClient httpClient, string url, IDictionary<string, string> queryParams)
        {
            return httpClient.GetAsync<R>(QueryHelpers.AddQueryString(url, queryParams));
        }

        /// <summary>
        /// Send GET request and process service response.
        /// </summary>
        public static Task<R> GetAsync<R>(this HttpClient httpClient, string url)
        {
            return httpClient.SendAsync<R>(HttpMethod.Get, url);
        }

        /// <summary>
        /// Send PUT request with optional data payload and process service response.
        /// </summary>
        public static Task<R> PutAsJsonAsync<R>(this HttpClient httpClient, string url, object data = null)
        {
            return httpClient.SendAsync<R>(HttpMethod.Put, url, CreateRequestContent(data));
        }

        /// <summary>
        /// Send POST request with optional data payload and process service response.
        /// </summary>
        public static Task<R> PostAsJsonAsync<R>(this HttpClient httpClient, string url, object data = null)
        {
            return httpClient.SendAsync<R>(HttpMethod.Post, url, CreateRequestContent(data));
        }

        /// <summary>
        /// Send PATCH request with optional data payload and process service response.
        /// </summary>
        public static Task<R> PatchAsJsonAsync<R>(this HttpClient httpClient, string url, object data = null)
        {
            return httpClient.SendAsync<R>(PatchMethod, url, CreateRequestContent(data));
        }

        /// <summary>
        /// Send DELETE request with optional data payload and process service response.
        /// </summary>
        public static Task<R> DeleteAsync<R>(this HttpClient httpClient, string url, object data = null)
        {
            return httpClient.SendAsync<R>(HttpMethod.Delete, url);
        }

        /// <summary>
        /// Send generic message and process service response.
        /// </summary>
        /// <typeparam name="R">Object class of service response.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="url">The request URI.</param>
        /// <param name="body">The optional HTTP content sent through the request.</param>
        public static async Task<R> SendAsync<R>(this HttpClient httpClient, HttpMethod method, string url, HttpContent body = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.UserAgent.Replace(HiperUserAgentHeader);
            if(body != null)
            {
                request.Content = body;
            }

            var response = await httpClient.SendAsync(request);

            return await ReadResponse<R>(response);
        }

        private static HttpContent CreateRequestContent(object data)
        {
            var dataAsString = data == null ? "{}" : JsonConvert.SerializeObject(data, SerializerSettings);

            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private static async Task<R> ReadResponse<R>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unexpected service error");
            }

            var result = await response.Content.ReadAsJsonAsync<ApiResultMessage<R>>();
            if (result.IsSuccessful)
            {
                return result.Data;
            }
            else
            {
                throw new ServiceException(result.UserMessage, result.ErrorCode, result.UserMessageCode, result.Message);
            }
        }

        /// <summary>
        /// Reads contents from a <see cref="HttpContent"/> instance and deserializes it as a JSON object.
        /// </summary>
        /// <typeparam name="T">Object class to deserialize.</typeparam>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var serializer = JsonSerializer.Create(SerializerSettings);
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
        }
    }
}
