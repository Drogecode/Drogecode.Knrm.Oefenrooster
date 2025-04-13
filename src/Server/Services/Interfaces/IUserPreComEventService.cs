using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserPreComEventService
{
    Task<List<UserPreComEvent>> GetEventsForUserForDay(Guid userId, Guid customerId, DateOnly date, CancellationToken clt);
    Task<bool> RemoveEvent(DrogeUser drogeUser, UserPreComEvent userPreComEvent, CancellationToken clt);
    Task<bool> AddEvent(DrogeUser drogeUser, DateTime start, DateTime end, DateOnly date, CancellationToken clt);
}