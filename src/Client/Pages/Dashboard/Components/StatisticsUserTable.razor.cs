using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components.Dialogs;
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
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DistinctType>? _distinctTypes;
    private List<DistinctType?>? _selectedTypes;
    private List<UserCounters>? _analyzeHours;
    private List<int>? _years;
    private List<int> _selectedYear = [];
    private decimal _compensation = 1.25m;
    private bool _showHistoricalIncorrectWarning = false;

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

    private async Task OpenConfigureDialog()
    {
        var parameters = new DialogParameters<UserTableConfigureDialog>
        {
            { x => x.DistinctTypes, _distinctTypes },
            { x => x.Years, _years },
            { x => x.SelectedTypes, _selectedTypes },
            { x => x.SelectedYear, _selectedYear },
            { x => x.Compensation, _compensation },
        };

        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        var dialog = await DialogProvider.ShowAsync<UserTableConfigureDialog>("", parameters, options);
        var result = await dialog.Result;
        if (result?.Canceled == false)
        {
            if (result.Data is StatisticsUserTableConfigureSettings data)
            {
                _selectedTypes = data.SelectedTypes.ToList();
                _selectedYear = data.SelectedYear.ToList();
                _compensation = data.Compensation;
                _showHistoricalIncorrectWarning = _selectedYear.FirstOrDefault() <= 2021;
                await TypeChanged();
                StateHasChanged();
            }
        }
    }

    private async Task TypeChanged()
    {
        if(_selectedTypes is null) return;
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
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}