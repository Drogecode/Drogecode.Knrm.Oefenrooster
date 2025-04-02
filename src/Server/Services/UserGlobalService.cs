using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserGlobalService : DrogeService, IUserGlobalService
{
    public UserGlobalService(
        ILogger<ScheduleService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeService dateTimeService) : base(logger, database, memoryCache, dateTimeService)
    {
    }

    public async Task<AllDrogeUserGlobalResponse> GetAllUserGlobals(CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AllDrogeUserGlobalResponse
        {
            GlobalUsers = await Database.UsersGlobal.Where(x => x.DeletedOn == null).Select(x => x.ToDrogeUserGlobal()).ToListAsync(clt)
        };
        result.TotalCount = result.GlobalUsers.Count;
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PutResponse> PutGlobalUser(Guid userId, DrogeUserGlobal globalUser, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutResponse();
        var newId = Guid.CreateVersion7();
        Database.UsersGlobal.Add(new DbUsersGlobal()
        {
            Id = newId,
            Name = globalUser.Name,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = userId
        });
        result.Success = await Database.SaveChangesAsync(clt) > 0;
        if (result.Success)
            result.NewId = newId;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}