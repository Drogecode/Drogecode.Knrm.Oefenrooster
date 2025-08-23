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
            CreatedOn = trainingTarget.CreatedOn,
            CreatedBy = trainingTarget.CreatedBy,
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
            CreatedOn = dbTrainingTarget.CreatedOn,
            CreatedBy = dbTrainingTarget.CreatedBy,
        };
    }
    public static TrainingSubject ToTrainingTargetSubjects(this DbTrainingTargetSubjects dbTrainingTargetSubject)
    {
        var subject = new TrainingSubject
        {
            Id = dbTrainingTargetSubject.Id,
            Name = dbTrainingTargetSubject.Name,
            Order = dbTrainingTargetSubject.Order,
            CreatedOn = dbTrainingTargetSubject.CreatedOn,
            CreatedBy = dbTrainingTargetSubject.CreatedBy,
        };
        if (dbTrainingTargetSubject.Children?.Any() == true)
        {
            subject.TrainingSubjects ??= [];
            foreach (var trainingTarget in dbTrainingTargetSubject.Children)
            {
                subject.TrainingSubjects.Add(trainingTarget.ToTrainingTargetSubjects());
            }
        }
        if (dbTrainingTargetSubject.TrainingTargets?.Any() == true)
        {
            subject.TrainingTargets ??= [];
            foreach (var trainingTarget in dbTrainingTargetSubject.TrainingTargets)
            {
                subject.TrainingTargets.Add(trainingTarget.ToTrainingTarget());
            }
        }
        return subject;
    }
}