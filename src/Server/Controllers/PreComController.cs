using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Extensions;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize(Roles = AccessesNames.AUTH_basic_access)]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "PreCom")]
public class PreComController : DrogeController
{
    private readonly IPreComService _preComService;
    private readonly PreComHub _preComHub;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuditService _auditService;

    private readonly Random _random = new();

    public PreComController(
        ILogger<PreComController> logger,
        IPreComService preComService,
        PreComHub preComHub,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IDateTimeProvider dateTimeProvider,
        IAuditService auditService) : base(logger)
    {
        _preComService = preComService;
        _preComHub = preComHub;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
        _auditService = auditService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("webhook/{customerId:guid}/{userId:guid}")]
    [Route("web-hook/{customerId:guid}/{userId:guid}")]
    public async Task<IActionResult> WebHook(Guid customerId, Guid userId, [FromBody] object? body = null, bool testMode = false, CancellationToken clt = default)
    {
        try
        {
            Logger.LogInformation("received PreCom message");
            var ip = GetRequesterIp();
            try
            {
                if (!testMode)
                {
                    // Sleep to prevent all webhooks to run on the same time.
                    var sleepFor = _random.Next(0, 1500);
                    await Task.Delay(sleepFor, clt);
                }

                var alert = _preComService.AnalyzeAlert(userId, customerId, body, ip, out var timestamp, out var priority);
                if (alert.Equals(CacheHelper.FOUND_IN_CACHE))
                {
                    // Found in cache, do not save to prevent duplicate
                    Logger.LogInformation("Found in cache, do not save to prevent duplicate");
                    return Ok();
                }

                await _preComService.WriteAlertToDb(userId, customerId, timestamp, alert, priority, JsonSerializer.Serialize(body), ip);

                if (!testMode)
                {
                    await _preComHub.SendMessage(userId, "PreCom", alert);
                    var forwards = await _preComService.GetAllForwards(30, 0, userId, customerId, clt);
                    if (forwards?.PreComForwards?.Any() == true)
                    {
                        using var client = _clientFactory.CreateClient();
                        foreach (var forward in forwards.PreComForwards)
                        {
                            try
                            {
                                if (Uri.IsWellFormedUriString(forward.ForwardUrl, UriKind.Absolute))
                                {
                                    await client.PostAsJsonAsync(forward.ForwardUrl, body, clt);
                                    Logger.LogInformation("Forwarded request to `{Uri}`", forward.ForwardUrl.Replace(Environment.NewLine, ""));
                                }
                                else
                                {
                                    Logger.LogWarning("Forward uri `{Uri}` not correct formatted", forward.ForwardUrl?.Replace(Environment.NewLine, ""));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "Error in PreComController WebHook forward `{testMode}`", testMode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in PreComController WebHook `{testMode}`", testMode);
                await _preComService.WriteAlertToDb(userId, customerId, DateTime.UtcNow, ex.Message, -1, body is null ? "body is null" : JsonSerializer.Serialize(body), ip);
                if (!testMode)
                    await _preComHub.SendMessage(userId, "PreCom", "piep piep");
            }

            return Ok();
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in PreCom WebHook");
            return BadRequest("Exception in PreCom WebHook");
        }
    }

    [HttpGet]
    [Route("{take:int}/{skip:int}")]
    [Authorize(Roles = $"{nameof(Licenses.L_PreCom)}")]
    public async Task<ActionResult<MultiplePreComAlertsResponse>> AllAlerts(int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var includeRaw = User.IsInRole(AccessesNames.AUTH_super_user);
            var result = await _preComService.GetAllAlerts(userId, customerId, take, skip, includeRaw, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in AllAlerts");
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("forwards")]
    [Authorize(Roles = $"{nameof(Licenses.L_PreCom)}")]
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
            Logger.LogError(ex, "Exception in PutItem");
            return BadRequest();
        }
    }

    [HttpPatch]
    [Route("forwards")]
    [Authorize(Roles = $"{nameof(Licenses.L_PreCom)}")]
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
            Logger.LogError(ex, "Exception in PutItem");
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("forwards/{take:int}/{skip:int}")]
    [Authorize(Roles = nameof(Licenses.L_PreCom))]
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
            Logger.LogError(ex, "Exception in AllForwards");
            return BadRequest();
        }
    }


    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_precom_manual)]
    [Authorize(Roles = nameof(Licenses.L_PreCom))]
    [Route("forwards/{userId:guid}/{take:int}/{skip:int}")]
    public async Task<ActionResult<MultiplePreComForwardsResponse>> AllForwardsForUser(Guid userId, int take, int skip, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var result = await _preComService.GetAllForwards(take, skip, userId, customerId, clt);
            return result;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in AllForwards");
            return BadRequest();
        }
    }

    [HttpPost]
    [Authorize(Roles = AccessesNames.AUTH_precom_manual)]
    [Authorize(Roles = nameof(Licenses.L_PreCom))]
    [Route("forward")]
    public async Task<ActionResult<bool>> PostForward([FromBody] PostForwardRequest body, CancellationToken clt = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(body.Message))
            {
                return false;
            }

            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            using var client = _clientFactory.CreateClient();
            var forward = await _preComService.GetForward(body.ForwardId, customerId, clt);
            if (forward == null)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var baseMessage =
                "{\"android_channel_id\":\"chirp\",\"content- available\":\"1\",\"message\":\"{message}\",\"messageData\":{\"MsgOutID\":\"135615552\",\"ControlID\":\"f\",\"Timestamp\":\"{datetime}\",\"notId\":\"135615552\",\"soundname\":\"chirp\",\"vibrationPattern\":\"[150,545]\",\"from\":\"788942585741\",\"messageId\":\"0:1694527951397184%af1e7638f9fd7ecd\",\"sentTime\":\"{sendtime}\",\"ttl\":2419200}}";
            var message = baseMessage
                .Replace("{message}", body.Message)
                .Replace("{datetime}", now.ToString("o")) // 2024-03-01T20:46:08.2 
                .Replace("{sendtime}", now.ConvertToTimestamp().ToString()); // 1709322368215
            var asObject = JsonSerializer.Deserialize<object>(message);
            await client.PostAsJsonAsync(forward.ForwardUrl, asObject, clt);
            return true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debugger.Break();
