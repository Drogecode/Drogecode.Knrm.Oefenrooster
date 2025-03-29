namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public class SwitchUserRequest
{
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
}