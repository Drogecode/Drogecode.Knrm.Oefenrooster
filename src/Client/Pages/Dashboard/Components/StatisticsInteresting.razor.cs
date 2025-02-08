using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public partial class StatisticsInteresting : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<StatisticsTab>? L { get; set; }
    [Inject, NotNull] private ReportActionRepository? ReportActionRepository { get; set; }
    private CancellationTokenSource _cls = new();
    private List<int>? _years;
    private List<int> _selectedYear = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var years = await ReportActionRepository.Distinct(DistinctReport.Year, _cls.Token);
            if (years?.Values is not null)
            {
                _years = [];
                foreach (var year in years.Values.OrderDescending())
                {
                    if (int.TryParse(year, out var yearAsInt))
                        _years.Add(yearAsInt);
                }

                _selectedYear.Add(_years.Max());
            }

            StateHasChanged();
        }
    }

    private async Task SelectedYearChanged(IEnumerable<int>? years)
    {
        _selectedYear = years?.ToList() ?? [];
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}