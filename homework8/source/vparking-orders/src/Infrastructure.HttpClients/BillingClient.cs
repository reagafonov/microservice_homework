using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Services.Abstractions;
using Services.Contracts;

namespace Infrastructure.HttpClient;

public class BillingClient(string baseAddress, ILogger<BillingClient> logger):System.Net.Http.HttpClient, IHttpPayment
{

    public async Task<bool> Pay(PaymentModel model)
    {
        logger.LogInformation($"Paying billing client:{baseAddress}");
        var request = new HttpRequestMessage(HttpMethod.Put, $"{baseAddress}/pay");
        request.Content = JsonContent.Create(model.Price, JsonTypeInfo.CreateJsonTypeInfo<decimal>(JsonSerializerOptions.Web));
        
        request.Headers.Add("X-Auth-Request-Preferred-Username", model.ClientID);
        
        var result = await SendAsync(request);

        if (result.IsSuccessStatusCode)
            return await result.Content.ReadFromJsonAsync<bool>();
        return false;
    }

    
}