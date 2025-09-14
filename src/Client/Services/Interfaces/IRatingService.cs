using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

public interface IRatingService : IDisposable
{
    Task UpdateResult(int i, bool fromCurrentUser, TrainingTargetResult resultObject, List<TrainingTarget> trainingTargets);
}