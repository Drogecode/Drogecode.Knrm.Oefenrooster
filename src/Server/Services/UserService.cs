using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly Database.DataContext _database;
    public UserService(ILogger<UserService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<DrogeUser>> GetAllUsers(Guid customerId)
    {
        var result = new List<DrogeUser>();
        var dbUsers = _database.Users.Where(u => u.CustomerId == customerId && u.DeletedOn == null);
        foreach (var dbUser in dbUsers)
        {
            result.Add(new DrogeUser
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Created = dbUser.Created,
                UserFunctionId = dbUser.UserFunctionId,
            });
        }
        return result;
    }

    public async Task<DrogeUser?> GetUserFromDb(Guid userId)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        return DbUserToSharedUser(userObj);
    }

    public async Task<DrogeUser> GetOrSetUserFromDb(Guid userId, string userName, string userEmail, Guid customerId)
    {
        var userObj = _database.Users.Where(u => u.Id == userId && u.DeletedOn == null).FirstOrDefault();
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
            userObj.LastLogin = DateTime.UtcNow;
            if (userObj.UserFunctionId == null || userObj.UserFunctionId == Guid.Empty)
            {
                var defaultFunction = await _database.UserFunctions.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Default);
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
        return DbUserToSharedUser(userObj);
    }

    private DrogeUser DbUserToSharedUser(DbUsers dbUsers)
    {
        return new DrogeUser
        {
            Id = dbUsers.Id,
            Name = dbUsers.Name,
            Created = dbUsers.Created
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
}
