using Keycloak.Net.Models.Users;
using Newtonsoft.Json;

namespace keycloak_userEditor;

public class UserInfo
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string AvatarUrl { get; set; }

    public int Age { get; set; }

    public string Login { get; set; }
    public string Password { get; set; }
    
    public DateTimeOffset Created { get; set; }
}