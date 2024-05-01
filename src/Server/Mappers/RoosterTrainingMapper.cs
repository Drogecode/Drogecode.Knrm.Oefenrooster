﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class RoosterTrainingMapper
{
    public static Training ToTraining(this DbRoosterTraining roosterTraining, Guid? userId = null)
    {
        var training = new Training
        {
            TrainingId = roosterTraining.Id,
            DefaultId = roosterTraining.RoosterDefaultId,
            RoosterTrainingTypeId = roosterTraining.RoosterTrainingTypeId,
            Name = roosterTraining.Name,
            DateStart = roosterTraining.DateStart,
            DateEnd = roosterTraining.DateEnd,
            CountToTrainingTarget = roosterTraining.CountToTrainingTarget,
            IsPinned = roosterTraining.IsPinned,
            ShowTime = roosterTraining?.ShowTime ?? false
        };
        if (roosterTraining?.RoosterAvailables is not null && userId is not null)
        {
            var userAva = roosterTraining.RoosterAvailables.FirstOrDefault(x=> x.UserId == userId);
            if (userAva is not null)
            {
                training.Assigned = userAva.Assigned;
                training.Availability = userAva.Available;
                training.SetBy = userAva.SetBy;
            }
        }
        return training;
    }
    public static PlannedTraining ToPlannedTraining(this DbRoosterTraining roosterTraining)
    {
        var training = new PlannedTraining
        {
            TrainingId = roosterTraining.Id,
            DefaultId = roosterTraining.RoosterDefaultId,
            RoosterTrainingTypeId = roosterTraining.RoosterTrainingTypeId,
            Name = roosterTraining.Name,
            DateStart = roosterTraining.DateStart,
            DateEnd = roosterTraining.DateEnd,
            CountToTrainingTarget = roosterTraining.CountToTrainingTarget,
            IsPinned = roosterTraining.IsPinned,
            ShowTime = roosterTraining.ShowTime ?? true,
            TrainingTypeName = roosterTraining.RoosterTrainingType?.Name
        };
        if (roosterTraining?.RoosterAvailables is not null)
        {
            foreach(var ava in roosterTraining.RoosterAvailables)
            {
                training.PlanUsers.Add(new PlanUser
                {
                    UserId = ava.UserId,
                    UserFunctionId = ava.User?.UserFunctionId,
                    PlannedFunctionId = ava.UserFunctionId ?? ava.User?.UserFunctionId,
                    Assigned = ava.Assigned,
                    Availability = ava.Available,
                    SetBy = ava.SetBy,
                    Name = ava.User?.Name ?? "Some dude",
                    VehicleId = ava.VehicleId,
                    CalendarEventId = ava.CalendarEventId,
                    Buddy = ava.User?.LinkedUserAsA?.FirstOrDefault(x=>x.LinkType == UserUserLinkType.Buddy)?.UserB?.Name
                });
            }
        }
        return training;
    }
}
