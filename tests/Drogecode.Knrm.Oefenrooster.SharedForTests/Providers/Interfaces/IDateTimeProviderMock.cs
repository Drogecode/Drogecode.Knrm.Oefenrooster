using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.SharedForTests.Providers.Interfaces;

public interface IDateTimeProviderMock : IDateTimeProvider
{
    void SetMockDateTime(DateTime? dateTime);
}
