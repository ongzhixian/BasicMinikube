namespace OidcServerWebApp.Models;


// Client here refers to Application that used by the Resource Owner (ie. user who is using that application).
public class Client
{
    public string ClientName { get; set; }
    public string ClientId { get; set; }

    /// <summary>
    /// Client Password
    /// </summary>
    public string ClientSecret { get; set; }

    public IList<string> GrantType { get; set; }

    /// <summary>
    /// by default false
    /// </summary>
    public bool IsActive { get; set; } = false;
    public IList<string> AllowedScopes { get; set; }

    public string ClientUri { get; set; }
    public string RedirectUri { get; set; }
}


// The OAuth 2.0 Authorization Framework
// https://www.rfc-editor.org/rfc/rfc6749
