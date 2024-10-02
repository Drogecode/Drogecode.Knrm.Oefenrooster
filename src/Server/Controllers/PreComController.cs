using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Server.Helpers.JsonConverters;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "PreCom")]
public class PreComController : ControllerBase
{
    private readonly ILogger<PreComController> _logger;
    private readonly IPreComService _preComService;
    private readonly PreComHub _preComHub;
    private readonly IHttpClientFactory _clientFactory;

    public PreComController(
        ILogger<PreComController> logger,
        IPreComService preComService,
        PreComHub preComHub,
        IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _preComService = preComService;
        _preComHub = preComHub;
        _clientFactory = clientFactory;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("web-hook/{customerId:guid}/{userId:guid}")]
    public async Task<IActionResult> WebHook(Guid customerId, Guid userId, [FromBody] object body, bool sendToHub = true, CancellationToken clt = default)
    {
        try
        {
            _logger.LogInformation("received PreCom message");
            var ip = GetRequesterIp();
            try
            {
                var alert = _preComService.AnalyzeAlert(userId, customerId, body, out DateTime timestamp, out int? priority);
                _preComService.WriteAlertToDb(userId, customerId, timestamp, alert, priority, JsonSerializer.Serialize(body), ip);

                if (sendToHub)
                {
                    await _preComHub.SendMessage(userId, "PreCom", alert);
                    var forwards = await _preComService.GetAllForwards(30, 0, userId, customerId, clt);
                    if (forwards?.PreComForwards?.Any() == true)
                    {
                        var client = _clientFactory.CreateClient();
                        foreach (var forward in forwards.PreComForwards)
                        {
                            try
                            {
                                if (Uri.IsWellFormedUriString(forward.ForwardUrl, UriKind.Absolute))
                                {
                                    await client.PostAsJsonAsync(forward.ForwardUrl, body, clt);
                                    _logger.LogInformation("Forwarded request to `{Uri}`", forward.ForwardUrl.Replace(Environment.NewLine, ""));
                                }
                                else
                                {
                                    _logger.LogWarning("Forward uri `{Uri}` not correct formatted", forward.ForwardUrl?.Replace(Environment.NewLine, ""));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error in PreComController WebHook forward `{sendToHub}`", sendToHub);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PreComController WebHook `{sendToHub}`", sendToHub);
                _preComService.WriteAlertToDb(userId, customerId, DateTime.UtcNow, ex.Message, -1, body is null ? "body is null" : JsonSerializer.Serialize(body), ip);
                if (sendToHub)
                    await _preComHub.SendMessage(userId, "PreCom", "piep piep");
            }

            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PreCom WebHook");
            return BadRequest("Exception in PreCom WebHook");
        }
    }

    [HttpGet]
    [Route("{take:int}/{skip:int}")]
    public async Task<ActionResult<MultiplePreComAlertsResponse>> AllAlerts(int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _preComService.GetAllAlerts(userId, customerId, take, skip, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in AllAlerts");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("forwards")]
    public async Task<ActionResult<PutPreComForwardResponse>> PutForward([FromBody] PreComForward forward, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _preComService.PutForward(forward, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutItem");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("forwards")]
    public async Task<ActionResult<PatchPreComForwardResponse>> PatchForward([FromBody] PreComForward forward, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var result = await _preComService.PatchForward(forward, customerId, userId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in PutItem");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("forwards/{take:int}/{skip:int}")]
    public async Task<ActionResult<MultiplePreComForwardsResponse>> AllForwards(int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _preComService.GetAllForwards(take, skip, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in AllForwards");
            return BadRequest();
        }
    }

    private string GetRequesterIp()
    {
        try
        {
            var ip = Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (!string.IsNullOrWhiteSpace(ip)) return "xf;" + ip;
            ip = Request.HttpContext.GetServerVariable("REMOTE_HOST");
            if (!string.IsNullOrWhiteSpace(ip)) return "rh;" + ip;
            ip = Request.HttpContext.GetServerVariable("REMOTE_ADDR");
            if (!string.IsNullOrWhiteSpace(ip)) return "ra;" + ip;
            ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(ip)) return "ri;" + ip;
            return "No ip";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while GetRequesterIp");
            return "Exception";
        }
    }

    /*[Route("{**catchAll}")]
    [AllowAnonymous]
    [HttpPost("post", Order = int.MaxValue)]
    public IActionResult Post([FromBody] object value, string? catchAll)
    {
        try
        {
            _auditService.Log(DefaultSettingsHelper.IdTaco, AuditType.CatchAll, DefaultSettingsHelper.KnrmHuizenId, $"{JsonSerializer.Serialize(value)} : {catchAll}", objectName: "POST catch all");
            return Ok($"Got it {JsonSerializer.Serialize(value)} : {catchAll}");
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            _logger.LogError(ex, "Exception in CatchallController CatchAll POST");
            return BadRequest("Exception in CatchallController CatchAll POST");
        }
    }*/
}