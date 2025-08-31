using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class TrainingTargetService : DrogeService, ITrainingTargetService
{
    public TrainingTargetService(ILogger<TrainingTargetService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider)
        : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<AllTrainingTargetsResponse> AllTrainingTargets(int count, int skip, Guid? subjectId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new AllTrainingTargetsResponse();

        var trainingTargets = Database.TrainingTargetSubjects
            .Where(x => x.CustomerId == customerId &&
                        x.ParentId == null &&
                        (subjectId == null || x.Id == subjectId))
            .AsNoTracking()
            .Include(x => x.Parent)
            .ThenInclude(x => x.TrainingTargets)
            .Include(x => x.Children)
            .ThenInclude(x => x.TrainingTargets)
            .Include(x => x.TrainingTargets)
            .Select(x => x.ToTrainingTargetSubjects());
        response.TrainingSubjects = await trainingTargets.Skip(skip).Take(count).ToListAsync(clt);
        response.TotalCount = await trainingTargets.CountAsync(clt);
        response.Success = true;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<GetSingleTargetSetResponse> GetSetLinkedToTraining(Guid trainingId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetSingleTargetSetResponse();

        var trainingTargetSet = await Database.RoosterTrainings
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.Id == trainingId)
            .Include(x => x.TrainingTargetSet)
            .Select(x => x.ToTrainingTargetSet())
            .FirstOrDefaultAsync(clt);
        response.TrainingTargetSet = trainingTargetSet;
        response.Success = true;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<ActionResult<GetAllTargetSetResponse>> GetAllReusableSets(Guid customerId, int count, int skip, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetAllTargetSetResponse();

        var trainingTargetSets = Database.TrainingTargetSets
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Select(x => x.ToTrainingTargetSet());
        response.TrainingTargetSets = await trainingTargetSets.ToListAsync(clt);
        response.Success = true;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<PutResponse> PutNewTemplateSet(TrainingTargetSet body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new PutResponse();

        var newId = Guid.CreateVersion7();
        body.Id = newId;
        body.CreatedBy = userId;
        body.CreatedOn = DateTime.UtcNow;
        body.ActiveSince = DateTime.UtcNow;

        Database.TrainingTargetSets.Add(body.ToDb(customerId));

        response.Success = await Database.SaveChangesAsync(clt) > 0;
        if (response.Success)
        {
            response.NewId = newId;
        }

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<PatchResponse> PatchTemplateSet(TrainingTargetSet body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new PatchResponse();
        var oldVersion = await Database.TrainingTargetSets.FirstOrDefaultAsync(
            u => u.Id == body.Id && u.CustomerId == customerId && u.DeletedOn == null, 
            cancellationToken: clt
        );
        if (oldVersion is null)
        {
            return response;
        }

        oldVersion.Name = body.Name;
        oldVersion.TrainingTargetIds = body.TrainingTargetIds;
        oldVersion.ActiveSince = body.ActiveSince;
        oldVersion.ReusableSince = body.ReusableSince;

        Database.TrainingTargetSets.Update(oldVersion);
        response.Success = await Database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }
}