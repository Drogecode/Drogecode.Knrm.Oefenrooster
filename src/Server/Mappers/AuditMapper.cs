using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class AuditMapper
{
    public static TrainingAudit ToTrainingAudit(this DbAudit dbAudit)
    {
        var note = System.Text.Json.JsonSerializer.Deserialize<AuditAssignedUser>(dbAudit.Note ?? string.Empty);
        return new TrainingAudit
        {
            UserId = note?.UserId,
            ByUser = dbAudit.UserId,
            TrainingId = dbAudit.ObjectKey,
            AuditType = dbAudit.AuditType,
            Assigned = note?.Assigned,
            Date = dbAudit.Created,
            Availabilty = note?.Availabilty,
            SetBy = note?.SetBy,
            AuditReason = note?.AuditReason,
        };
    }
}
