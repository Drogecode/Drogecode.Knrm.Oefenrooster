using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using System.Diagnostics;

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

    public async Task<List<Guid>> GetLinkUserRolesAsync(Guid userId, bool onlyExternal, CancellationToken clt)
    {
        var links = await _database.LinkUserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsSet && (!onlyExternal || x.SetExternal))
            .Select(x => x.RoleId)
            .ToListAsync(clt);
        return links;
    }

    public async Task<GetLinkedUsersByIdResponse> GetLinkedUsersById(Guid roleId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetLinkedUsersByIdResponse();
        var query = _database.LinkUserRoles
            .Where(x => x.RoleId == roleId && x.IsSet)
            .Select(x => x.UserId);
        result.LinkedUsers = await query.ToListAsync(clt);
        result.TotalCount = await query.CountAsync(clt);
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<bool> LinkUserToRoleAsync(Guid userId, DrogeUserRoleBasic? role, bool isSet, bool setExternal, bool modifySetByExternal, CancellationToken clt)
    {
        if (role is null) return false;
        var link = await _database.LinkUserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == role.Id, cancellationToken: clt);
        if (link is null)
        {
            if (!_database.UserRoles.Any(x => x.ExternalId == role.ExternalId))
                return false;
            _database.LinkUserRoles.Add(new DbLinkUserRole()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.Id,
                IsSet = isSet,
                SetExternal = setExternal
            });
        }
        else if ((link.IsSet != isSet || link.SetExternal != setExternal) && (modifySetByExternal || !link.SetExternal))
        {
            link.IsSet = isSet;
            link.SetExternal = setExternal;
            _database.LinkUserRoles.Update(link);
        }

        return await _database.SaveChangesAsync(clt) > 0;
    }
}