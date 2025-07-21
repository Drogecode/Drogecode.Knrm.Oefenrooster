using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class LicenseService : DrogeService, ILicenseService
{
    public LicenseService(ILogger<LicenseService> logger,
        DataContext database,
        IMemoryCache memoryCache,
        IDateTimeProvider dateTimeProvider) : base(logger, database, memoryCache, dateTimeProvider)
    {
    }

    public async Task<GetAllLicensesResponse> GetAllLicensesForCustomer(Guid customerId, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetAllLicensesResponse();

        var licenses = await Database.Licenses
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId
                        && (x.ValidFrom == null || x.ValidFrom <= DateTimeProvider.UtcNow())
                        && (x.ValidUntil == null || x.ValidUntil >= DateTimeProvider.UtcNow()))
            .Select(x => x.ToLicense())
            .ToListAsync(clt);
        if (licenses.Count != 0)
        {
            response.Licenses = licenses;
            response.TotalCount = licenses.Count;
            response.Success = true;
        }

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return response;
    }
}