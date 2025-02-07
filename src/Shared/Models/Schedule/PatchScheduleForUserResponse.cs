namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchScheduleForUserResponse : BaseResponse
{
    public Training? PatchedTraining { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
}
