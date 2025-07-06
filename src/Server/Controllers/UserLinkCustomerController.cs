using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "LinkedCustomer")]
public class UserLinkCustomerController : DrogeController
{
    private readonly IUserLinkCustomerService _userLinkCustomerService;

    public UserLinkCustomerController(ILogger<CustomerController> logger, ICustomerService customerService, IUserLinkCustomerService userLinkCustomerService) : base(logger)
    {
        _userLinkCustomerService = userLinkCustomerService;
    }

    [HttpGet]
    [Route("all/users/{linkedCustomerId:guid}")]
    public async Task<ActionResult<GetAllUsersWithLinkToCustomerResponse>> GetAllUsersWithLinkToCustomer(Guid linkedCustomerId, CancellationToken clt = default)
    {
        try
        {
            var currentCustomerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var response = await _userLinkCustomerService.GetAllUsersWithLinkToCustomer(currentCustomerId, linkedCustomerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllUsersWithLinkToCustomer");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("all/me")]
    public async Task<ActionResult<GetAllUserLinkCustomersResponse>> GetAllCustomersLinkedToMe(CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _userLinkCustomerService.GetAllLinkUserCustomers(userId, customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllLinkUserCustomers");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<LinkUserToCustomerResponse>> LinkUserToCustomer([FromBody] LinkUserToCustomerRequest body, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            body.SetBySync = false;
            body.SetBySync = false;
            var response = await _userLinkCustomerService.LinkUserToCustomer(userId, customerId, body, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllLinkUserCustomers");
            return BadRequest();
        }
    }
}