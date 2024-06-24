namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public class DistinctResponse : BaseMultipleResponse
{
    public List<string?>? Values { get; set; }
    public string? Message { get; set; }
}