using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract;

public abstract class DrogeManager : IDrogeManager
{
    internal readonly ILogger<DrogeManager> Logger;

    protected DrogeManager(ILogger<DrogeManager> logger)
    {
        Logger = logger;
    }
}