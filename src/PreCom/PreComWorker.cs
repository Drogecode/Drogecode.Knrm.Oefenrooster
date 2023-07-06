using Drogecode.Knrm.Oefenrooster.PreCom.Enums;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class PreComWorker
{
    private readonly PreComClient _preComClient;
    private readonly ILogger _logger;
    public PreComWorker(PreComClient preComClient, ILogger logger)
    {
        _preComClient = preComClient;
        _logger = logger;
    }

    public async Task<string> Work(NextRunMode nextRunMode)
    {
        var result = string.Empty;
        //var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(DateTime.Today, DateTime.Today.AddDays(7));
        var userGroups = await _preComClient.GetAllUserGroups();
        var d = await _preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
        var q = await _preComClient.GetOccupancyLevels(userGroups[0].GroupID, DateTime.Today, DateTime.Today.AddDays(7));
        var schipper = d.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM schipper"));
        var opstapper = d.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM opstapper"));
        var algemeen = d.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM algemeen"));
        switch (nextRunMode)
        {
            case NextRunMode.NextHour:
                var requestSchipper = schipper.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                var requestOpstapper = opstapper.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                var requestAlgemeen = algemeen.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                if (requestSchipper == true || requestOpstapper == true || requestAlgemeen == true)
                {
                    result += "Voor aankomend uur hebben we nog een ";
                    if (requestSchipper == true)
                        result += "schipper nodig ";
                    if (requestOpstapper == true)
                        result += "opstapper nodig ";
                    if (requestAlgemeen == true)
                        result += "algemeen nodig ";
                }

                break;
            case NextRunMode.TodayTomorrow:
                var today = q[DateTime.Today] == -1;
                var tomorrow = q[DateTime.Today.AddDays(1)] == -1;
                if (today || tomorrow)
                {
                    result += "We hebben ";
                    if (today) result += "vandaag";
                    if (today && tomorrow) result += " en ";
                    if (tomorrow) result += "morgen";
                    result += " nog gaten in het rooster";
                }
                break;
            case NextRunMode.NextWeek:
                var nextWeek = q.Where(x => x.Value == -1);
                if (nextWeek.Count() > 0)
                {
                    result += $"We hebben aankomende zeven dagen nog mogelijkheden {nextWeek.Count()}";
                }
                break;
        }
        return result;

    }

    private async Task SendMessage(Group[] userGroups)
    {
        if (false)
        {
            // Does not work
            await _preComClient.SendMessage(new MsgSend
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
    }
}
