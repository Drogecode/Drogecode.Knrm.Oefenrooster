using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class ReportActionSharedMapper
{
    public static DbReportActionShared ToDb(this ReportActionSharedConfiguration configuration, ILogger logger)
    {
        return new DbReportActionShared
        {
            Id = configuration.Id,
            SelectedUsers = configuration.SelectedUsers,
            Types = configuration.Types,
            Search = FilthyInputHelper.CleanList( configuration.Search, 5, logger),
            StartDate = configuration.StartDate,
            EndDate = configuration.EndDate,
            ValidUntil = configuration.ValidUntil,
        };
    }
    public static ReportActionSharedConfiguration ToDrogecode(this DbReportActionShared configuration)
    {
        return new ReportActionSharedConfiguration
        {
            Id = configuration.Id,
            CreatedBy = configuration.CreatedBy,
            SelectedUsers = configuration.SelectedUsers ?? [],
            Types = configuration.Types,
            Search = configuration.Search,
            StartDate = configuration.StartDate,
            EndDate = configuration.EndDate,
            ValidUntil = configuration.ValidUntil,
        };
    }
}