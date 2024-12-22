using System.Security.Cryptography;

namespace MobileWebApp.Services;

public class AppSecurityService
{
    public const byte DEFAULT_SALT_LENGTH = 8;

    public static byte[] CreateSalt(byte saltLength = DEFAULT_SALT_LENGTH)
    {
        byte[] resultBytes;

        if (saltLength >= 1)
            resultBytes = new byte[saltLength];
        else
            resultBytes = new byte[DEFAULT_SALT_LENGTH];

        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(resultBytes);

        return resultBytes;
    }
}
