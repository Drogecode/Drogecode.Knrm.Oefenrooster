﻿namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class GetAllUserLinkCustomersResponse : BaseMultipleResponse
{
    public List<LinkedCustomer>? UserLinkedCustomers { get; set; }
}