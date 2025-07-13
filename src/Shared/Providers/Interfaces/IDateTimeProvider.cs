namespace Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

public interface IDateTimeProvider
{
    DateTime Now();
    DateTime UtcNow();
    DateTime Today();
}
