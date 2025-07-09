using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Models.User;

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

    public async Task<GetGlobalUserByIdResponse> GetGlobalUserById(Guid globalUserId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetGlobalUserByIdResponse();
        var globalUser = await Database.UsersGlobal.Where(x => x.Id == globalUserId).Select(x=>x.ToDrogeUserGlobal()).FirstOrDefaultAsync(clt);
        if (globalUser is null)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }
        result.GlobalUser = globalUser;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DrogeUserGlobal> GetOrCreateGlobalUserByExternalId(DrogeUserServer user, CancellationToken clt)
    {
        var globalUser = await Database.UsersGlobal.Where(x => x.ExternalId == user.ExternalId).Select(x=>x.ToDrogeUserGlobal()).FirstOrDefaultAsync(clt);
        if (globalUser is not null)
        {
            return globalUser;
        }

        var dbGlobalUer = new DbUsersGlobal
        {
            Id = Guid.CreateVersion7(),
            ExternalId = user.ExternalId,
            CreatedOn = DateTimeService.UtcNow(),
            Name = user.Name
        };
        await Database.UsersGlobal.AddAsync(dbGlobalUer, clt);
        await Database.SaveChangesAsync(clt);
        return dbGlobalUer.ToDrogeUserGlobal();
    }

    public async Task<PutResponse> PutGlobalUser(Guid userId, DrogeUserGlobal globalUser, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutResponse();
        var newId = Guid.CreateVersion7();
        Database.UsersGlobal.Add(new DbUsersGlobal()
        {
            Id = newId,
            Name = globalUser.Name.Trim(),
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

    public async Task<PatchResponse> PatchGlobalUser(Guid userId, DrogeUserGlobal globalUser, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchResponse();
        var old = await Database.UsersGlobal.FirstOrDefaultAsync(x => x.Id == globalUser.Id, clt);
        if (old is null)
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }
        old.Name = globalUser.Name.Trim();
        Database.UsersGlobal.Update(old);
        result.Success = await Database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}