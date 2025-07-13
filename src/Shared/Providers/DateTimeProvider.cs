using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Shared.Providers;

public class DateTimeProvider : IDateTimeProvider
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
