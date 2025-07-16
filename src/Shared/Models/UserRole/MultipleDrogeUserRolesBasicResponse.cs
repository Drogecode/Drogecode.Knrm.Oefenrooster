namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

public class MultipleDrogeUserRolesBasicResponse : BaseMultipleResponse
{
    public List<DrogeUserRoleBasic>? Roles { get; set; }
}