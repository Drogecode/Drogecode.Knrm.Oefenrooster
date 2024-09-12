namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

public class GetAllDefaultScheduleResponse : BaseMultipleResponse
{
    public List<DefaultSchedule>? DefaultSchedules { get; set; }
}