using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services;

public class DateTimeServiceMock : IDateTimeServiceMock
{
    private DateTime? _mockDateTime;
    public void SetMockDateTime(DateTime? dateTime)
    {
        _mockDateTime = dateTime;
    }

    public DateTime Now()
    {
        if (_mockDateTime is null)
            return DateTime.Now;
        else
            return _mockDateTime.Value;
    }

    public DateTime UtcNow()
    {
        if (_mockDateTime is null)
            return DateTime.UtcNow;
        else
            return _mockDateTime.Value;
    }

    public DateTime Today()
    {
        if (_mockDateTime is null)
            return DateTime.Today;
        else
            return _mockDateTime.Value.Date;
    }
}
