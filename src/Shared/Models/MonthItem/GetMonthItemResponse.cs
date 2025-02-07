namespace Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

public sealed class GetMonthItemResponse : BaseResponse
{
    public RoosterItemMonth? MonthItem { get; set; }
}
