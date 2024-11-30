namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public class AnalyzeHoursResult : BaseMultipleResponse
{
    public List<UserCounters>? UserCounters { get; set; }
}

public class UserCounters
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int FullHours { get; set; }
    public double Minutes { get; set; }
}