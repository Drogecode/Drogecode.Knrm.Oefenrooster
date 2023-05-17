using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using System.Net.Http.Headers;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

var preComClient = new PreComClient(client, "drogecode");
await preComClient.Login("1267517Droogers", "WhvdI6bx8vAYGxm6YQ4g");
var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(DateTime.Today, DateTime.Today.AddDays(7));
var userGroups = await preComClient.GetAllUserGroups();
var d = await preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
if (false)
{
    // Does not work
    await preComClient.SendMessage(new MsgSend
    {
        SendBy = 37398,//7457,
        CalculateGroupID = userGroups[0].GroupID,
        ValidFrom = DateTime.Now,
        Message = "test",
        Priority = true,
        Receivers = new List<MsgReceivers>
        {
            new MsgReceivers
            {
                ID = 37398,
                Type = 0,
            }
        }
    });
}
await Task.Delay(1000);