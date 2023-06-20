using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public abstract class BaseResponse
{
    public bool Success { get; set; }
    public bool Offline { get; set; }
    public long ElapsedMilliseconds { get; set; } = -1;
}
