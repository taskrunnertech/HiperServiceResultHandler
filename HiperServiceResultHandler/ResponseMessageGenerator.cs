using HiperServiceResultHandler.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HiperServiceResultHandler
{
    public static class ResponseMessageGenerator
    {
        public static OkObjectResult Result<T>(this ControllerBase controller, T data, string userMessage = null, string userMessageCode = null)
        {
            return Result(data, userMessage, userMessageCode);
        }

        public static OkObjectResult Result(this ControllerBase controller, object data, string userMessage, string userMessageCode = null)
        {
            return Result(data, userMessage, userMessageCode);
        }

        public static OkObjectResult Result(Object data, string userMessage, string userMessageCode = null, int? errorCode = null, string message = null, Exception exception = null)
        {
            var result = new ApiResultMessage
            {
                Data = data,
                UserMessage = userMessage,
                UserMessageCode = userMessageCode,
                ErrorCode = errorCode ?? 0,
                Message = message,
                Exception = exception?.ToString()
            };
            
            if (errorCode.HasValue || !string.IsNullOrEmpty(message) || exception != null)
            {
                result.IsSuccessful = false;
            }
            else
            {
                result.IsSuccessful = true;
            }

            return new OkObjectResult(result);
        }
    }
}
