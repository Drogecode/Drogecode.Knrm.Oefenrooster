namespace Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;

public sealed partial class MonthSelector
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Parameter] public EventCallback<DateTime?> MonthChanged { get; set; }
    [Parameter] public bool CurrentMonth { get; set; }
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
        }
    }
    private DateTime? _month;
    private bool _updating;

    private void SetMonth(DateTime dateTime)
    {
        Month = dateTime;
        if (MonthChanged.HasDelegate)
        {
            MonthChanged.InvokeAsync(dateTime);
        }
    }
}
