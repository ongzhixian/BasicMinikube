namespace OidcServerWebApp.Models;

public class ClientStore
{
    public IEnumerable<Client> Clients = new[]
    {
        new Client
        {
            ClientName = "WareLogic OIDC Client",
            ClientId = "WareLogixOidcClientWebApp",
            ClientSecret = "123456789",
            AllowedScopes = new[]{ "openid", "profile"},
            GrantType = GrantTypes.Code,
            IsActive = true,
            ClientUri = "https://localhost:7017",
            RedirectUri = "https://localhost:7017/signin-oidc"
        }
    };
}
