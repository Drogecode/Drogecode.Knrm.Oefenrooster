using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class PatchDefaultScheduleForUserResponse : BaseResponse
{
    public DefaultSchedule? Patched { get; set; }
}
