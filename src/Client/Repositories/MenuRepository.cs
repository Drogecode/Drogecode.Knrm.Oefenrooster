using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class MenuRepository
{
    private readonly IMenuClient _menuClient;
    private readonly IOfflineService _offlineService;
    private const string MENUS = "menu";

    public MenuRepository(IMenuClient menuClient, IOfflineService offlineService)
    {
        _menuClient = menuClient;
        _offlineService = offlineService;
    }
    
    public async Task<List<DrogeMenu>?> GetAllAsync(bool forceCache, bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(MENUS),
            async () => await _menuClient.GetAllAsync(clt), 
            new ApiCachedRequest { OneCallPerSession = true, ForceCache = forceCache, CachedAndReplace = cachedAndReplace},
            clt: clt);
        return response?.Menus?.ToList();
    }
}