﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
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

    public User? GetUserFromDb(Guid userId)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        return DbUserToSharedUser(userObj);
    }

    public User GetOrSetUserFromDb(Guid userId, string userName, string userEmail)
    {
        var userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        if (userObj == null)
        {
            var result = _database.Users.Add(new DbUsers
            {
                Id = userId,
                Name = userName,
                Email= userEmail,
                Created = DateTime.UtcNow
            });
            _database.SaveChanges();
            userObj = _database.Users.Where(u => u.Id == userId).FirstOrDefault();
        }
        return DbUserToSharedUser(userObj);
    }

    private User DbUserToSharedUser(DbUsers dbUsers)
    {
        return new User
        {
            Id = dbUsers.Id,
            Name = dbUsers.Name,
            Created = dbUsers.Created
        };
    }
}
