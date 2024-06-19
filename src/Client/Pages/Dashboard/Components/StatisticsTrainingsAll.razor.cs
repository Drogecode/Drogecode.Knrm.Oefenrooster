using ApexCharts;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class StatisticsTrainingsAll : IDisposable
{
    [Inject] private IStringLocalizer<ChartsTab> L { get; set; } = default!;
    [Inject] private ReportTrainingRepository ReportTrainingRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] [EditorRequired] public IEnumerable<DrogeUser> SelectedUsers { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<ChartYear>? _data;
    private readonly ApexChartOptions<ChartMonth> _options = new() { Theme = new Theme() { Mode = Mode.Dark } };
    private string[]? _xAxisLabels;
    private long _elapsedMilliseconds = -1;
    private AnalyzeYearChartAllResponse? _analyzeData;
    private ApexChart<ChartMonth> _chart = null!;
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
        _data = new List<ChartYear>();
        _analyzeData = await ReportTrainingRepository.AnalyzeYearChartsAll(SelectedUsers, _cls.Token);
        if (_analyzeData is null) return;
        _elapsedMilliseconds = _analyzeData.ElapsedMilliseconds;
        foreach (var year in _analyzeData.Years.OrderByDescending(x => x.Year))
        {
            var month = new List<ChartMonth>();
            for (var i = 0; i < 12; i++)
            {
                if (year.Months.Any(x => x.Month == i + 1))
                {
                    month.Add(new ChartMonth()
                    {
                        Month = _xAxisLabels![i],
                        Count = year.Months.First(x => x.Month == i + 1).Count
                    });
                }
                else
                {
                    month.Add(new ChartMonth()
                    {
                        Month = _xAxisLabels![i],
                        Count = 0
                    });
                }
            }

            _data.Add(new ChartYear()
            {
                Name = year.Year.ToString(),
                Months = month,
            });
        }
        _renderChart = true;
        StateHasChanged();
    }

    private class ChartYear
    {
        public List<ChartMonth> Months { get; set; }
        public string Name { get; set; }
    }

    private class ChartMonth
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public void Dispose()
    {
        Global.DarkLightChangedAsync -= DarkModeChanged;
        _cls.Cancel();
    }
}