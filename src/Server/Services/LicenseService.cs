using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class LicenseService : DrogeService, ILicenseService
{
    private const string LICENSES_CACHE_KEY = "License_{0}";
    private const string CUSTOMERS_WITH_LICENSE_CACHE_KEY = "cus_wi_lic_{0}";
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
        
        var cacheKey = string.Format(LICENSES_CACHE_KEY, customerId);
        if (MemoryCache.TryGetValue(cacheKey, out GetAllLicensesResponse? cachedResponse) && cachedResponse is not null)
            return cachedResponse;
        

        var licenses = await Database.Licenses
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId
                        && (x.ValidFrom == null || x.ValidFrom <= DateTimeProvider.UtcNow())
                        && (x.ValidUntil == null || x.ValidUntil >= DateTimeProvider.UtcNow()))
            .Select(x => x.ToLicense())
            .ToListAsync(clt);
        if (licenses.Count > 0)
        {
            response.Licenses = licenses;
            response.TotalCount = licenses.Count;
            response.Success = true;
        }

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(31));
        MemoryCache.Set(cacheKey, response, cacheOptions);
        return response;
    }

    public async Task<GetAllCustomerIdsWithLicenseResponse> GetAllCustomerIdsWithLicense(Licenses license, CancellationToken clt)
    {
        var sw = StopwatchProvider.StartNew();
        var response = new GetAllCustomerIdsWithLicenseResponse();
        
        var cacheKey = string.Format(CUSTOMERS_WITH_LICENSE_CACHE_KEY, license);
        if (MemoryCache.TryGetValue(cacheKey, out GetAllCustomerIdsWithLicenseResponse? cachedResponse) && cachedResponse is not null)
            return cachedResponse;
        

        var customerIds = await Database.Licenses
            .AsNoTracking()
            .Where(x => x.License == license
                        && (x.ValidFrom == null || x.ValidFrom <= DateTimeProvider.UtcNow())
                        && (x.ValidUntil == null || x.ValidUntil >= DateTimeProvider.UtcNow()))
            .Select(x => x.CustomerId)
            .ToListAsync(clt);
        if (customerIds.Count > 0)
        {
            response.CustomerIds = customerIds;
            response.TotalCount = customerIds.Count;
            response.Success = true;
        }

        sw.Stop();
        response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        var cacheOptions = new MemoryCacheEntryOptions();
        cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(10));
        cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(31));
        MemoryCache.Set(cacheKey, response, cacheOptions);
        return response;
    }
}