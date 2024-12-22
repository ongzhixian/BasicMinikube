namespace MobileWebApp;

public static class ByteArrayExtensions
{
    public static string ToBase64(this byte[] src)
    {
        return Convert.ToBase64String(src);
    }
}