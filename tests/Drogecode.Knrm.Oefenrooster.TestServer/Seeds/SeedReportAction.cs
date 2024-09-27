using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public static class SeedReportAction
{
    public static void Seed(DataContext dataContext, Guid defaultCustomerId)
    {
        SeedForYear(dataContext, defaultCustomerId, 2022);
        SeedForYear(dataContext, defaultCustomerId, 2020);
        SeedForYear(dataContext, defaultCustomerId, 2019);
        SeedForYear(dataContext, defaultCustomerId, 2018);
        SeedForYear(dataContext, defaultCustomerId, 2016);
        SeedForYear(dataContext, defaultCustomerId, 2012);
        SeedForYear(dataContext, defaultCustomerId, 2004);
        dataContext.SaveChanges();
    }

    private static void SeedForYear(DataContext dataContext, Guid defaultCustomerId, int year)
    {
        var start = new DateTime(year, 3, 8, 8, 5, 41);
        dataContext.ReportActions.Add(new DbReportAction
        {
            Id = Guid.NewGuid(),
            CustomerId = defaultCustomerId,
            Description = "xUnit Description A",
            Start = start,
            Commencement = start.AddMinutes(5),
            Departure = start.AddMinutes(15),
            End = start.AddMinutes(121),
            Boat = "xUnit boat",
            Prio = "Prio 69",
            Users = new List<DbReportUser>{new DbReportUser{DrogeCodeId = DefaultSettingsHelper.IdTaco}},
        });
        start = start.AddDays(1);
        dataContext.ReportActions.Add(new DbReportAction
        {
            Id = Guid.NewGuid(),
            CustomerId = defaultCustomerId,
            Description = "xUnit Description B",
            Start = start,
            Commencement = start.AddMinutes(5),
            Departure = start.AddMinutes(15),
            End = start.AddMinutes(121),
            Boat = "xUnit boat",
            Prio = "Prio 1",
            Users = new List<DbReportUser>{new DbReportUser{DrogeCodeId = DefaultSettingsHelper.IdTaco}},
        });
        start = start.AddMonths(1);
        dataContext.ReportActions.Add(new DbReportAction
        {
            Id = Guid.NewGuid(),
            CustomerId = defaultCustomerId,
            Description = "xUnit Description C",
            Start = start,
            Commencement = start.AddMinutes(5),
            Departure = start.AddMinutes(15),
            End = start.AddMinutes(121),
            Boat = "xUnit boat",
            Prio = "Prio 1",
            Users = new List<DbReportUser>{new DbReportUser{DrogeCodeId = DefaultSettingsHelper.IdTaco}},
        });
    }
}