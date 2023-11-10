using Cashfree;

namespace StudioBooking.Infrastructure
{
    public interface ICashFreeClient
    {
        Task<HttpResponseMessage> GetAsync(string action);
        Task<HttpResponseMessage> PostAsync<T>(T request, string action);
    }

    public class CashFreeClientService : ICashFreeClient
    {
        private readonly HttpClient _client;
        public CashFreeClientService(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(T request, string action)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(PaymentGatewayConfig.Test + action));
            httpRequestMessage.Headers.Add("Accept", "application/json");
            httpRequestMessage.Headers.Add("x-api-version", PaymentGatewayConfig.APIVersion);
            httpRequestMessage.Headers.Add("x-client-id", PaymentGatewayConfig.TestClientAppID);
            httpRequestMessage.Headers.Add("x-client-secret", PaymentGatewayConfig.TestClientSecret);
            httpRequestMessage.Content = new StringContent(Converter.ConvertObjectToJsonString(request), null, "application/json");
            return await _client.SendAsync(httpRequestMessage);
        }

        public async Task<HttpResponseMessage> GetAsync(string action)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(PaymentGatewayConfig.Test + action));
            httpRequestMessage.Headers.Add("x-api-version", PaymentGatewayConfig.APIVersion);
            httpRequestMessage.Headers.Add("x-client-id", PaymentGatewayConfig.TestClientAppID);
            httpRequestMessage.Headers.Add("x-client-secret", PaymentGatewayConfig.TestClientSecret);
            return await _client.SendAsync(httpRequestMessage);
        }
    }
}
