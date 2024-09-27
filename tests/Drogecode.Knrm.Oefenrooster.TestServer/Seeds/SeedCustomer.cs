using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Seeds;

public class SeedCustomer
{
    public static void Seed(DataContext dataContext, Guid defaultCustomerId)
    {
        dataContext.Customers.Add(new DbCustomers
        {
            Id = defaultCustomerId,
            Name = "xUnit customer",
            TimeZone = "Europe/Amsterdam",
            Created = DateTime.UtcNow
        });
        dataContext.SaveChanges();
    }
}