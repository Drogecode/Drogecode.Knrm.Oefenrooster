using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class UserRoleLinkedMapper
{
    internal static List<DrogeUserRoleLinked> ToDrogeUserRoleLinked(this DbUsers dbUser)
    {
        var drogeUserRoles = new List<DrogeUserRoleLinked>();
        if (dbUser.LinkUserRoles is null)
            return drogeUserRoles;
        foreach (var userLinkUserRole in dbUser.LinkUserRoles)
        {
            drogeUserRoles.Add(new DrogeUserRoleLinked
            {
                
                Id = userLinkUserRole.Role.Id,
                ExternalId = userLinkUserRole.Role.ExternalId,
                Name = userLinkUserRole.Role.Name,
                SetExternal = userLinkUserRole.SetExternal,
                IsSet = userLinkUserRole.IsSet,
            });
        }

        return drogeUserRoles;
    }

    public static DbUserRoles ToDb(this DrogeUserRoleLinked userRole, Guid customerId)
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