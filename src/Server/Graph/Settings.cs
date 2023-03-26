using Microsoft.Extensions.Configuration;
namespace Drogecode.Knrm.Oefenrooster.Server.Graph;

public class Settings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }

    public static Settings LoadSettings(IConfiguration configuration)
    {
        return configuration.GetRequiredSection("AzureAd").Get<Settings>() ??
            throw new Exception("Could not load app settings. See README for configuration instructions.");
    }
}