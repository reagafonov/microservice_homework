using Newtonsoft.Json;

namespace keycloak_userEditor;

public class UserResult
{
    [JsonProperty("sub")]
    public string Id { get; set; }
    
    [JsonProperty("given_name")]
    public string FirstName { get; set; }

    [JsonProperty("family_name")]  
    public string LastName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("age")]
    public int Age { get; set; }

    [JsonProperty("preferred_username")]
    public string Login { get; set; }
}