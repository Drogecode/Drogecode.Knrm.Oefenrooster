using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

internal static class UserMapper
{
    public static DrogeUserServer ToSharedUser(this DbUsers dbUsers, bool includeLastLogin, bool includeHashedPassword)
    {
        var user = new DrogeUserServer
        {
            Id = dbUsers.Id,
            ExternalId = dbUsers.ExternalId,
            Name = dbUsers.Name,
            Nr = dbUsers.Nr,
            Created = dbUsers.CreatedOn,
            LastLogin = includeLastLogin ? dbUsers.LastLogin : DateTime.MinValue,
            UserFunctionId = dbUsers.UserFunctionId,
            CustomerId = dbUsers.CustomerId,
            SyncedFromSharePoint = dbUsers.SyncedFromSharePoint,
            RoleFromSharePoint = dbUsers.RoleFromSharePoint,
            DirectLogin = dbUsers.DirectLogin,
            
            Buddy = dbUsers.LinkedUserAsA?.FirstOrDefault(x => x.LinkType == UserUserLinkType.Buddy)?.UserB?.Name
        };

        if (includeHashedPassword)
        {
            user.HashedPassword = dbUsers.HashedPassword;
        }
        
        if (dbUsers.LinkedUserAsA?.Count > 0)
        {
            user.LinkedAsA = [];
            foreach (var link in dbUsers.LinkedUserAsA)
            {
                user.LinkedAsA.Add(new LinkedDrogeUser
                {
                    LinkedUserId = link.UserBId,
                    LinkType = link.LinkType,
                });
            }
        }

        if (dbUsers.LinkedUserAsB?.Count > 0)
        {
            user.LinkedAsB = [];
            foreach (var link in dbUsers.LinkedUserAsB)
            {
                user.LinkedAsB.Add(new LinkedDrogeUser
                {
                    LinkedUserId = link.UserAId,
                    LinkType = link.LinkType,
                });
            }
        }

        if (dbUsers.UserOnVersions?.Count > 0)
        {
            user.Versions = (List<string>) [];
            foreach (var version in dbUsers.UserOnVersions.OrderByDescending(x => x.LastSeenOnThisVersion))
            {
                if (version.LastSeenOnThisVersion.AddDays(7).CompareTo(dbUsers.LastLogin) >= 0)
                    user.Versions.Add(version.Version);
            }
        }

        return user;
    }
}