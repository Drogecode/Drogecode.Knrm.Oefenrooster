using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;


// Should probably replace with https://github.com/apexcharts/Blazor-ApexCharts

public sealed partial class ChartsTab : IDisposable
{
    [Inject] private IStringLocalizer<ChartsTab> L { get; set; } = default!;
    [Inject] private ReportActionRepository ReportActionRepository { get; set; } = default!;
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<ChartSeries>? _series;
    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    private readonly ChartOptions _options = new();
    private string[]? _xAxisLabels;
    private long _elapsedMilliseconds = -1;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _series = new List<ChartSeries>();
            _xAxisLabels = [L["Jan"], L["Feb"], L["Mar"], L["Apr"], L["May"], L["Jun"], L["Jul"], L["Aug"], L["Sep"], L["Oct"], L["Nov"], L["Dec"]];
            await UpdateAnalyzeYearChartAll();
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsers = selection;
        await UpdateAnalyzeYearChartAll();
        StateHasChanged();
    }

    private async Task UpdateAnalyzeYearChartAll()
    {
        _series = new List<ChartSeries>();
        var get = await ReportActionRepository.AnalyzeYearChartsAll(_selectedUsers, _cls.Token);
        if (get is null) return;
        _elapsedMilliseconds = get.ElapsedMilliseconds;
        _options.YAxisTicks = _selectedUsers.Any() ? 5 : 10;
        foreach (var year in get.Years.OrderByDescending(x=>x.Year))
        {
            var month = new double[12];
            for (var i = 0; i < 12; i++)
            {
                if (year.Months.Any(x => x.Month == i + 1))
                {
                    month[i] = year.Months.First(x => x.Month == i + 1).Count;
                }
                else
                {
                    month[i] = 0;
                }
            }
            _series.Add(new ChartSeries()
            {
                Name = year.Year.ToString(),
                Data = month,
            });
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}