using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CustomerService : DrogeService, ICustomerService
{

    public CustomerService(ILogger<CustomerService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService) : base(logger, database,
        memoryCache, dateTimeService)
    {
    }

    public async Task<GetAllCustomersResponse> GetAllCustomers(int take, int skip, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetAllCustomersResponse();
        var links = await Database.Customers
            .Skip(skip)
            .Take(take)
            .OrderBy(x=>x.Name)
            .ThenBy(x=>x.Id)
            .Select(x => x.ToCustomer())
            .ToListAsync(clt);
        result.Customers = links;
        result.TotalCount = links.Count;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetCustomerResponse> GetCustomerById(Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetCustomerResponse
        {
            Customer = await Database.Customers.Where(x => x.Id == customerId).Select(x => x.ToCustomer()).FirstOrDefaultAsync(clt)
        };
        result.Success = result.Customer is not null;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<List<CustomerAuthentication>> GetByTenantId(string? tenantId, CancellationToken clt)
    {
        var customers = await Database.Customers
            .Where(x => x.TenantId == tenantId)
            .Select(x=>x.ToCustomerAuthentication())
            .AsNoTracking()
            .ToListAsync(clt);
        return customers;
    }

    public async Task<PutResponse> PutNewCustomer(Customer customer, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutResponse();
        var newId = Guid.CreateVersion7();
        Database.Customers.Add(new DbCustomers
        {
            Id = newId,
            Name = customer.Name,
            TimeZone = customer.TimeZone,
            Created = DateTime.UtcNow,
            Instance = customer.Instance,
            Domain = customer.Domain,
            TenantId = customer.TenantId,
            GroupId = customer.GroupId,
        });

        result.Success = await Database.SaveChangesAsync(clt) > 0;
        if (result.Success)
            result.NewId = newId;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchResponse> PatchCustomer(Customer customer, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchResponse();
        var dbCustomer = await Database.Customers.FirstOrDefaultAsync(x => x.Id == customer.Id, clt);
        if (dbCustomer is not null)
        {
            dbCustomer.Name = customer.Name;
            dbCustomer.TimeZone = customer.TimeZone;
            dbCustomer.Instance = customer.Instance;
            dbCustomer.Domain = customer.Domain;
            dbCustomer.TenantId = customer.TenantId;
            dbCustomer.GroupId = customer.GroupId;
            Database.Customers.Update(dbCustomer);
        }

        result.Success = await Database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}