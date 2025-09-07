using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Models.TrainingTarget;
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
            UrlDescription = trainingTarget.UrlDescription,
            Order = trainingTarget.Order,
            Type = trainingTarget.Type,
            Group = trainingTarget.Group,
            CreatedOn = trainingTarget.CreatedOn,
            CreatedBy = trainingTarget.CreatedBy,
        };
    }
    
    public static TrainingTarget ToTrainingTarget(this DbTrainingTargets dbTrainingTarget)
    {
        var target = new TrainingTarget
        {
            Id = dbTrainingTarget.Id,
            SubjectId = dbTrainingTarget.SubjectId,
            Name = dbTrainingTarget.Name,
            Description = dbTrainingTarget.Description,
            Url = dbTrainingTarget.Url,
            UrlDescription = dbTrainingTarget.UrlDescription,
            Order = dbTrainingTarget.Order,
            Type = dbTrainingTarget.Type,
            Group = dbTrainingTarget.Group,
            CreatedOn = dbTrainingTarget.CreatedOn,
            CreatedBy = dbTrainingTarget.CreatedBy,
        };
        if (dbTrainingTarget.TrainingTargetUserResults is not null)
        {
            target.TargetResults = [];
            foreach (var result in dbTrainingTarget.TrainingTargetUserResults)
            {
                target.TargetResults.Add(new TrainingTargetResult
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    TrainingTargetId = result.TrainingTargetId,
                    RoosterAvailableId = result.RoosterAvailableId,
                    Result = result.Result,
                    ResultDate = result.ResultDate,
                    SetBy = result.SetBy,
                });
            }
        }
        return target;
    }
    
    public static TrainingTarget ToTrainingTarget(this DbTrainingTargets dbTrainingTarget, MultipleTrainingTargetResult trainingTargetResult)
    {
        var target = new TrainingTarget
        {
            Id = dbTrainingTarget.Id,
            SubjectId = dbTrainingTarget.SubjectId,
            Name = dbTrainingTarget.Name,
            Description = dbTrainingTarget.Description,
            Url = dbTrainingTarget.Url,
            UrlDescription = dbTrainingTarget.UrlDescription,
            Order = dbTrainingTarget.Order,
            Type = dbTrainingTarget.Type,
            Group = dbTrainingTarget.Group,
            CreatedOn = dbTrainingTarget.CreatedOn,
            CreatedBy = dbTrainingTarget.CreatedBy,
        };
        if (trainingTargetResult.TargetResults is null) return target;
        target.TargetResults = [];
        foreach (var targetResult in trainingTargetResult.TargetResults.Where(x=>x.TrainingTargetId == dbTrainingTarget.Id))
        {
            target.TargetResults.Add(targetResult);
        }
        return target;
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
        if (dbTrainingTargetSubject.TrainingTargets?.Count > 0)
        {
            subject.TrainingTargets ??= [];
            foreach (var trainingTarget in dbTrainingTargetSubject.TrainingTargets)
            {
                subject.TrainingTargets.Add(trainingTarget.ToTrainingTarget());
            }
        }
        return subject;
    }

    public static TrainingTargetSet? ToTrainingTargetSet(this DbRoosterTraining roosterTraining)
    {
        if (roosterTraining.TrainingTargetSet is null)
            return null;
        return roosterTraining.TrainingTargetSet.ToTrainingTargetSet();
    }

    public static TrainingTargetSet ToTrainingTargetSet(this DbTrainingTargetSets trainingTargetSet)
    {
        return new TrainingTargetSet
        {
            Id = trainingTargetSet.Id,
            Name = trainingTargetSet.Name,
            TrainingTargetIds = trainingTargetSet.TrainingTargetIds,
            ActiveSince = trainingTargetSet.ActiveSince,
            ReusableSince = trainingTargetSet.ReusableSince,
            CreatedOn = trainingTargetSet.CreatedOn,
            CreatedBy = trainingTargetSet.CreatedBy,
        };
    }

    public static DbTrainingTargetSets ToDb(this TrainingTargetSet trainingTargetSet, Guid customerId)
    {
        if (trainingTargetSet.Id is null)
        {
            throw new DrogeCodeNullException("DbTrainingTargetSets ToDb trainingTargetSet.Id is null");
        }
        return new DbTrainingTargetSets
        {
            Id = trainingTargetSet.Id.Value,
            CustomerId = customerId,
            Name = trainingTargetSet.Name,
            TrainingTargetIds = trainingTargetSet.TrainingTargetIds,
            ActiveSince = trainingTargetSet.ActiveSince,
            ReusableSince = trainingTargetSet.ReusableSince,
            CreatedOn = trainingTargetSet.CreatedOn,
            CreatedBy = trainingTargetSet.CreatedBy,
        };
    }

    public static DbTrainingTargetUserResult ToDb(this TrainingTargetResult trainingTargetResult)
    {
        if (trainingTargetResult.Id is null)
        {
            throw new DrogeCodeNullException("DbTrainingTargetUserResult ToDb trainingTargetResult.Id is null");
        }
        return new DbTrainingTargetUserResult
        {
            Id = trainingTargetResult.Id.Value,
            UserId = trainingTargetResult.UserId,
            TrainingTargetId = trainingTargetResult.TrainingTargetId,
            RoosterAvailableId = trainingTargetResult.RoosterAvailableId,
            Result = trainingTargetResult.Result,
            ResultDate = trainingTargetResult.ResultDate,
            TrainingDate = trainingTargetResult.TrainingDate,
            SetBy = trainingTargetResult.SetBy,
        };
    }

    public static MultipleTrainingTargetResult ToTrainingTargetResult(this DbRoosterTraining dbRoosterTraining)
    {
        var result = new MultipleTrainingTargetResult
        {
            TrainingTargetIds = dbRoosterTraining.TrainingTargetSet?.TrainingTargetIds
        };
        var trainingTargetResults = new List<TrainingTargetResult>();
        if (result.TrainingTargetIds is null || dbRoosterTraining.RoosterAvailables is null)
            return result;
        foreach (var roosterAvailable in dbRoosterTraining.RoosterAvailables)
        {
            if (roosterAvailable.TrainingTargetUserResults is null)
                return result;
            foreach (var trainingTargetUserResult in roosterAvailable.TrainingTargetUserResults)
            {
                trainingTargetResults.Add(new TrainingTargetResult
                {
                    Id = trainingTargetUserResult.Id,
                    UserId = trainingTargetUserResult.UserId,
                    TrainingTargetId = trainingTargetUserResult.TrainingTargetId,
                    RoosterAvailableId = trainingTargetUserResult.RoosterAvailableId,
                    Result = trainingTargetUserResult.Result,
                    ResultDate = trainingTargetUserResult.ResultDate,
                    SetBy = trainingTargetUserResult.SetBy,
                });
            }
        }
        result.TargetResults = trainingTargetResults;
        return result;
    }
}