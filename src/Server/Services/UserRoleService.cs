using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserRoleService : IUserRoleService
{
    private readonly ILogger<UserRoleService> _logger;
    private readonly Database.DataContext _database;
    private readonly ILinkUserRoleService _linkUserRoleService;

    public UserRoleService(ILogger<UserRoleService> logger, Database.DataContext database, ILinkUserRoleService linkUserRoleService)
    {
        _logger = logger;
        _database = database;
        _linkUserRoleService = linkUserRoleService;
    }

    public async Task<NewUserRoleResponse> NewUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new NewUserRoleResponse();
        var newId = Guid.NewGuid();
        userRole.Id = newId;
        _database.UserRoles.Add(userRole.ToDb(customerId));
        result.Success = await _database.SaveChangesAsync(clt) > 0;
        if (result.Success)
            result.NewId = userRole.Id;

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<List<string>> GetAccessForUser(Guid userId, Guid customerId, IEnumerable<Claim> claims, CancellationToken clt)
    {
        var result = new List<string>();
        try
        {
            var roles = await _database.UserRoles.Where(x => x.CustomerId == customerId).ToListAsync(clt);
            /*var linkedRoles = await _linkUserRoleService.GetLinkUserRolesAsync(userId, clt);*/
            foreach (var claim in claims.Where(x => x.Type.Equals("groups")))
            {
                var role = roles.FirstOrDefault(x => string.CompareOrdinal(x.ExternalId, claim.Value) == 0);
                if (role is null)continue;
                /*await _linkUserRoleService.LinkUserToRoleAsync(userId, role.Id, true, true, clt);
                linkedRoles.Remove(role.Id);*/
                var accesses = role.Accesses?.Split(',');
                if (accesses is null) continue;
                foreach (var access in accesses)
                {
                    if (!result.Contains(access))
                        result.Add(access);
                }
            }

            /*
             // Disabled for performance, is done by the background service every night.
             
             foreach (var linkedRole in linkedRoles)
            {
                await _linkUserRoleService.LinkUserToRoleAsync(userId, linkedRole, false, true, clt);
            }*/
        }
        catch (Exception ex)
        {
            //Catch all to prevent issues when database is not yet updated.
            _logger.LogError(ex, "Failed to get roles for user");
            return new List<string>();
        }

        return result;
    }

    public async Task<MultipleDrogeUserRolesResponse> GetAll(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleDrogeUserRolesResponse();

        var roles = await _database.UserRoles.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).Select(x => x.ToDrogeUserRole()).ToListAsync(clt);
        result.Roles = roles;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetUserRoleResponse> GetById(Guid id, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetUserRoleResponse();

        var role = await _database.UserRoles.Where(x => x.CustomerId == customerId && x.Id == id).Select(x => x.ToDrogeUserRole()).FirstOrDefaultAsync(clt);
        if (role is not null)
        {
            result.Role = role;
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<UpdateUserRoleResponse> PatchUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new UpdateUserRoleResponse();

        var role = await _database.UserRoles.Where(x => x.CustomerId == customerId && x.Id == userRole.Id).FirstOrDefaultAsync(clt);
        if (role is not null)
        {
            var updated = userRole.ToDb(customerId);
            role.Accesses = updated.Accesses;
            role.Name = updated.Name;
            role.ExternalId = updated.ExternalId;
            _database.UserRoles.Update(role);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}