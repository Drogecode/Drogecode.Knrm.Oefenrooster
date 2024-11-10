namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

public class GetLinkedUsersByIdResponse : BaseMultipleResponse
{
    public List<Guid>? LinkedUsers { get; set; }
}