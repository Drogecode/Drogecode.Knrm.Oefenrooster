using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Drogecode.Knrm.Oefenrooster.Server.Hubs;

public class ConfigurationHub : Hub
{
    [Authorize]
    public async Task SendMessage(ConfigurationUpdatedHub type)
    {
        if (Clients is not null)
        {
            await Clients.All.SendAsync($"configuration", type);
        }
    }
}