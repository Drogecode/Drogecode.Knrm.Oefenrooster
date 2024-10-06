namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class IsEnabledChangedRequest
{
    public Guid UserLinkedMailId { get; set; }
    public bool IsEnabled { get; set; }
}