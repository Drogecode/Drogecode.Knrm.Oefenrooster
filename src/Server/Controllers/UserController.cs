using Azure.Identity;
using Drogecode.Knrm.Oefenrooster.Server.Helpers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace Drogecode.Knrm.Oefenrooster.Server.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<DrogeUser> Get()
    {
        try
        {
            var settings = Settings.LoadSettings();
            InitializeGraph(settings);
            await DisplayAccessTokenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        var userId = new Guid(User?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? throw new Exception("No objectidentifier found"));
        var userName = User?.FindFirstValue("name") ?? throw new Exception("No userName found");
        var userEmail = User?.FindFirstValue("preferred_username") ?? throw new Exception("No userEmail found");
        var result = _userService.GetOrSetUserFromDb(userId, userName, userEmail);
        
        return result;
    }

    private void InitializeGraph(Settings settings)
    {
        GraphHelper.InitializeGraphForUserAuth(settings,
            (info, cancel) =>
            {
                // Display the device code message to
                // the user. This tells them
                // where to go to sign in and provides the
                // code to use.
                Console.WriteLine(info.Message);
                return Task.FromResult(0);
            });
    }
    private async Task DisplayAccessTokenAsync()
    {
        try
        {
            var userToken = await GraphHelper.GetUserTokenAsync();
            Console.WriteLine($"User token: {userToken}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user access token: {ex.Message}");
        }
    }
}
