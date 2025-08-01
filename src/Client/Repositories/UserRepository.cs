﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class UserRepository
{
    private readonly IUserClient _userClient;
    private readonly IOfflineService _offlineService;

    private const string ALL_USERS = "usr_all_{0}";
    private const string ALL_USERS_CUSTOMER = "usr_all_{0}_{1}";
    private const string USERID = "usr_{0}";
    private const string CURRENTUSER = "cur_usr";

    public UserRepository(IUserClient userClient, IOfflineService offlineService)
    {
        _userClient = userClient;
        _offlineService = offlineService;
    }

    public async Task<List<DrogeUser>?> GetAllUsersAsync(bool includeHidden, bool forceCache, bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(ALL_USERS, includeHidden),
            async () => await _userClient.GetAllAsync(includeHidden, cachedAndReplace, clt), 
            new ApiCachedRequest { OneCallPerSession = true, ForceCache = forceCache, CachedAndReplace = cachedAndReplace},
            clt: clt);
        return response?.DrogeUsers?.ToList();
    }
    
    public async Task<List<DrogeUser>?> GetAllDifferentCustomerAsync(Guid customerId, bool includeHidden, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(ALL_USERS_CUSTOMER, includeHidden, customerId),
            async () => await _userClient.GetAllDifferentCustomerAsync(customerId, includeHidden, clt), 
            new ApiCachedRequest { OneCallPerSession = false},
            clt: clt);
        return response?.DrogeUsers?.ToList();
    }

    public async Task<DrogeUser?> GetCurrentUserAsync(CancellationToken clt = default)
    {
        var dbUser = await _offlineService.CachedRequestAsync(string.Format(CURRENTUSER),
            async () => await _userClient.GetCurrentUserAsync(clt),
            new ApiCachedRequest { OneCallPerSession = true, ForceCache = false, ExpireSession = DateTime.UtcNow.AddMinutes(1), ExpireLocalStorage = DateTime.UtcNow.AddHours(2) },
            clt: clt);
        return dbUser?.DrogeUser;
    }

    public async Task<DrogeUser?> GetById(Guid id, bool oneCallPerSession, CancellationToken clt = default)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(USERID, id),
            async () => await _userClient.GetByIdAsync(id, clt),
            new ApiCachedRequest { OneCallPerSession = oneCallPerSession, ExpireSession = DateTime.UtcNow.AddMinutes(30) },
            clt: clt);
        return response?.User;
    }

    public async Task<bool> UpdateUserAsync(DrogeUser user)
    {
        var successful = await _userClient.UpdateUserAsync(user);
        return successful.Success;
    }
    public async Task<bool> AddUserAsync(DrogeUser user)
    {
        var successful = (await _userClient.AddUserAsync(user)).Success;
        return successful;
    }
    public async Task<bool> AddUserDifferentCustomer(Guid customerId, DrogeUser user)
    {
        var successful = (await _userClient.AddUserDifferentCustomerAsync(customerId, user)).Success;
        return successful;
    }
    public async Task<bool> SyncAllUsersAsync()
    {
        var request = await _userClient.SyncAllUsersAsync();
        return request.Success;
    }
    public async Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUserAsync(UpdateLinkUserUserForUserRequest body, CancellationToken clt)
    {
        var request = await _userClient.UpdateLinkUserUserForUserAsync(body, clt);
        return request;
    }

}
