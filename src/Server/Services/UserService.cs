using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly DataContext _database;
    private readonly IMemoryCache _memoryCache;

    public UserService(ILogger<UserService> logger, DataContext database, IMemoryCache memoryCache)
    {
        _logger = logger;
        _database = database;
        _memoryCache = memoryCache;
    }

    public async Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleDrogeUsersResponse { DrogeUsers = new List<DrogeUser>() };
        var dbUsers = await _database.Users
            .Where(u => u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser && (includeHidden || u.UserFunction == null || u.UserFunction.IsActive))
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
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

    public async Task<DrogeUser?> GetUserById(Guid customerId, Guid userId, CancellationToken clt)
    {
        var user = await _database.Users
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .Include(x => x.LinkedUserAsB!.Where(y => y.DeletedOn == null))
            .Where(u => u.CustomerId == customerId && u.Id == userId && u.DeletedOn == null)
            .Select(x=>x.ToSharedUser(false, false))
            .AsNoTracking()
            .FirstOrDefaultAsync(clt);
        return user;
    }

    public async Task<DrogeUser?> GetUserByPreComId(int preComUserId, CancellationToken clt)
    {
        var user = await _database.Users
            .Where(u => u.PreComId == preComUserId && u.DeletedOn == null)
            .Select(x=>x.ToSharedUser(false, false))
            .AsNoTracking()
            .FirstOrDefaultAsync(clt);
        return user;
    }

    public async Task<DrogeUserServer?> GetUserByNameForServer(string? name, CancellationToken clt)
    {
        if (name is null) return null;
        var userObj = await _database.Users
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
            userObj = await _database.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken: clt);
        else
            userObj = await _database.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: clt);
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
            _database.Users.Add(newUser);
            await _database.SaveChangesAsync(clt);
            userObj = await _database.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: clt);
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
                var defaultFunction = await _database.UserFunctions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsDefault, cancellationToken: clt);
                if (defaultFunction is not null)
                {
                    userObj.UserFunctionId = defaultFunction.Id;
                }
                else
                {
                    _logger.LogWarning("No default UserFunction found for {CustomerId}", customerId);
                }
            }

            _database.Users.Update(userObj);
            await _database.SaveChangesAsync(clt);
        }

        var sharedUser = userObj?.ToSharedUser(false, false);
        if (sharedUser is null)
            return null;
        sharedUser.IsNew = isNew;
        return sharedUser;
    }

    public async Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId)
    {
        var oldVersion = await _database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser);
        if (oldVersion is not null)
        {
            oldVersion.UserFunctionId = user.UserFunctionId;
            oldVersion.SyncedFromSharePoint = user.SyncedFromSharePoint;
            oldVersion.RoleFromSharePoint = user.RoleFromSharePoint;
            oldVersion.Nr = user.Nr;
            oldVersion.Name = user.Name.Trim();
            _database.Users.Update(oldVersion);
            await _database.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var result = new UpdateLinkUserUserForUserResponse();
        var sw = Stopwatch.StartNew();

        var userA = await _database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null))
            .FirstOrDefaultAsync(x => x.Id == body.UserAId && x.CustomerId == customerId && x.DeletedBy == null);
        if (userA?.LinkedUserAsA?.Any(x => x.UserBId == body.UserBId) != true)
        {
            var userB = await _database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null))
                .FirstOrDefaultAsync(x => x.Id == body.UserBId && x.CustomerId == customerId && x.DeletedBy == null);
            if (userB is not null)
            {
                var linkExistTest = await _database.LinkUserUsers.Where(x => x.UserAId == body.UserAId && x.UserBId == body.UserBId).FirstOrDefaultAsync();
                if (linkExistTest is null)
                {
                    var newLink = new DbLinkUserUser
                    {
                        UserAId = body.UserAId,
                        UserBId = body.UserBId,
                        LinkType = body.LinkType,
                    };
                    _database.LinkUserUsers.Add(newLink);
                    result.Success = (await _database.SaveChangesAsync()) > 0;
                }
                else if (linkExistTest.DeletedOn is not null)
                {
                    linkExistTest.DeletedOn = null;
                    linkExistTest.DeletedBy = null;
                    _database.LinkUserUsers.Update(linkExistTest);
                    result.Success = (await _database.SaveChangesAsync()) > 0;
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
        var link = await _database.LinkUserUsers.Where(x => x.UserAId == body.UserAId && x.UserBId == body.UserBId && x.DeletedOn == null).FirstOrDefaultAsync(clt);
        if (link is not null)
        {
            link.DeletedOn = DateTime.UtcNow;
            link.DeletedBy = userId;
            _database.LinkUserUsers.Update(link);
            result.Success = (await _database.SaveChangesAsync()) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public Task<bool> PatchLastOnline(Guid userId, CancellationToken clt)
    {
        return PatchLastOnline(userId, null, null, clt);
    }

    public async Task<bool> PatchLastOnline(Guid userId, Guid? customerId, string? clientVersion, CancellationToken clt)
    {
        if (clientVersion?.Length > 17 == true)
            clientVersion = clientVersion[..17];
        var cacheKey = "LastOnline_" + userId + clientVersion?.Replace(Environment.NewLine, "");
        var lastUpdated = _memoryCache.Get<DateTime?>(cacheKey);
        if (lastUpdated is not null && lastUpdated.Value.AddMinutes(1).CompareTo(DateTime.UtcNow) >= 0) return false;
        var userObj = await _database.Users.Where(u => u.Id == userId)
            .Include(x => x.UserOnVersions!.Where(y => clientVersion != null && y.Version == clientVersion))
            .FirstOrDefaultAsync(clt);
        if (userObj is null) return false;
        {
            userObj.LastLogin = DateTime.UtcNow;
            _database.Users.Update(userObj);
            _memoryCache.Set(cacheKey, DateTime.UtcNow, DateTimeOffset.Now.AddMinutes(1));
            if (customerId is null || clientVersion is null) return (await _database.SaveChangesAsync(clt) > 0);
            // Remember for multiple versions, because a user can be logged in on multiple devices running on different versions.
            // PWA can be on any version including years old versions.
            if (userObj.UserOnVersions?.Any(x => x.UserId == userId && x.CustomerId == customerId && x.Version == clientVersion) == true)
            {
                var userOnVersion = userObj.UserOnVersions.FirstOrDefault(x => x.UserId == userId && x.CustomerId == customerId && x.Version == clientVersion);
                userOnVersion!.LastSeenOnThisVersion = DateTime.UtcNow;
                _database.UserOnVersions.Update(userOnVersion);
            }
            else
            {
                var newUserOnVersion = new DbUserOnVersion
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CustomerId = customerId.Value,
                    Version = clientVersion,
                    LastSeenOnThisVersion = DateTime.UtcNow
                };
                _database.UserOnVersions.Add(newUserOnVersion);
            }

            return (await _database.SaveChangesAsync(clt) > 0);
        }
    }

    public async Task<AddUserResponse> AddUser(DrogeUser user, Guid customerId)
    {
        var result = new AddUserResponse();
        _database.Users.Add(new DbUsers
        {
            Id = user.Id,
            Name = user.Name,
            Email = "",
            CreatedOn = DateTime.UtcNow,
            CustomerId = customerId,
            UserFunctionId = user.UserFunctionId,
        });
        result.UserId = user.Id;
        result.Success = await _database.SaveChangesAsync() > 0;
        return result;
    }

    public async Task<bool> MarkUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId)
    {
        foreach (var user in existingUsers)
        {
            var dbUser = await _database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null && !u.IsSystemUser && u.SyncedFromSharePoint);
            if (dbUser is not null)
            {
                dbUser.DeletedOn = DateTime.UtcNow;
                dbUser.DeletedBy = userId;
            }
        }

        return await _database.SaveChangesAsync() > 0;
    }
}