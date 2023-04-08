using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models.ExternalConnectors;

namespace Drogecode.Knrm.Oefenrooster.Server.Graph;

public class Settings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }

    public static Settings LoadSettings(IConfiguration configuration)
    {
        return new Settings
        {
            ClientId = configuration.GetValue<string>("AzureAd:ClientId"),
            ClientSecret = configuration.GetValue<string>("AzureAd:ClientSecret"),
            TenantId = configuration.GetValue<string>("AzureAd:TenantId")
        };
    }

    public static Settings LoadSettingsLikeThis()
    {
        // Load settings
        IConfiguration config = new ConfigurationBuilder()
            // appsettings.json is required
            .AddJsonFile("appsettings.json", optional: false)
            // appsettings.Development.json" is optional, values override appsettings.json
            .AddJsonFile($"appsettings.Development.json", optional: true)
            // User secrets are optional, values override both JSON files
            .AddUserSecrets<Program>()
            .Build();

        return config.GetRequiredSection("AzureAd").Get<Settings>() ??
            throw new Exception("Could not load app settings. See README for configuration instructions.");
    }
}