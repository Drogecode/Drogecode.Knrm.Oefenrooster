﻿using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.RegularExpressions;

namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode
{
    public sealed partial class DateToString
    {
        [Inject] IStringLocalizer<DateToString> L { get; set; }
        [Parameter] public DateTime? DateTimeUtc { get; set; }
        [Parameter] public DateTime? DateTimeLocal { get; set; }
        [Parameter] public bool ShowDayOfWeek { get; set; }
        [Parameter] public bool ShowTime { get; set; }
        [Parameter] public bool? ShowDate { get; set; }
        private bool _showDate = true;
        public DateTime DateTimeNotNull { get; set; }
        public string DateStringFormat { get; set; } = string.Empty;
        public string DatePrefix { get; set; } = string.Empty;

        protected override void OnParametersSet()
        {
            //This could give the opposite issue if the production server is running in utc....
            DateTimeNotNull = DateTimeUtc?.ToLocalTime() ?? DateTimeLocal ?? DateTime.MinValue;
            if (ShowDate == null)
                _showDate = DateTimeNotNull.Date.CompareTo(DateTime.Today) != 0;
            else
                _showDate = (bool)ShowDate;
            SetDatePrefix();
            SetDateStringFormat();
        }

        private void SetDatePrefix()
        {
            if (!_showDate) return;
            var today = DateTime.Today;
            if (_showDate && DateTimeNotNull.Date.CompareTo(today.AddDays(-1)) == 0)
                DatePrefix = L["Yesterday"];
            else if (_showDate && DateTimeNotNull.Date.CompareTo(today.AddDays(0)) == 0)
                DatePrefix = L["Today"];
            else if (_showDate && DateTimeNotNull.Date.CompareTo(today.AddDays(1)) == 0)
                DatePrefix = L["Tomorrow"];
            if (!string.IsNullOrEmpty(DatePrefix))
                _showDate = false;
        }

        private void SetDateStringFormat()
        {
            var dsFormat = new StringBuilder();

            if (_showDate && ShowDayOfWeek)
                dsFormat.Append("dddd ");

            if (_showDate && DateTimeNotNull.Year == DateTime.Today.Year)
                dsFormat.Append("dd MMM");
            else if (_showDate)
                dsFormat.Append("dd MMM yyyy");

            if (ShowTime)
            {
                if (dsFormat.Length != 0 && !Regex.IsMatch(dsFormat.ToString(), @"\s+$"))
                    dsFormat.Append(' ');
                dsFormat.Append("H:mm");
            }

            DateStringFormat = dsFormat.ToString();
        }
    }
}