using System;

namespace HiperServiceResultHandler
{
    public class ServiceException : Exception
    {
        public ServiceException(string userMessage, int errorCode, string userMessageCode = null) : base()
        {
            UserMessage = userMessage;
            ErrorCode = errorCode;
            UserMessageCode = userMessageCode;
        }

        public ServiceException(string userMessage, int errorCode, string userMessageCode = null, string message = null, Exception innerException = null) : base(message, innerException)
        {
            UserMessage = userMessage;
            ErrorCode = errorCode;
            UserMessageCode = userMessageCode;
        }

        public string UserMessage { get; set; }

        public int ErrorCode { get; set; }

        public string UserMessageCode { get; set; }
    }
}
