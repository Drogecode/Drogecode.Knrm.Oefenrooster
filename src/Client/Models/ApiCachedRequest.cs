namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class ApiCachedRequest
{
    public DateTime ExpireLocalStorage { get; set; } = DateTime.UtcNow.AddDays(7);
    public DateTime ExpireSession { get; set; } = DateTime.UtcNow.AddMinutes(5);
    public bool OneCallPerSession { get; set; } = false;
    public bool ForceCache { get; set; } = false;
}
