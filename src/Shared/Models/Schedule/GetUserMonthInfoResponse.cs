namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class GetUserMonthInfoResponse : BaseMultipleResponse
{
    public List<UserMonthInfo> UserMonthInfo { get; set; } = new List<UserMonthInfo>();
}