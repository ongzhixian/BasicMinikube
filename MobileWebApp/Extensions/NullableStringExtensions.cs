namespace MobileWebApp;

public static class NullableStringExtensions
{
    public static bool IsTrue(this string? str)
    {
        return bool.TryParse(str, out bool result) && result == true;
    }
}