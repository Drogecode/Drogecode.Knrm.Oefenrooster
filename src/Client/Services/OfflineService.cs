using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class OfflineService : IOfflineService
{

    private readonly ILocalStorageService _localStorageService;
    private readonly ILocalStorageExpireService _localStorageExpireService;
    private readonly ISessionStorageService _sessionStorageService;
    private static bool _offline;
    private static bool _mockOffline;
    private static DateTime _lastOffline;
    public event Action? OfflineStatusChanged;

    public OfflineService(
        ILocalStorageService localStorageService,
        ILocalStorageExpireService localStorageExpireService,
        ISessionStorageService sessionStorageService)
    {
        _localStorageService = localStorageService;
        _localStorageExpireService = localStorageExpireService;
        _sessionStorageService = sessionStorageService;
    }

    private void CallOfflineStatusChanged()
    {
        OfflineStatusChanged?.Invoke();
    }

    public bool Offline
    {
        get => (_offline || MockOffline);
        set
        {
            if (_offline == value) return;
            _lastOffline = DateTime.UtcNow;
            _offline = value;
            CallOfflineStatusChanged();
        }
    }

    /// <summary>
    /// Not all HttpClient calls check the Offline value!
    /// </summary>
    public bool MockOffline
    {
        get => _mockOffline;
        set
        {
            _mockOffline = value;
            CallOfflineStatusChanged();
        }
    }

    public async Task<TRes> CachedRequestAsync<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest? request = null, CancellationToken clt = default)
    {
        try
        {
            request ??= new ApiCachedRequest();
            if (request.OneCallPerSession && !request.ForceCache)
            {
                var sessionResult = await _sessionStorageService.GetItemAsync<TRes>(cacheKey, clt);
                if (sessionResult != null)
                    return sessionResult;
            }

            if (!Offline)
            {
                var result = await function();
                await _localStorageExpireService.SetItemAsync(cacheKey, result, request.Expire, false, clt);
                if (request.OneCallPerSession)
                    await _sessionStorageService.SetItemAsync(cacheKey, result, clt);
                return result;
            }
        }
        catch (HttpRequestException)
        {
            Offline = true;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }

        var cacheResult = await _localStorageExpireService.GetItemAsync<TRes>(cacheKey, clt) ?? Activator.CreateInstance<TRes>();
        if (cacheResult is BaseResponse response)
        {
            response.Offline = true;
        }
        return cacheResult;
    }
}
