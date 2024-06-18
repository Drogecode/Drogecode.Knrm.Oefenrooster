using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionService : IReportActionService
{
    private readonly Database.DataContext _database;

    public ReportActionService(Database.DataContext database)
    {
        _database = database;
    }

    public async Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportActions.Include(x => x.Users).Where(x => x.CustomerId == customerId && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count());
        var sharePointActionsUser = new MultipleReportActionsResponse
        {
            Actions = await listWhere.OrderByDescending(x => x.Commencement).Skip(skip).Take(count).Select(x => x.ToDrogeAction()).ToListAsync(clt),
            TotalCount = listWhere.Count(),
            Success = true,
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = _database.ReportActions.Where(x=>x.CustomerId == customerId);
        var result = new AnalyzeYearChartAllResponse { TotalCount = allReports.Count() };
        foreach (var report in allReports)
        {
            if (result.Years.All(x => x.Year != report.Start.Year))
            {
                result.Years.Add(new AnalyzeYearDetails() { Year = report.Start.Year });
            }

            var year = result.Years.First(x => x.Year == report.Start.Year);
            if (year.Months.All(x => x.Month != report.Start.Month))
            {
                year.Months.Add(new AnalyzeMonthDetails() { Month = report.Start.Month });
            }

            var month = year.Months.First(x => x.Month == report.Start.Month);
            month.Count++;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }
}