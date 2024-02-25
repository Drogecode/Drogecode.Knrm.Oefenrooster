using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class PreComAlert
{
    public Guid Id { get; set; }
    public Guid? NotificationId { get; set; }
    public string? Alert { get; set; }
    public DateTime? SendTime { get; set; }
    public int? Priority { get; set; }
    public string? RawJson { get; set; }
}
