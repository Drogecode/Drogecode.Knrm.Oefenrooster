namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IConfigurationService
{
    Task<bool> UpgradeDatabase();
}
