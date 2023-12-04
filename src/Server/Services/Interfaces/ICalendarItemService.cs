using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces
{
    public interface ICalendarItemService
    {
        Task<GetMultipleMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt);
        Task<GetMultipleDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt);
        Task<GetMultipleDayItemResponse> GetAllFutureDayItems(Guid customerId, int count, int skip, bool forAllUsers, Guid userId, CancellationToken clt);
        Task<GetDayItemResponse> GetDayItemById(Guid customerId, Guid id, CancellationToken clt);
        Task<PutMonthItemResponse> PutMonthItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt);
        Task<PutDayItemResponse> PutDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt);
        Task<PatchDayItemResponse> PatchDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt);
        Task<bool> DeleteDayItem(Guid idToDelete, Guid customerId, Guid userId, CancellationToken clt);
        Task<bool> PatchCalendarEventId(Guid id1, Guid userId, Guid customerId, string? id2, CancellationToken clt);
    }
}
