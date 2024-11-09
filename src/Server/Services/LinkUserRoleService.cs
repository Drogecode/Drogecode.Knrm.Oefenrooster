using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class LinkUserRoleService : ILinkUserRoleService
{
    private readonly ILogger<LinkUserRoleService> _logger;
    private readonly Database.DataContext _database;

    public LinkUserRoleService(ILogger<LinkUserRoleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<Guid>> GetLinkUserRolesAsync(Guid userId, CancellationToken clt)
    {
        var links = await _database.LinkUserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync(clt);
        return links;
    }

    public async Task<bool> LinkUserToRoleAsync(Guid userId, Guid roleId, bool isSet, bool setExternal, CancellationToken clt)
    {
        var link = await _database.LinkUserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken: clt);
        if (link is null)
        {
            if (!_database.UserRoles.Any(x => x.Id == roleId))
                return false;
            _database.LinkUserRoles.Add(new DbLinkUserRole()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                IsSet = isSet,
                SetExternal = setExternal
            });
        }
        else if (link.IsSet != isSet || link.SetExternal != setExternal)
        {
            link.IsSet = isSet;
            link.SetExternal = setExternal;
            _database.LinkUserRoles.Update(link);
        }

        return await _database.SaveChangesAsync(clt) > 0;
    }
}