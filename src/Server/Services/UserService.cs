using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserService : DrogeService, IUserService
{
    public UserService(ILogger<UserService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService) : base(logger, database, memoryCache, dateTimeService)
    {
    }

    public async Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleDrogeUsersResponse { DrogeUsers = new List<DrogeUser>() };
        var dbUsers = await Database.Users
            .Where(u => u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser && (includeHidden || u.UserFunction == null || u.UserFunction.IsActive))
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null && y.UserB.DeletedOn == null))
            .ThenInclude(x => x.UserB)
            .Include(x => x.UserOnVersions!.Where(y => includeLastLogin && y.LastSeenOnThisVersion.CompareTo(DateTime.UtcNow.AddYears(-1)) >= 0))
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(clt);
        foreach (var dbUser in dbUsers)
        {
            result.DrogeUsers.Add(dbUser.ToSharedUser(includeLastLogin, false));
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DrogeUser?> GetUserById(Guid customerId, Guid userId, bool includePersonal, CancellationToken clt)
    {
        var user = await Database.Users
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .Include(x => x.LinkedUserAsB!.Where(y => y.DeletedOn == null))
            .Where(u => u.CustomerId == customerId && u.Id == userId && u.DeletedOn == null)
            .Select(x => x.ToSharedUser(includePersonal, false))
            .AsNoTracking()
            .FirstOrDefaultAsync(clt);
        return user;
    }

    public async Task<DrogeUser?> GetUserByPreComId(int preComUserId, CancellationToken clt)
    {
        var user = await Database.Users
            .Where(u => u.PreComId == preComUserId && u.DeletedOn == null)
            .Select(x => x.ToSharedUser(false, false))
            .AsNoTracking()
            .FirstOrDefaultAsync(clt);
        return user;
    }

    public async Task<DrogeUserServer?> GetUserByNameForServer(string? name, CancellationToken clt)
    {
        if (name is null) return null;
        var userObj = await Database.Users
            .Where(u => u.Name == name && u.DeletedOn == null)
            .AsNoTracking()
            .FirstOrDefaultAsync(clt);
        return userObj?.ToSharedUser(false, true);
    }

    public async Task<DrogeUserServer?> GetOrSetUserById(Guid? userId, string? externalId, string userName, string userEmail, Guid customerId, bool setLastOnline, CancellationToken clt)
    {
        var isNew = false;
        if (userId is null && externalId is null)
            throw new ArgumentException("Both userId and externalId are null");
        DbUsers? userObj;
        if (userId is null)
            userObj = await Database.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken: clt);
        else
            userObj = await Database.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: clt);
        if (userObj is null)
        {
            isNew = true;
            userId ??= Guid.NewGuid();
            var newUser = new DbUsers
            {
                Id = userId.Value,
                Name = userName,
                Email = userEmail,
                CreatedOn = DateTime.UtcNow,
                CustomerId = customerId,
                SyncedFromSharePoint = true,
            };
            if (externalId is not null)
                newUser.ExternalId = externalId;
            Database.Users.Add(newUser);
            await Database.SaveChangesAsync(clt);
            userObj = await Database.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: clt);
        }

        if (userObj is not null)
        {
            if (setLastOnline)
                userObj.LastLogin = DateTime.UtcNow;
            if (externalId is not null)
                userObj.ExternalId ??= externalId;
            userObj.Name = userName;
            userObj.Email = userEmail;
            userObj.DeletedOn = null;
            if (userObj.UserFunctionId is null || userObj.UserFunctionId == Guid.Empty)
            {
                var defaultFunction = await Database.UserFunctions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsDefault, cancellationToken: clt);
                if (defaultFunction is not null)
                {
                    userObj.UserFunctionId = defaultFunction.Id;
                }
                else
                {
                    Logger.LogWarning("No default UserFunction found for {CustomerId}", customerId);
                }
            }

            Database.Users.Update(userObj);
            await Database.SaveChangesAsync(clt);
        }

        var sharedUser = userObj?.ToSharedUser(false, false);
        if (sharedUser is null)
            return null;
        sharedUser.IsNew = isNew;
        return sharedUser;
    }

    public async Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId)
    {
        var oldVersion = await Database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser);
        if (oldVersion is not null)
        {
            oldVersion.UserFunctionId = user.UserFunctionId;
            oldVersion.SyncedFromSharePoint = user.SyncedFromSharePoint;
            oldVersion.RoleFromSharePoint = user.RoleFromSharePoint;
            oldVersion.Nr = user.Nr;
            oldVersion.Name = user.Name.Trim();
            Database.Users.Update(oldVersion);
            await Database.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var result = new UpdateLinkUserUserForUserResponse();
        var sw = Stopwatch.StartNew();

        var userA = await Database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null))
            .FirstOrDefaultAsync(x => x.Id == body.UserAId && x.CustomerId == customerId && x.DeletedBy == null);
        if (userA?.LinkedUserAsA?.Any(x => x.UserBId == body.UserBId) != true)
        {
            var userB = await Database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null))
                .FirstOrDefaultAsync(x => x.Id == body.UserBId && x.CustomerId == customerId && x.DeletedBy == null);
            if (userB is not null)
            {
                var linkExistTest = await Database.LinkUserUsers.Where(x => x.UserAId == body.UserAId && x.UserBId == body.UserBId).FirstOrDefaultAsync();
                if (linkExistTest is null)
                {
                    var newLink = new DbLinkUserUser
                    {
                        UserAId = body.UserAId,
                        UserBId = body.UserBId,
                        LinkType = body.LinkType,
                    };
                    Database.LinkUserUsers.Add(newLink);
                    result.Success = (await Database.SaveChangesAsync()) > 0;
                }
                else if (linkExistTest.DeletedOn is not null)
                {
                    linkExistTest.DeletedOn = null;
                    linkExistTest.DeletedBy = null;
                    Database.LinkUserUsers.Update(linkExistTest);
                    result.Success = (await Database.SaveChangesAsync()) > 0;
                }
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<UpdateLinkUserUserForUserResponse> RemoveLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var result = new UpdateLinkUserUserForUserResponse();
        var sw = Stopwatch.StartNew();
        var link = await Database.LinkUserUsers.Where(x => x.UserAId == body.UserAId && x.UserBId == body.UserBId && x.DeletedOn == null).FirstOrDefaultAsync(clt);
        if (link is not null)
        {
            link.DeletedOn = DateTime.UtcNow;
            link.DeletedBy = userId;
            Database.LinkUserUsers.Update(link);
            result.Success = (await Database.SaveChangesAsync()) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public Task<bool> PatchLastOnline(Guid userId, CancellationToken clt)
    {
        return PatchLastOnline(userId, null, null, clt);
    }

    public async Task<bool> PatchLastOnline(Guid? userId, Guid? customerId, string? clientVersion, CancellationToken clt)
    {
        if (userId is null)
            return false;
        if (clientVersion?.Length > 17 == true)
            clientVersion = clientVersion[..17];
        var cacheKey = "LastOnline_" + userId + clientVersion?.Replace(Environment.NewLine, "");
        var lastUpdated = MemoryCache.Get<DateTime?>(cacheKey);
        if (lastUpdated is not null && lastUpdated.Value.AddMinutes(1).CompareTo(DateTime.UtcNow) >= 0) return false;
        var userObj = await Database.Users.Where(u => u.Id == userId)
            .Include(x => x.UserOnVersions!.Where(y => clientVersion != null && y.Version == clientVersion))
            .FirstOrDefaultAsync(clt);
        if (userObj is null) return false;
        {
            userObj.LastLogin = DateTime.UtcNow;
            Database.Users.Update(userObj);
            MemoryCache.Set(cacheKey, DateTime.UtcNow, DateTimeOffset.Now.AddMinutes(1));
            if (customerId is null || clientVersion is null) return (await Database.SaveChangesAsync(clt) > 0);
            // Remember for multiple versions, because a user can be logged in on multiple devices running on different versions.
            // PWA can be on any version including years old versions.
            if (userObj.UserOnVersions?.Any(x => x.UserId == userId && x.CustomerId == customerId && x.Version == clientVersion) == true)
            {
                var userOnVersion = userObj.UserOnVersions.FirstOrDefault(x => x.UserId == userId && x.CustomerId == customerId && x.Version == clientVersion);
                userOnVersion!.LastSeenOnThisVersion = DateTime.UtcNow;
                Database.UserOnVersions.Update(userOnVersion);
            }
            else
            {
                var newUserOnVersion = new DbUserOnVersion
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                    CustomerId = customerId.Value,
                    Version = clientVersion,
                    LastSeenOnThisVersion = DateTime.UtcNow
                };
                Database.UserOnVersions.Add(newUserOnVersion);
            }

            return await Database.SaveChangesAsync(clt) > 0;
        }
    }

    public async Task<AddUserResponse> AddUser(DrogeUser user, Guid customerId)
    {
        var result = new AddUserResponse();
        Database.Users.Add(new DbUsers
        {
            Id = user.Id,
            Name = user.Name,
            Email = "",
            CreatedOn = DateTime.UtcNow,
            CustomerId = customerId,
            UserFunctionId = user.UserFunctionId,
        });
        result.UserId = user.Id;
        result.Success = await Database.SaveChangesAsync() > 0;
        return result;
    }

    public async Task<bool> MarkUserDeleted(DrogeUser user, Guid userId, Guid customerId, bool onlySyncedFromSharePoint)
    {
        var dbUser = await Database.Users
            .Where(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser && (!onlySyncedFromSharePoint || u.SyncedFromSharePoint))
            .Include(x => x.LinkedUserAsA)
            .Include(x => x.LinkedUserAsB)
            .FirstOrDefaultAsync();
        if (dbUser is null) return true;
        dbUser.DeletedOn = DateTime.UtcNow;
        dbUser.DeletedBy = userId;
        if (dbUser.LinkedUserAsA is not null)
        {
            foreach (var linkA in dbUser.LinkedUserAsA.Where(x => x.DeletedOn == null))
            {
                linkA.DeletedOn = DateTime.UtcNow;
                linkA.DeletedBy = userId;
            }
        }

        if (dbUser.LinkedUserAsB is not null)
        {
            foreach (var linkB in dbUser.LinkedUserAsB.Where(x => x.DeletedOn == null))
            {
                linkB.DeletedOn = DateTime.UtcNow;
                linkB.DeletedBy = userId;
            }
        }

        return true;
    }

    public async Task<bool> MarkMultipleUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId, bool onlySyncedFromSharePoint)
    {
        foreach (var user in existingUsers)
        {
            await MarkUserDeleted(user, userId, customerId, onlySyncedFromSharePoint);
        }

        return await Database.SaveChangesAsync() > 0;
    }
}