namespace Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;

public interface IDrogeService
{
    Task<int> SaveDb(CancellationToken clt);
}