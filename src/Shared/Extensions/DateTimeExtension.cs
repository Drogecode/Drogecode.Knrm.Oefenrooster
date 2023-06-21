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
}
