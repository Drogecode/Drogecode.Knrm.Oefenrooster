using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using ZXing;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class OfflineService : IOfflineService
{

    private readonly ILocalStorageExpireService _localStorageExpireService;
    private readonly ISessionExpireService _sessionStorageExpireService;

    public OfflineService(
        ILocalStorageExpireService localStorageExpireService,
        ISessionExpireService sessionStorageExpireService)
    {
        _localStorageExpireService = localStorageExpireService;
        _sessionStorageExpireService = sessionStorageExpireService;
    }

    public async Task<TRes?> CachedRequestAsync<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest? request = null, CancellationToken clt = default)
    {
        try
        {
            request ??= new ApiCachedRequest();
            if (request.CachedAndReplace)
                _ = Task.Run(function);

            if ((request.CachedAndReplace || request.OneCallPerSession) && !request.ForceCache)
            {
                var sessionResult = await _sessionStorageExpireService.GetItemAsync<TRes>(cacheKey, clt);
                if (sessionResult is not null)
                    return sessionResult;
            }

            if (!request.CachedAndReplace)
            {
                var result = await function();
                await _localStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireLocalStorage, clt);
                if (request.OneCallPerSession)
                    await _sessionStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireSession, clt);
                return result;
            }
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
