﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using System.Text.Json;

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
            if (clt.IsCancellationRequested) return default(TRes);
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
            var cacheResult = (await _localStorageExpireService.GetItemAsync<TRes?>(cacheKey, clt));
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
}