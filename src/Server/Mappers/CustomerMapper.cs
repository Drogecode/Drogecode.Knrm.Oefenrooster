using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class CustomerMapper
{
    public static LinkedCustomer ToLinkedCustomer(this DbLinkUserCustomer dbLinkUserCustomer, Guid currentCustomerId)
    {
        return new LinkedCustomer()
        {
            CustomerId = dbLinkUserCustomer.CustomerId,
            UserId = dbLinkUserCustomer.UserId,
            Name = dbLinkUserCustomer.Customer.Name,
            IsPrimary = dbLinkUserCustomer.IsPrimary,
            IsCurrent = dbLinkUserCustomer.CustomerId == currentCustomerId,
            Order = dbLinkUserCustomer.Order,
        };
    }

    public static Customer ToCustomer(this DbCustomers dbCustomer)
    {
        return new Customer()
        {
            Id = dbCustomer.Id,
            Name = dbCustomer.Name,
            TimeZone = dbCustomer.TimeZone,
            Created = dbCustomer.Created,
            Domain = dbCustomer.Domain,
            Instance = dbCustomer.Instance,
            TenantId = dbCustomer.TenantId,
        };
    }
}