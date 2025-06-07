using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class EditTraining : TrainingMiddle
{
    public Guid? Id { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? TimeStart { get; set; }
    public TimeSpan? TimeEnd { get; set; }
    public bool IsNew { get; set; }
    public bool IsNewFromDefault { get; set; }
}
