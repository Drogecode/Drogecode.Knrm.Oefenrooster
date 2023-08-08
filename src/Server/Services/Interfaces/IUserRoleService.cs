using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserRoleService
{
    Task<List<string>> GetAccessForUser(Guid customerId, IEnumerable<Claim> claims);
}
