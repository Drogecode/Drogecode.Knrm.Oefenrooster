﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

public class MultipleReportTrainingsResponse : BaseMultipleResponse
{
    public List<DrogeTraining>? Trainings { get; set; }
}
