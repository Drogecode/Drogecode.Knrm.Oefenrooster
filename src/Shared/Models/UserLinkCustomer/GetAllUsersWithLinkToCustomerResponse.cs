using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class GetAllUsersWithLinkToCustomerResponse : BaseMultipleResponse
{
    public List<LinkUserCustomerInfo>? LinkInfo { get; set; }
}

public class LinkUserCustomerInfo
{
    public DrogeUser? DrogeUserOther { get; set; }
    public DrogeUserGlobal? UserGlobal { get; set; }
}