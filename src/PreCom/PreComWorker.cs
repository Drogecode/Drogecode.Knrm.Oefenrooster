using System.Text;
using Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Microsoft.Extensions.Logging;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Primitives;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class PreComWorker
{
    private readonly IPreComClient _preComClient;
    private readonly ILogger _logger;

    public PreComWorker(IPreComClient preComClient, ILogger logger)
    {
        _preComClient = preComClient;
        _logger = logger;
    }

    public async Task<string> Work(NextRunMode nextRunMode)
    {
        var result = new StringBuilder();
        //var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(DateTime.Today, DateTime.Today.AddDays(7));
        var userGroups = await _preComClient.GetAllUserGroups();
        var groupInfo = await _preComClient.GetAllFunctions(userGroups[0].GroupID, DateTime.Today);
        var q = await _preComClient.GetOccupancyLevels(userGroups[0].GroupID, DateTime.Today, DateTime.Today.AddDays(7));
        var schipper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM schipper"));
        var opstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM opstapper"));
        var aankOpstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM Aank. Opstapper"));
        switch (nextRunMode)
        {
            case NextRunMode.NextHour:
                var requestSchipper = schipper?.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                var requestOpstapper = opstapper?.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                var requestAlgemeen = aankOpstapper?.OccupancyDays[DateTime.Today][$"Hour{DateTime.Now.Hour - 1}"];
                if (requestSchipper == true || requestOpstapper == true || requestAlgemeen == true)
                {
                    result.Append("Voor aankomend uur hebben we nog een ");
                    if (requestSchipper == true)
                        result.Append("schipper nodig ");
                    if (requestOpstapper == true)
                        result.Append("opstapper nodig ");
                    if (requestAlgemeen == true)
                        result.Append("algemeen nodig ");
                }

                break;
            case NextRunMode.TodayTomorrow:
                var today = q.ContainsKey(DateTime.Today) && q[DateTime.Today] == -1;
                var tomorrow = q.ContainsKey(DateTime.Today.AddDays(1)) && q[DateTime.Today.AddDays(1)] == -1;
                if (today)
                    result.Append(await GetMissingForDate(userGroups[0].GroupID, DateTime.Today));
                if (tomorrow)
                    result.Append(await GetMissingForDate(userGroups[0].GroupID, DateTime.Today.AddDays(1)));

                break;
            case NextRunMode.NextWeek:
                var nextWeek = q.Where(x => x.Value == -1);
                if (nextWeek.Count() > 0)
                {
                    result.Append($"We hebben aankomende zeven dagen nog mogelijkheden {nextWeek.Count()}");
                }

                foreach (var day in q)
                {
                    if(day.Value == -1)
                        result.Append(await GetMissingForDate(userGroups[0].GroupID, day.Key));
                }

                break;
        }

        return result.ToString();
    }

    private async Task<string> GetMissingForDate(long groupId, DateTime date)
    {
        var groupInfo = await _preComClient.GetAllFunctions(groupId, date);
        var schipper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM schipper"));
        var opstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM opstapper"));
        var aankOpstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM Aank. Opstapper"));
        var schipperProblems = await GetMissingForFunction(schipper, date);
        var opstapperProblems = await GetMissingForFunction(opstapper, date);
        var aankOpstapperProblems = await GetMissingForFunction(aankOpstapper, date);

        if (!string.IsNullOrWhiteSpace(schipperProblems) || !string.IsNullOrWhiteSpace(opstapperProblems) || !string.IsNullOrWhiteSpace(aankOpstapperProblems))
        {
            var result = new StringBuilder();
            result.AppendLine(date.DayOfWeek.ToString());
            if (!string.IsNullOrWhiteSpace(schipperProblems))
            {
                result.AppendLine("Schipper");
                result.AppendLine(schipperProblems);
            }

            if (!string.IsNullOrWhiteSpace(opstapperProblems))
            {
                result.AppendLine("Opstapper");
                result.AppendLine(opstapperProblems);
            }

            if (!string.IsNullOrWhiteSpace(aankOpstapperProblems))
            {
                result.AppendLine("Aankomend opstapper");
                result.AppendLine(aankOpstapperProblems);
            }

            return result.ToString();
        }

        return string.Empty;
    }

    private async Task<string> GetMissingForFunction(ServiceFuntion? function, DateTime date)
    {
        if (function?.OccupancyDays[date] is null) return string.Empty;
        var result = new StringBuilder();
        var last = false;
        var firstProblemHour = 0;
        var i = 0;
        foreach (var hour in function.OccupancyDays[date])
        {
            if (hour.Value == true)
            {
                if (!last)
                {
                    last = true;
                    firstProblemHour = i;
                }
            }
            else
            {
                last = AddProblemText();
            }

            i++;
        }

        AddProblemText();
        return result.ToString();

        bool AddProblemText()
        {
            if (last)
            {
                last = false;
                result.AppendLine($"van {firstProblemHour} tot {i}");
            }

            return last;
        }
    }

    private async Task SendMessage(Group[] userGroups)
    {
        if (false)
        {
            // Does not work
            await _preComClient.SendMessage(new MsgSend
            {
                SendBy = 37398, //7457,
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