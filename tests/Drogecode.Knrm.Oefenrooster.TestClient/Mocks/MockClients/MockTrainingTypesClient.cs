using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

public class MockTrainingTypesClient : ITrainingTypesClient
{
    public static readonly Guid DEFAULT_PLANNER_TYPE = new("27231399-ff3b-4924-865c-a0f61e6162cd");
    public Task<GetTraininTypeByIdResponse> GetByIdAsync(Guid id)
    {
        return GetByIdAsync(id, CancellationToken.None);
    }

    public Task<GetTraininTypeByIdResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypesAsync(bool callHub)
    {
        return await GetTrainingTypesAsync(callHub, CancellationToken.None);
    }

    public async Task<MultiplePlannerTrainingTypesResponse> GetTrainingTypesAsync(bool callHub, CancellationToken cancellationToken)
    {
        return new MultiplePlannerTrainingTypesResponse
        {
            Success = true,
            PlannerTrainingTypes = new List<PlannerTrainingType>
            {
                new PlannerTrainingType()
                {
                    Id = DEFAULT_PLANNER_TYPE,
                    Name = "GetTrainingTypesAsync Mock default",
                }
            }
        };
    }

    public Task<PatchTrainingTypeResponse> PatchTrainingTypeAsync(PlannerTrainingType body)
    {
        return PatchTrainingTypeAsync(body, CancellationToken.None);
    }

    public Task<PatchTrainingTypeResponse> PatchTrainingTypeAsync(PlannerTrainingType body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PutTrainingTypeResponse> PostNewTrainingTypeAsync(PlannerTrainingType body)
    {
        return PostNewTrainingTypeAsync(body, CancellationToken.None);
    }

    public Task<PutTrainingTypeResponse> PostNewTrainingTypeAsync(PlannerTrainingType body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
