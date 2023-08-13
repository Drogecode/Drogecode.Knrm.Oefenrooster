using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Diagnostics;

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
        var dbUsers = _database.Users.Where(u => u.CustomerId == customerId && u.DeletedOn == null && (includeHidden || u.UserFunction == null || u.UserFunction.IsActive)).OrderBy(x=>x.Name);
        foreach (var dbUser in dbUsers)
        {
            result.DrogeUsers.Add(new DrogeUser
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Created = dbUser.Created,
                LastLogin = includeLastLogin ? dbUser.LastLogin : DateTime.MinValue,
                UserFunctionId = dbUser.UserFunctionId,
            });
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DrogeUser?> GetUserFromDb(Guid userId)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        return DbUserToSharedUser(userObj);
    }

    public async Task<DrogeUser> GetOrSetUserFromDb(Guid userId, string userName, string userEmail, Guid customerId, bool setLastOnline)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (userObj == null)
        {
            var result = _database.Users.Add(new DbUsers
            {
                Id = userId,
                Name = userName,
                Email = userEmail,
                Created = DateTime.UtcNow,
                CustomerId = customerId
            });
            _database.SaveChanges();
            userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        }
        if (userObj != null)
        {
            if (setLastOnline)
                userObj.LastLogin = DateTime.UtcNow;
            userObj.Name = userName;
            userObj.Email = userEmail;
            userObj.DeletedOn = null;
            if (userObj.UserFunctionId == null || userObj.UserFunctionId == Guid.Empty)
            {
                var defaultFunction = await _database.UserFunctions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsDefault);
                if (defaultFunction != null)
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
        return DbUserToSharedUser(userObj!);
    }

    private DrogeUser DbUserToSharedUser(DbUsers dbUsers)
    {
        return new DrogeUser
        {
            Id = dbUsers.Id,
            Name = dbUsers.Name,
            Created = dbUsers.Created,
            LastLogin = dbUsers.LastLogin,
            UserFunctionId = dbUsers.UserFunctionId,
        };
    }

    public async Task<bool> UpdateUser(DrogeUser user, Guid userId, string userName, string userEmail, Guid customerId)
    {
        var oldVersion = await _database.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.CustomerId == customerId && u.DeletedOn == null);
        if (oldVersion != null)
        {
            oldVersion.UserFunctionId = user.UserFunctionId;
            _database.Users.Update(oldVersion);
            await _database.SaveChangesAsync();
            return true;
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
            Created = DateTime.UtcNow,
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
            if (dbUser != null)
            {
                dbUser.DeletedOn = DateTime.UtcNow;
                dbUser.DeletedBy = userId;
            }
        }
        return await _database.SaveChangesAsync() > 0;
    }
}
