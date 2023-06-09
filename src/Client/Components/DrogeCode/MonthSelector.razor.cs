namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class MonthSelector
{
    [Parameter] public EventCallback<DateTime?> MonthChanged { get; set; }
    [Parameter]
    public DateTime? Month
    {
        get
        {
            return _month;
        }
        set
        {
            if (_month == value) return;
            _month = value;
            if (MonthChanged.HasDelegate)
            {
                MonthChanged.InvokeAsync(value);
            }
        }
    }
    private DateTime? _month;
    private bool _updating;

    private void SetMonth(DateTime dateTime)
    {
        Month = dateTime;
    }
}
