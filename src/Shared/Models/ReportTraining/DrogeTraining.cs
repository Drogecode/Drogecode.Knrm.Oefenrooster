﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

public class DrogeTraining : SharePointListBase
{
    public string? Type { get; set; }
    public string? TypeTraining { get; set; }
}
