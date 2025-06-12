using Newtonsoft.Json;

namespace keycloak_userEditor;

public class TokenResult
{
    public string access_token { get; set; }

    public string refresh_token { get; set; }
}