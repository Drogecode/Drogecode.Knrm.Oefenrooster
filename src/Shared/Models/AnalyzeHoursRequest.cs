namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public class AnalyzeHoursRequest
{
    public int Year { get; set; }
    public string? Type { get; set; }
    public List<string>? Boats { get; set; }
}