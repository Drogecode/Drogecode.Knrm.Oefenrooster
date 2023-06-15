using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Drogecode.Knrm.Oefenrooster.Server.Hubs;

public class PreComHub : Hub
{
    [Authorize]
    public async Task SendMessage(string user, string message)
    {
        /*string username = Context.User.Identity.Name;
        if (string.Compare(username, user, true) == 0)*/
            await Clients.All.SendAsync("ReceivePrecomAlert", user, message);
    }
}
