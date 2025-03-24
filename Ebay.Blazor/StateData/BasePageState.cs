
using System.Text;
using System.Text.Json;

namespace Ebay.Blazor.StateData
{
    public class BasePageState
    {
        public HttpClient _httpClient;
        public BasePageState(HttpClient http)
        {
            _httpClient = http;
        }
        public async Task<T> GetRequest<T>(string url)
        {
            var result = await _httpClient.GetFromJsonAsync<T>(url);
            return result;
        }

        public async Task<TResult> PostRequest<TResult, TModelRequest>(string url, TModelRequest model)
        {
            // using StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsJsonAsync<TModelRequest>(url, model);
            var response = await res.Content.ReadFromJsonAsync<TResult>();
            return response;
        }

        public async Task<string> DeleteRequest<TResult, TModelRequest>(string url, TModelRequest requestModel)
        {
            var jsonContent = new StringContent(
               JsonSerializer.Serialize(requestModel),
               Encoding.UTF8,
               "application/json"
           );
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = jsonContent
            };
            try
            {
                var res = await _httpClient.SendAsync(request);
            }
            catch (System.Exception)
            {
                return "Thất bại";
            }
            return "Thành công";
        }

        public async Task<string> PutRequest<TResult, TModelRequest>(string url, TModelRequest requestModel)
        {
            try
            {
                var res = await _httpClient.PutAsJsonAsync(url, requestModel);
                var response = await res.Content.ReadFromJsonAsync<TResult>();
            }
            catch (System.Exception)
            {
                return "Thất bại";
            }
            return "Thành công";
        }
    }
}