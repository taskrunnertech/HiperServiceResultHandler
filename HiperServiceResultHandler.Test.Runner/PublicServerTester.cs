using HiperServiceResultHandler.Models;
using Newtonsoft.Json;

namespace HiperServiceResultHandler.Test.Runner
{
    public class PublicServerTester
    {
        [Fact]
        public async Task TestGetOk()
        {
            var http = new HttpClient();
            var s = await http.GetStringAsync("http://localhost:5045/getOk");
            Assert.Equal("Everything is fine", s);
        }

        [Fact]
        public async Task TestGetFailure()
        {
            var http = new HttpClient();
            var response = await http.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:5045/getFailure"));
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);

            var s = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ApiError>(s);
            Assert.False(error.IsSuccessful);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal("User message error", error.Message);
            Assert.Equal("USER_MESSAGE_CODE", error.MessageCode);
            Assert.Null(error.Exception); // If not in development mode
        }
    }
}
