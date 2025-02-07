using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

public class MultipleReportActionsResponse : BaseMultipleResponse
{
    public List<DrogeAction>? Actions { get; set; }
}
