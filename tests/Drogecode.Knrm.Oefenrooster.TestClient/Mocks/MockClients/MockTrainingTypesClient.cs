using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

public class MockTrainingTypesClient : ITrainingTypesClient
{
    public Task<GetTraininTypeByIdResponse> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<GetTraininTypeByIdResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypesAsync()
    {
        return await GetTrainingTypesAsync(CancellationToken.None);
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypesAsync(CancellationToken cancellationToken)
    {
        return new MultiplePlannerTrainingTypesResponse
        {
            Success = true,
            PlannerTrainingTypes = new List<PlannerTrainingType>
            {
                new PlannerTrainingType()
                {
                    Id = Guid.NewGuid(),
                    Name = "GetTrainingTypesAsync Mock default",
                }
            }
        };
    }

    public Task<PutTrainingTypeResponse> PostNewTrainingTypeAsync(PlannerTrainingType body)
    {
        throw new NotImplementedException();
    }

    public Task<PutTrainingTypeResponse> PostNewTrainingTypeAsync(PlannerTrainingType body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
