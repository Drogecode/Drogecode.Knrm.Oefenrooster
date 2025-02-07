namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

public sealed class GetMultipleDayItemResponse : BaseMultipleResponse
{
    public List<RoosterItemDay>? DayItems { get; set; }
}
