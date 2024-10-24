using Drogecode.Knrm.Oefenrooster.Server.Graph;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Mocks.Services
{
    public class GraphServiceMock : IGraphService
    {
        public Task<MultipleSharePointActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task<Event?> AddToCalendar(string? externalUserId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, List<UserLinkedMail> attendees)
        {
            throw new NotImplementedException();
        }

        public Task PatchCalender(string? externalUserId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, List<UserLinkedMail> attendees)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCalendarEvent(string? externalUserId, string calendarEventId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task PatchCalender(Guid userId, string eventId, string description, DateTime dateStart, DateTime dateEnd, bool isAllDay, List<UserLinkedMail> attendees)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCalendarEvent(Guid? userId, string calendarEventId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task<GetHistoricalResponse> SyncHistorical(Guid customerId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task SendMail(Guid? userId, string emailAddress, string subject, string body, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task<DirectoryObjectCollectionResponse?> GetGroupForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task GetLists()
        {
            throw new NotImplementedException();
        }

        public Task<MultipleSharePointTrainingsResponse> GetListTrainingUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public void InitializeGraph(Settings? settings = null)
        {
            // do nothing in mock
            return;
        }

        public Task<UserCollectionResponse?> ListUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserCollectionResponse> NextUsersPage(UserCollectionResponse users)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SyncSharePointActions(Guid customerId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SyncSharePointTrainings(Guid customerId, CancellationToken clt)
        {
            throw new NotImplementedException();
        }
    }
}
