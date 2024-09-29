using ApexCharts;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class StatisticsActionsAll : IDisposable
{
    [Inject] private IStringLocalizer<StatisticsTab> L { get; set; } = default!;
    [Inject] private ReportActionRepository ReportActionRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] [EditorRequired] public StatisticsTab StatisticsTab { get; set; } = default!;
    [Parameter] [EditorRequired] public IEnumerable<DrogeUser> SelectedUsers { get; set; } = default!;
    [Parameter] [EditorRequired] public bool AllYears { get; set; }
    private CancellationTokenSource _cls = new();
    private List<StatisticsTab.ChartYear>? _data;
    private readonly ApexChartOptions<StatisticsTab.ChartMonth> _options = new() { Theme = new Theme() { Mode = Mode.Dark } };
    private IEnumerable<string?>? _selectedPrio;
    private List<string?>? _prios;
    private long _elapsedMilliseconds = -1;
    private int _totalCount;
    private ApexChart<StatisticsTab.ChartMonth>? _chart;
    private bool _renderChart;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Global.DarkLightChangedAsync += DarkModeChanged;
            _options.Theme.Mode = Global.DarkMode ? Mode.Dark : Mode.Light;
            _prios = (await ReportActionRepository.Distinct(DistinctReportAction.Prio, _cls.Token))?.Values;
            await UpdateAnalyzeYearChartAll();
        }
    }

    private async Task DarkModeChanged(bool newValue)
    {
        DebugHelper.WriteLine($"rerender with darkmode {newValue}");
        _renderChart = false;
        StateHasChanged();
        await Task.Delay(1); // Will not update without this delay
        _options.Theme.Mode = newValue ? Mode.Dark : Mode.Light;
        _renderChart = true;
        StateHasChanged();
    }

    public async Task UpdateAnalyzeYearChartAll()
    {
        _renderChart = false;
        await Task.Delay(1); // Will not update without this delay
        StateHasChanged();
        var analyzeData = await ReportActionRepository.AnalyzeYearChartsAll(SelectedUsers, _selectedPrio, AllYears ? null : 5, _cls.Token);
        if (analyzeData is null) return;
        _elapsedMilliseconds = analyzeData.ElapsedMilliseconds;
        _totalCount = analyzeData.TotalCount;
        _data = StatisticsTab.DrawLineChartAll(analyzeData);
        _renderChart = true;
        StateHasChanged();
    }

    public async Task PrioChanged(IEnumerable<string?>? newPrio)
    {
        _selectedPrio = newPrio;
        await UpdateAnalyzeYearChartAll();
    }

    public void Dispose()
    {
        Global.DarkLightChangedAsync -= DarkModeChanged;
        _cls.Cancel();
    }
}