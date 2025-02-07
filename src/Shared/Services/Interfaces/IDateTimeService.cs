namespace Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;

public interface IDateTimeService
{
    DateTime Now();
    DateTime UtcNow();
    DateTime Today();
}
