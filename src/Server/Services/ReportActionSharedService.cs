using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ReportActionSharedService : IReportActionSharedService
{
    private readonly DataContext _database;
    private readonly ILogger<ReportActionSharedService> _logger;

    public ReportActionSharedService(DataContext database, ILogger<ReportActionSharedService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<PutReportActionSharedResponse> PutReportActionShared(ReportActionSharedConfiguration sharedConfiguration, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutReportActionSharedResponse();
        var password = SecretHelper.CreateSecret(12);
        sharedConfiguration.Id = Guid.NewGuid();
        var dbShared = sharedConfiguration.ToDb();
        dbShared.CreatedOn = DateTime.UtcNow;
        dbShared.CreatedBy = userId;
        dbShared.CustomerId = customerId;
        dbShared.HashedPassword = PasswordHasher.HashNewPassword(password);
        _database.ReportActionShares.Add(dbShared);
        if (await _database.SaveChangesAsync(clt) > 0)
        {
            result.NewId = sharedConfiguration.Id;
            result.Success = true;
            result.Password = password;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}