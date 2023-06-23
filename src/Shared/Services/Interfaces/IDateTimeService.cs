using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

public interface IDateTimeService
{
    DateTime Now();
    DateTime UtcNow();
    DateTime Today();
}
