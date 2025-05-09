namespace Drogecode.Knrm.Oefenrooster.Server.Models.Background;

public class PreComPeriod
{
    public DateOnly Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsFullDay { get; set; }
}