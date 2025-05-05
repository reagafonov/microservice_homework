using Newtonsoft.Json;

namespace keycloak_userEditor;

public class UserController
{
    public async Task<(UserResult? value, IResult? result)> GetUserInfoAsync(string accessToken, string url, string realmName, CancellationToken token = default)
    {
        var (hasError, wellKnown, result) = await GetWellKnown(url, realmName, token);
        if (hasError)
            return (null, result ?? Results.InternalServerError());
        var headers = new Dictionary<string, string>() { { "Authorization", "Bearer " + accessToken } };
        return await GetDataTypedAsync<UserResult>(wellKnown.UserInfoEndpoint, null, headers);
    }
    
    public async Task<(bool, WellKnownInfo wellKnown, IResult result)> GetWellKnown(string url, string realmName,
        CancellationToken cancellationToken)
    {
        var (value, errors) =
            await GetDataTypedAsync<WellKnownInfo>($"/realms/{realmName}/.well-known/openid-configuration", null, method:HttpMethod.Get);
        return (errors == null, value, errors);
    }

    async Task<(TResult? value, IResult? error)> GetDataTypedAsync<TResult>(string url, HttpContent content,
        Dictionary<string, string>? headers = null, HttpMethod? method = null)
        where TResult : class
    {
        var (value, error) = await GetDataAsync(url, content, headers, method);
        if (error != null)
            return (null, error);
        var result = JsonConvert.DeserializeObject<TResult>(value);
        return (result, null);
    }

    async Task<(string value, IResult? error)> GetDataAsync(string url, HttpContent content,
        Dictionary<string, string>? headers = null, HttpMethod? httpMethod = null)
    {
        using var insecureHandler = new HttpClientHandlerInsecure();
        using var httpClient = new HttpClient(insecureHandler);
        using var req = new HttpRequestMessage(httpMethod ?? HttpMethod.Post, url);
        req.Content = content;
        if (headers != null)
            foreach (var header in headers)
                req.Headers.Add(header.Key, new[] { header.Value });

        using var res = await httpClient.SendAsync(req);


        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return (null, Results.Unauthorized());
            return (null, Results.InternalServerError());
        }

        var contentString = await res.Content.ReadAsStringAsync();
        return (contentString, null);
    }


}