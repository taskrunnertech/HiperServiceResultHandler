using Newtonsoft.Json;

namespace HiperServiceResultHandler.Models
{
    public class ApiResultMessage
    {
        public object Data { get; set; }
        public bool IsSuccessful { get; set; }
        public string UserMessage { get; set; }
        public string UserMessageCode { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string Exception { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class ApiResultMessage<T>
    {
        public T Data { get; set; }
        public bool IsSuccessful { get; set; }
        public string UserMessage { get; set; }
        public string UserMessageCode { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string Exception { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
