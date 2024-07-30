using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserRoleService
{
    Task<NewUserRoleResponse> NewUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt);
    Task<List<string>> GetAccessForUser(Guid customerId, IEnumerable<Claim> claims, CancellationToken clt);
    Task<MultipleDrogeUserRolesResponse> GetAll(Guid userId, Guid customerId, CancellationToken clt);
}
