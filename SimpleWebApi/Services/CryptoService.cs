namespace SimpleWebApi.Services;

public sealed class CryptoService
{
    public static string ComputeBase64Sha256(string sourceString)
    {
        using System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();

        byte[] passwordHashByes = System.Text.Encoding.UTF8.GetBytes(sourceString);

        var passwordHashSha256 = sha256.ComputeHash(passwordHashByes);

        return Convert.ToBase64String(passwordHashSha256);
    }
}
