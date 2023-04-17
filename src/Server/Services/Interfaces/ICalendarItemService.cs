using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces
{
    public interface ICalendarItemService
    {
        Task<GetMonthItemResponse> GetMonthItem(int year, int month, Guid customerId);
    }
}
