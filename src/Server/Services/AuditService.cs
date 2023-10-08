using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly Database.DataContext _database;
    public AuditService(ILogger<AuditService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task Log(Guid user, AuditType auditType, Guid customer, string? note = null, Guid? objectKey = null, string? objectName = null)
    {
        await _database.Audits.AddAsync(
             new Database.Models.DbAudit()
             {
                 Id = Guid.NewGuid(),
                 UserId = user,
                 CustomerId = customer,
                 AuditType = auditType,
                 Note = note,
                 ObjectKey = objectKey,
                 ObjectName = objectName,
                 Created = DateTime.UtcNow
             }
        );
        await _database.SaveChangesAsync();
    }

    public async Task<GetTrainingAuditResponse> GetTrainingAudit(Guid customerId, Guid userId, Guid trainingId)
    {
        var response = new GetTrainingAuditResponse();
        var sw = Stopwatch.StartNew();
        var audits = await _database.Audits.Where(x => x.ObjectKey == trainingId).ToListAsync();
        if (audits.Any())
        {
            response.TrainingAudits = new List<TrainingAudit>();
            foreach (var audit in audits)
            {
                if (audit.AuditType == AuditType.PatchAssignedUser)
                    response.TrainingAudits.Add(audit.ToTrainingAudit());
            }
        }
        response.TotalCount = audits.Count;
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }

}
