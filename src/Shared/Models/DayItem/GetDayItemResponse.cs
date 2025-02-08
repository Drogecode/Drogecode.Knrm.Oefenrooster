namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

public sealed class GetDayItemResponse : BaseResponse
{
    public RoosterItemDay? DayItem { get; set; }
}
