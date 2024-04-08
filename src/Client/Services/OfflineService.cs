using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using ZXing;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class OfflineService : IOfflineService
{

    private readonly ILocalStorageService _localStorageService;
    private readonly ILocalStorageExpireService _localStorageExpireService;
    private readonly ISessionExpireService _sessionStorageExpireService;
    private static bool _offline;
    private static bool _mockOffline;
    public event Action? OfflineStatusChanged;

    public OfflineService(
        ILocalStorageService localStorageService,
        ILocalStorageExpireService localStorageExpireService,
        ISessionExpireService sessionStorageExpireService)
    {
        _localStorageService = localStorageService;
        _localStorageExpireService = localStorageExpireService;
        _sessionStorageExpireService = sessionStorageExpireService;
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

    public async Task<TRes?> CachedRequestAsync<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest? request = null, CancellationToken clt = default)
    {
        try
        {
            request ??= new ApiCachedRequest();
            if (!Offline && request.CachedAndReplace)
                _ = Task.Run(function);

            if ((request.CachedAndReplace || request.OneCallPerSession) && !request.ForceCache)
            {
                var sessionResult = await _sessionStorageExpireService.GetItemAsync<TRes>(cacheKey, clt);
                if (sessionResult is not null)
                    return sessionResult;
            }

            if (!Offline && !request.CachedAndReplace)
            {
                var result = await function();
                await _localStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireLocalStorage, clt);
                if (request.OneCallPerSession)
                    await _sessionStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireSession, clt);
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

        try
        {
            var cacheResult = (await _localStorageExpireService.GetItemAsync<TRes?>(cacheKey, clt));
            cacheResult ??= Activator.CreateInstance<TRes>();
            if (cacheResult is BaseResponse response)
            {
                response.Offline = true;
            }
            return cacheResult;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }
        return default(TRes);
    }
}
