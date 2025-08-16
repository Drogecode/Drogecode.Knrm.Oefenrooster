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

    public async Task<AllTrainingTargetsResponse> AllTrainingTargets(int count, int skip, Guid? subjectId, Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new AllTrainingTargetsResponse();

        var trainingTargets = Database.TrainingTargets
            .Where(x => x.CustomerId == customerId &&
                        (subjectId == null || x.SubjectId == subjectId))
            .Select(x => x.ToTrainingTarget());
        response.TrainingTargets = await trainingTargets.Skip(skip).Take(count).ToListAsync(clt);
        response.TotalCount = await trainingTargets.CountAsync(clt);
        response.Success = true;

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }
}