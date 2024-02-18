using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
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
    public Guid? ByUser { get; set; }
    public Guid? TrainingId { get; set; }
    public DateTime Date {  get; set; }
    public Training? Training { get; set; }
    public bool IsDeleted { get; set; }
}