namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class ApiCachedRequest
{
    public DateTime Expire { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool OneCallPerSession { get; set; } = false;
    public bool ForceCache { get; set; } = false;
}
