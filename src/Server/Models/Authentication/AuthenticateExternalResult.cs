namespace Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;

public class AuthenticateExternalResult : BaseResponse
{
    public Guid CustomerId { get; set; }
}