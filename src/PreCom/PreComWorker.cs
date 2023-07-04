using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task Work()
    {
        //var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(DateTime.Today, DateTime.Today.AddDays(7));
        var userGroups = await _preComClient.GetAllUserGroups();
        var d = await _preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
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
