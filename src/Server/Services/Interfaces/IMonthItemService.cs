using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IMonthItemService
{
    Task<GetMultipleMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt);
    Task<PutMonthItemResponse> PutMonthItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt);
}
