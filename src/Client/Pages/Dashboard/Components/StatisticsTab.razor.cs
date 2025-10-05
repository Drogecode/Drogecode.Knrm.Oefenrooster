using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class StatisticsTab
{
    [Inject, NotNull] private IStringLocalizer<StatisticsTab>? L { get; set; }
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    [Parameter] public string? Tab { get; set; }

    private StatisticsActionsAll _statisticsActionsAll = null!;
    private StatisticsTrainingsAll _statisticsTrainingsAll = null!;
    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    private MudTabs _tabs;
    private string[]? _xAxisLabels;
    private bool _allYears;
    private bool _total;
    private bool _showHistoricalIncorrectWarning;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (!string.IsNullOrEmpty(Tab))
            {
                switch (Tab.ToLower())
                {
                    case "year-chard":
                        _tabs.ActivatePanel("pn_one");
                        break;
                    case "interesting":
                        _tabs.ActivatePanel("pn_two");
                        break;
                    case "user-table":
                        _tabs.ActivatePanel("pn_three");
                        break;
                }

                StateHasChanged();
            }

            _xAxisLabels = [L["Jan"], L["Feb"], L["Mar"], L["Apr"], L["May"], L["Jun"], L["Jul"], L["Aug"], L["Sep"], L["Oct"], L["Nov"], L["Dec"]];
        }
    }

    private void ActiveTabChanged(int arg)
    {
        switch (arg)
        {
            case 0:
                Navigation.NavigateTo($"dashboard/statistics/year-chard");
                break;
            case 1:
                Navigation.NavigateTo($"dashboard/statistics/interesting");
                break;
            case 2:
                Navigation.NavigateTo($"dashboard/statistics/user-table");
                break;
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

    public List<ChartYear> DrawLineChartAll(AnalyzeYearChartAllResponse? analyzeData)
    {
        if (_showHistoricalIncorrectWarning)
        {
            _showHistoricalIncorrectWarning = false;
            StateHasChanged();
        }

        var data = new List<ChartYear>();
        if (analyzeData is null) return data;
        var yearCount = 0;
        foreach (var year in analyzeData.Years.OrderByDescending(x => x.Year))
        {
            if (year.Year <= 2021)
            {
                _showHistoricalIncorrectWarning = true;
                StateHasChanged();
            }

            var month = new List<ChartMonth>();
            var count = 0;
            for (var i = 0; i < 12; i++)
            {
                if (!_total) count = 0;
                if (year.Months.Any(x => x.Month == i + 1))
                {
                    count += year.Months.First(x => x.Month == i + 1).Count;
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