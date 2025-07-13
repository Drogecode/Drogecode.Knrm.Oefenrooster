using Drogecode.Knrm.Oefenrooster.SharedForTests.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.SharedForTests.Providers;

public class DateTimeProviderMock : IDateTimeProviderMock
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
