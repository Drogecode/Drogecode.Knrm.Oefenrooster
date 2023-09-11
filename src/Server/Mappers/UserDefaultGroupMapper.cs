using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class UserDefaultGroupMapper
{
    public static DefaultGroup ToDefaultGroups(this DbUserDefaultGroup defaultGroup)
    {
        return new DefaultGroup
        {
            Id = defaultGroup.Id,
            Name = defaultGroup.Name,
            ValidFrom = defaultGroup.ValidFrom,
            ValidUntil = defaultGroup.ValidUntil,
            IsDefault = defaultGroup.IsDefault,
        };
    }
    public static DbUserDefaultGroup ToDbUserDefaultGroup(this DefaultGroup defaultGroup, Guid customerId, Guid userId)
    {
        return new DbUserDefaultGroup
        {
            Id = defaultGroup.Id,
            CustomerId = customerId,
            UserId = userId,
            Name = defaultGroup.Name,
            ValidFrom = defaultGroup.ValidFrom,
            ValidUntil = defaultGroup.ValidUntil,
            IsDefault = defaultGroup.IsDefault,
        };
    }
}
