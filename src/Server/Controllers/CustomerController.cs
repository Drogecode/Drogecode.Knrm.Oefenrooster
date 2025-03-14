﻿using System.Diagnostics;
using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
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
    [Route("linked/me/all")]
    public async Task<ActionResult<GetAllUserLinkCustomersResponse>> GetAllLinkUserCustomers()
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _customerService.GetAllLinkUserCustomers(userId, customerId);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllLinkUserCustomers");
            return BadRequest();
        }
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<GetAllCustomersResponse>> GetAllCustomers()
    {
        try
        {
            var response = await _customerService.GetAllCustomers();
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllCustomers");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("linked")]
    [Authorize(Roles = AccessesNames.AUTH_super_user)]
    public async Task<ActionResult<LinkUserToCustomerResponse>> LinkUserToCustomer([FromBody] LinkUserToCustomerRequest body)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var response = await _customerService.LinkUserToCustomer(userId, customerId, body);
            return response;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in GetAllLinkUserCustomers");
            return BadRequest();
        }
    }
}