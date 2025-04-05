using System.Net.Http;
using System.Text;
using Blazorise;
using Blazorise.Components;
using Ebay.Model.Models.Api;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;


namespace Ebay.Blazor.Data.ServiceUltil
{
    public class HttpBackendRequestService
    {
        public HttpClient _httpClient;
        public INotificationService _notificationService;
        public HttpBackendRequestService(HttpClient http, INotificationService notiService)
        {
            _httpClient = http;
            _httpClient.BaseAddress = new Uri("http://localhost:5059");
            _notificationService = notiService;
        }

        private Dictionary<int, string> _httpStatusMessage = new List<KeyValuePair<int, string>>()
        {
            new KeyValuePair<int, string>(100, "Continue"),
            new KeyValuePair<int, string>(101, "Switching Protocols"),
            new KeyValuePair<int, string>(200, "OK"),
            new KeyValuePair<int, string>(201, "Created"),
            new KeyValuePair<int, string>(202, "Accepted"),
            new KeyValuePair<int, string>(203, "Non-Authoritative Information"),
            new KeyValuePair<int, string>(204, "No Content"),
            new KeyValuePair<int, string>(205, "Reset Content"),
            new KeyValuePair<int, string>(206, "Partial Content"),
            new KeyValuePair<int, string>(300, "Multiple Choices"),
            new KeyValuePair<int, string>(301, "Moved Permanently"),
            new KeyValuePair<int, string>(302, "Found"),
            new KeyValuePair<int, string>(303, "See Other"),
            new KeyValuePair<int, string>(304, "Not Modified"),
            new KeyValuePair<int, string>(305, "Use Proxy"),
            new KeyValuePair<int, string>(306, "(Unused)"),
            new KeyValuePair<int, string>(307, "Temporary Redirect"),
            new KeyValuePair<int, string>(400, "Bad Request"),
            new KeyValuePair<int, string>(401, "Unauthorized"),
            new KeyValuePair<int, string>(402, "Payment Required"),
            new KeyValuePair<int, string>(403, "Forbidden"),
            new KeyValuePair<int, string>(404, "Not Found"),
            new KeyValuePair<int, string>(405, "Method Not Allowed"),
            new KeyValuePair<int, string>(406, "Not Acceptable"),
            new KeyValuePair<int, string>(407, "Proxy Authentication Required"),
            new KeyValuePair<int, string>(408, "Request Timeout"),
            new KeyValuePair<int, string>(409, "Conflict"),
            new KeyValuePair<int, string>(410, "Gone"),
            new KeyValuePair<int, string>(411, "Length Required"),
            new KeyValuePair<int, string>(412, "Precondition Failed"),
            new KeyValuePair<int, string>(413, "Request Entity Too Large"),
            new KeyValuePair<int, string>(414, "Request-URI Too Long"),
            new KeyValuePair<int, string>(415, "Unsupported Media Type"),
            new KeyValuePair<int, string>(416, "Requested Range Not Satisfiable"),
            new KeyValuePair<int, string>(417, "Expectation Failed"),
            new KeyValuePair<int, string>(500, "Internal Server Error"),
            new KeyValuePair<int, string>(501, "Not Implemented"),
            new KeyValuePair<int, string>(502, "Bad Gateway"),
            new KeyValuePair<int, string>(503, "Service Unavailable"),
            new KeyValuePair<int, string>(504, "Gateway Timeout"),
            new KeyValuePair<int, string>(505, "HTTP Version Not Supporte")
        }.ToDictionary();
        public delegate Task<HttpResponseMessage> ActionCallRestFullApi(string url);

        private async Task<Acknowledgement<T>> _CallApi<T>(string url, ActionCallRestFullApi call, Action<Acknowledgement<T>>? callback = null)
        {
            var ack = new Acknowledgement<T>();
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await call.Invoke(url);
            ack.IsSuccess = response.IsSuccessStatusCode;
            ack.StatusCode = response.StatusCode.ToString();
            if (ack.IsSuccess == false)
            {
                Console.WriteLine(
                    "Error occurred, the status code is: {0}",
                    response.StatusCode
                );
                var statusCode = (int)response.StatusCode;
                if (_httpStatusMessage.ContainsKey((int)statusCode) == false)
                {
                    ack.AddMessage("Có lỗi Api");
                }
                else
                {
                    ack.AddMessage(_httpStatusMessage[(int)statusCode]);
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                using JsonReader reader = new JsonTextReader(new StringReader(content));
                var s = GetSerialiser(null);
                ack.Data = s.Deserialize<T>(reader);
            }
            if (callback != null)
            {
                callback.Invoke(ack);
            }
            client.Dispose();

            if (ack.IsSuccess == false)
            {
                _notificationService.Error(ack.Message);
            }
            return ack;

        }


        public async Task<Acknowledgement<T>> GetRequest<T>(string url, Action<Acknowledgement<T>>? callback = null)
        {
            ActionCallRestFullApi callapi = async (string api) =>
            {
                var result = await _httpClient.GetAsync(api);
                return result;
            };
            var result = await _CallApi<T>(url, callapi, callback);
            return result;
        }

        private JsonSerializer GetSerialiser(Action<JsonSerializer> settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
            settings?.Invoke(jsonSerializer);
            return jsonSerializer;
        }



        public async Task<Acknowledgement<TResult>> PostRequest<TResult, TModelRequest>(string url, TModelRequest model, Action<Acknowledgement<TResult>> callback = null)
        {
            ActionCallRestFullApi callapi = async (string api) =>
            {
                //JsonContent content = JsonContent.Create(model, null, new System.Text.Json.JsonSerializerOptions()
                //{
                //    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                //});



                using StringWriter requestWriter = new StringWriter();
                var requestSerializer = GetSerialiser(null);
                requestSerializer.Serialize(requestWriter, model);
                var content = new StringContent(
                               requestWriter.ToString(),
                               Encoding.UTF8,
                               "application/json"
                           );
                Console.WriteLine(requestWriter.ToString());

                var result = await _httpClient.PostAsync(api, content);
                return result;
            };

            var result = await _CallApi<TResult>(url, callapi, callback);
            return result;
        }

        public async Task<Acknowledgement<TResult>> DeleteRequest<TModelRequest, TResult>(string url, TModelRequest requestModel, Action<Acknowledgement<TResult>> callback = null)
        {

            using StringWriter requestWriter = new StringWriter();
            var requestSerializer = GetSerialiser(null);
            requestSerializer.Serialize(requestWriter, requestModel);
            var jsonContent = new StringContent(
               requestWriter.ToString(),
               Encoding.UTF8,
               "application/json"
           );
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = jsonContent
            };

            ActionCallRestFullApi callapi = async (string api) =>
                {
                    var result = await _httpClient.SendAsync(request);
                    return result;
                };

            var result = await _CallApi<TResult>(url, callapi, callback);
            return result;
        }

        public async Task<Acknowledgement<TResult>> PutRequest<TResult, TModelRequest>(string url, TModelRequest requestModel, Action<Acknowledgement<TResult>> callback = null)
        {
            ActionCallRestFullApi callapi = async (string api) =>
            {
                JsonContent content = JsonContent.Create(requestModel, null, new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                });
                var result = await _httpClient.PutAsync(api, content);
                return result;
            };

            var result = await _CallApi<TResult>(url, callapi, callback);
            return result;
        }


    }
}