using Drogecode.Knrm.Oefenrooster.Shared.Models.Errors;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class PutDefaultScheduleResponse : BaseResponse
{
    public Guid? NewId { get; set; }
    public PutError Error { get; set; }

}