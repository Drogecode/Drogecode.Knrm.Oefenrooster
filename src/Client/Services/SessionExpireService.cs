using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class SessionExpireService : ISessionExpireService
{
    private readonly ISessionStorageService _sessionStorageService;

    private static string SESMONTH = "SesMonth";

    public SessionExpireService(ISessionStorageService sessionStorageService)
    {
        _sessionStorageService = sessionStorageService;
    }

    public async ValueTask<T?> GetItemAsync<T>(string key, CancellationToken clt = default)
    {
        var value = await _sessionStorageService.GetItemAsync<ExpiryStorageModel<T?>>(key, clt);
        var ttl = DateTime.UtcNow.Ticks;
        if (value is null || value.Ttl <= ttl) return default;
        var result = value.Data;
        return result;
    }

    public async ValueTask SetItemAsync<T>(string key, T data, DateTime expire, CancellationToken clt = default)
    {
        var value = new ExpiryStorageModel<T>
        {
            Data = data,
            Ttl = expire.Ticks
        };
        await _sessionStorageService.SetItemAsync(key, value, clt);
    }

    public async Task<DateTime> GetSelectedMonth(CancellationToken clt)
    {
        var value = await _sessionStorageService.GetItemAsync<DateTime?>(SESMONTH, clt);
        return value ?? DateTime.Today;
    }

    public async Task SetSelectedMonth(DateTime? dateTime, CancellationToken clt)
    {
        await _sessionStorageService.SetItemAsync(SESMONTH, dateTime, clt);
    }
}
