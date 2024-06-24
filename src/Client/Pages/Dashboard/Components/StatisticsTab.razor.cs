using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
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
    private bool _allYears;

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
}