namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class ValidateUserLinkedActivateKeyResponse : BaseResponse
{
    public ValidateUserLinkedActivateKeyError Error { get; set; }
}

public enum ValidateUserLinkedActivateKeyError
{
    None = 0,
    TooManyTries = 1
}