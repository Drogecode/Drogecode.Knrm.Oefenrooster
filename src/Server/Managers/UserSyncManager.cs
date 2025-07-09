using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Server.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Microsoft.Graph.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers;

public class UserSyncManager : DrogeManager, IUserSyncManager
{
    private readonly ILogger<UserSyncManager> _logger;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly ILinkUserRoleService _linkUserRoleService;
    private readonly IUserLinkCustomerService _userLinkCustomerService;
    private readonly IUserGlobalService _userGlobalService;
    private readonly IGraphService _graphService;
    private readonly IFunctionService _functionService;
    private readonly ICustomerService _customerService;

    public UserSyncManager(ILogger<UserSyncManager> logger, IUserService userService, IUserRoleService userRoleService, ILinkUserRoleService linkUserRoleService,
        IUserLinkCustomerService userLinkCustomerService,
        IUserGlobalService userGlobalService, IGraphService graphService, IFunctionService functionService, ICustomerService customerService)
    {
        _logger = logger;
        _userService = userService;
        _userRoleService = userRoleService;
        _linkUserRoleService = linkUserRoleService;
        _userLinkCustomerService = userLinkCustomerService;
        _userGlobalService = userGlobalService;
        _graphService = graphService;
        _functionService = functionService;
        _customerService = customerService;
    }

    public async Task<SyncAllUsersResponse> SyncAllUsers(Guid userId, Guid customerId, CancellationToken clt)
    {
        _graphService.InitializeGraph();
        var customer = await _customerService.GetCustomerById(customerId, clt);
        var existingUsers = (await _userService.GetAllUsers(customerId, true, false, clt)).DrogeUsers ?? [];
        var functions = (await _functionService.GetAllFunctions(customerId, clt)).Functions ?? [];
        var users = await _graphService.ListUsersAsync(customer.Customer?.GroupId);
        var customersInTenant = await _customerService.GetByTenantId(customer.Customer?.TenantId, clt);

        if (users?.Value is not null)
        {
            while (true)
            {
                foreach (var user in users!.Value!)
                {
                    if (string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(user.DisplayName))
                    {
                        continue;
                    }

                    var groups = await _graphService.GetGroupForUser(user.Id);
                    if (!ValidateCustomerGroup(customer, groups, user))
                    {
                        continue;
                    }

                    var newUserResponse = await _userService.GetOrSetUserById(null, user.Id, user.DisplayName, user.Mail ?? "not set", customerId, false, clt);
                    if (newUserResponse is null)
                    {
                        continue;
                    }

                    if (!newUserResponse.IsNew)
                    {
                        var index = existingUsers.FindIndex(x => x.Id == newUserResponse.Id);
                        if (index != -1)
                            existingUsers.RemoveAt(index);
                    }

                    newUserResponse.SyncedFromSharePoint = true;
                    UpdateUserFunctionAndRole(groups, functions, newUserResponse);

                    await LinkUserToGlobalUser(customerId, newUserResponse, groups, customersInTenant, clt);
                    await LinkUserToRoles(customerId, newUserResponse, groups, clt);
                    await _userService.UpdateUser(newUserResponse, userId, customerId);
                }

                if (users.OdataNextLink is not null)
                {
                    users = await _graphService.NextUsersPage(users);
                }
                else
                {
                    break;
                }
            }

            if (existingUsers?.Count > 0)
            {
                await _userService.MarkMultipleUsersDeleted(existingUsers, userId, customerId, true);
            }
        }
        else
            return new SyncAllUsersResponse { Success = false };

        return new SyncAllUsersResponse { Success = true };
    }

    private bool ValidateCustomerGroup(GetCustomerResponse customer, DirectoryObjectCollectionResponse? groups, User user)
    {
        if (customer.Customer?.GroupId is not null)
        {
            if (groups?.Value is not null && groups.Value.All(x => x.Id != customer.Customer.GroupId))
            {
                _logger.LogInformation("User {userId} is not in group {CustomerGroupId}", user.Id, customer.Customer.GroupId);
                return false;
            }
        }

        return true;
    }

