using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly DataContext _database;
    public UserService(ILogger<UserService> logger, DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleDrogeUsersResponse { DrogeUsers = new List<DrogeUser>() };
        var dbUsers = _database.Users
            .Where(u => u.CustomerId == customerId && u.DeletedOn == null && (includeHidden || u.UserFunction == null || u.UserFunction.IsActive))
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .ThenInclude(x => x.UserB)
            .OrderBy(x => x.Name)
            .ToList();
        foreach (var dbUser in dbUsers)
        {
            result.DrogeUsers.Add(new DrogeUser
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Created = dbUser.CreatedOn,
                LastLogin = includeLastLogin ? dbUser.LastLogin : DateTime.MinValue,
                UserFunctionId = dbUser.UserFunctionId,
                Buddy = dbUser.LinkedUserAsA?.FirstOrDefault(x => x.LinkType == UserUserLinkType.Buddy)?.UserB?.Name
            });
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DrogeUser?> GetUserFromDb(Guid userId)
    {
        var userObj = await _database.Users
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedOn == null))
            .Include(x => x.LinkedUserAsB!.Where(y => y.DeletedOn == null))
            .Where(u => u.Id == userId).FirstOrDefaultAsync();
        return userObj?.ToSharedUser();
    }

    public async Task<DrogeUser?> GetOrSetUserFromDb(Guid userId, string userName, string userEmail, Guid customerId, bool setLastOnline)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (userObj is null)
        {
            var result = _database.Users.Add(new DbUsers
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                CreatedOn = DateTime.UtcNow,
                CustomerId = customerId
            });
            _database.SaveChanges();
            userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        }
        if (userObj is not null)
        {
            if (setLastOnline)
                userObj.LastLogin = DateTime.UtcNow;
            userObj.Name = userName;
            userObj.Email = userEmail;
            userObj.DeletedOn = null;
            if (userObj.UserFunctionId is null || userObj.UserFunctionId == Guid.Empty)
            {
                var defaultFunction = await _database.UserFunctions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsDefault);
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
            await _database.SaveChangesAsync();
        }
        return userObj?.ToSharedUser();
    }

    public async Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId)
    {
        var oldVersion = await _database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null);
        if (oldVersion is not null)
        {
            oldVersion.UserFunctionId = user.UserFunctionId;
            oldVersion.SyncedFromSharePoint = user.SyncedFromSharePoint;
            oldVersion.RoleFromSharePoint = user.RoleFromSharePoint;
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

        var userA = await _database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null)).FirstOrDefaultAsync(x => x.Id == body.UserAId && x.CustomerId == customerId && x.DeletedBy == null);
        if (userA?.LinkedUserAsA?.Any(x => x.UserBId == body.UserBId) != true)
        {
            var userB = await _database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null)).FirstOrDefaultAsync(x => x.Id == body.UserBId && x.CustomerId == customerId && x.DeletedBy == null);
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
                else if (linkExistTest.DeletedOn is  not null)
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


    public async Task<bool> PatchLastOnline(Guid userId, CancellationToken clt)
    {

        var userObj = await _database.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(clt);
        if (userObj is not null && userObj.LastLogin.AddMinutes(1).CompareTo(DateTime.UtcNow) < 0)
        {
            userObj.LastLogin = DateTime.UtcNow;
            _database.Users.Update(userObj);
            return (await _database.SaveChangesAsync(clt) > 0);
        }
        return false;
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
            var dbUser = await _database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null);
            if (dbUser is not null)
            {
                dbUser.DeletedOn = DateTime.UtcNow;
                dbUser.DeletedBy = userId;
            }
        }
        return await _database.SaveChangesAsync() > 0;
    }
}
