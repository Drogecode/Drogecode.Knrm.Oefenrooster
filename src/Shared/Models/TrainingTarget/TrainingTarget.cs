using System.Text.Json.Serialization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingTarget
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlDescription { get; set; }
    public int Order { get; set; }
    public TrainingTargetType Type { get; set; }
    public TrainingTargetGroup Group { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    [JsonIgnore] public bool IsVisible { get; set; } = true;

    public List<TrainingTargetResult>? TargetResults { get; set; }

    [JsonIgnore]
    public string Icon
    {
        get
        {
            return Group switch
            {
                TrainingTargetGroup.Single => Icons.Material.Filled.Person,
                TrainingTargetGroup.TwoPersons => Icons.Material.Filled.People,
                TrainingTargetGroup.Group => Icons.Material.Filled.Groups,
                _ => Icons.Material.Filled.Error
            };
        }
    }

    [JsonIgnore]
    public Color IconColor
    {
        get
        {
            return Type switch
            {
                TrainingTargetType.Knowledge => Color.Secondary,
                TrainingTargetType.Exercise => Color.Primary,
                _ => Color.Error
            };
        }
    }
}