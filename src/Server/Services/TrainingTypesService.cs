﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using System.Diagnostics;
using ZXing.Aztec.Internal;

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

    public async Task<PutTrainingTypeResponse> PostTrainingType(PlannerTrainingType plannerTrainingType, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var newId = Guid.NewGuid();
        var dbType = plannerTrainingType.ToDb();
        dbType.Id = newId;
        dbType.CustomerId = customerId;
        _database.RoosterTrainingTypes.Add(dbType);
        var success = (await _database.SaveChangesAsync(clt)) > 0;
        sw.Stop();
        return new PutTrainingTypeResponse
        {
            NewId = newId,
            Success = success,
            ElapsedMilliseconds = sw.ElapsedMilliseconds
        };
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypes(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultiplePlannerTrainingTypesResponse { PlannerTrainingTypes = new List<PlannerTrainingType>() };
        var typesFromDb = await _database.RoosterTrainingTypes.Where(x => x.CustomerId == customerId).ToListAsync(cancellationToken: clt);
        foreach (var type in typesFromDb)
        {
            var newType = type.ToDrogecode();
            result.PlannerTrainingTypes.Add(newType);
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetTraininTypeByIdResponse> GetById(Guid id, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetTraininTypeByIdResponse();
        var typeFromDb = await _database.RoosterTrainingTypes.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == id, cancellationToken: clt);
        if (typeFromDb is not null)
        {
            var drogeType = typeFromDb.ToDrogecode();
            result.TrainingType = drogeType;
            result.Success = true;
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }
}
