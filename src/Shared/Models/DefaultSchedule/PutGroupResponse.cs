using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class PutGroupResponse : BaseResponse
{
    public DefaultGroup? Group { get; set; }
    public PutError Error { get; set; }
}
