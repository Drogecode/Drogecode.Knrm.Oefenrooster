using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static class KeyVaultHelper
{
    private static SecretClientOptions _options = new SecretClientOptions()
    {
        Retry =
        {
            Delay = TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
        }
    };

    private static SecretClient? _secretClient;

    public static string? KeyVaultUri { get; set; }

    public static KeyVaultSecret? GetSecret(string key, ILogger? logger = null)
    {
        try
        {
            if (KeyVaultUri is null) return null;
            _secretClient ??= new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential(), _options);
            KeyVaultSecret value = _secretClient.GetSecret(key);
            return value;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Get key vault secret failed");
            return null;
        }
    }
}