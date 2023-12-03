using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class UserRepository
{
    private readonly IUserClient _userClient;
    private readonly IOfflineService _offlineService;

    private const string MONTHITEMS = "usr_all_{0}";

    public UserRepository(IUserClient userClient, IOfflineService offlineService)
    {
        _userClient = userClient;
        _offlineService = offlineService;
    }

    public async Task<List<DrogeUser>?> GetAllUsersAsync(bool includeHidden, bool forceCache, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(MONTHITEMS, includeHidden),
            async () => await _userClient.GetAllAsync(includeHidden), 
            new ApiCachedRequest { OneCallPerSession = true, ForceCache = forceCache},
            clt: clt);
        return response.DrogeUsers?.ToList();
    }
    public async Task<DrogeUser?> GetCurrentUserAsync()
    {
        var dbUser = await _userClient.GetCurrentUserAsync();
        return dbUser.DrogeUser;
    }

    public async Task<DrogeUser?> GetById(Guid id)
    {
        var result = await _userClient.GetByIdAsync(id);
        return result.User;
    }

    public async Task<bool> UpdateUserAsync(DrogeUser user)
    {
        var successfull = await _userClient.UpdateUserAsync(user);
        return successfull.Success;
    }
    public async Task<bool> AddUserAsync(DrogeUser user)
    {
        var successfull = (await _userClient.AddUserAsync(user)).Success;
        return successfull;
    }
    public async Task<bool> SyncAllUsersAsync()
    {
        var request = await _userClient.SyncAllUsersAsync();
        return request.Success;
    }
}
