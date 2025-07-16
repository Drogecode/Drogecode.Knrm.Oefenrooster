using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class MultipleLinkedUserRolesResponse : BaseMultipleResponse
{
    public List<DrogeUserRoleLinked>? Roles { get; set; }
}