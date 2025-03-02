using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CustomerService : DrogeService, ICustomerService
{
    public CustomerService(ILogger<CustomerService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService) : base(logger, database, memoryCache, dateTimeService)
    {
    }

    public async Task<GetAllUserLinkCustomersResponse> GetAllLinkUserCustomers(Guid userId, Guid customerId)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetAllUserLinkCustomersResponse();
        var links = await Database.LinkUserCustomers
            .Include(x => x.Customer)
            .Where(x => x.UserId == userId && x.IsActive)
            .Select(x => x.ToCustomer(customerId))
            .ToListAsync();
        result.UserLinkedCustomers = links;
        result.TotalCount = links.Count;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<PatchResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchResponse();
        var links = await Database.LinkUserCustomers.Where(x => x.UserId == body.UserId).ToListAsync();
        if (links.Count == 0)
        {
            var user = await Database.Users.FirstOrDefaultAsync(x => x.Id == body.UserId);
            if (user is null)
            {
                sw.Stop();
                result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
                result.Success = false;
                return result;
            }

            Database.LinkUserCustomers.Add(new DbLinkUserCustomer()
            {
                Id = Guid.CreateVersion7(),
                UserId = body.UserId,
                CustomerId = user.CustomerId,
                IsPrimary = true,
                IsActive = true,
                Order = 0,
                LinkedBy = userId,
                LinkedOn = DateTime.UtcNow
            });
        }

        if (links.Any(x => x.CustomerId == body.CustomerId))
        {
            var link = links.First(x => x.CustomerId == body.CustomerId);
            if (link.IsActive != body.IsActive)
            {
                link.IsActive = body.IsActive;
                link.LinkedBy = userId;
                link.LinkedOn = DateTime.UtcNow;
                Database.LinkUserCustomers.Update(link);
            }
            else
            {
                Logger.LogInformation("Link to customer not changed `{user}` `{customer}` `{isActive}`", body.UserId, body.CustomerId, body.IsActive);
            }
        }
        else
        {
            Database.LinkUserCustomers.Add(new DbLinkUserCustomer()
            {
                Id = Guid.CreateVersion7(),
                UserId = body.UserId,
                CustomerId = body.CustomerId,
                IsPrimary = false,
                IsActive = true,
                Order = links.OrderByDescending(x => x.Order).First().Order + 10,
                LinkedBy = userId,
                LinkedOn = DateTime.UtcNow
            });
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = await Database.SaveChangesAsync() > 0;
        return result;
    }
}