namespace keycloak_userEditor;

public record TokenRequestParameters( string login, string password, string grantType = "password") {}