using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleTable : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ScheduleTable>? L { get; set; }
    [Inject, NotNull] private ISessionExpireService? SessionExpireService { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private DayItemRepository? DayItemRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter] public MainLayout MainLayout { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; } = default!;
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; } = default!;
    [Parameter, EditorRequired] public bool CanEdit { get; set; }

    private CancellationTokenSource _cls = new();
    private Guid? _specialFunctionId = null;
    private bool _updating;
    private bool _working;
    private List<PlannedTraining> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private List<RoosterItemDay>? _dayItems;
    private DateTime? _month;

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
            _specialFunctionId = Functions?.FirstOrDefault(x => x.Special)?.Id;
            var month = await SessionExpireService.GetSelectedMonth(_cls.Token);
            await SetMonth(month);
            StateHasChanged();
        }
    }

    private async Task SetMonth(DateTime? dateTime)
    {
        if (dateTime == null) return;
        _month = dateTime;
        DateRange dateRange = new DateRange
        {
            Start = new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1),
            End = new DateTime(dateTime.Value.Year, dateTime.Value.Month, DateTime.DaysInMonth(dateTime.Value.Year, dateTime.Value.Month))
        };
        await SetCalenderForMonth(dateRange);
        await SessionExpireService.SetSelectedMonth(dateTime, _cls.Token);
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        TrainingWeek scheduleForUser = new();
        var scheduleForAll = await ScheduleRepository.ScheduleForAll(dateRange, true, _cls.Token);
        if (scheduleForAll == null) return;
        _dayItems = (await DayItemRepository.GetDayItemsAsync(dateRange, Guid.Empty, _cls.Token))?.DayItems;
        _userTrainingCounter = scheduleForAll.UserTrainingCounters;
        var trainingsInRange = scheduleForAll.Planners;
        if (trainingsInRange.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInRange[0].DateStart);
            foreach (var training in trainingsInRange)
            {
                _events.Add(training);
            }
        }

        _updating = false;
        StateHasChanged();
    }

    private async Task Click(PlanUser? user, PlannedTraining? training)
    {
        if (!CanEdit || _working || user is null || training is null) return;
        _working = true;
        user.Assigned = !user.Assigned;
        await ScheduleRepository.PatchAssignedUser(training.TrainingId, training, user, AuditReason.Assigned);
        MainLayout.ShowSnackbarAssignmentChanged(user, training);
        StateHasChanged();
        _working = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}