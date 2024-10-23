using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;
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
    Task<Event?> AddToCalendar(string? externalUserId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, List<UserLinkedMail> attendees);
    Task PatchCalender(string? externalUserId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, List<UserLinkedMail> attendees);
    Task DeleteCalendarEvent(string? externalUserId, string calendarEventId, CancellationToken clt);
    Task<GetHistoricalResponse> SyncHistorical(Guid customerId, CancellationToken clt);
    Task SendMail(Guid? userId, string emailAddress, string subject, string body, CancellationToken clt);
}
