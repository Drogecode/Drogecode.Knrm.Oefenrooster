namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class PutUserLinkedMailResponse : BaseResponse
{
    public Guid? NewId { get; set; }
    public PutUserLinkedMailError Error { get; set; }
    public string? ActivateKey { get; set; }
}

public enum PutUserLinkedMailError
{
    None = 0,
    MailAlreadyExists = 1,
    TooMany = 2
}