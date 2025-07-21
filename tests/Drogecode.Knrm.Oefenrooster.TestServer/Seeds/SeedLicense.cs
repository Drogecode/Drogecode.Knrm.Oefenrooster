using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public static class SeedLicense
{
    public static Guid Seed(DataContext dataContext, Guid customerId, Licenses license, DateTime? validFrom = null, DateTime? validUntil = null)
    {
        var newId = Guid.NewGuid();
        dataContext.Licenses.Add(new DbLicenses()
        {
            Id = newId,
            CustomerId = customerId,
            License = license,
            ValidFrom = validFrom,
            ValidUntil = validUntil
        });
        dataContext.SaveChanges();
        return newId;
    }
}