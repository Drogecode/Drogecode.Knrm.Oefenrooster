namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class StatisticsUserTableConfigureSettings
{
    public IEnumerable<DistinctType?> SelectedTypes { get; set; } = [];
    public IEnumerable<int> SelectedYear { get; set; } = [];
    public decimal Compensation { get; set; } = 1.25M;
}