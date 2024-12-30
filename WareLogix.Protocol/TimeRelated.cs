namespace WareLogix.Protocol;

internal sealed class TimeRelated
{
    static readonly TimeZoneInfo Est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public static DateTime EstNow
    {
        get
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Est);
        }
    }

    public static DateTime GetEstFromUtc(DateTime date)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(date, Est);
    }
}
