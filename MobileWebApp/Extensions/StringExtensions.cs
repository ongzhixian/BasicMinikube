using System.Text;

namespace MobileWebApp;

public static class StringExtensions
{
    public static byte[] ToUtf8Bytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static byte[] FromBase64(this string str)
    {
        return Convert.FromBase64String(str);
    }

    //public static bool IsTrue(this string str)
    //{
    //    return bool.TryParse(str, out bool result) && result == true;
    //}
}
