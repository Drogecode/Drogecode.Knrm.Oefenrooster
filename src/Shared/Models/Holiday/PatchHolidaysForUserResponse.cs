using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

public class PatchHolidaysForUserResponse : BaseResponse
{
    public Holiday? Patched { get; set; }
    public bool IsNew { get; set; }
}
