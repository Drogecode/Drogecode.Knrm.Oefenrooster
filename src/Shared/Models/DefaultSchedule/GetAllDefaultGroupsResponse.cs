namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class GetAllDefaultGroupsResponse : BaseMultipleResponse
{
    public List<DefaultGroup>? Groups { get; set; }
}
