using System.ComponentModel;

namespace OidcServerWebApp.Models;

public enum TokenTypeEnum : byte
{
    [Description("Bearer")]
    Bearer
}
