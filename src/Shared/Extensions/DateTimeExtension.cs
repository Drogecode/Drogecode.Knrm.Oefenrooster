namespace Drogecode.Knrm.Oefenrooster.Shared.Extensions;

public static class DateTimeExtension
{
    public static DateTime DateTimeWithZone(this DateTime dateTime, string timeZoneString)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
        var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
        return utcDateTime;
    }

    public static bool InPast(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime.CompareTo(DateTime.UtcNow.Date) <= 0;
        }
        else
            return dateTime.CompareTo(DateTime.Now.Date) <= 0;
    }
}
