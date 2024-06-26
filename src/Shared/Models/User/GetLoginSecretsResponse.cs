﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class GetLoginSecretsResponse : BaseResponse
{
    public string? LoginSecret { get; set; }
    public string? LoginNonce { get; set; }
    public string? CodeChallenge { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClientId { get; set; }
}

public class CacheLoginSecrets : GetLoginSecretsResponse
{
    public string? CodeVerifier { get; set; }
}
