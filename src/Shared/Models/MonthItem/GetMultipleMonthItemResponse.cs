namespace Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

public sealed class GetMultipleMonthItemResponse : BaseMultipleResponse
{
    public List<RoosterItemMonth>? MonthItems { get; set; }
}