    private static void UpdateUserFunctionAndRole(DirectoryObjectCollectionResponse? groups, List<DrogeFunction> functions, DrogeUserServer newUserResponse)
    {
        if (groups?.Value is not null && functions.Any(x => groups.Value.Any(y => y.Id == x.RoleId.ToString())))
        {
            newUserResponse.RoleFromSharePoint = true;
            var newFunction = functions.FirstOrDefault(x => groups.Value.Any(y => y.Id == x.RoleId.ToString()));
            if (newFunction is not null && newUserResponse.UserFunctionId != newFunction.Id)
            {
                newUserResponse.UserFunctionId = newFunction.Id;
            }
        }
        else
        {
            newUserResponse.RoleFromSharePoint = false;
        }
    }

    private async Task LinkUserToGlobalUser(Guid customerId, DrogeUserServer newUserResponse, DirectoryObjectCollectionResponse? groups, List<CustomerAuthentication> customersInTenant,
        CancellationToken clt)
    {
        var allLinkedCustomers = await _userLinkCustomerService.GetAllLinkUserCustomers(newUserResponse.Id, customerId, clt);
        if (groups?.Value is not null && customersInTenant.Any(x => x.Id != customerId && groups.Value.Any(y => y.Id == x.GroupId?.ToString())))
        {
            var globalUser = await _userGlobalService.GetOrCreateGlobalUserByExternalId(newUserResponse, clt);
            foreach (var customerInTenant in customersInTenant.Where(x => groups.Value.Any(y => y.Id == x.GroupId?.ToString())))
            {
                if (allLinkedCustomers.UserLinkedCustomers?.Any(x => x.CustomerId == customerInTenant.Id && x.SetBySync) == true)
                {
                    allLinkedCustomers.UserLinkedCustomers.Remove(allLinkedCustomers.UserLinkedCustomers.First(x => x.CustomerId == customerInTenant.Id && x.SetBySync));
                    continue;
                }

                if (allLinkedCustomers.UserLinkedCustomers?.Any(x => x.CustomerId == customerInTenant.Id) == true)
                {
                    var link = allLinkedCustomers.UserLinkedCustomers.First(x => x.CustomerId == customerInTenant.Id);
                    allLinkedCustomers.UserLinkedCustomers.Remove(link);
                    await _userLinkCustomerService.LinkUserToCustomer(newUserResponse.Id, new LinkUserToCustomerRequest
                    {
                        CustomerId = customerInTenant.Id,
                        UserId = newUserResponse.Id,
                        GlobalUserId = globalUser.Id,
                        IsActive = true,
                        SetBySync = true
                    }, clt);
                }
            }

            if (allLinkedCustomers.UserLinkedCustomers?.Any(x => x.SetBySync) != true)
            {
                return;
            }
            foreach (var oldLink in allLinkedCustomers.UserLinkedCustomers.Where(x => x.SetBySync))
            {
                await _userLinkCustomerService.LinkUserToCustomer(newUserResponse.Id, new LinkUserToCustomerRequest
                {
                    CustomerId = oldLink.CustomerId,
                    UserId = oldLink.UserId,
                    GlobalUserId = oldLink.GlobalUserId,
                    IsActive = false,
                    SetBySync = false
                }, clt);
            }
        }
    }

    private async Task LinkUserToRoles(Guid customerId, DrogeUserServer newUserResponse, DirectoryObjectCollectionResponse? groups, CancellationToken clt)
    {
        var linkedRoles = await _linkUserRoleService.GetLinkUserRolesAsync(newUserResponse.Id, clt);
        var roles = await _userRoleService.GetAll(customerId, clt);
        if (groups?.Value is not null && roles.Roles is not null && roles.Roles.Count != 0)
        {
            foreach (var group in groups.Value.Where(x => roles.Roles.Any(y => y.ExternalId?.ToString().Equals(x.Id) == true)))
            {
                if (group.Id is null) continue;
                var role = roles.Roles.FirstOrDefault(x => x.ExternalId?.Equals(group.Id) == true);
                if (role is null) continue;
                linkedRoles.Remove(role.Id);
                await _linkUserRoleService.LinkUserToRoleAsync(newUserResponse.Id, role, true, true, clt);
            }

            foreach (var linkedRole in linkedRoles)
            {
                var role = roles.Roles.FirstOrDefault(x => x.Id.Equals(linkedRole));
                if (role is null) continue;
                await _linkUserRoleService.LinkUserToRoleAsync(newUserResponse.Id, role, false, true, clt);
            }
        }
    }
}