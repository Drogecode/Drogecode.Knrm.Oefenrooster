using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces
{
    public interface ICalendarItemService
    {
        Task<GetMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken token);
        Task<GetDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, CancellationToken token);
    }
}
