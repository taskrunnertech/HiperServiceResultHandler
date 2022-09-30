namespace HiperServiceResultHandler.Test.Runner
{
    public class DirectServiceTester
    {
        [Fact]
        public async Task TestGetOk()
        {
            var http = new HttpClient();
            var s = await http.GetAsync<string>("http://localhost:5046/getOk");
            Assert.Equal("Everything is fine", s);
        }

        [Fact]
        public async Task TestGetFailure()
        {
            var http = new HttpClient();
            var exception = await Assert.ThrowsAsync<ServiceException>(() =>
            {
                return http.GetAsync<string>("http://localhost:5046/getFailure");
            });
            Assert.Equal("User message error", exception.UserMessage);
            Assert.Equal("USER_MESSAGE_CODE", exception.UserMessageCode);
            Assert.Equal("Internal message", exception.Message);
            Assert.Equal(1234, exception.ErrorCode);
        }

        [Fact]
        public async Task TestPostFailure()
        {
            var http = new HttpClient();
            var exception = await Assert.ThrowsAsync<ServiceException>(() =>
            {
                return http.PostAsJsonAsync<string>("http://localhost:5046/postFailure", new
                {
                    ParamA = "Something",
                    ParamB = true,
                });
            });
            Assert.Equal("User message error", exception.UserMessage);
            Assert.Equal("USER_MESSAGE_CODE", exception.UserMessageCode);
            Assert.Equal("Internal message", exception.Message);
            Assert.Equal(1234, exception.ErrorCode);
        }

        [Fact]
        public async Task TestPostFailureParameterA()
        {
            var http = new HttpClient();
            var exception = await Assert.ThrowsAsync<ServiceException>(() =>
            {
                return http.PostAsJsonAsync<string>("http://localhost:5046/postFailure", new
                {
                    ParamB = true,
                });
            });
            Assert.Equal("Parameter empty", exception.UserMessage);
            Assert.Equal("PARAM_INVALID", exception.UserMessageCode);
            Assert.Equal("There was an issue with parameter A", exception.Message);
            Assert.Equal(2345, exception.ErrorCode);
        }

        [Fact]
        public async Task TestPostFailureParameterB()
        {
            var http = new HttpClient();
            var exception = await Assert.ThrowsAsync<ServiceException>(() =>
            {
                return http.PostAsJsonAsync<string>("http://localhost:5046/postFailure", new
                {
                    ParamA = "Something",
                });
            });
            Assert.Equal("Parameter false", exception.UserMessage);
            Assert.Equal("PARAM_INVALID", exception.UserMessageCode);
            Assert.Equal("There was an issue with parameter B", exception.Message);
            Assert.Equal(2345, exception.ErrorCode);
        }
    }
}