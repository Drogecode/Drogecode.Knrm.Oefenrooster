using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces
{
    public interface ICalendarItemService
    {
        Task<GetMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt);
        Task<GetDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt);
        Task<PutMonthItemResponse> PutMonthItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt);
        Task<PutDayItemResponse> PutDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt);
    }
}
