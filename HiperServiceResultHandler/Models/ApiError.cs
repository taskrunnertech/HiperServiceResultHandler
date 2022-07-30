using Newtonsoft.Json;
using System;
using System.Net;

namespace HiperServiceResultHandler.Models
{
    public class ApiError
    {
        public bool IsSuccessful { get; set; } = false;

        public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;

        public string Message { get; set; }

        public string MessageCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exception { get; set; }

        public ApiError()
        {
            // No-op for deserialization
        }

        public ApiError(int statusCode, string message, string messageCode, Exception exception = null)
        {
            StatusCode = statusCode;
            Message = message;
            MessageCode = messageCode;
            Exception = exception?.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
