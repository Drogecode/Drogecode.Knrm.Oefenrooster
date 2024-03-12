using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class MultiplePreComAlertsResponse : BaseMultipleResponse
{
    public List<PreComAlert>? PreComAlerts { get; set; }
}
