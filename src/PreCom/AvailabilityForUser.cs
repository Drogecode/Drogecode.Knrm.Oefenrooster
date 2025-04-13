using Drogecode.Knrm.Oefenrooster.PreCom.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class AvailabilityForUser(IPreComClient preComClient, ILogger logger, IDateTimeService dateTimeService)
{
    private readonly IPreComClient _preComClient = preComClient;
    private readonly ILogger _logger = logger;
    private IDateTimeService _dateTimeService = dateTimeService;

    public async Task<GetResponse> Get(List<int> userIds, DateTime date)
    {
        var response = new GetResponse
        {
            Users = []
        };
        var userGroups = await preComClient.GetAllUserGroups();
        var groupInfo = await preComClient.GetAllFunctions(userGroups[0].GroupID, date);
        if (groupInfo?.ServiceFuntions.Length > 0 != true)
            return response;
        foreach (var function in groupInfo.ServiceFuntions)
        {
            foreach (var preComUser in function.Users)
            {
                if (!userIds.Contains(preComUser.UserID))
                    continue;
                if (response.Users.Any(x => x.UserId != null && userIds.Contains(x.UserId.Value)))
                    continue;
                if (response.Users.All(x => x.UserId != preComUser.UserID))
                {
                    response.Users.Add(new UserInfo()
                    {
                        UserId = preComUser.UserID,
                        AvailabilitySets = [],
                    });
                }

                var userInfo = response.Users.FirstOrDefault(x => x.UserId == preComUser.UserID);
                foreach (var day in preComUser.SchedulerDays)
                {
                    foreach (var set in day.Value)
                    {
                        try
                        {
                            var subInfo = set.Key.Substring(4).Split('_');
                            var hour = int.Parse(subInfo[0]);
                            var minute = int.Parse(subInfo.Length > 1 ? subInfo[1] : "0");
                            userInfo!.AvailabilitySets!.Add(new AvailabilitySet
                            {
                                Start = day.Key.AddHours(hour).AddMinutes(minute),
                                Available = set.Value ?? false
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to parse {setKey}", set.Key);
                        }
                    }
                }
            }
        }

        return response;
    }
}

public class GetResponse
{
    public List<UserInfo>? Users { get; set; }
}

public class UserInfo
{
    public int? UserId { get; set; }
    public List<AvailabilitySet>? AvailabilitySets { get; set; }
}

public class AvailabilitySet
{
    public DateTime Start { get; set; }
    public bool Available { get; set; }
}