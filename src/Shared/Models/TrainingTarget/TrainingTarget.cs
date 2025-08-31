using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingTarget
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public int Order { get; set; }
    public TrainingTargetType Type { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    
    public List<TrainingTargetResult>? TargetResults { get; set; }
}