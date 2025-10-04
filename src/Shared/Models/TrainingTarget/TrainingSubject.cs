using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingSubject
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    [JsonIgnore] public bool IsVisible { get; set; } = true;
    
    public List<TrainingTarget>? TrainingTargets { get; set; }
    public List<TrainingSubject>? TrainingSubjects { get; set; }
}