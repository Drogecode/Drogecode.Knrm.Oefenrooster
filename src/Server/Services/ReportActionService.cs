using Drogecode.Knrm.Oefenrooster.Server.Extensions;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionService : IReportActionService
{
    private readonly DataContext _database;
    private readonly ILogger<ReportActionService> _logger;
    private static readonly List<string> _typeInzetToIgnore = new() { "Boot uitgemeld" };

    public ReportActionService(DataContext database, ILogger<ReportActionService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid> users, List<string>? types, List<string>? search, int count, int skip, Guid customerId,
        bool minimal, DateTime? startDate, DateTime? endDate, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportActions.Include(x => x.Users)
            .Where(x => x.CustomerId == customerId
                        && (startDate == null || x.Start > startDate)
                        && (endDate == null || x.End < endDate)
                        && x.Users.Count(y => users.Contains(y.DrogeCodeId!.Value)) == users.Count
                        && (types == null || !types.Any() || types.Contains(x.Type))
                        && (search == null || !search.Any() || search.Any(y => EF.Functions.ILike(x.ShortDescription, "%" + y + "%"))
                            || search.Any(y => EF.Functions.ILike(x.Description, "%" + y + "%"))));

        List<DrogeAction>? drogeActions = null;
        if (minimal)
            drogeActions = await listWhere.OrderByDescending(x => x.Commencement).Skip(skip).Take(count).Select(x => x.ToMinimalDrogeAction()).ToListAsync(clt);
        else
            drogeActions = await listWhere.OrderByDescending(x => x.Commencement).Skip(skip).Take(count).Select(x => x.ToDrogeAction()).ToListAsync(clt);
        var sharePointActionsUser = new MultipleReportActionsResponse
        {
            Actions = drogeActions,
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
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        foreach (var report in allReports)
        {
            if (report.Number is not null && report.Number?.FloatingEquals(0, 0.1) != true && (report.Number % 1) != 0 &&
                allReports.Count(x => x.Start.Year == report.Start.Year && x.Number?.FloatingEquals((int)report.Number, 0.3) == true) >= 2)
            {
                skipped++;
                continue;
            }

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
                var prio = _database.ReportActions.Where(x => x.CustomerId == customerId).Select(x => x.Prio).Distinct();
                if (await prio.AnyAsync(clt))
                {
                    result.Values = await prio.ToListAsync(clt);
                    result.Success = true;
                    result.TotalCount = await prio.CountAsync(clt);
                }

                break;
            case DistinctReport.Type:
                var type = _database.ReportActions.Where(x => x.CustomerId == customerId).Select(x => x.Type).Distinct();
                if (await type.AnyAsync(clt))
                {
                    result.Values = await type.ToListAsync(clt);
                    result.Success = true;
                    result.TotalCount = await type.CountAsync(clt);
                }

                break;
            case DistinctReport.Year:
                var years = _database.ReportActions.Where(x => x.CustomerId == customerId).Select(x => x.Start.Year).Distinct();
                if (await years.AnyAsync(clt))
                {
                    result.Values = [];
                    foreach (var year in await years.ToListAsync(clt))
                    {
                        result.Values.Add(year.ToString());
                    }

                    result.Success = true;
                    result.TotalCount = await years.CountAsync(clt);
                }

                break;
            case DistinctReport.Boat:
                var boats = _database.ReportActions.Where(x => x.CustomerId == customerId).Select(x => x.Boat).Distinct();
                if (await boats.AnyAsync(clt))
                {
                    result.Values = [];
                    foreach (var boat in await boats.ToListAsync(clt))
                    {
                        result.Values.Add(boat);
                    }

                    result.Success = true;
                    result.TotalCount = await boats.CountAsync(clt);
                }
                break;
            default:
                result.Message = $"Invalid column type: `{column}`";
                break;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<AnalyzeHoursResult> AnalyzeHours(int year, string type, List<string>? boats, string timeZone, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AnalyzeHoursResult
        {
            UserCounters = []
        };
        var y = new List<int>() { year, year + 1, year - 1 };
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

        var dbActions = await _database.ReportActions.Where(x => 
                x.CustomerId == customerId && 
                x.Type == type && y.Contains(x.Start.Year) &&
                (boats == null || !boats.Any() || boats.Contains(x.Boat!)))
            .Select(x => new { x.Start, x.Commencement, x.Departure, x.End, x.Users })
            .ToListAsync(clt);

        foreach (var action in dbActions.Where(x => x.Users is not null))
        {
            var start = TimeZoneInfo.ConvertTimeFromUtc(action.Start, zone);
            if (start.Year != year) continue;
            var minutes = (action.End - action.Start).TotalMinutes;
            var fullHours = Convert.ToInt32(Math.Ceiling(minutes / 60));
            foreach (var user in action.Users!)
            {
                var userCounter = result.UserCounters.FirstOrDefault(x => x.UserId == user.DrogeCodeId && x.Type == type);
                if (userCounter is null)
                {
                    userCounter = new UserCounters()
                    {
                        UserId = user.DrogeCodeId ?? Guid.Empty,
                        Count = 1,
                        Minutes = minutes,
                        FullHours = fullHours,
                        Type = type,
                    };
                    result.UserCounters.Add(userCounter);
                }
                else
                {
                    userCounter.Count++;
                    userCounter.Minutes += minutes;
                    userCounter.FullHours += fullHours;
                }
            }
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