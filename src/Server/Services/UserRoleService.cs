using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserRoleService : DrogeService, IUserRoleService
{
    private readonly ILinkUserRoleService _linkUserRoleService;

    public UserRoleService(ILogger<UserRoleService> logger, 
        DataContext database, 
        IMemoryCache memoryCache, 
        IDateTimeProvider dateTimeProvider, 
        ILinkUserRoleService linkUserRoleService)
        : base(logger, database, memoryCache, dateTimeProvider)
    {
        _linkUserRoleService = linkUserRoleService;
    }

    public async Task<NewUserRoleResponse> NewUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new NewUserRoleResponse();
        var newId = Guid.NewGuid();
        userRole.Id = newId;
        Database.UserRoles.Add(userRole.ToDb(customerId));
        result.Success = await Database.SaveChangesAsync(clt) > 0;
        if (result.Success)
            result.NewId = userRole.Id;

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<List<string>> GetAccessForUserByClaims(Guid userId, Guid customerId, IEnumerable<Claim> claims, CancellationToken clt)
    {
        var roles = await Database.UserRoles.Where(x => x.CustomerId == customerId).ToListAsync(clt);
        var linkedRoles = await _linkUserRoleService.GetLinkUserRolesAsync(userId, true, clt);
        foreach (var claim in claims.Where(x => x.Type.Equals("groups")))
        {
            var role = roles.FirstOrDefault(x => string.CompareOrdinal(x.ExternalId, claim.Value) == 0);
            if (role is null) continue;
            await _linkUserRoleService.LinkUserToRoleAsync(userId, role.ToDrogeUserRole(), true, true, true, clt);
            linkedRoles.Remove(role.Id);
            Logger.LogInformation("Removed role `{role}` from user `{userId}`", role.Name, userId);
        }

        foreach (var linkedRole in linkedRoles)
        {
            var role = roles.FirstOrDefault(x => x.Id == linkedRole);
            if (role is null) continue;
            await _linkUserRoleService.LinkUserToRoleAsync(userId, role.ToDrogeUserRole(), false, false, true, clt);
            Logger.LogInformation("Linked role `{role}` to user `{userId}`", role.Name, userId);       
        }

        return await GetAccessForUserByUserId(userId, customerId, clt); // including access from roles linked by user.
    }

    public async Task<List<string>> GetAccessForUserByUserId(Guid userId, Guid customerId, CancellationToken clt)
    {
        var result = new List<string>();
        var linkedRoles = await _linkUserRoleService.GetLinkUserRolesAsync(userId, false, clt);
        foreach (var linkedRole in linkedRoles)
        {
            var accesses = await Database.UserRoles
                .Where(x => x.Id == linkedRole)
                .Select(x => x.Accesses)
                .FirstOrDefaultAsync(clt);
            if (accesses is null) continue;
            foreach (var access in accesses.Split(','))
            {
                if (!result.Contains(access.Trim()))
                    result.Add(access.Trim());
            }
        }

        return result;
    }

    public async Task<MultipleDrogeUserRolesResponse> GetAll(Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new MultipleDrogeUserRolesResponse();

        var roles = await Database.UserRoles
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Name) // Also has Order value, but not used.
            .Select(x => x.ToDrogeUserRole())
            .ToListAsync(clt);
        result.Roles = roles;
        result.TotalCount = roles.Count;
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultipleDrogeUserRolesBasicResponse> GetAllBasic(Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new MultipleDrogeUserRolesBasicResponse();

        var roles = await Database.UserRoles
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Name) // Also has Order value, but not used.
            .Select(x => x.ToDrogeUserRoleBasic())
            .ToListAsync(clt);
        result.Roles = roles;
        result.TotalCount = roles.Count;
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetUserRoleResponse> GetById(Guid id, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new GetUserRoleResponse();

        var role = await Database.UserRoles
            .Where(x => x.CustomerId == customerId && x.Id == id)
            .Select(x => x.ToDrogeUserRole())
            .FirstOrDefaultAsync(clt);
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
        var sw = StopwatchProvider.StartNew();
        var result = new UpdateUserRoleResponse();

        var role = await Database.UserRoles
            .Where(x => x.CustomerId == customerId && x.Id == userRole.Id)
            .FirstOrDefaultAsync(clt);
        if (role is not null)
        {
            var updated = userRole.ToDb(customerId);
            role.Accesses = updated.Accesses;
            role.Name = updated.Name;
            role.ExternalId = updated.ExternalId;
            Database.UserRoles.Update(role);
            result.Success = await Database.SaveChangesAsync(clt) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}