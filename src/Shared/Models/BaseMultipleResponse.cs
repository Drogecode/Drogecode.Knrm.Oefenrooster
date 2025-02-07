namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public abstract class BaseMultipleResponse : BaseResponse
{
    public int TotalCount { get; set; } = -1;
}