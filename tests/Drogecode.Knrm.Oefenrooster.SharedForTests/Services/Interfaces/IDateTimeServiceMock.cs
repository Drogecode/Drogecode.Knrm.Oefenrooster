using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.SharedForTests.Services.Interfaces;

public interface IDateTimeServiceMock : IDateTimeService
{
    void SetMockDateTime(DateTime? dateTime);
}
