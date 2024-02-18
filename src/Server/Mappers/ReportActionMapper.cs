using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using System;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class ReportActionMapper
{
    public static DbReportAction ToDbDefaultSchedule(this SharePointAction spAction, Guid customerId)
    {
        var dbReports = new DbReportAction
        {
            Id = spAction.Id,
            CustomerId = customerId,
            Number = spAction.Number,
            ShortDescription = spAction.ShortDescription,
            Prio = spAction.Prio,
            Title = spAction.Title,
            Description = spAction.Description,
            Type = spAction.Type,
            Request = spAction.Request,
            ForTheBenefitOf = spAction.ForTheBenefitOf,
            Causes = spAction.Causes,
            Implications = spAction.Implications,
            Area = spAction.Area,
            WindDirection = spAction.WindDirection,
            WindPower = spAction.WindPower,
            WaterTemperature = spAction.WaterTemperature,
            GolfHight = spAction.GolfHight,
            Sight = spAction.Sight,
            WeatherCondition = spAction.WeatherCondition,
            CallMadeBy = spAction.CallMadeBy,
            CountSailors = spAction.CountSailors,
            CountSaved = spAction.CountSaved,
            CountAnimals = spAction.CountAnimals,
            Boat = spAction.Boat,
            FunctioningMaterial = spAction.FunctioningMaterial,
            ProblemsWithWeed = spAction.ProblemsWithWeed,
            Completedby = spAction.Completedby,

            LastUpdated = spAction.LastUpdated,
            Start = spAction.Start,
            End = spAction.End,
            Date = spAction.Date,
            Commencement = spAction.Commencement,

            Users = new List<DbReportUser>()
        };
        foreach (var user in spAction.Users)
        {
            dbReports.Users.Add(new()
            {
                Id = Guid.NewGuid(),
                SharePointID = user.SharePointID,
                Name = user.Name,
                DrogeCodeId = user.DrogeCodeId,
                Role = user.Role,
            });
        }
        return dbReports;
    }

    public static void UpdateDbDefaultSchedule(this DbReportAction dbAction, SharePointAction spAction, Guid customerId)
    {
        dbAction.Number = spAction.Number;
        dbAction.CustomerId = customerId;
        dbAction.ShortDescription = spAction.ShortDescription;
        dbAction.Prio = spAction.Prio;
        dbAction.Title = spAction.Title;
        dbAction.Description = spAction.Description;
        dbAction.Type = spAction.Type;
        dbAction.Request = spAction.Request;
        dbAction.ForTheBenefitOf = spAction.ForTheBenefitOf;
        dbAction.Causes = spAction.Causes;
        dbAction.Implications = spAction.Implications;
        dbAction.Area = spAction.Area;
        dbAction.WindDirection = spAction.WindDirection;
        dbAction.WindPower = spAction.WindPower;
        dbAction.WaterTemperature = spAction.WaterTemperature;
        dbAction.GolfHight = spAction.GolfHight;
        dbAction.Sight = spAction.Sight;
        dbAction.WeatherCondition = spAction.WeatherCondition;
        dbAction.CallMadeBy = spAction.CallMadeBy;
        dbAction.CountSailors = spAction.CountSailors;
        dbAction.CountSaved = spAction.CountSaved;
        dbAction.CountAnimals = spAction.CountAnimals;
        dbAction.Boat = spAction.Boat;
        dbAction.FunctioningMaterial = spAction.FunctioningMaterial;
        dbAction.ProblemsWithWeed = spAction.ProblemsWithWeed;
        dbAction.Completedby = spAction.Completedby;

        dbAction.LastUpdated = spAction.LastUpdated;
        dbAction.Start = spAction.Start;
        dbAction.End = spAction.End;
        dbAction.Date = spAction.Date;
        dbAction.Commencement = spAction.Commencement;

        if (dbAction.Users is not null)
        {
            foreach (var dbUser in dbAction.Users)
            {
                if (spAction.Users.Any(x => x.DrogeCodeId == dbUser.DrogeCodeId))
                {
                    var user = spAction.Users.FirstOrDefault(x => x.DrogeCodeId == dbUser.DrogeCodeId);
                    dbUser.IsDeleted = false;
                    dbUser.Role = user!.Role;
                    dbUser.Name = user!.Name;
                }
                else
                {
                    dbUser.IsDeleted = true;
                }
            }
            foreach (var user in spAction.Users)
            {
                if (dbAction.Users.Any(x => x.DrogeCodeId == user.DrogeCodeId))
                    continue;
                dbAction.Users.Add(new()
                {
                    Id = Guid.NewGuid(),
                    SharePointID = user.SharePointID,
                    Name = user.Name,
                    DrogeCodeId = user.DrogeCodeId,
                    Role = user.Role,
                });
            }
        }
    }
}
