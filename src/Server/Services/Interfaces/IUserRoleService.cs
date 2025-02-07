using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserRoleService
{
    Task<NewUserRoleResponse> NewUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt);
    Task<List<string>> GetAccessForUser(Guid userId, Guid customerId, IEnumerable<Claim> claims, CancellationToken clt);
    Task<MultipleDrogeUserRolesResponse> GetAll(Guid customerId, CancellationToken clt);
    Task<GetUserRoleResponse> GetById(Guid id, Guid userId, Guid customerId, CancellationToken clt);
    Task<UpdateUserRoleResponse> PatchUserRole(DrogeUserRole userRole, Guid userId, Guid customerId, CancellationToken clt);
}
