using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class EditTraining : TrainingBase
{
    public Guid? Id { get; set; }
    public Guid? DefaultId { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? TimeStart { get; set; }
    public TimeSpan? TimeEnd { get; set; }
    public bool IsNew { get; set; }
    public bool IsNewFromDefault { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsPinned { get; set; }
    public bool ShowTime { get; set; }
}
