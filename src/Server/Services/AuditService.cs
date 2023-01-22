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
                 AuditType = (int)auditType,
                 Note = note,
                 ObjectKey = objectKey,
                 ObjectName = objectName,
                 Created = DateTime.UtcNow
             }
        );
        await _database.SaveChangesAsync();
    }

}
