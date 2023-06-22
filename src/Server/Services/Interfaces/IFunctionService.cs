﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IFunctionService
{
    Task<AddFunctionResponse> AddFunction(DrogeFunction function, Guid customerId, CancellationToken clt);
    Task<MultipleFunctionsResponse> GetAllFunctions(Guid customerId, CancellationToken clt);
}
