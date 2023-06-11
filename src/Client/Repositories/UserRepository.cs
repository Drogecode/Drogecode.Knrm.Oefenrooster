using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class UserRepository
{
    private readonly IUserClient _userClient;

    public UserRepository(IUserClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<List<DrogeUser>?> GetAllUsersAsync(bool includeHidden)
    {
        var dbUser = await _userClient.GetAllAsync(includeHidden);
        return dbUser.DrogeUsers?.ToList();
    }
    public async Task<DrogeUser?> GetCurrentUserAsync()
    {
        var dbUser = await _userClient.GetAsync();
        return dbUser.DrogeUser;
    }
    public async Task<bool> UpdateUserAsync(DrogeUser user)
    {
        var successfull = await _userClient.UpdateUserAsync(user);
        return successfull.Success;
    }
    public async Task<bool> AddUserAsync(DrogeUser user)
    {
        var successfull = await _userClient.AddUserAsync(user);
        return successfull;
    }
    public async Task<bool> SyncAllUsersAsync()
    {
        var request = await _userClient.SyncAllUsersAsync();
        return request.Success;
    }
}
