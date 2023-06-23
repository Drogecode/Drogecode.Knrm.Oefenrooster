using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services.Interfaces;

public interface IDateTimeServiceMock : IDateTimeService
{
    void SetMockDateTime(DateTime? dateTime);
}
