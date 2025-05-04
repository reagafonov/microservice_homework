using Newtonsoft.Json;

namespace keycloak_userEditor;

public class TokenResult
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
}