using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using System.Diagnostics;

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

    public async Task<MultipleReportActionShareConfigurationResponse> GetAllReportActionSharedConfiguration(Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var query = _database.ReportActionShares.Where(x=>x.CustomerId == customerId && x.ValidUntil >= DateTime.UtcNow).Select(x=>x.ToDrogecode());
        var result = new MultipleReportActionShareConfigurationResponse
        {
            ReportActionSharedConfiguration = await query.ToListAsync(clt),
            TotalCount = await query.CountAsync(clt),
            Success = true
        };
        
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PutReportActionSharedResponse> PutReportActionShared(ReportActionSharedConfiguration sharedConfiguration, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutReportActionSharedResponse();
        var password = SecretHelper.CreateSecret(22);
        sharedConfiguration.Id = Guid.NewGuid();
        var dbShared = sharedConfiguration.ToDb(_logger);
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

    public async Task<ReportActionSharedConfiguration?> GetReportActionSharedConfiguration(Guid customerId, Guid sharedId, CancellationToken clt)
    {
        return await _database.ReportActionShares.Where(x => x.CustomerId == customerId && x.Id == sharedId).Select(x => x.ToDrogecode()).FirstOrDefaultAsync(clt);
    }

    public async Task<AuthenticateExternalResult> AuthenticateExternal(AuthenticateExternalRequest body, CancellationToken clt)
    {
        var response = new AuthenticateExternalResult();
        var sharedReport = await _database.ReportActionShares.Where(x => x.Id == body.ExternalId).Select(x => new { x.HashedPassword, x.CustomerId, x.ValidUntil }).FirstOrDefaultAsync(clt);
        if (sharedReport is null || sharedReport.ValidUntil < DateTime.UtcNow)
            return response;
        if (PasswordHasher.ComparePassword(body.Password, sharedReport?.HashedPassword))
        {
            response.Success = true;
            response.CustomerId = sharedReport!.CustomerId;
        }

        return response;
    }

    public async Task<DeleteResponse> DeleteReportActionSharedConfiguration(Guid itemId, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new DeleteResponse();
        var reportActionShared = await _database.ReportActionShares.Where(x => x.CustomerId == customerId && x.Id == itemId).FirstOrDefaultAsync(clt);
        if (reportActionShared is not null)
        {
            reportActionShared.ValidUntil = DateTime.UtcNow;
            _database.ReportActionShares.Update(reportActionShared);
            result.Success = await _database.SaveChangesAsync(clt) > 0;
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}