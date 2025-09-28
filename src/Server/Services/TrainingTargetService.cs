using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class TrainingTargetService : DrogeService, ITrainingTargetService
{
    public TrainingTargetService(ILogger<TrainingTargetService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider)
        : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<AllTrainingTargetSubjectsResponse> AllTrainingTargets(int count, int skip, Guid? subjectId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new AllTrainingTargetSubjectsResponse();

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

    public async Task<AllTrainingTargetsResponse> GetTargetsLinkedToTraining(Guid trainingId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new AllTrainingTargetsResponse();

        var trainingTargetResult = await Database.RoosterTrainings
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.Id == trainingId)
            .Include(x => x.TrainingTargetSet)
            .Include(x => x.RoosterAvailables!)
            .ThenInclude(x => x.TrainingTargetUserResults!)
            .Select(x => x.ToTrainingTargetResult())
            .FirstOrDefaultAsync(clt);

        if (trainingTargetResult?.TrainingTargetIds is null)
        {
            Logger.LogInformation("No results found for training {trainingId}", trainingId);
            return response;
        }

        var targets = await Database.TrainingTargets
            .Where(x => trainingTargetResult.TrainingTargetIds.Contains(x.Id))
            .Select(x => x.ToTrainingTarget(trainingTargetResult))
            .ToListAsync(clt);
        response.TrainingTargets = targets;
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

    public async Task<GetTargetSetWithTargetsResult> GetSetWithTargetsLinkedToTraining(Guid trainingId, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetTargetSetWithTargetsResult();

        var trainingTargetSet = await Database.RoosterTrainings
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.Id == trainingId)
            .Include(x => x.TrainingTargetSet)
            .Include(x=>x.RoosterAvailables!.Where(y=>y.UserId == userId))
            .ThenInclude(x=>x.TrainingTargetUserResults!.Where(y=>y.UserId == userId))
            .Select(x => x.ToTrainingTargetSetWithUserResults())
            .FirstOrDefaultAsync(clt);
        if (trainingTargetSet is null)
        {
            sw.Stop();
            response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return response;
        }
        
        var targets = await Database.TrainingTargets
            .Where(x=> trainingTargetSet.TrainingTargetIds.Contains(x.Id))
            .Select(x=>x.ToTrainingTarget())
            .ToListAsync(clt);
        response.RoosterAvailableId = trainingTargetSet.RoosterAvailableId;
        response.Assigned = trainingTargetSet.Assigned;
        response.TrainingTargetSet = trainingTargetSet;
        response.TrainingTargetResults = trainingTargetSet.TrainingTargetResults;
        response.TrainingTargets = targets;
        response.TotalCount = targets.Count + trainingTargetSet.TrainingTargetResults?.Count ?? 0;
        response.Success = true;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<GetAllTargetSetResponse> GetAllReusableSets(Guid customerId, int count, int skip, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetAllTargetSetResponse();

        var trainingTargetSets = Database.TrainingTargetSets
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.ReusableSince)
            .Select(x => x.ToTrainingTargetSet());
        response.TrainingTargetSets = await trainingTargetSets.Skip(skip).Take(count).ToListAsync(clt);
        response.TotalCount = await trainingTargetSets.CountAsync(clt);
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

    public async Task<PutResponse> PutUserResponse(TrainingTargetResult body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new PutResponse();

        var newId = Guid.CreateVersion7();
        body.Id = newId;
        body.ResultDate = DateTime.UtcNow;
        body.SetBy = userId;

        var dbObject = body.ToDb();
        dbObject.SetInBulk = false;
        Database.TrainingTargetUserResults.Add(dbObject);

        response.Success = await Database.SaveChangesAsync(clt) > 0;
        if (response.Success)
        {
            response.NewId = newId;
        }

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<PatchResponse> PatchUserResponse(TrainingTargetResult body, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new PatchResponse();

        var oldVersion = await Database.TrainingTargetUserResults.FirstOrDefaultAsync(
            u => u.Id == body.Id && u.DeletedOn == null,
            cancellationToken: clt
        );
        if (oldVersion is null)
        {
            return response;
        }

        oldVersion.Result = body.Result;
        oldVersion.ResultDate = DateTime.UtcNow;
        oldVersion.SetBy = userId;
        oldVersion.SetInBulk = false;
        Database.TrainingTargetUserResults.Update(oldVersion);
        response.Success = await Database.SaveChangesAsync(clt) > 0;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }


    public async Task<PatchResponse> PatchUserResponseForTraining(Guid trainingId, TrainingTargetType forType, int result, Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new PatchResponse();

        var trainingWithTargetResults = await Database.RoosterTrainings
            .Where(x => x.CustomerId == customerId && x.Id == trainingId && x.DeletedOn == null)
            .Include(x => x.TrainingTargetSet)
            .Include(x => x.RoosterAvailables!.Where(y => y.Assigned))
            .ThenInclude(x => x.TrainingTargetUserResults!.Where(y=> y.DeletedOn == null))
            .FirstOrDefaultAsync(clt);

        if (trainingWithTargetResults?.RoosterAvailables is null || trainingWithTargetResults.TrainingTargetSet is null)
        {
            Logger.LogWarning("Trying to PatchUserResponseForTraining but no results found for training {trainingId}", trainingId);
            sw.Stop();
            response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return response;
        }

        var targets = await Database.TrainingTargets
            .Where(x => trainingWithTargetResults.TrainingTargetSet.TrainingTargetIds.Contains(x.Id) && x.Type == forType)
            .ToListAsync(clt);

        if (targets.Count == 0)
        {
            Logger.LogWarning("Trying to PatchUserResponseForTraining but no no training targets set for training {trainingId} with set {SetId}", trainingId,
                trainingWithTargetResults.TrainingTargetSet.Id);
            sw.Stop();
            response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return response;
        }

        foreach (var roosterAvailable in trainingWithTargetResults.RoosterAvailables.Where(x => x.Assigned))
        {
            roosterAvailable.TrainingTargetUserResults ??= [];
            foreach (var target in targets)
            {
                if (roosterAvailable.TrainingTargetUserResults.Any(x => x.TrainingTargetId == target.Id))
                {
                    var userResult = roosterAvailable.TrainingTargetUserResults.FirstOrDefault(x => x.TrainingTargetId == target.Id);
                    if (userResult!.ResultDate is not null && !userResult.SetInBulk)
                    {
                        continue;
                    }
                    userResult.Result = result;
                    userResult.ResultDate = DateTime.UtcNow;
                    userResult.SetBy = userId;
                    userResult.SetInBulk = true;
                    
                    Database.TrainingTargetUserResults.Update(userResult);
                }
                else
                {
                    var newUserResult = new DbTrainingTargetUserResult()
                    {
                        Id = Guid.CreateVersion7(),
                        TrainingTargetId = target.Id,
                        RoosterAvailableId = roosterAvailable.Id,
                        UserId = roosterAvailable.UserId,
                        Result = result,
                        TrainingDate = trainingWithTargetResults.DateStart,
                        ResultDate = DateTime.UtcNow,
                        SetBy = userId,
                        SetInBulk = true
                    };
                    Database.TrainingTargetUserResults.Add(newUserResult);
                }
            }
        }

        response.Success = await Database.SaveChangesAsync(clt) > 0;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

    public async Task<AllResultForUserResponse> GetAllResultForUser(Guid userIdResult, Guid userId, RatingPeriod period, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new AllResultForUserResponse();
        var allFrom = DateTimeProvider.UtcNow();
        var allUntil = DateTimeProvider.MaxValue();
        switch (period)
        {
            case RatingPeriod.OneMonth:
                allFrom = allFrom.AddMonths(-1);
                break;
            case RatingPeriod.PastYear:
                allFrom = new DateTime(DateTimeProvider.Today().Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                allUntil = new DateTime(DateTimeProvider.Today().Year - 1, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                break;
            case RatingPeriod.CurrentYear:
                allFrom = new DateTime(DateTimeProvider.Today().Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                allUntil = new DateTime(DateTimeProvider.Today().Year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                break;
            case RatingPeriod.OneYear:
                allFrom = allFrom.AddYears(-1);
                break;
            case RatingPeriod.TwoYear:
                allFrom = allFrom.AddYears(-2);
                break;
            case RatingPeriod.ThreeYear:
                allFrom = allFrom.AddYears(-3);
                break;
            case RatingPeriod.FourYear:
                allFrom = allFrom.AddYears(-4);
                break;
            case RatingPeriod.FiveYear:
                allFrom = allFrom.AddYears(-5);
                break;
            case RatingPeriod.All:
                allFrom = DateTime.MinValue;
                break;
        }

        var userResults = await Database.TrainingTargetUserResults
            .AsNoTracking()
            .Where(x => x.UserId == userIdResult && x.TrainingDate >= allFrom && x.TrainingDate <= allUntil && x.DeletedOn == null)
            .OrderBy(x => x.TrainingTargetId)
            .ToListAsync(clt);
        response.TotalCount = userResults.Count;
        if (userResults.Count > 0)
        {
            response.UserResultForTargets = [];
            foreach (var userResult in userResults)
            {
                var set = response.UserResultForTargets.FirstOrDefault(x => x.TrainingTargetId == userResult.TrainingTargetId);
                if (set is null)
                {
                    set = new UserResultForTarget
                    {
                        TrainingTargetId = userResult.TrainingTargetId,
                    };
                    response.UserResultForTargets.Add(set);
                }

                set.Result = ((set.Result * set.Count) + userResult.Result) / (set.Count + 1);
                set.Count++;
            }
        }

        response.Success = true;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }
}