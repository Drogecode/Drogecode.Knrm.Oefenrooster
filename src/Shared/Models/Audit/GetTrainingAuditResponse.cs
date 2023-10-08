using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;

public class GetTrainingAuditResponse : BaseMultipleResponse
{
    public List<TrainingAudit>? TrainingAudits { get; set; }
}

public class TrainingAudit : AuditAssignedUser
{
    public AuditType AuditType { get; set; }

}