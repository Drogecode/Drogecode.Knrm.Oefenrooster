namespace Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

public interface ISessionExpireService
{
    ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default);
    ValueTask SetItemAsync<T>(string key, T data, DateTime expire, CancellationToken cancellationToken = default);
}
