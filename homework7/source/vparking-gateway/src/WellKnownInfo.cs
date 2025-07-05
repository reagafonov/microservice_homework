using Newtonsoft.Json;

namespace keycloak_userEditor;

public class WellKnownInfo
{
    [JsonProperty("token_endpoint")]
    public string TokenEndpoint { get; set; }
    
    [JsonProperty("end_session_endpoint")]
    public string LogoutEndpoint { get; set; }
    
    [JsonProperty("userinfo_endpoint")]
    public string UserInfoEndpoint { get; set; }
}