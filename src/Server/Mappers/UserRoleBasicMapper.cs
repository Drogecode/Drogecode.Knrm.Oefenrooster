using System.Text;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

internal static class UserRoleBasicMapper
{
    internal static DrogeUserRoleBasic ToDrogeUserRoleBasic(this DbUserRoles dbUserRoles)
    {
        var drogeUserRole = new DrogeUserRoleBasic
        {
            Id = dbUserRoles.Id,
            ExternalId = dbUserRoles.ExternalId,
            Name = dbUserRoles.Name
        };

        return drogeUserRole;
    }

    public static DbUserRoles ToDb(this DrogeUserRoleBasic userRole, Guid customerId)
    {
        var dbUserRole = new DbUserRoles
        {
            Id = userRole.Id,
            ExternalId = userRole.ExternalId,
            CustomerId = customerId,
            Name = userRole.Name
        };

        return dbUserRole;
    }
}