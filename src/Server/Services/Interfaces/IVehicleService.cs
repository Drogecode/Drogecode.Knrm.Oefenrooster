﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IVehicleService
{
    Task<List<DrogeVehicle>> GetAllVehicles(Guid customerId);
    Task<MultipleVehicleTrainingLinkResponse> GetForTraining(Guid customerId, Guid trainingId, CancellationToken clt);
    Task<MultipleVehicleTrainingLinkResponse> GetForDefault(Guid customerId, Guid trainingId, CancellationToken clt);
    Task<Guid?> PutVehicle(DrogeVehicle vehicle, Guid customerId, Guid userId, CancellationToken clt);
    Task<DrogeLinkVehicleTrainingResponse> UpdateLinkVehicleTraining(Guid customerId, DrogeLinkVehicleTraining link);
}
