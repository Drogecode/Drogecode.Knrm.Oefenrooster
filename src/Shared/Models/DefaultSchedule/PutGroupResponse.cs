using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class PutGroupResponse : BaseResponse
{
    public DefaultGroup? Group { get; set; }
    public PutError Error { get; set; }
}
