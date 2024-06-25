using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class ReportActionMapper
{
    public static DbReportAction ToDbReportAction(this SharePointAction spAction, Guid customerId)
    {
        var dbReport = new DbReportAction
        {
            Id = spAction.Id,
            CustomerId = customerId,
            OdataEtag = spAction.OdataEtag,
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
            Departure = spAction.Departure,

            Users = new List<DbReportUser>()
        };
        if (spAction.Users is null) return dbReport;
        foreach (var user in spAction.Users)
        {
            dbReport.Users.Add(new()
            {
                Id = Guid.NewGuid(),
                DbReportActionId = dbReport.Id,
                SharePointID = user.SharePointID,
                Name = user.Name,
                DrogeCodeId = user.DrogeCodeId,
                Role = user.Role,
                Order = user.Order,
            });
        }

        return dbReport;
    }

    public static DbReportTraining ToDbReportTraining(this SharePointTraining spTraining, Guid customerId)
    {
        var dbTraining = new DbReportTraining
        {
            Id = spTraining.Id,
            CustomerId = customerId,
            OdataEtag = spTraining.OdataEtag,
            Title = spTraining.Title,
            Description = spTraining.Description,
            Type = spTraining.Type,
            TypeTraining = spTraining.TypeTraining,
            Area = spTraining.Area,
            WindDirection = spTraining.WindDirection,
            WindPower = spTraining.WindPower,
            WaterTemperature = spTraining.WaterTemperature,
            GolfHight = spTraining.GolfHight,
            Sight = spTraining.Sight,
            WeatherCondition = spTraining.WeatherCondition,
            FunctioningMaterial = spTraining.FunctioningMaterial,
            ProblemsWithWeed = spTraining.ProblemsWithWeed,
            LastUpdated = spTraining.LastUpdated,
            Start = spTraining.Start,
            Commencement = spTraining.Commencement,
            End = spTraining.End,
            Boat = spTraining.Boat,

            Users = new List<DbReportUser>()
        };
        foreach (var user in spTraining.Users)
        {
            dbTraining.Users.Add(new()
            {
                Id = Guid.NewGuid(),
                DbReportTrainingId = dbTraining.Id,
                SharePointID = user.SharePointID,
                Name = user.Name,
                DrogeCodeId = user.DrogeCodeId,
                Role = user.Role,
                Order = user.Order,
            });
        }

        return dbTraining;
    }

    public static DrogeAction ToDrogeAction(this DbReportAction dbAction)
    {
        return new DrogeAction
        {
            Number = dbAction.Number,
            ShortDescription = dbAction.ShortDescription,
            Prio = dbAction.Prio,
            Type = dbAction.Type,
            Request = dbAction.Request,
            ForTheBenefitOf = dbAction.ForTheBenefitOf,
            Causes = dbAction.Causes,
            Implications = dbAction.Implications,
            Area = dbAction.Area,
            WindDirection = dbAction.WindDirection,
            WindPower = dbAction.WindPower,
            WaterTemperature = dbAction.WaterTemperature,
            GolfHight = dbAction.GolfHight,
            Sight = dbAction.Sight,
            WeatherCondition = dbAction.WeatherCondition,
            CallMadeBy = dbAction.CallMadeBy,
            CountSailors = dbAction.CountSailors,
            CountSaved = dbAction.CountSaved,
            CountAnimals = dbAction.CountAnimals,
            FunctioningMaterial = dbAction.FunctioningMaterial,
            ProblemsWithWeed = dbAction.ProblemsWithWeed,
            Completedby = dbAction.Completedby,
            Departure = dbAction.Departure,

            //Shared
            Id = dbAction.Id,
            LastUpdated = dbAction.LastUpdated,
            Title = dbAction.Title,
            Description = dbAction.Description,
            Boat = dbAction.Boat,
            Date = dbAction.Date,
            Start = dbAction.Start,
            Commencement = dbAction.Commencement,
            End = dbAction.End,
            Users = dbAction.Users?.Select(x => x.ToSharePointUser()).ToList()
        };
    }

    public static DrogeTraining ToDrogeTraining(this DbReportTraining dbTraining)
    {
        return new DrogeTraining
        {
            Type = dbTraining.Type,
            Area = dbTraining.Area,
            WindDirection = dbTraining.WindDirection,
            WindPower = dbTraining.WindPower,
            WaterTemperature = dbTraining.WaterTemperature,
            GolfHight = dbTraining.GolfHight,
            Sight = dbTraining.Sight,
            WeatherCondition = dbTraining.WeatherCondition,
            FunctioningMaterial = dbTraining.FunctioningMaterial,
            ProblemsWithWeed = dbTraining.ProblemsWithWeed,

            //Shared
            Id = dbTraining.Id,
            LastUpdated = dbTraining.LastUpdated,
            Title = dbTraining.Title,
            Description = dbTraining.Description,
            Boat = dbTraining.Boat,
            Date = dbTraining.Date,
            Start = dbTraining.Start,
            Commencement = dbTraining.Commencement,
            End = dbTraining.End,
            Users = dbTraining.Users?.Select(x => x.ToSharePointUser()).ToList()
        };
    }

    public static SharePointUser ToSharePointUser(this DbReportUser dbReportUser)
    {
        return new SharePointUser
        {
            SharePointID = dbReportUser.SharePointID,
            DrogeCodeId = dbReportUser.DrogeCodeId,
            Name = dbReportUser.Name,
            Role = dbReportUser.Role,
            Order = dbReportUser.Order,
        };
    }

    public static void UpdateDbReportAction(this DbReportAction dbAction, SharePointAction spAction, Guid customerId)
    {
        if (!Equals(dbAction.CustomerId, customerId)) dbAction.CustomerId = customerId;
        if (!Equals(dbAction.Number, spAction.Number)) dbAction.Number = spAction.Number;
        if (!Equals(dbAction.ShortDescription, spAction.ShortDescription)) dbAction.ShortDescription = spAction.ShortDescription;
        if (!Equals(dbAction.Prio, spAction.Prio)) dbAction.Prio = spAction.Prio;
        if (!Equals(dbAction.Title, spAction.Title)) dbAction.Title = spAction.Title;
        if (!Equals(dbAction.Description, spAction.Description)) dbAction.Description = spAction.Description;
        if (!Equals(dbAction.Type, spAction.Type)) dbAction.Type = spAction.Type;
        if (!Equals(dbAction.Request, spAction.Request)) dbAction.Request = spAction.Request;
        if (!Equals(dbAction.ForTheBenefitOf, spAction.ForTheBenefitOf)) dbAction.ForTheBenefitOf = spAction.ForTheBenefitOf;
        if (!Equals(dbAction.Causes, spAction.Causes)) dbAction.Causes = spAction.Causes;
        if (!Equals(dbAction.Implications, spAction.Implications)) dbAction.Implications = spAction.Implications;
        if (!Equals(dbAction.Area, spAction.Area)) dbAction.Area = spAction.Area;
        if (!Equals(dbAction.WindDirection, spAction.WindDirection)) dbAction.WindDirection = spAction.WindDirection;
        if (!Equals(dbAction.WindPower, spAction.WindPower)) dbAction.WindPower = spAction.WindPower;
        if (!Equals(dbAction.WaterTemperature, spAction.WaterTemperature)) dbAction.WaterTemperature = spAction.WaterTemperature;
        if (!Equals(dbAction.GolfHight, spAction.GolfHight)) dbAction.GolfHight = spAction.GolfHight;
        if (!Equals(dbAction.Sight, spAction.Sight)) dbAction.Sight = spAction.Sight;
        if (!Equals(dbAction.WeatherCondition, spAction.WeatherCondition)) dbAction.WeatherCondition = spAction.WeatherCondition;
        if (!Equals(dbAction.CallMadeBy, spAction.CallMadeBy)) dbAction.CallMadeBy = spAction.CallMadeBy;
        if (!Equals(dbAction.CountSailors, spAction.CountSailors)) dbAction.CountSailors = spAction.CountSailors;
        if (!Equals(dbAction.CountSaved, spAction.CountSaved)) dbAction.CountSaved = spAction.CountSaved;
        if (!Equals(dbAction.CountAnimals, spAction.CountAnimals)) dbAction.CountAnimals = spAction.CountAnimals;
        if (!Equals(dbAction.Boat, spAction.Boat)) dbAction.Boat = spAction.Boat;
        if (!Equals(dbAction.FunctioningMaterial, spAction.FunctioningMaterial)) dbAction.FunctioningMaterial = spAction.FunctioningMaterial;
        if (!Equals(dbAction.ProblemsWithWeed, spAction.ProblemsWithWeed)) dbAction.ProblemsWithWeed = spAction.ProblemsWithWeed;
        if (!Equals(dbAction.Completedby, spAction.Completedby)) dbAction.Completedby = spAction.Completedby;

        if (!Equals(dbAction.LastUpdated, spAction.LastUpdated)) dbAction.LastUpdated = spAction.LastUpdated;
        if (!Equals(dbAction.Start, spAction.Start)) dbAction.Start = spAction.Start;
        if (!Equals(dbAction.End, spAction.End)) dbAction.End = spAction.End;
        if (!Equals(dbAction.Date, spAction.Date)) dbAction.Date = spAction.Date;
        if (!Equals(dbAction.Commencement, spAction.Commencement)) dbAction.Commencement = spAction.Commencement;
        if (!Equals(dbAction.Departure, spAction.Departure)) dbAction.Departure = spAction.Departure;

        if (dbAction.Users is not null)
        {
            foreach (var dbUser in dbAction.Users)
            {
                if (spAction.Users.Any(x => (x.DrogeCodeId is not null && x.DrogeCodeId == dbUser.DrogeCodeId)
                                            || (x.SharePointID is not null && x.SharePointID == dbUser.SharePointID)
                                            || (x.SharePointID == null && x.Name == dbUser.Name)))
                {
                    var user = spAction.Users.FirstOrDefault(x =>
                        (x.DrogeCodeId is not null && x.DrogeCodeId == dbUser.DrogeCodeId)
                        || (x.SharePointID is not null && x.SharePointID == dbUser.SharePointID)
                        || (x.SharePointID == null && x.Name == dbUser.Name));
                    if (!Equals(dbUser.IsDeleted, false)) dbUser.IsDeleted = false;
                    if (!Equals(dbUser.Role, user!.Role)) dbUser.Role = user.Role;
                    if (!Equals(dbUser.Name, user.Name)) dbUser.Name = user.Name;
                    if (!Equals(dbUser.DbReportActionId, dbAction.Id)) dbUser.DbReportActionId = dbAction.Id;
                }
                else
                {
                    if (!Equals(dbUser.IsDeleted, true)) dbUser.IsDeleted = true;
                }
            }

            foreach (var user in spAction.Users)
            {
                if (dbAction.Users.Any(x => (x.DrogeCodeId is not null && x.DrogeCodeId == user.DrogeCodeId)
                                            || (x.SharePointID is not null && x.SharePointID == user.SharePointID)
                                            || (x.SharePointID == null && x.Name == user.Name)))
                    continue;
                dbAction.Users.Add(new()
                {
                    IsNew = true,
                    Id = Guid.NewGuid(),
                    SharePointID = user.SharePointID,
                    Name = user.Name,
                    DrogeCodeId = user.DrogeCodeId,
                    Role = user.Role,
                    DbReportActionId = dbAction.Id,
                });
            }
        }
    }

    public static void UpdateDbReportTraining(this DbReportTraining dbTraining, SharePointTraining spTraining, Guid customerId)
    {
        if (!Equals(dbTraining.CustomerId, customerId)) dbTraining.CustomerId = customerId;
        if (!Equals(dbTraining.Title, spTraining.Title)) dbTraining.Title = spTraining.Title;
        if (!Equals(dbTraining.Description, spTraining.Description)) dbTraining.Description = spTraining.Description;
        if (!Equals(dbTraining.Start, spTraining.Start)) dbTraining.Start = spTraining.Start;
        if (!Equals(dbTraining.Type, spTraining.Type)) dbTraining.Type = spTraining.Type;
        if (!Equals(dbTraining.TypeTraining, spTraining.TypeTraining)) dbTraining.TypeTraining = spTraining.TypeTraining;
        if (!Equals(dbTraining.Area, spTraining.Area)) dbTraining.Area = spTraining.Area;
        if (!Equals(dbTraining.WindDirection, spTraining.WindDirection)) dbTraining.WindDirection = spTraining.WindDirection;
        if (!Equals(dbTraining.WindPower, spTraining.WindPower)) dbTraining.WindPower = spTraining.WindPower;
        if (!Equals(dbTraining.WaterTemperature, spTraining.WaterTemperature)) dbTraining.WaterTemperature = spTraining.WaterTemperature;
        if (!Equals(dbTraining.GolfHight, spTraining.GolfHight)) dbTraining.GolfHight = spTraining.GolfHight;
        if (!Equals(dbTraining.Sight, spTraining.Sight)) dbTraining.Sight = spTraining.Sight;
        if (!Equals(dbTraining.WeatherCondition, spTraining.WeatherCondition)) dbTraining.WeatherCondition = spTraining.WeatherCondition;
        if (!Equals(dbTraining.FunctioningMaterial, spTraining.FunctioningMaterial)) dbTraining.FunctioningMaterial = spTraining.FunctioningMaterial;
        if (!Equals(dbTraining.ProblemsWithWeed, spTraining.ProblemsWithWeed)) dbTraining.ProblemsWithWeed = spTraining.ProblemsWithWeed;
        if (!Equals(dbTraining.Commencement, spTraining.Commencement)) dbTraining.Commencement = spTraining.Commencement;
        if (!Equals(dbTraining.End, spTraining.End)) dbTraining.End = spTraining.End;
        if (!Equals(dbTraining.Boat, spTraining.Boat)) dbTraining.Boat = spTraining.Boat;

        if (dbTraining.Users is not null)
        {
            foreach (var dbUser in dbTraining.Users)
            {
                if (spTraining.Users.Any(x => x.DrogeCodeId == dbUser.DrogeCodeId || x.SharePointID == dbUser.SharePointID || (x.SharePointID == null && x.Name?.Equals(dbUser.Name) == true)))
                {
                    var user = spTraining.Users.FirstOrDefault(x =>
                        x.DrogeCodeId == dbUser.DrogeCodeId || x.SharePointID == dbUser.SharePointID || (x.SharePointID == null && x.Name?.Equals(dbUser.Name) == true));
                    if (!Equals(dbUser.IsDeleted, false)) dbUser.IsDeleted = false;
                    if (!Equals(dbUser.Role, user!.Role)) dbUser.Role = user.Role;
                    if (!Equals(dbUser.Name, user.Name)) dbUser.Name = user.Name;
                    if (!Equals(dbUser.DbReportTrainingId, dbTraining.Id)) dbUser.DbReportTrainingId = dbTraining.Id;
                }
                else
                {
                    if (!Equals(dbUser.IsDeleted, true)) dbUser.IsDeleted = true;
                }
            }

            foreach (var user in spTraining.Users)
            {
                if (dbTraining.Users.Any(x => (x.DrogeCodeId is not null && x.DrogeCodeId == user.DrogeCodeId)
                                              || (x.SharePointID is not null && x.SharePointID == user.SharePointID)
                                              || (x.SharePointID == null && x.Name == user.Name)))
                    continue;
                dbTraining.Users.Add(new()
                {
                    IsNew = true,
                    Id = Guid.NewGuid(),
                    SharePointID = user.SharePointID,
                    Name = user.Name,
                    DrogeCodeId = user.DrogeCodeId,
                    Role = user.Role,
                    DbReportTrainingId = dbTraining.Id
                });
            }
        }
    }
}