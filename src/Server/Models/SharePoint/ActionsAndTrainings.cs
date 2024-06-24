using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Server.Models.SharePoint;

public class ActionsAndTrainings
{
    public List<SharePointAction>? Actions { get; set; }
    public List<SharePointTraining>? Trainings { get; set; }
}