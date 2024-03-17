using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IMonthItemService
{
    Task<GetMultipleMonthItemResponse> GetItems(int year, int month, Guid customerId, CancellationToken clt);
    Task<GetMultipleMonthItemResponse> GetAllItems(int take, int skip, bool includeExpired, Guid customerId, CancellationToken clt);
    Task<PutMonthItemResponse> PutItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt);
}
