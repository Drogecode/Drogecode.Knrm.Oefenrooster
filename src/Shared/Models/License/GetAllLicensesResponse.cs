namespace Drogecode.Knrm.Oefenrooster.Shared.Models.License;

public class GetAllLicensesResponse : BaseMultipleResponse
{
    public List<DrogeLicense>? Licenses { get; set; }
}