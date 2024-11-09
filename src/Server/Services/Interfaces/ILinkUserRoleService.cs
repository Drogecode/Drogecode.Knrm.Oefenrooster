namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ILinkUserRoleService
{
    Task<List<Guid>> GetLinkUserRolesAsync(Guid userId, CancellationToken clt);
    Task<bool> LinkUserToRoleAsync(Guid userId, Guid roleId, bool isSet, bool setExternal, CancellationToken clt);
}