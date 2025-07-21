using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class LicensesMapper
{
    public static DrogeLicense ToLicense(this DbLicenses dbLicenses)
    {
        return new DrogeLicense
        {
            License = dbLicenses.License,
        };
    }

    public static DbLicenses ToDb(this DrogeLicense drogeLicense, Guid customerId)
    {
        return new DbLicenses
        {
            License = drogeLicense.License,
            CustomerId = customerId,
        };
    }
}