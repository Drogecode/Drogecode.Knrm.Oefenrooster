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

    public async Task<GetTrainingAuditResponse> GetTrainingAudit(Guid customerId, Guid userId, int count, int skip, Guid trainingId, CancellationToken clt)
    {
        var response = new GetTrainingAuditResponse();
        var sw = Stopwatch.StartNew();
        var audits = _database.Audits
            .Where(x => x.AuditType == AuditType.PatchAssignedUser && (trainingId.Equals(Guid.Empty) || x.ObjectKey == trainingId))
            .AsNoTracking()
            .OrderByDescending(x => x.Created);
        if (await audits.AnyAsync(clt))
        {
            response.TrainingAudits = new List<TrainingAudit>();
            foreach (var auditDb in await audits.Skip(skip).Take(count).ToListAsync(clt))
            {
                var audit = auditDb.ToTrainingAudit();
                if (trainingId.Equals(Guid.Empty))
                {
                    var dbTraining = await _database.RoosterTrainings
                        .Include(x => x.RoosterAvailables)
                        .FirstOrDefaultAsync(x => x.Id == audit.TrainingId, clt);
                    var training = dbTraining?.ToTraining();
                    audit.Training = training;
                    audit.IsDeleted = dbTraining?.DeletedOn is not null;
                }

                response.TrainingAudits.Add(audit);
            }
        }

        response.TotalCount = await audits.CountAsync(clt);
        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }
}