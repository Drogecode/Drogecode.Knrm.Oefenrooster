using Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;
using Drogecode.Knrm.Oefenrooster.PreCom.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class FutureProblems
{
    private readonly IPreComClient _preComClient;
    private readonly ILogger _logger;
    private IDateTimeProvider _dateTimeProvider;

    public FutureProblems(IPreComClient preComClient, ILogger logger, IDateTimeProvider dateTimeProvider)
    {
        _preComClient = preComClient;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GetProblemsResponse> Work(NextRunMode nextRunMode)
    {
        var response = new GetProblemsResponse()
        {
            Dates = []
        };
        //var schedulerAppointments = await preComClient.GetUserSchedulerAppointments(_dateTimeService.Today(), _dateTimeService.Today().AddDays(7));
        var userGroups = await _preComClient.GetAllUserGroups();
        var groupInfo = await _preComClient.GetAllFunctions(userGroups[0].GroupID, _dateTimeProvider.Today());
        var schipper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM schipper"));
        var opstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM opstapper"));
        var aankOpstapper = groupInfo.ServiceFuntions.FirstOrDefault(x => x.Label.Equals("KNRM Aank. Opstapper"));
        switch (nextRunMode)
        {
            case NextRunMode.NextHour:
                var requestSchipper = schipper?.OccupancyDays[_dateTimeProvider.Today()][$"Hour{_dateTimeProvider.Now().Hour - 1}"];
                var requestOpstapper = opstapper?.OccupancyDays[_dateTimeProvider.Today()][$"Hour{_dateTimeProvider.Now().Hour - 1}"];
                var requestAlgemeen = aankOpstapper?.OccupancyDays[_dateTimeProvider.Today()][$"Hour{_dateTimeProvider.Now().Hour - 1}"];
                if (requestSchipper == true || requestOpstapper == true || requestAlgemeen == true)
                {
                    var problems = new StringBuilder();
                    problems.Append("Voor aankomend uur hebben we nog een ");
                    if (requestSchipper == true)
                        problems.Append("schipper nodig ");
                    if (requestOpstapper == true)
                        problems.Append("opstapper nodig ");
                    if (requestAlgemeen == true)
                        problems.Append("aankomend opstapper nodig ");
                    response.Problems = problems.ToString();
                }

                break;
            case NextRunMode.TodayTomorrow:
                var twoDays = await _preComClient.GetOccupancyLevels(userGroups[0].GroupID, _dateTimeProvider.Today(), _dateTimeProvider.Today().AddDays(2));
                var today = twoDays.ContainsKey(_dateTimeProvider.Today()) && twoDays[_dateTimeProvider.Today()] == -1;
                var tomorrow = twoDays.ContainsKey(_dateTimeProvider.Today().AddDays(1)) && twoDays[_dateTimeProvider.Today().AddDays(1)] == -1;
                if (today)
                    await GetMissingForDate(response, userGroups[0].GroupID, _dateTimeProvider.Today());
                if (tomorrow)
                    await GetMissingForDate(response, userGroups[0].GroupID, _dateTimeProvider.Today().AddDays(1));

                break;
            case NextRunMode.NextWeek:
                var eightDays = await _preComClient.GetOccupancyLevels(userGroups[0].GroupID, _dateTimeProvider.Today(), _dateTimeProvider.Today().AddDays(8));
                foreach (var day in eightDays.Where(day => day.Value == -1))
                {
                    await GetMissingForDate(response, userGroups[0].GroupID, day.Key);
                }

                break;
            case NextRunMode.NextSunday:
                var untilSunday = await _preComClient.GetOccupancyLevels(
                    userGroups[0].GroupID, _dateTimeProvider.Today(), _dateTimeProvider.Today().AddDays(8 - (int)_dateTimeProvider.Today().DayOfWeek)
                );
                foreach (var day in untilSunday.Where(day => day.Value == -1))
                {
                    await GetMissingForDate(response, userGroups[0].GroupID, day.Key);
                }

                break;
        }

        if (response.Problems is not null)
            response.Problems = response.Problems.TrimString("<br />");
        return response;
    }

    private async Task GetMissingForDate(GetProblemsResponse response, long groupId, DateTime date)
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
            result.Append("{");
            result.Append(response.Dates!.Count);
            result.Append("}<br />");
            if (!string.IsNullOrWhiteSpace(schipperProblems))
            {
                result.Append("Schipper<br />");
                result.Append(schipperProblems);
            }

            if (!string.IsNullOrWhiteSpace(opstapperProblems))
            {
                result.Append("Opstapper<br />");
                result.Append(opstapperProblems);
            }

            if (!string.IsNullOrWhiteSpace(aankOpstapperProblems))
            {
                result.Append("Aankomend opstapper<br />");
                result.Append(aankOpstapperProblems);
            }

            result.Append("<br />");
            response.Problems += result.ToString();
            response.Dates!.Add(date);
        }
    }

    private async Task<string> GetMissingForFunction(ServiceFuntion? function, DateTime date)
    {
        if (function?.OccupancyDays[date] is null) return string.Empty;
        var result = new StringBuilder();
        var last = false;
        var firstProblemHour = 0;
        var i = 0;
        var isToday = _dateTimeProvider.Today().Equals(date.Date);
        foreach (var hour in function.OccupancyDays[date].Where(x => !x.Key.Contains('_')))
        {
            if (isToday && i < _dateTimeProvider.Now().Hour)
            {
                i++;
                continue;
            }

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
                result.Append($"van {firstProblemHour} tot {i}<br />");
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