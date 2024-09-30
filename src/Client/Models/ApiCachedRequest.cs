﻿namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class ApiCachedRequest
{
    /// <summary>
    /// Will be deleted from localstorge when this time has passed.
    /// </summary>
    public DateTime ExpireLocalStorage { get; set; } = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// If one call per session, expire after set time
    /// </summary>
    public DateTime ExpireSession { get; set; } = DateTime.UtcNow.AddMinutes(5);

    /// <summary>
    /// One call per session, but not expired.
    /// </summary>
    public bool OneCallPerSession { get; set; } = false;

    /// <summary>
    /// Ignore session cache
    /// </summary>
    public bool ForceCache { get; set; } = false;

    /// <summary>
    /// Return cached, but also call for update
    /// </summary>
    public bool CachedAndReplace { get; set; } = false;

    /// <summary>
    /// Retry on JsonException.
    /// </summary>
    public bool RetryOnJsonException { get; set; } = true;
}
