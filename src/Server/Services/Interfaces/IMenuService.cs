using Drogecode.Knrm.Oefenrooster.Shared.Models.Menu;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IMenuService
{
    Task<MultipleMenuResponse> GetAllMenus(Guid customerId, CancellationToken clt);
}