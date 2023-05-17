using Drogecode.Knrm.Oefenrooster.PreCom;
using System.Net.Http.Headers;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

var preComClient = new PreComClient(client, "drogecode");
await preComClient.Login("username", "passwoord");
var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(DateTime.Today, DateTime.Today.AddDays(7));
var userGroups = await preComClient.GetAllUserGroups();
var d = await preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
await Task.Delay(1000);