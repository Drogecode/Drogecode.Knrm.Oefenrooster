﻿namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;

public class VersionDetailResponse : BaseResponse
{
    public bool NewVersionAvailable { get; set; }
    public string CurrentVersion { get; set; }
    public int UpdateVersion { get; set; }
    public int ButtonVersion { get; set; }
}
