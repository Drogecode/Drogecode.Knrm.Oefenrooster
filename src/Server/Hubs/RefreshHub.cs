using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Drogecode.Knrm.Oefenrooster.Server.Hubs;

public class RefreshHub : Hub
{
    [Authorize]
    public async Task SendMessage(Guid id, ItemUpdated type)
    {
        if (Clients is not null)
        {
            await Clients.All.SendAsync($"Refresh_{id}", type);
        }
    }
}
