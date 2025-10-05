using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Enums;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components.Dialogs;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public partial class StatisticsUserTable : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<StatisticsTab>? L { get; set; }
    [Inject, NotNull] private ReportActionRepository? ReportActionRepository { get; set; }
    [Inject, NotNull] private ReportTrainingRepository? ReportTrainingRepository { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private FunctionRepository? FunctionRepository { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    private CancellationTokenSource _cls = new();
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private List<DistinctType>? _distinctTypes;
    private List<DistinctType?>? _selectedTypes;
    private List<UserCounters>? _analyzeHours;
    private List<string>? _boats;
    private List<string>? _selectedBoats;
    private List<int>? _years;
    private List<int> _selectedYear = [];
    private decimal _compensation = 1.25m;
    private bool _showHistoricalIncorrectWarning = false;
    private bool _loading = false;
    private int? _fullCompensation;
    private int _tableHeight = 255;
    
    private TableGroupDefinition<DrogeUser> _groupBy = new()
    {
        GroupName = "Group",
        Indentation = false,
        Expandable = true,
        IsInitiallyExpanded = true,
        Selector = (user) => user.UserFunctionId ?? Guid.Empty
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

            var boats = await ReportTrainingRepository.Distinct(DistinctReport.Boat, _cls.Token);
            if (boats?.Values is not null)
            {
                _boats = boats.Values.Where(x => x is not null).ToList()!;
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
            { x => x.Boats, _boats },
            { x => x.SelectedBoats, _selectedBoats },
        };

        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true,
            FullWidth = true
        };
        var dialog = await DialogProvider.ShowAsync<UserTableConfigureDialog>("", parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: StatisticsUserTableConfigureSettings data })
        {
            _loading = true;
            StateHasChanged();
            _tableHeight = 255;
            _selectedTypes = data.SelectedTypes.ToList();
            _selectedYear = data.SelectedYear.ToList();
            _selectedBoats = data.SelectedBoats?.ToList();
            _compensation = data.Compensation;
            _showHistoricalIncorrectWarning = _selectedYear.FirstOrDefault() <= 2021;
            if (_showHistoricalIncorrectWarning)
                _tableHeight += 50;
            await TypeChanged();
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task TypeChanged()
    {
        if (_selectedTypes is null) return;
        await SetAnalyzeHours();
        CalculateFullCompensation();
    }

    private async Task SetAnalyzeHours()
    {
        _analyzeHours = [];
        var year = _selectedYear.FirstOrDefault();
        foreach (var types in _selectedTypes!.Where(x => x?.Type is not null))
        {
            switch (types!.From)
            {
                case DistinctFromWhere.Action:
                    var action = await ReportActionRepository.AnalyzeHoursAsync(year, types!.Type!, _selectedBoats, _cls.Token);
                    if (action?.UserCounters is not null)
                    {
                        _analyzeHours.AddRange(action.UserCounters);
                    }

                    break;
                case DistinctFromWhere.Training:
                    var training = await ReportTrainingRepository.AnalyzeHoursAsync(year, types!.Type!, _cls.Token);
                    if (training?.UserCounters is not null)
                    {
                        _analyzeHours.AddRange(training.UserCounters);
                    }

                    break;
            }
        }
    }

    private void CalculateFullCompensation()
    {
        _fullCompensation = 0;
        foreach (var analyzeHour in _analyzeHours!.Where(x => _users?.Any(y => y.Id == x.UserId) == true))
        {
            _fullCompensation += analyzeHour.FullHours;
        }

        _tableHeight += 24;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}