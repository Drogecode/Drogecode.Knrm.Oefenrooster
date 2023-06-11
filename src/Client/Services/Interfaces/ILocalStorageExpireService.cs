namespace Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

public interface ILocalStorageExpireService
{
    ValueTask<T> GetItemAsync<T>(string key, CancellationToken cancellationToken = default);
    ValueTask SetItemAsync<T>(string key, T data, DateTime expire, bool postRequest = false, CancellationToken cancellationToken = default);
}
