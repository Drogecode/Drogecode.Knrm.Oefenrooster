﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class TrainingTypesService : ITrainingTypesService
{
    private readonly ILogger<TrainingTypesService> _logger;
    private readonly Database.DataContext _database;
    public TrainingTypesService(ILogger<TrainingTypesService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypes(Guid customerId, CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultiplePlannerTrainingTypesResponse { PlannerTrainingTypes = new List<PlannerTrainingType>() };
        var typesFromDb = await _database.RoosterTrainingTypes.Where(x => x.CustomerId == customerId).ToListAsync(cancellationToken: token);
        foreach (var type in typesFromDb)
        {
            var newType = new PlannerTrainingType
            {
                Id = type.Id,
                Name = type.Name,
                CountToTrainingTarget = type.CountToTrainingTarget,
                IsDefault = type.IsDefault,
                Order = type.Order,
            };
            if (!string.IsNullOrEmpty(type.ColorLight))
                newType.ColorLight = type.ColorLight;
            if (!string.IsNullOrEmpty(type.ColorDark))
                newType.ColorDark = type.ColorDark;
            if (!string.IsNullOrEmpty(type.TextColorLight))
                newType.TextColorLight = type.TextColorLight;
            if (!string.IsNullOrEmpty(type.TextColorDark))
                newType.TextColorDark = type.TextColorDark;
            result.PlannerTrainingTypes.Add(newType);
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }
}