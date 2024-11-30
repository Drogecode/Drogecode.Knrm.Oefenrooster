using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public partial class StatisticsUserTable : IDisposable
{
    [Inject] private IStringLocalizer<StatisticsTab> L { get; set; } = default!;
    [Inject] private ReportActionRepository ReportActionRepository { get; set; } = default!;
    [Inject] private ReportTrainingRepository ReportTrainingRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DistinctType>? _distinctTypes;
    private List<DistinctType?>? _selectedTypes;
    private List<UserCounters>? _analyzeHours;
    private List<int>? _years;
    private List<int> _selectedYear = [];

    private TableGroupDefinition<DrogeUser> _groupBy = new()
    {
        GroupName = "Group",
        Indentation = false,
        Expandable = true,
        IsInitiallyExpanded = true,
        Selector = (e) => e.UserFunctionId
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _distinctTypes = [];
            _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);

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

            var actionTypes = await ReportActionRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (actionTypes?.Values is not null)
            {
                foreach (var actionType in actionTypes.Values)
                {
                    _distinctTypes.Add(new DistinctType() { Type = actionType, From = DistinctFromWhere.Action });
                }
            }

            var trainingTypes = await ReportTrainingRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (trainingTypes?.Values is not null)
            {
                foreach (var trainingType in trainingTypes.Values)
                {
                    _distinctTypes.Add(new DistinctType() { Type = trainingType, From = DistinctFromWhere.Training });
                }
            }

            StateHasChanged();
        }
    }

    private async Task YearChanged(IEnumerable<int> arg)
    {
        _selectedYear = arg.ToList();
        await TypeChanged(_selectedTypes);
    }

    private async Task TypeChanged(IEnumerable<DistinctType?>? arg)
    {
        if (arg is null) return;
        _selectedTypes = arg.ToList();
        _analyzeHours = [];
        var year = _selectedYear.FirstOrDefault();
        foreach (var types in _selectedTypes.Where(x => x?.Type is not null))
        {
            switch (types!.From)
            {
                case DistinctFromWhere.Action:
                    var d = await ReportActionRepository.AnalyzeHoursAsync(year, types!.Type!, _cls.Token);
                    if (d?.UserCounters is not null)
                    {
                        _analyzeHours.AddRange(d.UserCounters);
                    }

                    break;
                case DistinctFromWhere.Training:
                    var g = await ReportTrainingRepository.AnalyzeHoursAsync(year, types!.Type!, _cls.Token);
                    if (g?.UserCounters is not null)
                    {
                        _analyzeHours.AddRange(g.UserCounters);
                    }

                    break;
            }
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }

    private class DistinctType
    {
        public string? Type { get; set; }
        public DistinctFromWhere From { get; set; }
    }

    private enum DistinctFromWhere
    {
        None = 0,
        Action = 1,
        Training = 2,
    }
}