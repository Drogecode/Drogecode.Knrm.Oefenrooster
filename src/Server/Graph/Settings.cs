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
}