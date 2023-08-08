using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserRoleService : IUserRoleService
{
    private readonly ILogger<UserRoleService> _logger;
    private readonly Database.DataContext _database;
    public UserRoleService(ILogger<UserRoleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<string>> GetAccessForUser(Guid customerId, IEnumerable<Claim> claims)
    {
        var result = new List<string>();
        try
        {
            var roles = _database.UserRoles.Where(x => x.CustomerId == customerId).ToList();
            foreach (var claim in claims.Where(x => x.Type.Equals("groups")))
            {
                var role = roles.FirstOrDefault(x => string.Compare(x.Id.ToString(), claim.Value, false) == 0);
                var accesses = role?.Accesses?.Split(',');
                if (accesses is null) continue;
                foreach (var acces in accesses)
                {
                    if (!result.Contains(acces))
                        result.Add(acces);
                }
            }
        }
        catch (Exception ex)
        {
            //Catch all to prevent issues when database is not yet updated.
            _logger.LogError(ex, "Failed to get roles for user");
            return new List<string>();
        }
        return result;
    }
}
