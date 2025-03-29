using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class UserLinkCustomerService : DrogeService, IUserLinkCustomerService
{
    public readonly IUserService _userService;

    public UserLinkCustomerService(ILogger<CustomerService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService, IUserService userService) : base(logger, database,
        memoryCache, dateTimeService)
    {
        _userService = userService;
    }

    public async Task<GetAllUsersWithLinkToCustomerResponse> GetAllUsersWithLinkToCustomer(Guid currentCustomerId, Guid linkedCustomerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetAllUsersWithLinkToCustomerResponse();
        var links = await Database.LinkUserCustomers
            .Include(x => x.Customer)
            .Include(x => x.User)
            .Include(x => x.LinkedUser)
            .Where(x => x.CustomerId == linkedCustomerId && x.IsActive && x.User.CustomerId == linkedCustomerId)
            .ToListAsync(clt);
        result.LinkInfo = [];
        foreach (var link in links)
        {
            result.LinkInfo.Add(new LinkUserCustomerInfo()
            {
                DrogeUser = link.User.ToSharedUser(false, false),
                UserGlobal = link.LinkedUser.ToDrogeUserGlobal()
            });
        }

        result.TotalCount = links.Count;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetAllUserLinkCustomersResponse> GetAllLinkUserCustomers(Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetAllUserLinkCustomersResponse();
        var globalUsers = await Database.LinkUserCustomers
            .Where(x => x.UserId == userId && x.IsActive)
            .Select(x => x.GlobalUserId)
            .ToListAsync(clt);
        if (globalUsers.Count != 0)
        {
            if (globalUsers.Count > 1)
            {
                Logger.LogWarning("Multiple global users found for `{userId}` in `{customerId}`", userId, customerId);
            }

            var links = await Database.LinkUserCustomers
                .Include(x => x.Customer)
                .Where(x => x.GlobalUserId == globalUsers.FirstOrDefault() && x.IsActive)
                .Select(x => x.ToLinkedCustomer(customerId))
                .ToListAsync(clt);
            result.UserLinkedCustomers = links;
            result.CurrentCustomerId = customerId;
            result.TotalCount = links.Count;
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<LinkUserToCustomerResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new LinkUserToCustomerResponse();
        var links = await Database.LinkUserCustomers.Where(x => x.GlobalUserId == body.GlobalUserId).ToListAsync(clt);
        var globalUser = await Database.UsersGlobal.FirstOrDefaultAsync(x => x.Id == body.GlobalUserId, clt);
        if (globalUser is null)
        {
            return ResponseFailed();
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
            if (body.UserId is null && body.CreateNew)
            {
                var drogeUser = new DrogeUser
                {
                    Id = Guid.CreateVersion7(),
                    Name = globalUser.Name
                };
                var newUser = await _userService.AddUser(drogeUser, body.CustomerId);
                if (newUser.Success != true)
                {
                    return ResponseFailed();
                }

                body.UserId = drogeUser.Id;
                result.NewUserId = drogeUser.Id;
            }
            else if (body.UserId is null)
            {
                return ResponseFailed();
            }

            Database.LinkUserCustomers.Add(new DbLinkUserCustomer()
            {
                Id = Guid.CreateVersion7(),
                UserId = body.UserId.Value,
                GlobalUserId = body.GlobalUserId,
                CustomerId = body.CustomerId,
                IsPrimary = false,
                IsActive = true,
                Order = links.OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? 0 + 10,
                LinkedBy = userId,
                LinkedOn = DateTime.UtcNow
            });
        }

        result.Success = await Database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
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