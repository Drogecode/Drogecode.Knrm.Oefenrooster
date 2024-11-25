using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Server.Extensions;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionService : IReportActionService
{
    private readonly Database.DataContext _database;
    private readonly ILogger<ReportActionService> _logger;
    private static readonly List<string> _typeInzetToIgnore = new() { "Boot uitgemeld" };

    public ReportActionService(Database.DataContext database, ILogger<ReportActionService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportActions.Include(x => x.Users).Where(x => x.CustomerId == customerId && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count);
        var sharePointActionsUser = new MultipleReportActionsResponse
        {
            Actions = await listWhere.OrderByDescending(x => x.Commencement).Skip(skip).Take(count).Select(x => x.ToDrogeAction()).ToListAsync(clt),
            TotalCount = await listWhere.CountAsync(clt),
            Success = true,
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeActionRequest actionRequest, Guid customerId, string timeZone, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = await _database.ReportActions
            .Where(x => x.CustomerId == customerId
                        && (actionRequest.Prio == null || !actionRequest.Prio.Any() || actionRequest.Prio.Contains(x.Prio))
                        && x.Users!.Count(y => actionRequest.Users!.Contains(y.DrogeCodeId)) == actionRequest.Users!.Count
                        && (x.Type == null || !_typeInzetToIgnore.Contains(x.Type)))
            .Select(x => new { x.Start, x.Number })
            .OrderByDescending(x => x.Start)
            .ToListAsync(clt);
        var result = new AnalyzeYearChartAllResponse();
        var skipped = 0;
        var totalCount = 0;
        List<int> years = new();
        foreach (var report in allReports)
        {
            if (report.Number is not null && report.Number?.FloatingEquals(0, 0.1) != true && (report.Number % 1) != 0 &&
                allReports.Count(x => x.Start.Year == report.Start.Year && x.Number?.FloatingEquals((int)report.Number, 0.3) == true) >= 2)
            {
                skipped++;
                continue;
            }

            var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var start = TimeZoneInfo.ConvertTimeFromUtc(report.Start, zone);

            if (!years.Contains(start.Year)) // Could not find a way to check this in the db request.
            {
                if (years.Count >= actionRequest.Years)
                {
                    break;
                }

                years.Add(start.Year);
            }

            if (result.Years.All(x => x.Year != start.Year))
            {
                result.Years.Add(new AnalyzeYearDetails() { Year = start.Year });
            }

            var year = result.Years.First(x => x.Year == start.Year);
            if (year.Months.All(x => x.Month != start.Month))
            {
                year.Months.Add(new AnalyzeMonthDetails() { Month = start.Month });
            }

            var month = year.Months.First(x => x.Month == start.Month);
            month.Count++;
            totalCount++;
        }

        _logger.LogInformation("Skipped {skipped} reports in analyzes", skipped);
        result.TotalCount = totalCount;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DistinctResponse> Distinct(DistinctReport column, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new DistinctResponse();

        switch (column)
        {
            case DistinctReport.None:
                result.Message = "None is not valid";
                break;
            case DistinctReport.Prio:
                var prio = _database.ReportActions.Select(x => x.Prio).Distinct();
                if (await prio.AnyAsync(clt))
                {
                    result.Values = await prio.ToListAsync(clt);
                    result.Success = true;
                }

                break;
            case DistinctReport.Type:
                var type = _database.ReportActions.Select(x => x.Type).Distinct();
                if (await type.AnyAsync(clt))
                {
                    result.Values = await type.ToListAsync(clt);
                    result.Success = true;
                }

                break;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<KillDbResponse> KillDb(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new KillDbResponse();
        result.KillCount += await _database.ReportUsers.ExecuteDeleteAsync(clt);
        result.KillCount += await _database.ReportActions.ExecuteDeleteAsync(clt);
        result.KillCount += await _database.ReportTrainings.ExecuteDeleteAsync(clt);
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }
}