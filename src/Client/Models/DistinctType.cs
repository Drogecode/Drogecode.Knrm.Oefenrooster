using Drogecode.Knrm.Oefenrooster.Client.Enums;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class DistinctType
{
    public string? Type { get; set; }
    public DistinctFromWhere From { get; set; }
}