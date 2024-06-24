using ApexCharts;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class StatisticsTrainingsAll : IDisposable
{
    [Inject] private IStringLocalizer<StatisticsTab> L { get; set; } = default!;
    [Inject] private ReportTrainingRepository ReportTrainingRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] [EditorRequired] public StatisticsTab StatisticsTab { get; set; } = default!;
    [Parameter] [EditorRequired] public IEnumerable<DrogeUser> SelectedUsers { get; set; } = default!;
    [Parameter] [EditorRequired] public bool AllYears { get; set; }
    private CancellationTokenSource _cls = new();
    private List<StatisticsTab.ChartYear>? _data;
    private readonly ApexChartOptions<StatisticsTab.ChartMonth> _options = new() { Theme = new Theme() { Mode = Mode.Dark } };
    private string[]? _xAxisLabels;
    private long _elapsedMilliseconds = -1;
    private AnalyzeYearChartAllResponse? _analyzeData;
    private ApexChart<StatisticsTab.ChartMonth> _chart = null!;
    private bool _renderChart;

    protected override async Task OnInitializedAsync()
    {
        Global.DarkLightChangedAsync += DarkModeChanged;
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
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _xAxisLabels = [L["Jan"], L["Feb"], L["Mar"], L["Apr"], L["May"], L["Jun"], L["Jul"], L["Aug"], L["Sep"], L["Oct"], L["Nov"], L["Dec"]];
            _options.Theme.Mode = Global.DarkMode ? Mode.Dark : Mode.Light;
            await UpdateAnalyzeYearChartAll();
        }
    }

    public async Task UpdateAnalyzeYearChartAll()
    {
        _renderChart = false;
        await Task.Delay(1); // Will not update without this delay
        StateHasChanged();
        var analyzeData = await ReportTrainingRepository.AnalyzeYearChartsAll(SelectedUsers, _cls.Token);
        if (analyzeData is null) return;
        _elapsedMilliseconds = analyzeData.ElapsedMilliseconds;
        _data = await StatisticsTab.DrawLineChartAll(analyzeData, AllYears);
        _renderChart = true;
        StateHasChanged();
    }

    public void Dispose()
    {
        Global.DarkLightChangedAsync -= DarkModeChanged;
        _cls.Cancel();
    }
}