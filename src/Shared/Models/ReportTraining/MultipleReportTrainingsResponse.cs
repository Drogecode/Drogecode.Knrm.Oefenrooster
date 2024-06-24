using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;

public class MultipleReportTrainingsResponse : BaseMultipleResponse
{
    public List<DrogeTraining>? Trainings { get; set; }
}
