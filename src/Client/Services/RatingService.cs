using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Services;

public class RatingService : IRatingService
{
    private readonly TrainingTargetRepository _trainingTargetRepository;
    private readonly CancellationTokenSource _cls = new();
    public RatingService(TrainingTargetRepository trainingTargetRepository)
    {
        _trainingTargetRepository = trainingTargetRepository;
    }
    
    public async Task UpdateResult(int i, bool fromCurrentUser, TrainingTargetResult resultObject, List<TrainingTarget> trainingTargets)
    {
        if (resultObject.RoosterAvailableId == Guid.Empty)
        {
            Console.WriteLine("RoosterAvailableId is empty");
        }

        Console.WriteLine($"Updating result {i} - id = {resultObject.Id} - {resultObject.RoosterAvailableId}");

        if (resultObject.Id is null)
        {
            trainingTargets?.Where(x => x.Id == resultObject.TrainingTargetId).FirstOrDefault()?.TargetResults?.Add(resultObject);
            var response = await _trainingTargetRepository.PutUserResponse(resultObject, fromCurrentUser, _cls.Token);
            if (response?.Success == true)
            {
                resultObject.Id = response.NewId;
            }
        }
        else
        {
            await _trainingTargetRepository.PatchUserResponse(resultObject, _cls.Token);
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}