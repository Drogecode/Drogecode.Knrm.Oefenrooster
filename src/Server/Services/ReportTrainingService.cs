using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Microsoft.Graph.Models;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportTrainingService : IReportTrainingService
{
    private readonly Database.DataContext _database;
    private readonly ILogger<ReportTrainingService> _logger;

    public ReportTrainingService(ILogger<ReportTrainingService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid?> users, List<string>? types, Guid userId, int count, int skip, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var listWhere = _database.ReportTrainings.Include(x => x.Users)
            .Where(x => x.CustomerId == customerId 
                        && x.Users.Count(y => users.Contains(y.DrogeCodeId)) == users.Count
                        && (types == null || !types.Any() || types.Contains(x.Type)));
        var sharePointActionsUser = new MultipleReportTrainingsResponse
        {
            Trainings = await listWhere.OrderByDescending(x => x.Start).Skip(skip).Take(count).Select(x => x.ToDrogeTraining()).ToListAsync(clt),
            TotalCount = await listWhere.CountAsync(clt),
            Success = true,
        };
        sw.Stop();
        sharePointActionsUser.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return sharePointActionsUser;
    }


    public async Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeTrainingRequest trainingRequest, Guid customerId, string timeZone, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var allReports = await _database.ReportTrainings
            .Where(x => x.CustomerId == customerId && x.Users!.Count(y => trainingRequest.Users!.Contains(y.DrogeCodeId)) == trainingRequest.Users!.Count)
            .Select(x => new { x.Start })
            .OrderByDescending(x => x.Start)
            .ToListAsync(clt);
        var result = new AnalyzeYearChartAllResponse();
        List<int> years = new();
        var count = 0;
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        foreach (var report in allReports)
        {
            var start = TimeZoneInfo.ConvertTimeFromUtc(report.Start, zone);

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
            count++;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        result.TotalCount = count;
        return result;
    }

    public async Task<DistinctResponse> Distinct(DistinctReport column, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new DistinctResponse();

        switch (column)
        {
            case DistinctReport.Type:
                var type = _database.ReportTrainings.Select(x => x.Type).Distinct();
                if (await type.AnyAsync(clt))
                {
                    result.Values = await type.ToListAsync(clt);
                    result.Success = true;
                }

                break;
            case DistinctReport.None:
            case DistinctReport.Prio:
            default:
                result.Message = $"`{column}` is not valid for report trainings";
                _logger.LogInformation("`{column}` is not valid for report trainings by user `{userId}`", column, userId);
                break;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<AnalyzeHoursResult> AnalyzeHours(int year, string type, string timeZone, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new AnalyzeHoursResult
        {
            UserCounters = []
        };
        var y = new List<int>() { year, year + 1, year - 1 };
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var dbTrainings = await _database.ReportTrainings.Where(x => x.CustomerId == customerId && x.Type == type && y.Contains(x.Start.Year))
            .Select(x => new { x.Start, x.Commencement, x.End, x.Users })
            .ToListAsync(clt);

        foreach (var training in dbTrainings.Where(x => x.Users is not null))
        {
            var start = TimeZoneInfo.ConvertTimeFromUtc(training.Start, zone);
            if (start.Year != year) continue;
            var differentStart = ReportSettingsHelper.TrainingDifferentStart.Contains(type ?? string.Empty);
            var minutes = differentStart ? (training.End - training.Commencement).TotalMinutes : (training.End - training.Start).TotalMinutes;
            var fullHours = Convert.ToInt32(Math.Ceiling(minutes / 60));
            foreach (var user in training.Users!)
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
}