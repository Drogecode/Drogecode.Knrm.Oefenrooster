using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportTrainingService : IReportTrainingService
{
    public Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        throw new NotImplementedException();
    }
}