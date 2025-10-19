using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class MenuService : IMenuService
{
    private readonly ILogger<MenuService> _logger;
    private readonly DataContext _database;

    public MenuService(ILogger<MenuService> logger, DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultipleMenuResponse> GetAllMenus(Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var result = new MultipleMenuResponse();

        var dbMenus = _database.Menus.Where(x => x.CustomerId == customerId).OrderBy(x => x.Order).Select(x => x.ToDrogeMenu());
        if (await dbMenus.AnyAsync(clt))
        {
            result.Menus = await dbMenus.ToListAsync(clt);
            result.TotalCount = await dbMenus.CountAsync(clt);
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}