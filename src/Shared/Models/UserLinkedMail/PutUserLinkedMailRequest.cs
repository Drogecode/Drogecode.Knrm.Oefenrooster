namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkedMail;

public class PutUserLinkedMailRequest
{
    public UserLinkedMail? UserLinkedMail { get; set; }
    public bool SendMail { get; set; }
}