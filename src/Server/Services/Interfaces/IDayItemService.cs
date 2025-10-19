using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces
{
    public interface IDayItemService
    {
        Task<GetMultipleDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt);
        Task<GetMultipleDayItemResponse> GetAllFutureDayItems(Guid customerId, int count, int skip, bool forAllUsers, Guid userId, CancellationToken clt);
        Task<GetMultipleDayItemResponse> GetDayItemDashboard(Guid userId, Guid customerId, CancellationToken clt);
        Task<GetDayItemResponse> GetDayItemById(Guid customerId, Guid id, CancellationToken clt);
        Task<PutDayItemResponse> PutDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt);
        Task<PatchDayItemResponse> PatchDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt);
        Task<bool> DeleteDayItem(Guid idToDelete, Guid customerId, Guid userId, CancellationToken clt);
        Task<bool> PatchCalendarEventId(Guid dayItemId, Guid userId, Guid customerId, string? calendarEventId, CancellationToken clt);
    }
}
