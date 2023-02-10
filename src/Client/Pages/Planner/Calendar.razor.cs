﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<TrainingWeek> _calendarForUser = new();
    private CancellationTokenSource _cls = new();
    private int? _month;
    private int _high = -1;
    private int _low = -2;

    protected override async Task OnInitializedAsync()
    {
        Global.NewTrainingAddedAsync += HandleNewTraining;
        for (int i = -1; i < 6; i++)
        {
            await AddWeekToCalendar(true);
        }
    }

    private async Task AddWeekToCalendar(bool high)
    {
        TrainingWeek scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(high ? _high : _low, _cls.Token))?.Trainings;
        if (trainingsInWeek != null)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInWeek[0].Date);
            foreach (var training in trainingsInWeek)
            {
                scheduleForUser.Trainings.AddLast(training);
                scheduleForUser.Till = DateOnly.FromDateTime(training.Date);
            }
        }
        if (high)
        {
            _calendarForUser.AddLast(scheduleForUser);
            _high++;
        }
        else
        {
            _calendarForUser.AddFirst(scheduleForUser);
            _low--;
        }
        StateHasChanged();
    }
    private async Task HandleNewTraining(NewTraining newTraining, Guid newId)
    {
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(newTraining));
        if (newTraining.Date == null) return;
        var asTraining = new Training
        {
            TrainingId = newId,
            Name= newTraining.Name,
            Date = ((newTraining.Date ?? throw new ArgumentNullException("Date is null")) + (newTraining.StartTime ?? throw new ArgumentNullException("StartTime is null"))).ToUniversalTime(),
            Duration = Convert.ToInt32((newTraining.EndTime ?? throw new ArgumentNullException("EndTime is null")).Subtract(newTraining.StartTime ?? throw new ArgumentNullException("StartTime is null")).TotalMinutes)
        };
        var date = DateOnly.FromDateTime(newTraining.Date ?? throw new UnreachableException("newTraining.Date is null after null check"));
        bool set = false;
        foreach (var week in _calendarForUser)
        {
            Console.WriteLine($"{week.From} <= {date} && {week.Till} >= {date}");
            Console.WriteLine($"week.From <= date && week.Till >= date {week.From <= date} {week.Till >= date}");
            if (week.From <= date && week.Till >= date)
            {
                week.Trainings.AddLast(asTraining);
                StateHasChanged();
                //ToDo correct moment of week.
                /*Training last;
                foreach (var training in week.Trainings)
                {
                    last = training;
                    if (training.Date.CompareTo(newTraining.Date) >= 0)
                        continue;
                    week.Trainings.AddAfter(last, new Training { });

                }*/
                return;
            }
        }
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(asTraining));
    }

    public void Dispose()
    {
        Global.NewTrainingAddedAsync -= HandleNewTraining;
        _cls.Cancel();
    }
}
