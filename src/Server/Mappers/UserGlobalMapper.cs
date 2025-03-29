using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class UserGlobalMapper
{
    public static DrogeUserGlobal ToDrogeUserGlobal(this DbUsersGlobal dbUsersGlobal)
    {
        return new DrogeUserGlobal
        {
            Id = dbUsersGlobal.Id,
            Name = dbUsersGlobal.Name,
            CreatedOn = dbUsersGlobal.CreatedOn,
            CreatedBy = dbUsersGlobal.CreatedBy
        };
    }
}