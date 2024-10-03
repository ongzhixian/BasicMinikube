using System.ComponentModel;

namespace OidcServerWebApp.Models;

internal enum AuthorizationGrantTypesEnum : byte
{
    [Description("code")]
    Code,

    [Description("Implicit")]
    Implicit,

    [Description("ClientCredentials")]
    ClientCredentials,

    [Description("ResourceOwnerPassword")]
    ResourceOwnerPassword
}

