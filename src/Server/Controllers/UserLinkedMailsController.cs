using Drogecode.Knrm.Oefenrooster.Server.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "UserLinkedMails")]
public class UserLinkedMailsController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IAuditService _auditService;
    private readonly IGraphService _graphService;
    private readonly IFunctionService _functionService;
    private readonly RefreshHub _refreshHub;

    public UserLinkedMailsController(ILogger<UserController> logger, IUserService userService, IAuditService auditService, IGraphService graphService, IFunctionService functionService, RefreshHub refreshHub)
    {
        _logger = logger;
        _userService = userService;
        _auditService = auditService;
        _graphService = graphService;
        _functionService = functionService;
        _refreshHub = refreshHub;
    }
}