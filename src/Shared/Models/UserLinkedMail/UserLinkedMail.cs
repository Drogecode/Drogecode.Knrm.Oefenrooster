namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class UserLinkedMail
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public DateTime ActivateRequestedOn { get; set; }
    public int ActivationFailedAttempts { get; set; }
    public bool IsActive { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDrogeCodeUser { get; set; }
}