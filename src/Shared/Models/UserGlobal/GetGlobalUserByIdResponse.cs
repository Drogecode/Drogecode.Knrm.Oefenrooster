namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

public class GetGlobalUserByIdResponse : BaseResponse
{
    public DrogeUserGlobal? GlobalUser { get; set; }
}