using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class CustomerMapper
{
    public static Customer ToCustomer(this DbLinkUserCustomer dbLinkUserCustomer, Guid currentCustomerId)
    {
        return new Customer()
        {
            Id = dbLinkUserCustomer.CustomerId,
            Name = dbLinkUserCustomer.Customer.Name,
            IsPrimary = dbLinkUserCustomer.IsPrimary,
            IsCurrent = dbLinkUserCustomer.CustomerId == currentCustomerId,
            Order = dbLinkUserCustomer.Order,
        };
    }
}