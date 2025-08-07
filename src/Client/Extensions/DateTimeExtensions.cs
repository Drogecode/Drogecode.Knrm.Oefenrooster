using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Drogecode.Knrm.Oefenrooster.Client.Extensions;

public static partial class DateTimeExtensions
{
    public static string ToNiceString(this DateTime dateTime, IStringLocalizer<DateToString> localizer, bool isUtc = true, bool showDayOfWeek = false, bool showDate = true, bool showtime = true)
    {
        var dateTimeNotNull = isUtc ? dateTime.ToLocalTime() : dateTime;
        var datePrefix = SetDatePrefix(localizer, dateTimeNotNull, showDate);
        if (!string.IsNullOrEmpty(datePrefix))
            showDate = false;
        var dateString = SetDateStringFormat(dateTimeNotNull, showDayOfWeek, showDate, showtime);
        if (string.IsNullOrEmpty(dateString) && !string.IsNullOrEmpty(datePrefix))
        {
            return datePrefix;
        }
        var result = string.Empty;
        if (!string.IsNullOrEmpty(datePrefix))
        {
            result += $"{datePrefix} ";
        }

        result += dateTimeNotNull.ToString(dateString, CultureInfo.CurrentUICulture);
        return result;
    }

    private static string SetDatePrefix(IStringLocalizer<DateToString> localizer, DateTime dateTimeNotNull, bool showDate)
    {
        if (!showDate) return string.Empty;
        var today = DateTime.Today;
        if (dateTimeNotNull.Date.CompareTo(today.AddDays(-1)) == 0)
            return localizer["Yesterday"];
        if (dateTimeNotNull.Date.CompareTo(today.AddDays(0)) == 0)
            return localizer["Today"];
        if (dateTimeNotNull.Date.CompareTo(today.AddDays(1)) == 0)
            return localizer["Tomorrow"];
        return string.Empty;
    }

    private static string SetDateStringFormat(DateTime dateTimeNotNull, bool showDayOfWeek, bool showDate, bool showTime)
    {
        var dsFormat = new StringBuilder();

        if (showDate && showDayOfWeek)
            dsFormat.Append("dddd ");

        if (showDate && dateTimeNotNull.Year == DateTime.Today.Year)
            dsFormat.Append("dd MMM");
        else if (showDate)
            dsFormat.Append("dd MMM yyyy");

        if (showTime)
        {
            if (dsFormat.Length != 0 && !MyRegex().IsMatch(dsFormat.ToString()))
                dsFormat.Append(' ');
            dsFormat.Append("H:mm");
        }

        return dsFormat.ToString();
    }

    [GeneratedRegex(@"\s+$")]
    private static partial Regex MyRegex();
}