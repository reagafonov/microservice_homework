using Newtonsoft.Json;

namespace keycloak_userEditor;

public class WellKnownInfo
{
    [JsonProperty("token_endpoint")]
    public string TokenEndpoint { get; set; }
}