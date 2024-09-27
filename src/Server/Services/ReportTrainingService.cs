using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportTrainingService : IReportTrainingService
{
    private readonly Database.DataContext _database;

    public ReportTrainingService(Database.DataContext database)
    {
        _database = database;
    }

    public async Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
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


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeTrainingRequest trainingRequest, Guid customerId, string timeZone, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = _database.ReportTrainings
            .Where(x => x.CustomerId == customerId && x.Users!.Count(y => trainingRequest.Users!.Contains(y.DrogeCodeId)) == trainingRequest.Users!.Count)
            .Select(x => new { x.Start });
        var result = new AnalyzeYearChartAllResponse { TotalCount = allReports.Count() };
        List<int> years = new();
        foreach (var report in allReports)
        {
            var start = report.Start.ToDateTimeTimeZone(timeZone).ToDateTime();

            if (!years.Contains(start.Year)) // Could not find a way to check this in the db request.
            {
                if (years.Count >= trainingRequest.Years)
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
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }
}