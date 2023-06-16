using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

public class MultipleHolidaysResponse : BaseResponse
{
    public List<Holiday>? Holidays { get; set; }
}
