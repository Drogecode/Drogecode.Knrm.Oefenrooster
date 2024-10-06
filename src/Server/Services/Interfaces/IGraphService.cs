using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IGraphService
{
    void InitializeGraph(Settings? settings = null);
    Task<UserCollectionResponse?> ListUsersAsync();
    Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId);
    Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users);
    Task GetLists();
    Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<bool> SyncSharePointActions(Guid customerId, CancellationToken clt);
    Task<bool> SyncSharePointTrainings(Guid customerId, CancellationToken clt);
    Task<MultipleSharePointActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<Event?> AddToCalendar(Guid userId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay);
    Task PatchCalender(Guid userId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay);
    Task DeleteCalendarEvent(Guid? userId, string calendarEventId, CancellationToken clt);
    Task<GetHistoricalResponse> SyncHistorical(Guid customerId, CancellationToken clt);
    Task SendMail(Guid? userId, string emailAddress, string subject, string body, CancellationToken clt);
}
