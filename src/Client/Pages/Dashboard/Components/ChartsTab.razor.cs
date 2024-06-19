using ApexCharts;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class ChartsTab : IDisposable
{
    [Inject] private IStringLocalizer<ChartsTab> L { get; set; } = default!;
    [Inject] private ReportActionRepository ReportActionRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<ChartYear>? _data;
    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    private readonly ApexChartOptions<ChartMonth> _options = new() { Theme = new Theme() { Mode = Mode.Dark } };
    private string[]? _xAxisLabels;
    private long _elapsedMilliseconds = -1;
    private AnalyzeYearChartAllResponse? _analyzeData;
    private ApexChart<ChartMonth> _chart = null!;
    private ulong _updateKey = 0;
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
            _renderChart = true;
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsers = selection;
        _renderChart = false;
        StateHasChanged();
        await Task.Delay(1); // Will not update without this delay
        await UpdateAnalyzeYearChartAll();
        _renderChart = true;
        StateHasChanged();
    }

    private async Task UpdateAnalyzeYearChartAll()
    {
        _data = new List<ChartYear>();
        _analyzeData = await ReportActionRepository.AnalyzeYearChartsAll(_selectedUsers, _cls.Token);
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
                MyData = month,
                updateKey = _updateKey
            });
            _updateKey++;
        }
    }

    private class ChartYear
    {
        public List<ChartMonth> MyData { get; set; }
        public string Name { get; set; }
        public ulong updateKey { get; set; }
    }

    private class ChartMonth
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}