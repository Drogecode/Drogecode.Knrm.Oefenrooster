using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Shared.Mappers;

public static class DrogeUserRoleMapper
{
    public static DrogeUserRoleLinked ToDrogeUserRoleLinked(this DrogeUserRoleBasic role)
    {
        return new DrogeUserRoleLinked()
        {
            Id = role.Id,
            ExternalId = role.ExternalId,
            Name = role.Name,
        };
    }
}