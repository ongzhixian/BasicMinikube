using System.Text;

namespace PkceAuthConsoleApp;

internal class Base64Url
{
    public static string Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var base64String = Convert.ToBase64String(plainTextBytes);
        base64String = base64String.Replace('+', '-')
                                  .Replace('/', '_')
                                  .TrimEnd('=');
        return base64String;
    }

    public static string Decode(string base64UrlString)
    {
        base64UrlString = base64UrlString.Replace('-', '+')
                                        .Replace('_', '/');
        var padding = base64UrlString.Length % 4;
        if (padding > 0)
        {
            base64UrlString += new string('=', 4 - padding);
        }
        var base64Bytes = Convert.FromBase64String(base64UrlString);
        return Encoding.UTF8.GetString(base64Bytes);
    }
}
