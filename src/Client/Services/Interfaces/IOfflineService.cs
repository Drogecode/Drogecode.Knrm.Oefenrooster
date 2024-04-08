using Drogecode.Knrm.Oefenrooster.Client.Models;

namespace Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

public interface IOfflineService
{
    event Action? OfflineStatusChanged;
    bool Offline { get; set; }
    bool MockOffline { get; set; }
    Task<TRes?> CachedRequestAsync<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest? request = null, CancellationToken clt = default);
}
