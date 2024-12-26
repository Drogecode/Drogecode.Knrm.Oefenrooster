using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class GetLoginSecretsResponse : BaseResponse
{
    public IdentityProvider IdentityProvider { get; set; }
    public string? Instance { get; set; }
    public string? LoginSecret { get; set; }
    public string? LoginNonce { get; set; }
    public string? CodeChallenge { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
}

public class CacheLoginSecrets : GetLoginSecretsResponse
{
    public string? CodeVerifier { get; set; }
}
