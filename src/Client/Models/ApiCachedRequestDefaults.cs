namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public static class ApiCachedRequestDefaults
{
    public static ApiCachedRequest LongCache = new()
    {
        OneCallPerSession = true,
        OneCallPerCache = true,
        ExpireSession = DateTime.UtcNow.AddDays(14),
        ExpireLocalStorage = DateTime.UtcNow.AddDays(14)
    };
}