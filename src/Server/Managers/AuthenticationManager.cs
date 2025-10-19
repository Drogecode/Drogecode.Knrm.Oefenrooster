using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract;
using Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;
using Drogecode.Knrm.Oefenrooster.Server.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers;

public class AuthenticationManager : DrogeManager, IAuthenticationManager
{
    
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly DataContext _database;
    private IAuthenticationService? _authService;

    public AuthenticationManager(
        ILogger<AuthenticationManager> logger,
        IMemoryCache memoryCache,
        IConfiguration configuration,
        HttpClient httpClient,
        DataContext database) : base(logger)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
        _httpClient = httpClient;
        _database = database;
    }

    public IAuthenticationService GetAuthenticationService()
    {
        if (_authService is not null)
            return _authService;
        var identityProvider = _configuration.GetValue<IdentityProvider>("IdentityProvider");
        switch (identityProvider)
        {
            case IdentityProvider.Azure:
                _authService = new AuthenticationAzureService(Logger, _memoryCache, _configuration, _httpClient, _database);
                break;
            case IdentityProvider.KeyCloak:
                _authService = new AuthenticationKeyCloakService(Logger, _memoryCache, _configuration, _httpClient, _database);
                break;
            case IdentityProvider.None:
            default:
                throw new ArgumentOutOfRangeException($"identityProvider: `{identityProvider}` is not supported");
        }

        return _authService;
    }
}