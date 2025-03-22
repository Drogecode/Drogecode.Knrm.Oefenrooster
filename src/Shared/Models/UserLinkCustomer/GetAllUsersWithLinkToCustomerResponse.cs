using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class GetAllUsersWithLinkToCustomerResponse : BaseMultipleResponse
{
    public List<LinkUserCustomerInfo>? LinkInfo { get; set; }
}

public class LinkUserCustomerInfo
{
    public DrogeUser? DrogeUserCurrent { get; set; }
    public DrogeUser? DrogeUserOther { get; set; }
}