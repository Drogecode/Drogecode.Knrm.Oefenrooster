﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IReportTrainingService
{
    Task<MultipleReportTrainingsResponse> GetListTrainingUser(List<Guid?> users, Guid userId, int count, int skip, Guid customerId, CancellationToken clt);
    Task<AnalyzeYearChartAllResponse> AnalyzeYearChartsAll(AnalyzeTrainingRequest trainingRequest, Guid customerId, string timeZone, CancellationToken clt);
    Task<DistinctResponse> Distinct(DistinctReport column, Guid customerId, Guid userId, CancellationToken clt);
    Task<AnalyzeHoursResult> AnalyzeHours(int year, string type, string timeZone, Guid customerId, CancellationToken clt);
}