using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class CustomerMapper
{
    public static LinkedCustomer ToLinkedCustomer(this DbLinkUserCustomer dbLinkUserCustomer, Guid currentCustomerId)
    {
        return new LinkedCustomer
        {
            Id = dbLinkUserCustomer.Id,
            CustomerId = dbLinkUserCustomer.CustomerId,
            UserId = dbLinkUserCustomer.UserId,
            GlobalUserId = dbLinkUserCustomer.GlobalUserId,
            Name = dbLinkUserCustomer.Customer.Name,
            IsPrimary = dbLinkUserCustomer.IsPrimary,
            IsCurrent = dbLinkUserCustomer.CustomerId == currentCustomerId,
            SetBySync = dbLinkUserCustomer.SetBySync,
            Order = dbLinkUserCustomer.Order,
        };
    }

    public static Customer ToCustomer(this DbCustomers dbCustomer)
    {
        return new Customer
        {
            Id = dbCustomer.Id,
            Name = dbCustomer.Name,
            TimeZone = dbCustomer.TimeZone,
            Created = dbCustomer.Created,
            Domain = dbCustomer.Domain,
            Instance = dbCustomer.Instance,
            TenantId = dbCustomer.TenantId,
            GroupId = dbCustomer.GroupId,
        };
    }

    public static CustomerAuthentication ToCustomerAuthentication(this DbCustomers dbCustomer)
    {
        return new CustomerAuthentication
        {
            Id = dbCustomer.Id,
            GroupId = dbCustomer.GroupId,
        };
    }
}