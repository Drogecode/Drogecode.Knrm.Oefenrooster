using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class UserTrainingCounter
{
    public Guid UserId { get; set; }
    public int Count { get; set; }
}
