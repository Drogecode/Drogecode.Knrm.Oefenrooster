using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_super_user)]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[ApiExplorerSettings(GroupName = "Customer")]
public class CustomerController : DrogeController
{
    private readonly ICustomerService _customerService;

    public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService) : base(logger)
    {
        _customerService = customerService;
    }
    
    [HttpGet]
    [Route("all/{take:int}/{skip:int}")]
    public async Task<ActionResult<GetAllCustomersResponse>> GetAllCustomers(int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var response = await _customerService.GetAllCustomers(take, skip, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetAllCustomers");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("{customerId:guid}")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<GetCustomerResponse>> GetCustomerById(Guid customerId, CancellationToken clt = default)
    {
        try
        {
            var response = await _customerService.GetCustomerById(customerId, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in GetCustomerById");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<PutResponse>> PutNewCustomer([FromBody] Customer customer, CancellationToken clt = default)
    {
        try
        {
            var response = await _customerService.PutNewCustomer(customer, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PutNewCustomer");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<PatchResponse>> PatchCustomer([FromBody] Customer customer, CancellationToken clt = default)
    {
        try
        {
            var response = await _customerService.PatchCustomer(customer, clt);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PatchCustomer");
            return BadRequest();
        }
    }
}