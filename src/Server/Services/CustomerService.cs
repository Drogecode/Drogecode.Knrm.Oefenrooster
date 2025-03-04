using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CustomerService : DrogeService, ICustomerService
{
    public readonly IUserService _userService;
    public CustomerService(ILogger<CustomerService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService, IUserService userService) : base(logger, database, memoryCache, dateTimeService)
    {
        _userService = userService;
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

    public async Task<LinkUserToCustomerResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body)
    {
        var sw = Stopwatch.StartNew();
        var result = new LinkUserToCustomerResponse();
        var links = await Database.LinkUserCustomers.Where(x => x.UserId == body.UserId).ToListAsync();
        var mainUser = await Database.Users.FirstOrDefaultAsync(x => x.Id == body.UserId);
        if (mainUser is null)
        {
            return ResponseFailed();
        }
        if (links.Count == 0)
        {
            Database.LinkUserCustomers.Add(new DbLinkUserCustomer()
            {
                Id = Guid.CreateVersion7(),
                UserId = body.UserId,
                LinkUserId = body.UserId,
                CustomerId = mainUser.CustomerId,
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
        else if (body.CreateNew)
        {
            var drogeUser = new DrogeUser()
            {
                Id = Guid.CreateVersion7(),
                Name = mainUser.Name
            };
            var newUser = await _userService.AddUser(drogeUser, body.CustomerId);
            if (newUser.Success != true)
            {
                return ResponseFailed();
            }
            Database.LinkUserCustomers.Add(new DbLinkUserCustomer()
            {
                Id = Guid.CreateVersion7(),
                UserId = body.UserId,
                LinkUserId = drogeUser.Id,
                CustomerId = body.CustomerId,
                IsPrimary = false,
                IsActive = true,
                Order = links.OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? 0 + 10,
                LinkedBy = userId,
                LinkedOn = DateTime.UtcNow
            });
            result.NewUserId = drogeUser.Id;
        }
        else
        {
            return ResponseFailed();
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = await Database.SaveChangesAsync() > 0;
        return result;

        LinkUserToCustomerResponse ResponseFailed()
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            result.Success = false;
            return result;
        }
    }
}