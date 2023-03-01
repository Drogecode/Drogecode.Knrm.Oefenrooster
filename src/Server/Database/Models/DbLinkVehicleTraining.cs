﻿namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbLinkVehicleTraining
{
    public Guid Id { get; set; }
    public Guid RoosterTrainingId { get; set; }
    public Guid Vehicle { get; set; }
    public Guid CustomerId { get; set; }
    public bool IsSelected { get; set; }

    public DbCustomers Customer { get; set; }
}