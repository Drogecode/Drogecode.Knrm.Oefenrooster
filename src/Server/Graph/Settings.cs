using Drogecode.Knrm.Oefenrooster.Server.Helpers;
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
            ClientSecret = InternalGetClientSecret(configuration),
            TenantId = configuration.GetValue<string>("AzureAd:TenantId")
        };
    }

    private static string InternalGetClientSecret(IConfiguration configuration)
    {
        var fromKeyVault = KeyVaultHelper.GetSecret("ClientSecret");
        if (fromKeyVault is not null) return fromKeyVault.Value;
        return configuration.GetValue<string>("AzureAd:ClientSecret") ?? throw new DrogeCodeConfigurationException("no secret found for azure login");
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