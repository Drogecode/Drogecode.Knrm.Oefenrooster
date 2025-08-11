using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class OfflineService : IOfflineService
{
    private readonly ILocalStorageExpireService _localStorageExpireService;
    private readonly ISessionExpireService _sessionStorageExpireService;
    private readonly CustomStateProvider _customStateProvider;

    private static Guid? _userId;

    public OfflineService(
        ILocalStorageExpireService localStorageExpireService,
        ISessionExpireService sessionStorageExpireService,
        CustomStateProvider customStateProvider)
    {
        _localStorageExpireService = localStorageExpireService;
        _sessionStorageExpireService = sessionStorageExpireService;
        _customStateProvider = customStateProvider;
    }

    public async Task<TRes?> CachedRequestAsync<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest? request = null, CancellationToken clt = default)
    {
        try
        {
            cacheKey += $"__{_userId}";
            if (clt.IsCancellationRequested) return default(TRes);
            request ??= new ApiCachedRequest();
            if (request.CachedAndReplace)
            {
                var requestCopy = request;
                _ = Task.Run(async () => await RunSaveAndReturn(cacheKey, function, requestCopy, clt), clt);
            }

            if ((request.CachedAndReplace || request.OneCallPerSession) && !request.ForceCache)
            {
                var sessionResult = await _sessionStorageExpireService.GetItemAsync<TRes>(cacheKey, clt);
                if (sessionResult is not null)
                    return sessionResult;
            }

            if ((request.CachedAndReplace || request.OneCallPerCache) && !request.ForceCache)
            {
                var cacheResult = await _localStorageExpireService.GetItemAsync<TRes?>(cacheKey, clt);
                if (cacheResult is not null)
                    return cacheResult;
            }

            if (!request.CachedAndReplace)
            {
                return await RunSaveAndReturn(cacheKey, function, request, clt);
            }
        }
        catch (HttpRequestException)
        {
            DebugHelper.WriteLine("a HttpRequestException");
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }

        try
        {
            var cacheResult = await _localStorageExpireService.GetItemAsync<TRes?>(cacheKey, clt);
            cacheResult ??= Activator.CreateInstance<TRes>();
            if (cacheResult is BaseResponse response)
            {
                response.Offline = true;
            }

            return cacheResult;
        }
        catch (HttpRequestException)
        {
            DebugHelper.WriteLine("b HttpRequestException");
        }
        catch (TaskCanceledException)
        {
        }
        catch (JsonException)
        {
            // The object definition could be changed with an update. Deleting old version and retrying again to get latest version.
            DebugHelper.WriteLine($"JsonException for {cacheKey}, Deleting");
            await _localStorageExpireService.DeleteItemAsync(cacheKey, clt);
            request ??= new ApiCachedRequest();
            if (request.RetryOnJsonException) // Only retry once
            {
                DebugHelper.WriteLine($"Retry calling {cacheKey}");
                request.RetryOnJsonException = false;
                return await CachedRequestAsync<TRes>(cacheKey, function, request, clt);
            }

            DebugHelper.WriteLine($"Will not retry {cacheKey}");
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }

        return default(TRes);
    }

    private async Task<TRes> RunSaveAndReturn<TRes>(string cacheKey, Func<Task<TRes>> function, ApiCachedRequest request, CancellationToken clt)
    {
        var result = await function();
        await _localStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireLocalStorage, clt);
        if (request.OneCallPerSession)
            await _sessionStorageExpireService.SetItemAsync(cacheKey, result, request.ExpireSession, clt);
        return result;
    }

    public async Task<bool> SetUser()
    {
        var authState = await _customStateProvider.GetAuthenticationStateAsync();
        var userClaims = authState.User;
        if (userClaims?.Identity?.IsAuthenticated ?? false)
        {
            if (!Guid.TryParse(userClaims.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value,
                    out var userId))
                return false;
            _userId = userId;
        }
        else
        {
            // Should never happen.
            return false;
        }

        return true;
    }
}