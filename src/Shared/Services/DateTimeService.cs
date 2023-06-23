using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now()
    {
        return DateTime.Now;
    }

    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public DateTime Today()
    {
        return DateTime.Today;
    }
}
