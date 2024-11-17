using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace SimpleWebApi.Services;

public class JsonWebTokenService
{
    private readonly ILogger<JsonWebTokenService> logger;
    private readonly string securitySecretKey;
    private readonly string issuer;
    private readonly string audiences;


    public JsonWebTokenService(ILogger<JsonWebTokenService> logger, IConfiguration configuration)
    {
        this.logger = logger;
        securitySecretKey = configuration["DailyWorkJournalSecurityKey"] ?? throw new NullReferenceException("DailyWorkJournalSecurityKey configuration missing");
        issuer = configuration["Authentication:Schemes:Bearer:Issuer"] ?? string.Empty;
        audiences = configuration["Authentication:Schemes:Bearer:Audiences"] ?? string.Empty;
    }

    public string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(securitySecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
            Issuer = issuer,
            Audience = "https://localhost:5215/",
            Expires = DateTime.UtcNow.AddHours(1), // Set the token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
