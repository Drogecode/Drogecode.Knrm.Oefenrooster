using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Function;

public class MultipleFunctionsResponse : BaseResponse
{
    public List<DrogeFunction>? Functions { get; set; }
}
