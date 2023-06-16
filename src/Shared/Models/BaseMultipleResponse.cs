using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public abstract class BaseMultipleResponse : BaseResponse
{
    public int TotalCount { get; set; } = -1;
}
