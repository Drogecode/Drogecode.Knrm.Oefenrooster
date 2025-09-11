using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingTargetResult
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TrainingTargetId { get; set; }
    public Guid RoosterAvailableId { get; set; }
    public int Result { get; set; }
    public DateTime? TrainingDate { get; set; }
    public DateTime? ResultDate { get; set; }
    public Guid SetBy { get; set; }
    [JsonIgnore] public bool IsUpdating { get; set; }
}