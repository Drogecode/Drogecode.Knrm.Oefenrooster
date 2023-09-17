using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IGraphService
{
    void InitializeGraph(Settings? settings = null);

    Task<string?> GetAccessTokenAsync();

    Task<UserCollectionResponse?> ListUsersAsync();
    Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId);
    Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users);
    Task GetLists();
    Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<string> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<MultipleSharePointActionsResponse> GetListActionsUser(List<string> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<Event> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd);
    Task DeleteCalendarEvent(Guid? userId, string calendarEventId, CancellationToken clt);
}
