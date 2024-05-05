using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.RegularExpressions;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode
{
    public sealed partial class DateToString
    {
        [Inject] IStringLocalizer<DateToString> L { get; set; }
        [Parameter] public DateTime? DateTimeUtc { get; set; }
        [Parameter] public DateTime? DateTimeLocal { get; set; }
        [Parameter] public bool ShowDayOfWeek { get; set; }
        [Parameter] public bool ShowTime { get; set; }
        [Parameter] public bool ShowDate { get; set; } = true;
        public DateTime DateTimeNotNull { get; set; }
        public string DateStringFormat { get; set; } = string.Empty;
        public string DatePrefix { get; set; } = string.Empty;

        protected override void OnParametersSet()
        {
            DateTimeNotNull = DateTimeUtc?.ToLocalTime() ?? DateTimeLocal ?? DateTime.MinValue;
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