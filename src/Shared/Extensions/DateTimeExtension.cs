namespace Drogecode.Knrm.Oefenrooster.Shared.Extensions;

public static class DateTimeExtension
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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

        return dateTime.CompareTo(DateTime.Now.Date) <= 0;
    }

    public static long ConvertToTimestamp(this DateTime value)
    {
        var elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalSeconds * 1000;
    }

    public static string ToHourWithOptionalMinutes(this DateTime dateTime)
    {
        return dateTime.ToString(dateTime.Minute == 0 ? "HH" : "HH:mm");
    }
}