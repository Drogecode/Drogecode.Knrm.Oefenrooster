using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchTrainingResponse : BaseResponse
{
    [JsonIgnore] public bool ShouldUpdateOutlookEvent { get; set; }
}