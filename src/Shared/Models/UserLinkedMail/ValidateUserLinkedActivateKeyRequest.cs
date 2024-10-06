namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class ValidateUserLinkedActivateKeyRequest
{
    public Guid UserLinkedMailId { get; set; }
    public string ActivationKey { get; set; } = string.Empty;
}