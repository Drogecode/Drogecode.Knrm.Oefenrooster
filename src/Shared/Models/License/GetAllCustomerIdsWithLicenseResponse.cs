namespace Drogecode.Knrm.Oefenrooster.Shared.Models.License;

public class GetAllCustomerIdsWithLicenseResponse : BaseMultipleResponse
{
    public List<Guid>? CustomerIds { get; set; }
}