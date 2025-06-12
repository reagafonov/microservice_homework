// using System.Text.Json;
// using System.Text.Json.Serialization.Metadata;
// using Services.Abstractions;
// using Services.Contracts;
//
// namespace keycloak_userEditor;
//
// public class BillingClient(string baseAddress):HttpClient,IPayment
// {
//     public async Task<bool> Pay(PaymentDto dto)
//     {
//         var result = await PutAsync(baseAddress,
//             JsonContent.Create(dto, JsonTypeInfo.CreateJsonTypeInfo<PaymentDto>(JsonSerializerOptions.Web)));
//
//         if (result.IsSuccessStatusCode)
//             return await result.Content.ReadFromJsonAsync<bool>();
//         return false;
//     }
// }