using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode
{
    public sealed partial class DateToString
    {
        [Inject, NotNull] private CustomerSettingRepository? CustomerSettingRepository { get; set; }
        [Inject, NotNull] IStringLocalizer<DateToString>? L { get; set; }
        [Parameter] public DateTime? DateTimeUtc { get; set; }
        [Parameter] public DateTime? DateTimeLocal { get; set; }
        [Parameter] public bool ShowDayOfWeek { get; set; }
        [Parameter] public bool ShowTime { get; set; }
        [Parameter] public bool ShowDate { get; set; } = true;
        public DateTime DateTimeNotNull { get; set; }
        public string DateStringFormat { get; set; } = string.Empty;
        public string DatePrefix { get; set; } = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            if (DateTimeUtc is not null)
            {
                var timeZone = await CustomerSettingRepository.GetTimeZone();
                var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTimeNotNull = TimeZoneInfo.ConvertTimeFromUtc(DateTimeUtc.Value, zone);
            }
            else
            {
                DateTimeNotNull =  DateTimeLocal ?? DateTime.MinValue;
            }
            SetDatePrefix();
            SetDateStringFormat();
            StateHasChanged();
        }

        private void SetDatePrefix()
        {
            if (!ShowDate) return;
            var today = DateTime.Today;
            if (DateTimeNotNull.Date.CompareTo(today.AddDays(-1)) == 0)
                DatePrefix = L["Yesterday"];
            else if (DateTimeNotNull.Date.CompareTo(today.AddDays(0)) == 0)
                DatePrefix = L["Today"];
            else if (DateTimeNotNull.Date.CompareTo(today.AddDays(1)) == 0)
                DatePrefix = L["Tomorrow"];
            if (!string.IsNullOrEmpty(DatePrefix))
                ShowDate = false;
        }

        private void SetDateStringFormat()
        {
            var dsFormat = new StringBuilder();

            if (ShowDate && ShowDayOfWeek)
                dsFormat.Append("dddd ");

            if (ShowDate && DateTimeNotNull.Year == DateTime.Today.Year)
                dsFormat.Append("dd MMM");
            else if (ShowDate)
                dsFormat.Append("dd MMM yyyy");

            if (ShowTime)
            {
                if (dsFormat.Length != 0 && !MyRegex().IsMatch(dsFormat.ToString()))
                    dsFormat.Append(' ');
                dsFormat.Append("H:mm");
            }

            DateStringFormat = dsFormat.ToString();
        }

        [GeneratedRegex(@"\s+$")]
        private static partial Regex MyRegex();
    }
}