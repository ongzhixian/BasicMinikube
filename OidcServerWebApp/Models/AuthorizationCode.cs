﻿using System.Security.Claims;

namespace OidcServerWebApp.Models;

public class AuthorizationCode
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    public bool IsOpenId { get; set; }
    public IList<string> RequestedScopes { get; set; }

    public ClaimsPrincipal Subject { get; set; }
    public string Nonce { get; set; }
}
