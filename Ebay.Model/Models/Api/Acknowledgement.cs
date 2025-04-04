using System.Net;

namespace Ebay.Model.Models.Api
{
    public class Acknowledgement
    {
        public Acknowledgement()
        {
            Messages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
        public string StatusCode { get; set; }
        public void AddMessage(string message)
        {
            this.Messages.Add(message);
        }
        public void AddMessage(List<string> messages)
        {
            this.Messages.AddRange(messages);
        }

        public string GetMessage()
        {
            return string.Join(",", this.Messages);
        }

        public string Message
        {
            get { return GetMessage(); }
        }
    }

    public class Acknowledgement<T> : Acknowledgement
    {
        public T Data { get; set; }
    }
}