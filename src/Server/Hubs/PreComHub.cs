using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Drogecode.Knrm.Oefenrooster.Server.Hubs;

public class PreComHub : Hub
{
    [Authorize]
    public async Task SendMessage(Guid id, string user, string message)
    {
        if (Clients is not null)
        {
            await Clients.All.SendAsync($"ReceivePrecomAlert_{id}", user, message);
        }
    }
}
