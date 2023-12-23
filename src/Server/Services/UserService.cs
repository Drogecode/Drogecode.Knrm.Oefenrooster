﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
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
        var dbUsers = _database.Users.Where(u => u.CustomerId == customerId && u.DeletedOn == null && (includeHidden || u.UserFunction == null || u.UserFunction.IsActive)).OrderBy(x => x.Name);
        foreach (var dbUser in dbUsers)
        {
            result.DrogeUsers.Add(new DrogeUser
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Created = dbUser.CreatedOn,
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
        var userObj = await _database.Users
            .Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null))
            .Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null))
            .FirstOrDefaultAsync(u => u.Id == userId);
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
        return DbUserToSharedUser(userObj!);
    }

    private DrogeUser DbUserToSharedUser(DbUsers dbUsers)
    {
        var user = new DrogeUser
        {
            Id = dbUsers.Id,
            Name = dbUsers.Name,
            Created = dbUsers.CreatedOn,
            LastLogin = dbUsers.LastLogin,
            UserFunctionId = dbUsers.UserFunctionId,
        };
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
        return user;
    }

    public async Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId)
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
    public async Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId)
    {
        var result = new UpdateLinkUserUserForUserResponse();
        var sw = Stopwatch.StartNew();

        var userA = await _database.Users.Include(x=>x.LinkedUserAsA!.Where(y=> y.DeletedBy == null)).Include(x=>x.LinkedUserAsB!.Where(y => y.DeletedBy == null)).FirstOrDefaultAsync(x=>x.Id == body.UserAId && x.CustomerId == customerId && x.DeletedBy == null);
        if (userA?.LinkedUserAsA?.Any(x=>x.UserBId == body.UserBId ) != true)
        {
            var userB = await _database.Users.Include(x => x.LinkedUserAsA!.Where(y => y.DeletedBy == null)).Include(x => x.LinkedUserAsB!.Where(y => y.DeletedBy == null)).FirstOrDefaultAsync(x => x.Id == body.UserBId && x.CustomerId == customerId && x.DeletedBy == null);
            if (userB is not null)
            {
                var link = new DbLinkUserUser
                {
                    UserAId = body.UserAId,
                    UserBId = body.UserBId,
                    LinkType = body.LinkType,
                };
                _database.LinkUserUsers.Add(link);
                result.Success = (await _database.SaveChangesAsync()) > 0;
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<bool> PatchLastOnline(Guid userId, CancellationToken clt)
    {

        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (userObj is not null && userObj.LastLogin.AddMinutes(1).CompareTo(DateTime.UtcNow) < 0)
        {
            userObj.LastLogin = DateTime.UtcNow;
            _database.Users.Update(userObj);
            return (await _database.SaveChangesAsync() > 0);
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
            if (dbUser != null)
            {
                dbUser.DeletedOn = DateTime.UtcNow;
                dbUser.DeletedBy = userId;
            }
        }
        return await _database.SaveChangesAsync() > 0;
    }
}
