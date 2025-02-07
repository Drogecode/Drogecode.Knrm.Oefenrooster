namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

public class MultipleDrogeUserRolesResponse : BaseMultipleResponse
{
    public List<DrogeUserRole>? Roles { get; set; }
}