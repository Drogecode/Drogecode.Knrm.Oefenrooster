namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingTargetSet
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<Guid> TrainingTargetIds { get; set; } = [];
    public bool IsActive { get; set; }
    public bool IsReusable { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
}