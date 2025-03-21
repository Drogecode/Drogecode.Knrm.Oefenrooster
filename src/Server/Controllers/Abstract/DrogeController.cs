

using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers.Abstract;

[Controller]
public abstract class DrogeController : ControllerBase
{
    internal readonly ILogger Logger;

    protected DrogeController(ILogger logger)
    {
        Logger = logger;
    }

    internal string GetRequesterIp()
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
            Logger.LogError(ex, "Exception while GetRequesterIp");
            return "Exception";
        }
    }
    
}