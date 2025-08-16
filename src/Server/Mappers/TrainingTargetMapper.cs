using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class TrainingTargetMapper
{
    public static DbTrainingTargets ToDb(this TrainingTarget trainingTarget)
    {
        return new DbTrainingTargets
        {
            Id = trainingTarget.Id,
            SubjectId = trainingTarget.SubjectId,
            Name = trainingTarget.Name,
            Description = trainingTarget.Description,
            Url = trainingTarget.Url,
            Order = trainingTarget.Order,
            Type = trainingTarget.Type,
            ValidFrom = trainingTarget.ValidFrom,
            ValidUntil = trainingTarget.ValidUntil
        };
    }
    
    public static TrainingTarget ToTrainingTarget(this DbTrainingTargets dbTrainingTarget)
    {
        return new TrainingTarget
        {
            Id = dbTrainingTarget.Id,
            SubjectId = dbTrainingTarget.SubjectId,
            Name = dbTrainingTarget.Name,
            Description = dbTrainingTarget.Description,
            Url = dbTrainingTarget.Url,
            Order = dbTrainingTarget.Order,
            Type = dbTrainingTarget.Type,
            ValidFrom = dbTrainingTarget.ValidFrom,
            ValidUntil = dbTrainingTarget.ValidUntil
        };
    }
}