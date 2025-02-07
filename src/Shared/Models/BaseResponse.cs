namespace Drogecode.Knrm.Oefenrooster.Shared.Models;

public abstract class BaseResponse
{
    public bool Success { get; set; }
    public bool Offline { get; set; }
    public long ElapsedMilliseconds { get; set; } = -1;
}