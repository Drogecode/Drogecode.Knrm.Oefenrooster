using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class StatisticsTab
{
    [Inject] private IStringLocalizer<StatisticsTab> L { get; set; } = default!;
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;

    private StatisticsActionsAll _statisticsActionsAll = null!;
    private StatisticsTrainingsAll _statisticsTrainingsAll = null!;
    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    private string[]? _xAxisLabels;
    private bool _allYears;
    private bool _total;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _xAxisLabels = [L["Jan"], L["Feb"], L["Mar"], L["Apr"], L["May"], L["Jun"], L["Jul"], L["Aug"], L["Sep"], L["Oct"], L["Nov"], L["Dec"]];
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _selectedUsers = selection;
        await _statisticsActionsAll.UpdateAnalyzeYearChartAll();
        await _statisticsTrainingsAll.UpdateAnalyzeYearChartAll();
        StateHasChanged();
    }

    private async Task AllYearsChanged(bool newValue)
    {
        _allYears = newValue;
        await _statisticsActionsAll.UpdateAnalyzeYearChartAll();
        await _statisticsTrainingsAll.UpdateAnalyzeYearChartAll();
        StateHasChanged();
    }

    private async Task TotalChanged(bool newValue)
    {
        _total = newValue;
        await _statisticsActionsAll.UpdateAnalyzeYearChartAll();
        await _statisticsTrainingsAll.UpdateAnalyzeYearChartAll();
        StateHasChanged();
    }

    public List<ChartYear> DrawLineChartAll(AnalyzeYearChartAllResponse? analyzeData, bool allYears)
    {
        var data = new List<ChartYear>();
        if (analyzeData is null) return data;
        var yearCount = 0;
        foreach (var year in analyzeData.Years.OrderByDescending(x => x.Year))
        {
            var month = new List<ChartMonth>();
            var count = 0;
            for (var i = 0; i < 12; i++)
            {
                if(!_total) count = 0;
                if (year.Months.Any(x => x.Month == i + 1))
                {
                    count +=year.Months.First(x => x.Month == i + 1).Count;
                    month.Add(new ChartMonth()
                    {
                        Month = _xAxisLabels![i],
                        Count = count
                    });
                }
                else
                {
                    month.Add(new ChartMonth()
                    {
                        Month = _xAxisLabels![i],
                        Count = count
                    });
                }
            }

            if (!allYears && yearCount >= 5)
                break;
            data.Add(new ChartYear()
            {
                Name = year.Year.ToString(),
                Months = month,
            });
            yearCount++;
        }

        return data;
    }

    public class ChartYear
    {
        public List<ChartMonth>? Months { get; set; }
        public string? Name { get; set; }
    }

    public class ChartMonth
    {
        public string? Month { get; set; }
        public int Count { get; set; }
    }
}