#endif
            Logger.LogError(ex, "Exception in AllForwards");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize(Roles = AccessesNames.AUTH_precom_problems)]
    [Authorize(Roles = nameof(Licenses.L_PreCom))]
    [Route("problems")]
    public async Task<ActionResult<GetProblemsResponse>> GetProblems(NextRunMode nextRunMode, CancellationToken clt = default)
    {
        try
        {
            var customerId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? throw new DrogeCodeNullException("customerId not found"));
            var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new DrogeCodeNullException("No object identifier found"));
            await _auditService.Log(userId, AuditType.PreComProblems, customerId, nextRunMode.ToString());
            // ReSharper disable once RedundantAssignment
            var whatsAppBearer = _configuration.GetValue<string>("WhatsApp:Bearer");

            var preComClient = await _preComService.GetPreComClient();
            if (preComClient is null)
                return BadRequest();

            if (false && string.IsNullOrWhiteSpace(whatsAppBearer))
            {
                whatsAppBearer = KeyVaultHelper.GetSecret("WhatsAppBearer", Logger)?.Value;
            }

            var preComWorker = new FutureProblems(preComClient, Logger, _dateTimeProvider);
            var problems = await preComWorker.Work(nextRunMode);
            if (false && !string.IsNullOrWhiteSpace(whatsAppBearer) && problems.Problems is not null)
            {
                using var client = _clientFactory.CreateClient();
                var whatsAppClient = new WhatsAppClient(client, whatsAppBearer, Logger);
                await whatsAppClient.SendMessage("", problems.Problems);
            }

            return problems;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in GetProblems");
#if DEBUG
            Debugger.Break();
            return new GetProblemsResponse() { Problems = ex.Message };
#endif
            return BadRequest();
        }
    }

    [HttpDelete]
    [Authorize(Roles = AccessesNames.AUTH_configure_global_all)]
    [Authorize(Roles = nameof(Licenses.L_PreCom))]
    [Route("duplicates")]
    public async Task<ActionResult<DeleteResponse>> DeleteDuplicates(CancellationToken clt = default)
    {
        try
        {
            var result = await _preComService.DeleteDuplicates(clt);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in DeleteDuplicates");
#if DEBUG
            Debugger.Break();
#endif
            return BadRequest();
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