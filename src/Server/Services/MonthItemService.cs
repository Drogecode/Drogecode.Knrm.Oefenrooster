using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using System.Diagnostics;
using System.Linq.Expressions;

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

    public async Task<GetMonthItemResponse> GetById(Guid customerId, Guid id, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var monthItem = await _database.RoosterItemMonths.Where(x => x.CustomerId == customerId && x.Id == id && x.DeletedOn == null)
            .Select(DbSelectItem()).FirstOrDefaultAsync(clt);
        var result = new GetMonthItemResponse
        {
            MonthItem = monthItem,
        };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetMultipleMonthItemResponse> GetItems(int year, int month, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var monthItems = await _database.RoosterItemMonths.Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.Month == month && (x.Year == null || x.Year == 0 || x.Year == year))
            .Select(DbSelectItem()).ToListAsync(clt);
        var result = new GetMultipleMonthItemResponse
        {
            MonthItems = monthItems,
        };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetMultipleMonthItemResponse> GetAllItems(int take, int skip, bool includeExpired, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var now = DateTime.UtcNow;
        var dbCall = _database.RoosterItemMonths.Where(x =>
            x.CustomerId == customerId && x.DeletedOn == null && (includeExpired || ((x.Year == null || x.Year == 0 || x.Year > now.Year || (x.Year == now.Year && x.Month >= now.Month)))));
        var monthItems = await dbCall.Select(DbSelectItem()).OrderByDescending(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Order).Skip(skip).Take(take).ToListAsync(clt);
        var result = new GetMultipleMonthItemResponse
        {
            MonthItems = monthItems,
            TotalCount = await dbCall.CountAsync(clt)
        };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<PutMonthItemResponse> PutItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt)
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

    public async Task<PatchMonthItemResponse> PatchItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchMonthItemResponse();

        if (roosterItemMonth is not null)
        {
            if (false) // Does not work for xUnit in memory db
            {
                var dbResult = await _database.RoosterItemMonths
                    .Where(x => x.CustomerId == customerId && x.Id == roosterItemMonth.Id && x.DeletedOn == null)
                    .ExecuteUpdateAsync(b => b
                            .SetProperty(p => p.Severity, roosterItemMonth.Severity)
                            .SetProperty(p => p.Month, roosterItemMonth.Month)
                            .SetProperty(p => p.Year, roosterItemMonth.Year)
                            .SetProperty(p => p.Order, roosterItemMonth.Order)
                            .SetProperty(p => p.Text, roosterItemMonth.Text)
                            .SetProperty(p => p.Type, roosterItemMonth.Type)
                        , clt);
            }
            else
            {
                var monthItem = await _database.RoosterItemMonths.Where(x => x.CustomerId == customerId && x.Id == roosterItemMonth.Id && x.DeletedOn == null).FirstOrDefaultAsync(clt);
                if (monthItem is not null)
                {
                    monthItem.Severity = roosterItemMonth.Severity;
                    monthItem.Month = roosterItemMonth.Month;
                    monthItem.Year = roosterItemMonth.Year;
                    monthItem.Order = roosterItemMonth.Order;
                    monthItem.Text = roosterItemMonth.Text;
                    monthItem.Type = roosterItemMonth.Type;
                    _database.RoosterItemMonths.Update(monthItem);
                    result.Success = (await _database.SaveChangesAsync(clt)) > 0;
                }
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<bool> DeleteItem(Guid idToDelete, Guid customerId, Guid userId, CancellationToken clt)
    {
        var item = await _database.RoosterItemMonths.FirstOrDefaultAsync(x => x.Id == idToDelete && x.CustomerId == customerId && x.DeletedOn == null);
        if (item is null)
            return false;
        item.DeletedOn = DateTime.UtcNow;
        item.DeletedBy = userId;
        _database.RoosterItemMonths.Update(item);
        return (await _database.SaveChangesAsync(clt)) > 0;
    }

    private static Expression<Func<DbRoosterItemMonth, RoosterItemMonth>> DbSelectItem()
    {
        return x => new RoosterItemMonth
        {
            Id = x.Id,
            Month = x.Month,
            Severity = x.Severity,
            Year = x.Year,
            Type = x.Type,
            Text = x.Text,
            Order = x.Order,
        };
    }
}