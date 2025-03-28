﻿using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

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
            Availability = note?.Availability,
            SetBy = note?.SetBy,
            AuditReason = note?.AuditReason,
        };
    }
}
