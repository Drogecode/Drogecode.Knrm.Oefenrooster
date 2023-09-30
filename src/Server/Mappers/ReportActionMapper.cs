using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class ReportActionMapper
{
    //DbReportAction
    public static DbReportAction ToDefaultSchedule(this SharePointAction spAction)
    {
        var dbReports = new DbReportAction
        {
            Id = spAction.Id,
            Number = spAction.Number,
            ShortDescription = spAction.ShortDescription,
            Prio = spAction.Prio,
            Title = spAction.Title,
            Description = spAction.Description,
            Start = spAction.Start,
            Users = new List<DbReportUser>()
        };
        foreach(var user in spAction.Users) {
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
}
