using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class MonthItemService : IMonthItemService
{
    private readonly ILogger<MonthItemService> _logger;
    private readonly Database.DataContext _database;
    public MonthItemService(ILogger<MonthItemService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<GetMultipleMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var monthItems = await _database.RoosterItemMonths.Where(x => x.CustomerId == customerId && x.Month == month && (x.Year == null || x.Year == 0 || x.Year == year)).Select(
            x => new RoosterItemMonth
            {
                Id = x.Id,
                Month = x.Month,
                Severity = x.Severity,
                Year = x.Year,
                Type = x.Type,
                Text = x.Text,
                Order = x.Order,
            }).ToListAsync(clt);
        var result = new GetMultipleMonthItemResponse
        {
            MonthItems = monthItems,
        };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<PutMonthItemResponse> PutMonthItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutMonthItemResponse();
        var dbItem = roosterItemMonth.ToDbRoosterItemMonth();
        dbItem.Id = Guid.NewGuid();
        dbItem.CustomerId = customerId;
        dbItem.CreatedBy = userId;
        dbItem.CreatedOn = DateTime.UtcNow;
        _database.RoosterItemMonths.Add(dbItem);
        result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        result.NewId = dbItem.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
