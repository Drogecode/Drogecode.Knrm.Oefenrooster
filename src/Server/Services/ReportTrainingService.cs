﻿using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportTrainingService : IReportTrainingService
{
    private readonly Database.DataContext _database;

    public ReportTrainingService(Database.DataContext database)
    {
        _database = database;
    }

    public async Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportTrainings.Include(x => x.Users).Where(x => x.CustomerId == customerId && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count);
        var sharePointActionsUser = new MultipleReportTrainingsResponse
        {
            Trainings = await listWhere.OrderByDescending(x => x.Start).Skip(skip).Take(count).Select(x => x.ToDrogeTraining()).ToListAsync(clt),
            TotalCount = listWhere.Count(),
            Success = true,
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(List<Guid> users, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = _database.ReportTrainings
            .Where(x => x.CustomerId == customerId && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count)
            .Select(x => new { x.Start });
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