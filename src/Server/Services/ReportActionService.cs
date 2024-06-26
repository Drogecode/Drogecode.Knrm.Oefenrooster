﻿using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionService : IReportActionService
{
    private readonly Database.DataContext _database;

    public ReportActionService(Database.DataContext database)
    {
        _database = database;
    }

    public async Task<MultipleReportActionsResponse> GetListActionsUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportActions.Include(x => x.Users).Where(x => x.CustomerId == customerId && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count);
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


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeActionRequest actionRequest, Guid customerId, string timeZone, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = _database.ReportActions
            .Where(x => x.CustomerId == customerId
                        && (actionRequest.Prio == null || !actionRequest.Prio.Any() || actionRequest.Prio.Contains(x.Prio))
                        && x.Users!.Count(y => actionRequest.Users!.Contains(y.DrogeCodeId)) == actionRequest.Users!.Count)
            .Select(x => new { x.Start });
        var result = new AnalyzeYearChartAllResponse { TotalCount = allReports.Count() };
        foreach (var report in allReports)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var start = TimeZoneInfo.ConvertTimeFromUtc(report.Start, zone);
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
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<DistinctResponse> Distinct(DistinctReportAction column, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new DistinctResponse();

        switch (column)
        {
            case DistinctReportAction.None:
                result.Message = "None is not valid";
                break;
            case DistinctReportAction.Prio:
                var prio = _database.ReportActions.Select(x => x.Prio).Distinct();
                if (prio.Any())
                {
                    result.Values = prio.ToList();
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